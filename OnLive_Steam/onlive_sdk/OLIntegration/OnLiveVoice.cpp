///////////////////////////////////////////////////////////////////////////////
//
//  OnLiveVoice.cpp $Revision: 0 $
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

#include "OnLiveVoice.h"
#include "OnLiveFunctions.h"
#include "OnLiveIntegration.h"

using namespace olapi;

///////////////////////////////////////////////////////////////////////////////
//                              OnLiveVoice									 //
///////////////////////////////////////////////////////////////////////////////

OnLiveVoice::OnLiveVoice()
	: mVoiceEventHandler(NULL)
	, mVoiceEventCallback(NULL)
{
	
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveVoice::init() // Warning, this must be called before OLStartServices!
{
	OLStatus status = olapi::OLGetVoice(OLAPI_VERSION, &mVoice);

	if (status == OL_SUCCESS)
	{
		mVoiceEventHandler = new VoiceEventHandler();
		mVoice->SetEventHandler(mVoiceEventHandler);

		mVoiceEventCallback = new VoiceEventCallback();
		mVoice->SetEventWaitingCallback(mVoiceEventCallback);

		return;
	}

	onlive_function::exit_game(true);
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveVoice::deinit()
{
	if (mVoice)
	{
		olapi::OLStopVoice(mVoice);
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

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::GetVoiceCapabilities( void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mVoice->GetVoiceCapabilities(context);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::InitVoice( void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mVoice->InitChat(context);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::StopVoice( void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mVoice->StopChat(context);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::JoinChannel(const char *channel_uri, const char *channel_password, bool join_muted, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLReserveNextContext(olapi::OL_DURATION_USER_SESSION);
	OLStatus status = mVoice->JoinChannel(context, channel_uri, channel_password, 
		(olapi::OLVoiceJoinChannelFlags) (join_muted ? olapi::OL_VOICE_PUSH_TO_TALK_MODE | olapi::OL_VOICE_JOIN_MUTED : 0));

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback, true);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::LeaveChannel(const char *channel_uri, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mVoice->LeaveChannel(context, channel_uri);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::CreateChannel(const char *channel_name, const char *channel_password, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mVoice->CreateChannel(context, channel_name, channel_password);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::DestroyChannel(const char *channel_uri, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mVoice->DestroyChannel(context, channel_uri);

	//destroy channel may be called with a NULL callback if we are cleaning up.  Don't register for a NULL callback.
	if (status == OL_SUCCESS && callback != NULL)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::StartVoiceMonitor(bool joinEchoChannel, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mVoice->StartVoiceMonitor(context, joinEchoChannel);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::StopVoiceMonitor( void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mVoice->StopVoiceMonitor(context);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::SetParticipantPlaybackLevel(const char *channel_uri, olapi::OLID participantId, int level, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mVoice->SetParticipantPlaybackLevel(context, channel_uri, participantId, level);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::SetParticipantPlaybackMuteStatus( const char *channel_uri, olapi::OLID participantId, bool mute, void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mVoice->SetParticipantPlaybackMuteStatus(context, channel_uri, participantId, mute);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::SetSpeakingIndicatorStatus( bool visible )
{
	if (!mVoice) return olapi::OL_SERVICES_NOT_STARTED;

	OLStatus status = mVoice->SetSpeakingIndicatorStatus(visible);

	return status;
}


///////////////////////////////////////////////////////////////////////////////

OnLiveVoice::VoiceEventHandler::VoiceEventHandler() 
{

}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::VoiceEventHandler::StatusResult(OLContext context, OLStatus status)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchStatusResult(context, status))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveVoice::VoiceEventHandler::ContainerIDResult(olapi::OLContext context, olapi::OLID id)
{
	bool status = OnLiveContextEventDispatcher::getInstance().DispatchContainerIDResult(context, id);

	OnLiveService::getInstance().mService->DestroyContainer(id);

	if (status == false)
		return olapi::OL_FAIL;
	else
		return olapi::OL_SUCCESS;
}

olapi::OLStatus OnLiveVoice::VoiceEventHandler::ChannelJoinedEvent(olapi::OLContext context, OLID channelOwner, const char *channelURI, const char *channelName)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchChannelJoinedEvent(context, channelOwner, channelURI, channelName))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

olapi::OLStatus OnLiveVoice::VoiceEventHandler::ChannelExitedEvent(OLContext context, OLID channelOwner, const char *channelURI, const char *channelName)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchChannelExitedEvent(context, channelOwner, channelURI, channelName))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

olapi::OLStatus OnLiveVoice::VoiceEventHandler::ParticipantEvent(OLContext context, OLID participantID, const char *participantTag, OLVoiceParticipantEventType type)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchParticipantEvent(context, participantID, participantTag, type))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

olapi::OLStatus OnLiveVoice::VoiceEventHandler::VoiceMonitorUpdate(OLContext context, bool voiceDetected, double energy, int smoothedEnergy)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchVoiceMonitorEvent(context, voiceDetected, energy, smoothedEnergy))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

olapi::OLStatus OnLiveVoice::VoiceEventHandler::VoiceChatUserStatus(olapi::OLVoiceUserStatusFlags flags)
{
	return onlive_function::voice_chat_user_status(flags);
}

///////////////////////////////////////////////////////////////////////////////

OnLiveVoice::VoiceEventCallback::VoiceEventCallback()
{

}

///////////////////////////////////////////////////////////////////////////////

void OnLiveVoice::VoiceEventCallback::EventWaiting()
{
	OnLiveVoice::getInstance().mVoice->HandleEvent(0, 0);
}

