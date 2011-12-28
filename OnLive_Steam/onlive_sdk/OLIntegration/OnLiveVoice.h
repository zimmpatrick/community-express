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

#ifndef __ONLIVE_VOICE_H__
#define __ONLIVE_VOICE_H__

//#include "OLAPI.h"
//#include "OLUser.h"
#include "OLVoice.h"

#include "OnLiveContextEventDispatcher.h"

///////////////////////////////////////////////////////////////////////////////
//                              OnLive Voice                           //
///////////////////////////////////////////////////////////////////////////////

class OnLiveVoice
{
public:

	static OnLiveVoice& getInstance()
	{
		static OnLiveVoice instance;
		return instance;
	}

	OnLiveVoice();

	void init();
	void deinit();

	void Log(olapi::OLLogLevel level, char* format, ...);
	
	olapi::OLStatus InitVoice( void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus StopVoice( void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus GetVoiceCapabilities( void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus CreateChannel(const char *channel_name, const char *channel_password, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus DestroyChannel(const char *channel_uri, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus JoinChannel(const char *channel_uri, const char *channel_password, bool join_muted, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus LeaveChannel(const char *channel_uri, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus StartVoiceMonitor( bool joinEchoChannel, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus StopVoiceMonitor( void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus SetParticipantPlaybackLevel( const char *channel_uri, olapi::OLID participantId, int level, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus SetParticipantPlaybackMuteStatus( const char *channel_uri, olapi::OLID participantId, bool mute, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus SetSpeakingIndicatorStatus( bool visible );

private:

	olapi::OLVoice * mVoice;

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
};

#endif // __ONLIVE_VOICE_H__
