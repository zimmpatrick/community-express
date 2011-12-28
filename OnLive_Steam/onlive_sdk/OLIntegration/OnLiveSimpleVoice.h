///////////////////////////////////////////////////////////////////////////////
//
// OnLiveVoice.h $Revision: 0 $
//
// SDK Integrated
//
///////////////////////////////////////////////////////////////////////////////
// 
// This class is used as a basic starting point for integrating OnLive Voice into your
// game.  Most documentation to the classes defined herein are documented in the
// cpp file with minimal documentation in the header file.
// 
///////////////////////////////////////////////////////////////////////////////
// 
//           Copyright(c) 2009 - 2011 OnLive, Inc.  All Rights Reserved.
// 
// NOTICE TO USER:
// 
// This code contains confidential material proprietary to OnLive, Inc. Your access
// to and use of these confidential materials is subject to the terms and conditions
// of your confidentiality and SDK license agreements with OnLive. This document and
// information and ideas herein may not be disclosed, copied, reproduced or distributed
// to anyone outside your company without the prior written consent of OnLive.
// 
// You may not modify, reverse engineer, or otherwise attempt to use this code for
// purposes not specified in your SDK license agreement with OnLive. This material,
// including but not limited to documentation and related code, is protected by U.S.
// and international copyright laws and other intellectual property laws worldwide
// including, but not limited to, U.S. and international patents and patents pending.
// OnLive, MicroConsole, Brag Clips, Mova, Contour, the OnLive logo and the Mova logo
// are trademarks or registered trademarks of OnLive, Inc. The OnLive logo and the Mova
// logo are copyrights or registered copyrights of OnLive, Inc. All other trademarks
// and other intellectual property assets are the property of their respective owners
// and are used by permission. The furnishing of these materials does not give you any
// license to said intellectual property.
// 
// THIS SOFTWARE IS PROVIDED BY ONLIVE "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
// USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

#ifndef __ONLIVE_SIMPLE_VOICE_H__
#define __ONLIVE_SIMPLE_VOICE_H__

#include "OLVoice.h"

#include "OnLiveContextEventDispatcher.h"

///////////////////////////////////////////////////////////////////////////////
// THIS DEPENDS ON:
//
// OnLiveIntegration.h
// OnLiveIntegration.cpp
// OnLiveContextEventDispatcher.h
// OnLiveContextEventDispatcher.cpp
// OnLiveFunctions.h
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//                              Simple Voice                                 //
///////////////////////////////////////////////////////////////////////////////

// Notes: Look at the sample for how to use this Class

class SimpleVoiceChannel
{
public:

	#define OL_INVALID_CHANNEL						(OL_INVALID_CONTEXT)
	#define MAX_PARTICIPANTS						(50)

	typedef olapi::OLContext OLChannel;

	typedef enum ParticipantStatus					// These are bits, except ERROR
	{
		PARTICIPANT_ERROR		= -1,				// API ERROR: User not found or other voice error
		PARTICIPANT_IDLE		= 0,
		PARTICIPANT_MUTED		= 1,				// User is muted
		PARTICIPANT_TALKING		= 2,				// User is talking 
		PARTICIPANT_LISTEN_ONLY = 4,				// User is Listen Only
		PARTICIPANT_NOT_INITIALIZED = 8,			// No user exists here
	};

	typedef enum ChannelStatus						// These are bits, except CHANNEL_NOT_FOUND
	{
		CHANNEL_NOT_FOUND		= 64,
		CHANNEL_NORMAL			= 0,				// Not empty, not full...
		CHANNEL_EMPTY			= 1,				// Channel is Empty
		CHANNEL_FULL			= 2,				// Channel is Full
		CHANNEL_NOT_INITIALIZED = 8,				// No channel exists here
		CHANNEL_NOT_JOINED		= 16,				// Channel created only
		CHANNEL_FAILED			= 32,				// Channel created but failed initialization, channel must be deleted.
	};

	typedef enum InitStatus
	{
		VOICE_NOT_INITIALIZED = 0,
		VOICE_INITIALIZING,
		VOICE_INITIALIZED,
		VOICE_FAILED,
	};

	class Participant
	{
		public:
		olapi::OLID	id;								// if of user
		std::string tag;							// tag of user
		olapi::OLVoiceParticipantEventType state;	// event type
		unsigned status;							// ParticipantStatus (consolidated from state events)

		Participant() : id( 0 )
		{
			tag.clear();
			state = olapi::OL_VOICE_PARTICIPANT_EXITED;
			status = SimpleVoiceChannel::PARTICIPANT_NOT_INITIALIZED;
		}

