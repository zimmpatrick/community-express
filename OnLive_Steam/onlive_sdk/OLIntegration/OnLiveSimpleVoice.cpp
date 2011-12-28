///////////////////////////////////////////////////////////////////////////////
//
//  SimpleVoiceChannel.cpp $Revision: 0 $
//
///////////////////////////////////////////////////////////////////////////////
//
// This class is used as a starting point for integrating the OnLive Voice SDK into
// your game.
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

#include "OnLiveSimpleVoice.h"
#include "OnLiveFunctions.h"
#include "OnLiveIntegration.h"

using namespace olapi;

#ifdef _DEBUG
#define DEBUG_LOGGING			  // *** TURN THIS OFF BEFORE CERTIFICATION ***
#endif

///////////////////////////////////////////////////////////////////////////////
//                              Static Info for all Channel Objects		     //
///////////////////////////////////////////////////////////////////////////////

static SimpleVoiceChannel::ChannelMap Channels;
static SimpleVoiceChannel::InitStatus gVoiceInitialized = SimpleVoiceChannel::VOICE_NOT_INITIALIZED;
static olapi::OLVoice *gVoice;
static bool gOnLiveVoiceInit = false;
static int gOnLiveVoiceRefCount = 0;

static void ParticipantEvent(void * pClassPtr, OnLiveContextEventDispatcher::CallbackResult result);
static void ChannelExitedEvent(void * pClassPtr, OnLiveContextEventDispatcher::CallbackResult result);
static void ChannelJoinedEvent(void * pClassPtr, OnLiveContextEventDispatcher::CallbackResult result);
static void VoiceInitEvent(void * pClassPtr, OnLiveContextEventDispatcher::CallbackResult result);
static void HostCreateChannelEvent(void * pClassPtr, OnLiveContextEventDispatcher::CallbackResult result);

///////////////////////////////////////////////////////////////////////////////
//                              Helper Routines                     	     //
///////////////////////////////////////////////////////////////////////////////

void normalizetag(char *dst, const char *src, int dst_size)
{
	if (!dst || !src || !dst_size) return;

	int size = 0;

	while (*src && size < dst_size - 1)
	{
		char in = *src;
		
		if (isupper(in)) in = _tolower(in);

		if (isalnum(in)) // only keep these...   isalpha
		{
			*dst++ = in;
			size++;
		}

		src++;
	}

	*dst = '\0';
}

///////////////////////////////////////////////////////////////////////////////
//                              SimpleVoiceChannel						     //
///////////////////////////////////////////////////////////////////////////////

SimpleVoiceChannel::ChannelMap &SimpleVoiceChannel::GetChannelMap()	
{
	return Channels; 
}

SimpleVoiceChannel::Channel &SimpleVoiceChannel::GetChannel() 
{ 
	return Channels[mChannelId]; 
}

SimpleVoiceChannel::ParticipantMap &SimpleVoiceChannel::GetParticipantMap() 
{
	return Channels[mChannelId].Participants; 
}

SimpleVoiceChannel::Participant &SimpleVoiceChannel::GetParticipant(olapi::OLID userId) 
{
	return Channels[mChannelId].Participants[userId];
}

olapi::OLID SimpleVoiceChannel::GetParticipantOLID(char *tagName, bool normalize)
{
	char name1[OL_MAX_VALUE_STRING_SIZE];

	if (!tagName) return olapi::OLID(0);

	if (!normalize)	strncpy(name1, tagName, OL_MAX_VALUE_STRING_SIZE);
	else			normalizetag(name1, tagName, OL_MAX_VALUE_STRING_SIZE);

	for (SimpleVoiceChannel::ParticipantMap::iterator itr = Channels[mChannelId].Participants.begin(); itr != Channels[mChannelId].Participants.end(); )
	{
		SimpleVoiceChannel::ParticipantMap::iterator itr_cur = itr;
		++itr;

		if (itr_cur->second.status != SimpleVoiceChannel::PARTICIPANT_NOT_INITIALIZED && itr_cur->second.status != SimpleVoiceChannel::PARTICIPANT_ERROR)
		{
			char name2[OL_MAX_VALUE_STRING_SIZE];

			if (!normalize)	strncpy(name2, itr_cur->second.GetTag().c_str(), OL_MAX_VALUE_STRING_SIZE);
			else			normalizetag(name2, itr_cur->second.GetTag().c_str(), OL_MAX_VALUE_STRING_SIZE);

			if (!strcmp(name1,name2))
			{
				return itr_cur->second.GetID();
			}
		}
	}

	return olapi::OLID(0);
}


SimpleVoiceChannel::SimpleVoiceChannel()
	: mUserId(olapi::OLID())
	, mHost(false)
	, mChannelId(OL_INVALID_CHANNEL)
	, mVoiceEventHandler(NULL)
	, mVoiceEventCallback(NULL)
	, mBusyWait(false)
{

}

///////////////////////////////////////////////////////////////////////////////

void SimpleVoiceChannel::init() // Warning, this must/should be called before OLStartServices
{
	if (gVoiceInitialized == VOICE_NOT_INITIALIZED && !gOnLiveVoiceInit)
	{
		gOnLiveVoiceInit = true;
		gOnLiveVoiceRefCount = 0;
		
		OLStatus status = olapi::OLGetVoice(OLAPI_VERSION, &gVoice);

		if (status == OL_SUCCESS)
		{
			mVoiceEventHandler = new VoiceEventHandler();
			gVoice->SetEventHandler(mVoiceEventHandler);

			mVoiceEventCallback = new VoiceEventCallback();
			gVoice->SetEventWaitingCallback(mVoiceEventCallback);
		}
		else
		{
			onlive_function::exit_game(true);
		}
	}

	gOnLiveVoiceRefCount++;

	#ifdef DEBUG_LOGGING
		INITLOG(); // Set the log level to DEBUG (only do this for test harness and pre-certification
	#endif
}

///////////////////////////////////////////////////////////////////////////////