		std::string &GetTag()			{ return tag; }
		olapi::OLID GetID()				{ return id; }
		ParticipantStatus GetStatus()	{ return (ParticipantStatus)status; }
	};

	typedef std::map<olapi::OLID, SimpleVoiceChannel::Participant> ParticipantMap;

	class Channel
	{
		public:
		olapi::OLID	channelOwner;					// owner of the channel
		std::string channelURI;						// vivox uri
		std::string channelName;					// channel name
		unsigned status;							// ChannelStatus (consolidated from all events)

		Channel() : channelOwner(0)
		{
			channelURI.clear();
			channelName.clear();
			status = SimpleVoiceChannel::CHANNEL_NOT_INITIALIZED;
		}

		std::string &GetName()		{ return channelName; }
		std::string &GetURI()		{ return channelURI; }
		olapi::OLID GetOwner()		{ return channelOwner; }
		ChannelStatus GetStatus()	{ return (ChannelStatus)status; }

		SimpleVoiceChannel::ParticipantMap Participants;
	};

	typedef std::map<SimpleVoiceChannel::OLChannel, Channel> ChannelMap;

	///////////////////////////////////////////////////////////////////////////////
	// Methods

	// only use this if you want 1 channel only
	static SimpleVoiceChannel& getSingleChannelInstance() 
	{
		static SimpleVoiceChannel instance;
		return instance;
	}

	SimpleVoiceChannel();

	// initialization (required)
	void init();
	void deinit();

	// Main Functions (Required)
	olapi::OLStatus InitVoice(bool busy_wait = true);
	olapi::OLStatus HostInitChannel( const char *channel_name, olapi::OLID hostId, bool createOnly = false, bool busy_wait = true);
	olapi::OLStatus ClientInitChannel( const char *channel_uri, olapi::OLID userId, bool busy_wait = true);
	olapi::OLStatus StopChannel( );

	// Optional use user functions
	olapi::OLStatus SetUserVolume( olapi::OLID userId, int level);
	olapi::OLStatus SetUserMute( olapi::OLID userId, bool mute);
	olapi::OLStatus SetSpeakingIndicatorStatus( bool visible );

	olapi::OLID GetUserID();
	ParticipantStatus GetUserStatus(olapi::OLID userId);
	const char *GetUserTag(olapi::OLID userId);

	// Optional use channel functions
	SimpleVoiceChannel::OLChannel GetChannelID();
	const char *GetChannelName();
	const char *GetChannelURI();
	olapi::OLID GetChannelOwner();
	ChannelStatus GetChannelStatus();

	// List functionality
	ChannelMap &GetChannelMap();
	Channel &GetChannel();
	ParticipantMap &GetParticipantMap();
	Participant &GetParticipant(olapi::OLID userId);
	olapi::OLID GetParticipantOLID(char *tagName, bool normalize);
	
	// Optional use init functions
	InitStatus GetInitStatus();

	class VoiceEventHandler : public olapi::OLVoiceEventHandler
	{
		friend class OnLiveVoice;

		public:
			VoiceEventHandler();

			void Log(olapi::OLLogLevel level, char* format, ...);

			// Inherited events
			virtual olapi::OLStatus StatusResult(olapi::OLContext context, olapi::OLStatus status);
			virtual olapi::OLStatus ContainerIDResult(olapi::OLContext context, olapi::OLID id);
			virtual olapi::OLStatus ChannelJoinedEvent(olapi::OLContext context, olapi::OLID channelOwner, const char *channelURI, const char *channelName);
			virtual olapi::OLStatus ChannelExitedEvent(olapi::OLContext context, olapi::OLID channelOwner, const char *channelURI, const char *channelName);
			virtual olapi::OLStatus ParticipantEvent(olapi::OLContext context, olapi::OLID participantID, const char *participantTag, olapi::OLVoiceParticipantEventType type);
			virtual olapi::OLStatus VoiceMonitorUpdate(olapi::OLContext context, bool voiceDetected, double energy, int smoothedEnergy);
			virtual olapi::OLStatus VoiceChatUserStatus(olapi::OLVoiceUserStatusFlags flags);
	};

	class VoiceEventCallback : public olapi::OLVoiceCallbacks
	{
		public:
			VoiceEventCallback();

			void EventWaiting();

		private:
			// No assignment operator
			VoiceEventCallback& operator=(VoiceEventCallback const&) {}
	};

	VoiceEventHandler *mVoiceEventHandler;
	VoiceEventCallback *mVoiceEventCallback;

	olapi::OLID mUserId;
	SimpleVoiceChannel::OLChannel mChannelId;
	bool mHost;
	bool mCreateOnly;
	std::string mChannelURI;
	bool mBusyWait;
};


#endif // __ONLIVE_SIMPLE_VOICE_H__