void SimpleVoiceChannel::deinit()
{
	if (gOnLiveVoiceRefCount)
	{
		gOnLiveVoiceRefCount--;

		if (!gOnLiveVoiceRefCount)
		{
			if (gVoice)
			{
				olapi::OLStopVoice(gVoice);
			}

			if (mVoiceEventHandler)
			{
				delete mVoiceEventHandler;
			}

			if (mVoiceEventCallback)
			{
				delete mVoiceEventCallback;
			}

			mVoiceEventHandler = NULL;
			mVoiceEventCallback = NULL;
		}
	}

	mUserId = olapi::OLID();
	mHost = false;
	mChannelId = OL_INVALID_CHANNEL;
	gVoiceInitialized = VOICE_NOT_INITIALIZED;
	gOnLiveVoiceInit = false;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLID SimpleVoiceChannel::GetUserID()
{
	return mUserId;
}

///////////////////////////////////////////////////////////////////////////////

SimpleVoiceChannel::OLChannel SimpleVoiceChannel::GetChannelID()
{
	return mChannelId;
}

///////////////////////////////////////////////////////////////////////////////

SimpleVoiceChannel::InitStatus SimpleVoiceChannel::GetInitStatus()
{
	return gVoiceInitialized;
}

///////////////////////////////////////////////////////////////////////////////

const char *SimpleVoiceChannel::GetChannelName()
{
	if (mCreateOnly && !mChannelURI.empty())
	{
		LOG( "SimpleVoiceChannel::GetChannelName:Channel has not been joined.");
		return NULL;
	}

	if (mChannelId == OL_INVALID_CHANNEL)
	{
		LOG( "SimpleVoiceChannel::GetChannelName:Channel is not initialized.");
		return NULL;
	}

	if (Channels.find(mChannelId) == Channels.end())
	{
		LOG( "SimpleVoiceChannel::GetChannelName:Channel not does exist.");
		return NULL;
	}

	return Channels[mChannelId].channelName.c_str();
}

///////////////////////////////////////////////////////////////////////////////

const char *SimpleVoiceChannel::GetChannelURI()
{
	if (mCreateOnly && !mChannelURI.empty())
	{
		return mChannelURI.c_str();
	}

	if (mChannelId == OL_INVALID_CHANNEL)
	{
		LOG( "SimpleVoiceChannel::GetChannelURI:Channel is not initialized.");
		return NULL;
	}

	if (Channels.find(mChannelId) == Channels.end())
	{
		LOG( "SimpleVoiceChannel::GetChannelURI:Channel not does exist");
		return NULL;
	}

	return Channels[mChannelId].channelURI.c_str();
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLID SimpleVoiceChannel::GetChannelOwner()
{
	if (mCreateOnly && !mChannelURI.empty())
	{
		LOG( "SimpleVoiceChannel::GetChannelOwner:Channel has not been joined.");
		return olapi::OLID();
	}

	if (mChannelId == OL_INVALID_CHANNEL)
	{
		LOG( "SimpleVoiceChannel::GetChannelOwner:Channel is not initialized.");
		return olapi::OLID();
	}

	if (Channels.find(mChannelId) == Channels.end())
	{
		LOG( "SimpleVoiceChannel::GetChannelURI:Channel not does exist");
		return olapi::OLID();
	}

	return Channels[mChannelId].channelOwner;
}

///////////////////////////////////////////////////////////////////////////////

SimpleVoiceChannel::ChannelStatus SimpleVoiceChannel::GetChannelStatus()
{
	if (mCreateOnly && !mChannelURI.empty())
	{
		return SimpleVoiceChannel::CHANNEL_NOT_JOINED;
	}

	if (mChannelId == OL_INVALID_CHANNEL)
	{
		return SimpleVoiceChannel::CHANNEL_NOT_INITIALIZED;
	}

	if (Channels.find(mChannelId) == Channels.end())
	{
		return SimpleVoiceChannel::CHANNEL_NOT_FOUND;
	}

	return (SimpleVoiceChannel::ChannelStatus)Channels[mChannelId].status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus SimpleVoiceChannel::InitVoice(bool busy_wait)
{
	if (gVoiceInitialized != SimpleVoiceChannel::VOICE_NOT_INITIALIZED) return olapi::OL_INTERNAL_ERROR;

	if (!gVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = gVoice->InitChat(context);

	if (status == OL_SUCCESS)
	{
		gVoiceInitialized = VOICE_INITIALIZING;

		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, this, &VoiceInitEvent);

		if (busy_wait)
		{
			while (gVoiceInitialized == SimpleVoiceChannel::VOICE_INITIALIZING)
			{
				Sleep(0);
			}

			if (gVoiceInitialized != SimpleVoiceChannel::VOICE_INITIALIZED)
			{
				status = olapi::OL_INTERNAL_ERROR;
			}
		}
	}

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus SimpleVoiceChannel::HostInitChannel( const char *channel_name, olapi::OLID hostId, bool createOnly, bool busy_wait )
{
	olapi::OLStatus status = olapi::OL_FAIL;

	if (gVoiceInitialized != VOICE_INITIALIZED) return olapi::OL_FAIL;

	if (hostId == OL_IDENTITY_ID)
	{
		mUserId = hostId;

		mChannelURI.clear();
		mHost = true;
		mCreateOnly = createOnly;
		// This is the host, create the channel
		if (gVoice)
		{
			OLContext context = olapi::OLGetNextContext();
			status = gVoice->CreateChannel(context, channel_name, NULL);

			if (status == OL_SUCCESS)
			{
				mBusyWait = busy_wait; 

				OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, this, &HostCreateChannelEvent);

				while (busy_wait && (GetChannelStatus() < SimpleVoiceChannel::CHANNEL_NORMAL || GetChannelStatus() > SimpleVoiceChannel::CHANNEL_FULL)) 
				{
					// if failed, clean up the dummy channel. Dummy channel is only created when busy wait flag is true.
					if (GetChannelStatus() & SimpleVoiceChannel::CHANNEL_FAILED)
					{
						OLChannel channelId = context;
						Channels[channelId].Participants.clear();
						Channels.erase(channelId);
						mChannelId = OL_INVALID_CHANNEL;
						mChannelURI.clear();
						status = olapi::OL_FAIL;
						break;
					}
					Sleep(0);
				}
			}
		}
		else
		{
			status =  olapi::OL_SERVICES_NOT_STARTED;
		}
	}

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus SimpleVoiceChannel::ClientInitChannel( const char *channel_uri, olapi::OLID userId, bool busy_wait)
{
	olapi::OLStatus status = olapi::OL_FAIL;

	if (gVoiceInitialized != VOICE_INITIALIZED) return olapi::OL_FAIL;

	if (userId == OL_IDENTITY_ID)
	{
		mUserId = userId;
		mHost = false;

		// This is the client, join the channel
		if (gVoice)
		{
			OLContext context = olapi::OLReserveNextContext(olapi::OL_DURATION_USER_SESSION);
			status = gVoice->JoinChannel(context, channel_uri, NULL, 
				(olapi::OLVoiceJoinChannelFlags) (false ? olapi::OL_VOICE_PUSH_TO_TALK_MODE | olapi::OL_VOICE_JOIN_MUTED : 0));

			if (status == OL_SUCCESS)
			{
				mBusyWait = busy_wait; 

				OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, this, &ChannelJoinedEvent, true);

				while (busy_wait && (GetChannelStatus() < SimpleVoiceChannel::CHANNEL_NORMAL || GetChannelStatus() > SimpleVoiceChannel::CHANNEL_FULL))
				{
					// if failed, clean up the dummy channel. Dummy channel is only created when busy wait flag is true.
					if (GetChannelStatus() & SimpleVoiceChannel::CHANNEL_FAILED)
					{
						OLChannel channelId = context;
						Channels[channelId].Participants.clear();
						Channels.erase(channelId);
						mChannelId = OL_INVALID_CHANNEL;
						mChannelURI.clear();
						status = olapi::OL_FAIL;
						break;
					}
					Sleep(0);
				}
			}
		}
		else
		{
			status = olapi::OL_SERVICES_NOT_STARTED;
		}
	}

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus SimpleVoiceChannel::StopChannel( )
{
	OLStatus status = olapi::OL_FAIL;

	if (gVoiceInitialized != VOICE_INITIALIZED) return olapi::OL_FAIL;

	if (mCreateOnly == false)
	{
		if (mChannelId == OL_INVALID_CHANNEL) return olapi::OL_FAIL;

		if (gVoice)
		{
			OLContext context = olapi::OLGetNextContext();
			status = gVoice->LeaveChannel(context, Channels[mChannelId].channelURI.c_str());

			if (status == OL_SUCCESS)
			{
				OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, this, &ChannelExitedEvent);
			}
		}
		else
		{
			status = olapi::OL_SERVICES_NOT_STARTED;
		}
	}
	else
	{
		if (mHost)
		{
			// I'm the host, kill it.
			if (gVoice)
			{
				OLContext context = olapi::OLGetNextContext();
				status = gVoice->DestroyChannel(context, mChannelURI.c_str());
			}
			else
			{
				status = olapi::OL_SERVICES_NOT_STARTED;
			}
		}

		mChannelURI.clear();
		mHost = true;
		mCreateOnly = false;
		mChannelId = OL_INVALID_CHANNEL;
	}

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus SimpleVoiceChannel::SetUserVolume( olapi::OLID userId, int level)
{
	if (gVoiceInitialized != VOICE_INITIALIZED) return olapi::OL_FAIL;
	if (Channels[mChannelId].Participants.find(userId) == Channels[mChannelId].Participants.end())
		return olapi::OL_INVALID_IDENTITY_ID;

	OLStatus status =  olapi::OL_SERVICES_NOT_STARTED;

	if (gVoice)
	{
		OLContext context = olapi::OLGetNextContext();
		status = gVoice->SetParticipantPlaybackLevel(context, Channels[mChannelId].channelURI.c_str(), userId, level);

		if (status == OL_SUCCESS)
			OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, this, NULL);
	}

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus SimpleVoiceChannel::SetUserMute( olapi::OLID userId, bool mute)
{
	if (gVoiceInitialized != VOICE_INITIALIZED) return olapi::OL_FAIL;
	if (Channels[mChannelId].Participants.find(userId) == Channels[mChannelId].Participants.end())
		return olapi::OL_INVALID_IDENTITY_ID;

	OLStatus status =  olapi::OL_SERVICES_NOT_STARTED;

	if (gVoice)
	{
		OLContext context = olapi::OLGetNextContext();
		status = gVoice->SetParticipantPlaybackMuteStatus(context, Channels[mChannelId].channelURI.c_str(), userId, mute);

		if (status == OL_SUCCESS)
			OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, this, NULL);
	}

	return status;
}

///////////////////////////////////////////////////////////////////////////////

SimpleVoiceChannel::ParticipantStatus SimpleVoiceChannel::GetUserStatus(olapi::OLID userId)
{
	if (gVoiceInitialized != VOICE_INITIALIZED) return SimpleVoiceChannel::PARTICIPANT_ERROR;
	if (mChannelId == OL_INVALID_CHANNEL ) return SimpleVoiceChannel::PARTICIPANT_ERROR;

	if (Channels[mChannelId].Participants.find(userId) == Channels[mChannelId].Participants.end())
		return SimpleVoiceChannel::PARTICIPANT_ERROR;

	return (SimpleVoiceChannel::ParticipantStatus)Channels[mChannelId].Participants[userId].status;
}

///////////////////////////////////////////////////////////////////////////////

const char *SimpleVoiceChannel::GetUserTag(olapi::OLID userId)
{
	if (gVoiceInitialized != VOICE_INITIALIZED) return NULL;
	if (mChannelId == OL_INVALID_CHANNEL ) return NULL;

	if (Channels[mChannelId].Participants.find(userId) == Channels[mChannelId].Participants.end())
		return NULL;

	return Channels[mChannelId].Participants[userId].tag.c_str();
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus SimpleVoiceChannel::SetSpeakingIndicatorStatus( bool visible )
{
	if (gVoiceInitialized != VOICE_INITIALIZED) return olapi::OL_FAIL;

	if (!gVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLStatus status = gVoice->SetSpeakingIndicatorStatus(visible);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

static void VoiceInitEvent(void * pClassPtr, OnLiveContextEventDispatcher::CallbackResult result)
{
	UNUSED_PARAM(pClassPtr);

	DEBUGLOG( "OnLiveSimpleVoice::StatusResult:VoiceInitEvent" );

	if(	result.validData != OnLiveContextEventDispatcher::CallbackResult::DATA_TYPE_STATUS_RESULT )
	{
		LOG( "OnLiveSimpleVoice::ContainerIDEvent:VoiceInit - failed, wrong event type: %s", OnLiveContextEventDispatcher::getInstance().OnLiveDispatcherDataTypeToString(result.validData));
		gVoiceInitialized = SimpleVoiceChannel::VOICE_FAILED;
		return;
	}

	if (result.status != olapi::OL_SUCCESS)
	{
		LOG( "OnLiveSimpleVoice::ContainerIDEvent:VoiceInit - failed, %s", olapi::OLStatusToString(result.status));
		gVoiceInitialized = SimpleVoiceChannel::VOICE_FAILED;
		return;
	}

	LOG( "OnLiveSimpleVoice::Voice Initialized.");
	gVoiceInitialized = SimpleVoiceChannel::VOICE_INITIALIZED;
}

///////////////////////////////////////////////////////////////////////////////

static void HostCreateChannelEvent(void * pClassPtr, OnLiveContextEventDispatcher::CallbackResult result)
{
	UNUSED_PARAM(pClassPtr);

	DEBUGLOG( "OnLiveSimpleVoice::ContainerIDEvent:HostCreateChannelEvent" );

	if (result.validData == OnLiveContextEventDispatcher::CallbackResult::DATA_TYPE_STATUS_RESULT )
	{
		LOG( "OnLiveSimpleVoice::ContainerIDEvent:HostCreateChannel - failed %s", olapi::OLStatusToString(result.status));
		if (((SimpleVoiceChannel *)pClassPtr)->mBusyWait)
		{
			SimpleVoiceChannel::OLChannel channelId = result.context;
			((SimpleVoiceChannel *)pClassPtr)->mChannelId = channelId;
			((SimpleVoiceChannel *)pClassPtr)->mChannelURI = "";
			Channels[channelId].status = SimpleVoiceChannel::CHANNEL_FAILED;
			Channels[channelId].channelOwner = olapi::OLID(0);
			Channels[channelId].channelURI = "";
			Channels[channelId].channelName = "";
		}
		return;
	}
	else if(result.validData != OnLiveContextEventDispatcher::CallbackResult::DATA_TYPE_OLID )
	{
		LOG( "OnLiveSimpleVoice::ContainerIDEvent:HostCreateChannel - failed, wrong event type: %s", OnLiveContextEventDispatcher::getInstance().OnLiveDispatcherDataTypeToString(result.validData));
		return;
	}

	olapi::OLID id = result.id;
	char channel_uri[OL_MAX_VALUE_STRING_SIZE];
	olapi::OLStatus status;
	channel_uri[0] = 0;

	status = OnLiveService::getInstance().mService->GetContainerValue( id, olapi::VoiceContainerKeyNames[olapi::OLKEY_RO_VoiceChannelURI], OL_MAX_VALUE_STRING_SIZE, channel_uri );

	((SimpleVoiceChannel *)pClassPtr)->mChannelURI = std::string(channel_uri); // save so we can stop a non-joined channel

	if (status == olapi::OL_SUCCESS)
	{
		if (((SimpleVoiceChannel *)pClassPtr)->mCreateOnly == false)
		{
			if (gVoice)
			{
				OLContext context = olapi::OLReserveNextContext(olapi::OL_DURATION_USER_SESSION);
				status = gVoice->JoinChannel(context, channel_uri, NULL, 
					(olapi::OLVoiceJoinChannelFlags) (false ? olapi::OL_VOICE_PUSH_TO_TALK_MODE | olapi::OL_VOICE_JOIN_MUTED : 0));

				if (status == OL_SUCCESS)
				{
					OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, &ChannelJoinedEvent, true);
				}
			}
			else
			{
				status =  olapi::OL_SERVICES_NOT_STARTED;
			}
		}
	}

	if (status != OL_SUCCESS)
	{
		LOG("OnLiveSimpleVoice::ContainerIDEvent:HostCreateChannel - Could not create channel");
		((SimpleVoiceChannel *)pClassPtr)->mChannelId = OL_INVALID_CHANNEL;
	}

}

///////////////////////////////////////////////////////////////////////////////

static void ChannelJoinedEvent(void * pClassPtr, OnLiveContextEventDispatcher::CallbackResult result)
{
	UNUSED_PARAM(pClassPtr);

	DEBUGLOG( "OnLiveSimpleVoice::ChannelJoinedEvent" );

	if(	result.validData == OnLiveContextEventDispatcher::CallbackResult::DATA_TYPE_PARTICIPANT_EVENT)
	{
		LOG("OnLiveSimpleVoice:ChannelJoinedEvent:Out of order packet dlivery to ParticipantEvent");
		ParticipantEvent(pClassPtr, result);
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(result.context, pClassPtr, &ChannelJoinedEvent, true);
		return;
	}
	else if( result.validData == OnLiveContextEventDispatcher::CallbackResult::DATA_TYPE_STATUS_RESULT)
	{
		// do nothing.
	}
	else if( result.validData != OnLiveContextEventDispatcher::CallbackResult::DATA_TYPE_CHANNEL_EVENT)
	{
		LOG( "OnLiveSimpleVoice:ChannelJoinedEvent: failed, wrong event type: %s", OnLiveContextEventDispatcher::getInstance().OnLiveDispatcherDataTypeToString(result.validData));
		return;
	}

	olapi::OLContext context = result.context;
	SimpleVoiceChannel::OLChannel channelId = ((SimpleVoiceChannel *)pClassPtr)->mChannelId;

	if (channelId != OL_INVALID_CHANNEL)
	{
		LOG( "OnLiveSimpleVoice::ChannelJoinedEvent:Channel already existed, throwing out event %08x", channelId);
		return;
	}

	if (Channels.find(context) != Channels.end())
	{
		LOG( "OnLiveSimpleVoice::ChannelJoinedEvent:Channel already existed, throwing out event %08x", channelId);
		return;
	}

	((SimpleVoiceChannel *)pClassPtr)->mChannelId = context;

	if( result.validData == OnLiveContextEventDispatcher::CallbackResult::DATA_TYPE_STATUS_RESULT )
	{
		LOG( "OnLiveSimpleVoice::ChannelJoinedEvent: failed, %s", olapi::OLStatusToString(result.status));
		if (((SimpleVoiceChannel *)pClassPtr)->mBusyWait)
		{
			Channels[context].status = SimpleVoiceChannel::CHANNEL_FAILED;
			Channels[context].channelOwner = olapi::OLID(0);
			Channels[context].channelURI = "";
			Channels[context].channelName = "";
		}
	}
	else
	{
		Channels[context].status &= ~(SimpleVoiceChannel::CHANNEL_NOT_INITIALIZED | SimpleVoiceChannel::CHANNEL_FULL);
		Channels[context].status |= SimpleVoiceChannel::CHANNEL_EMPTY;
		Channels[context].channelOwner = result.channelEvent.channelOwner;
		Channels[context].channelURI = std::string(result.channelEvent.channelURI);
		Channels[context].channelName = std::string(result.channelEvent.channelName);

		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, &ParticipantEvent, true);
	}
}

///////////////////////////////////////////////////////////////////////////////

static void ChannelExitedEvent(void * pClassPtr, OnLiveContextEventDispatcher::CallbackResult result)
{
	UNUSED_PARAM(pClassPtr);

	DEBUGLOG( "OnLiveSimpleVoice::ChannelExitedEvent" );

	if (result.validData == OnLiveContextEventDispatcher::CallbackResult::DATA_TYPE_STATUS_RESULT)
	{
		LOG( "OnLiveSimpleVoice::ChannelExitedEvent:StatusResult %s, deleting channel", olapi::OLStatusToString(result.status));
	}
	else if(result.validData != OnLiveContextEventDispatcher::CallbackResult::DATA_TYPE_CHANNEL_EVENT)
	{
		LOG( "OnLiveSimpleVoice::ChannelExitedEvent- failed, wrong event type: %s", OnLiveContextEventDispatcher::getInstance().OnLiveDispatcherDataTypeToString(result.validData));
		return;
	}
		

	SimpleVoiceChannel::OLChannel channelId = ((SimpleVoiceChannel *)pClassPtr)->mChannelId;

	if (channelId == OL_INVALID_CHANNEL)
	{
		LOG( "OnLiveSimpleVoice::ChannelExitedEvent:Invalid Channel ID, throwing out event %08x", channelId);
		return;
	}

	if (Channels.find(channelId) == Channels.end())
	{
		LOG("OnLiveSimpleVoice::ChannelExitedEvent:Channel does not exist, throwing out event %08x", channelId);
		return;
	}

	if (((SimpleVoiceChannel *)pClassPtr)->mHost)
	{
		// I'm the host, kill it.
		// We dont wait for the status for destroying the Channel because we are cleaning up and may not be around then.
		if (gVoice)
		{
			OLContext context = olapi::OLGetNextContext();
			OLStatus status = gVoice->DestroyChannel(context, Channels[channelId].channelURI.c_str());
		}
	}

	Channels[channelId].Participants.clear();
	Channels.erase(channelId);
	((SimpleVoiceChannel *)pClassPtr)->mChannelId = OL_INVALID_CHANNEL;
	((SimpleVoiceChannel *)pClassPtr)->mChannelURI.clear();

	OnLiveContextEventDispatcher::getInstance().RemoveDispatchContext(channelId); // remove the reserved context
}

///////////////////////////////////////////////////////////////////////////////

static void ParticipantEvent(void * pClassPtr, OnLiveContextEventDispatcher::CallbackResult result)
{
	UNUSED_PARAM(pClassPtr);

	DEBUGLOG( "OnLiveSimpleVoice::ParticipantEvent" );

	if(	result.validData != OnLiveContextEventDispatcher::CallbackResult::DATA_TYPE_PARTICIPANT_EVENT )
	{
		LOG( "OnLiveSimpleVoice::ParticipantEvent: failed, wrong event type: %s", OnLiveContextEventDispatcher::getInstance().OnLiveDispatcherDataTypeToString(result.validData));
		return;
	}

	const char *msg = NULL;
	olapi::OLContext context = result.context;
	olapi::OLID participantID = result.participantEvent.participantID;
	const char *participantTag = result.participantEvent.participantTag;
	olapi::OLVoiceParticipantEventType type = result.participantEvent.type;

	char id[OLID_STRING_SIZE];

	SimpleVoiceChannel::OLChannel channelId = ((SimpleVoiceChannel *)pClassPtr)->mChannelId;

	if (channelId == OL_INVALID_CHANNEL || channelId != context)
	{
		LOG( "OnLiveSimpleVoice::ParticipantEvent:Invalid Channel ID, throwing out event %08x", channelId);
		return;
	}

	if (Channels.find(context) == Channels.end())
	{
		LOG("OnLiveSimpleVoice::ParticipantEvent:Channel does not exist, throwing out event %08x", channelId);
		return;
	}

	DEBUGLOG("OnLiveSimpleVoice::ParticipantEvent( %u, %s,  %s, %d )", context, participantID.getIDString(id), participantTag, type);

	if (Channels.find(context) == Channels.end())
	{
		LOG("OnLiveSimpleVoice::ParticipantEvent:Invalid Context, throwing out event");
		return;
	}

	if (participantID == olapi::OL_INVALID_ID || !strcmp(participantID.getIDString(id), "unknown")) 
	{
		LOG("OnLiveSimpleVoice::ParticipantEvent:Invalid OLID, throwing out event");
		return;
	}

	if (Channels[context].Participants.find(participantID) == Channels[context].Participants.end())
	{
		// New User
		Channels[context].Participants[participantID].status = SimpleVoiceChannel::PARTICIPANT_IDLE;
	}

	Channels[context].Participants[participantID].id = participantID;
	Channels[context].Participants[participantID].tag = std::string(participantTag);
	Channels[context].Participants[participantID].state = type;
	
	switch (type)
	{
		case olapi::OL_VOICE_PARTICIPANT_MUTE:
		{
			Channels[context].Participants[participantID].status |=  SimpleVoiceChannel::PARTICIPANT_MUTED;
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_TALKING;
			msg = "Participant muted: ";
		}
		break;

		case olapi::OL_VOICE_PARTICIPANT_IDLE:
		{
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_MUTED;
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_TALKING;
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_LISTEN_ONLY;
			msg = "Participant idle: ";
		}
		break;
		case olapi::OL_VOICE_PARTICIPANT_SPEAKING:
		{
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_MUTED;
			Channels[context].Participants[participantID].status |=  SimpleVoiceChannel::PARTICIPANT_TALKING;
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_LISTEN_ONLY;
			msg = "Participant speaking: ";
		}
		break;

		case olapi::OL_VOICE_PARTICIPANT_ENTERED:
		{
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_MUTED;
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_TALKING;
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_LISTEN_ONLY;
			msg = "Participant entered: ";
		}
		break;
		case olapi::OL_VOICE_PARTICIPANT_EXITED:
		{
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_MUTED;
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_TALKING;
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_LISTEN_ONLY;

			Channels[context].Participants.erase(participantID);
			msg = "Participant exited: ";
		}
		break;

		case olapi::OL_VOICE_PARTICIPANT_LISTEN_ONLY:
		{
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_TALKING;
			Channels[context].Participants[participantID].status |=  SimpleVoiceChannel::PARTICIPANT_LISTEN_ONLY;
			msg = "Participant is Listen Only: ";
		}
		break;
		case olapi::OL_VOICE_PARTICIPANT_MIC_ACTIVE:
		{
//			Channels[context].Participants[participantID].status &= ~PARTICIPANT_MUTED;
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_TALKING;
			Channels[context].Participants[participantID].status &= ~SimpleVoiceChannel::PARTICIPANT_LISTEN_ONLY;
			msg = "Participant Mic Active: ";
		}
		break;
	}

	if (msg) 
	{
		DEBUGLOG("%s %s %s", msg,  participantTag, participantID.getIDString(id));
	}

	// update channel status
	Channels[context].status &= ~SimpleVoiceChannel::CHANNEL_NOT_INITIALIZED;
	if (Channels[context].Participants.size() >= MAX_PARTICIPANTS)
	{
		Channels[context].status &= ~SimpleVoiceChannel::CHANNEL_EMPTY;
		Channels[context].status |= SimpleVoiceChannel::CHANNEL_FULL;
	}
	else if (Channels[context].Participants.size() == 0)
	{
		Channels[context].status &= ~SimpleVoiceChannel::CHANNEL_FULL;
		Channels[context].status |= SimpleVoiceChannel::CHANNEL_EMPTY;
	}

}


///////////////////////////////////////////////////////////////////////////////

SimpleVoiceChannel::VoiceEventHandler::VoiceEventHandler() 
{

}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus SimpleVoiceChannel::VoiceEventHandler::StatusResult(OLContext context, OLStatus status)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchStatusResult(context, status))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus SimpleVoiceChannel::VoiceEventHandler::ContainerIDResult(olapi::OLContext context, olapi::OLID id)
{
	bool status = OnLiveContextEventDispatcher::getInstance().DispatchContainerIDResult(context, id);

	OnLiveService::getInstance().mService->DestroyContainer(id);

	if (status == false)
		return olapi::OL_FAIL;
	else
		return olapi::OL_SUCCESS;
}

olapi::OLStatus SimpleVoiceChannel::VoiceEventHandler::ChannelJoinedEvent(olapi::OLContext context, OLID channelOwner, const char *channelURI, const char *channelName)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchChannelJoinedEvent(context, channelOwner, channelURI, channelName))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

olapi::OLStatus SimpleVoiceChannel::VoiceEventHandler::ChannelExitedEvent(OLContext context, OLID channelOwner, const char *channelURI, const char *channelName)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchChannelExitedEvent(context, channelOwner, channelURI, channelName))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

olapi::OLStatus SimpleVoiceChannel::VoiceEventHandler::ParticipantEvent(OLContext context, OLID participantID, const char *participantTag, OLVoiceParticipantEventType type)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchParticipantEvent(context, participantID, participantTag, type))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

olapi::OLStatus SimpleVoiceChannel::VoiceEventHandler::VoiceMonitorUpdate(OLContext context, bool voiceDetected, double energy, int smoothedEnergy)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchVoiceMonitorEvent(context, voiceDetected, energy, smoothedEnergy))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

olapi::OLStatus SimpleVoiceChannel::VoiceEventHandler::VoiceChatUserStatus(olapi::OLVoiceUserStatusFlags flags)
{
	return onlive_function::voice_chat_user_status(flags);
}

///////////////////////////////////////////////////////////////////////////////

SimpleVoiceChannel::VoiceEventCallback::VoiceEventCallback()
{

}

///////////////////////////////////////////////////////////////////////////////

void SimpleVoiceChannel::VoiceEventCallback::EventWaiting()
{
	gVoice->HandleEvent(0, 0);
}

