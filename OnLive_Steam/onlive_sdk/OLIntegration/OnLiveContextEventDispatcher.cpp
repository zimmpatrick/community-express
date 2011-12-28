///////////////////////////////////////////////////////////////////////////////
//
// OnLiveContextEventDispatcher.h $Revision: 56658 $
//
///////////////////////////////////////////////////////////////////////////////
//
// This class is used to catch and dispatch context-based event requests.
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

#include "OnLiveContextEventDispatcher.h"
#include "OnLiveIntegration.h"

///////////////////////////////////////////////////////////////////////////////
//                              OnLiveService                                //
///////////////////////////////////////////////////////////////////////////////

OnLiveContextEventDispatcher::OnLiveContextEventDispatcher()
{
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::RegisterContextEventCallback(olapi::OLContext context, void *pClassPtr, ContextEventCallback pCallbackFunction, bool reservedContext)
{
	if (pCallbackFunction == NULL)
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not add context %08x because of null callback", __FUNCTION__, context);
		return false;
	}

	ContextEvent contextEvent;

	contextEvent.pClassPtr = pClassPtr;
	contextEvent.pCallback = pCallbackFunction;
	contextEvent.reservedContext = reservedContext;

//	if (reservedContext == true)
//	{
//		LOG("OnLiveContextEventDispatcher:RegisterContextEventCallback:Using reserved context %08x", context);
//	}

	mContextEventsMap[context] = contextEvent;

	return true;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::RemoveDispatchContext(olapi::OLContext context)
{
	contextEventsMapType::iterator contextEventIterator = mContextEventsMap.find(context);
	if(contextEventIterator != mContextEventsMap.end())
	{
		mContextEventsMap.erase(context);
//		LOG("OnLiveContextEventDispatcher::RemoveDispatchContext::Erasing Dispatcher Context %08x", context);

		return true;
	}

	return false;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchContextEventResult(olapi::OLContext context, CallbackResult result)
{
	contextEventsMapType::iterator contextEventIterator = mContextEventsMap.find(context);
	if(contextEventIterator != mContextEventsMap.end())
	{
		//Stuff the context into the result for events that use it
		result.context = context;

		//Trigger the callback
		ContextEvent contextEvent = (*contextEventIterator).second;
		contextEvent.pCallback(contextEvent.pClassPtr, result);

		// read read the reserved flag incase it changed.
		contextEvent = (*contextEventIterator).second; 

		//Clear this callback
		if (contextEvent.reservedContext == false)
		{
			mContextEventsMap.erase(context);
//			LOG("OnLiveContextEventDispatcher::DispatchContextEventResult::Erasing Dispatcher Context %08x", context);
		}

		return true;
	}

	return false;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchApplicationLoginStateResult(olapi::OLContext context, olapi::OLLoginState loginState)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_LOGIN_STATE;
	result.loginState = loginState;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchUserGamePrivacyStatusResult(olapi::OLContext context, olapi::OLGameStatus privacyStatus)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_PRIVACY_STATUS;
	result.privacyStatus = privacyStatus;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////

#ifdef USE_OVERLAY
bool OnLiveContextEventDispatcher::DispatchMessageBoxResult(olapi::OLContext context, int buttons)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_BUTTONS;
	result.buttons = buttons;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}
#endif

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchDataListResult(olapi::OLContext context, olapi::OLDataList* dataList)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_DATA_LIST;
	result.dataList = dataList;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchContainerIDResult(olapi::OLContext context, olapi::OLID olid)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_OLID;
	result.id = olid;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchStatusResult(olapi::OLContext context, olapi::OLStatus status)
{
	CallbackResult result;
	result.status = status;
	result.validData = CallbackResult::DATA_TYPE_STATUS_RESULT;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchIDListResult(olapi::OLContext context, olapi::OLIDList *idList, olapi::OLID *containerListId)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_ID_LIST_RESULT;
	result.idList = idList;
	result.containerListId = containerListId;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

#ifdef USE_ACHIEVEMENT

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchAchievementUpdatedBoolResult(olapi::OLContext context, olapi::OLID achievementID)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_ACHIEVEMENT;
	result.achievementValue.achievementID = achievementID;
	result.achievementValue.achievementType = olapi::OL_AchievementType_Bool;
	result.achievementValue.bBoolVal = true;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchAchievementUpdatedIntResult(olapi::OLContext context, olapi::OLID achievementID, unsigned int value, int prevValue)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_ACHIEVEMENT;
	result.achievementValue.achievementID = achievementID;
	result.achievementValue.achievementType = olapi::OL_AchievementType_Int;
	result.achievementValue.intVal = value;
	result.achievementValue.intPrevVal = prevValue;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchAchievementUpdatedOneOfSetResult(olapi::OLContext context, olapi::OLID achievementID, olapi::OLID value)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_ACHIEVEMENT;
	result.achievementValue.achievementID = achievementID;
	result.achievementValue.achievementType = olapi::OL_AchievementType_OneOfSet;
	result.achievementValue.oneOfSetVal = value;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveContextEventDispatcher::DispatchPointsResult(olapi::OLContext context, unsigned int points)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_POINTS;
	result.points = points;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

#endif

#ifdef USE_VOICE
bool OnLiveContextEventDispatcher::DispatchChannelJoinedEvent(olapi::OLContext context, olapi::OLID channelOwner, const char *channelURI, const char *channelName)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_CHANNEL_EVENT;
	result.channelEvent.channelOwner = channelOwner;
	result.channelEvent.channelURI = channelURI;
	result.channelEvent.channelName = channelName;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

bool OnLiveContextEventDispatcher::DispatchChannelExitedEvent(olapi::OLContext context, olapi::OLID channelOwner, const char *channelURI, const char *channelName)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_CHANNEL_EVENT;
	result.channelEvent.channelOwner = channelOwner;
	result.channelEvent.channelURI = channelURI;
	result.channelEvent.channelName = channelName;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

bool OnLiveContextEventDispatcher::DispatchParticipantEvent(olapi::OLContext context, olapi::OLID participantID, const char *participantTag, olapi::OLVoiceParticipantEventType type)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_PARTICIPANT_EVENT;
	result.participantEvent.participantID = participantID;
	result.participantEvent.participantTag = participantTag;
	result.participantEvent.type = type;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;
}

bool OnLiveContextEventDispatcher::DispatchVoiceMonitorEvent(olapi::OLContext context, bool voiceDetected, double energy, int smoothedEnergy)
{
	CallbackResult result;
	result.status = olapi::OL_SUCCESS;
	result.validData = CallbackResult::DATA_TYPE_VOICE_MONITOR_EVENT;
	result.voiceMonitorEvent.voiceDetected = voiceDetected;
	result.voiceMonitorEvent.energy = energy;
	result.voiceMonitorEvent.smoothedEnergy = smoothedEnergy;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;

}
#endif

#ifdef USE_LEADERBOARD
bool OnLiveContextEventDispatcher::DispatchLeaderboardRowRetrieved(olapi::OLContext context, olapi::OLID boardID, olapi::OLID userID, const char * tag, unsigned int rowRank, unsigned int rowIndex, olapi::OLIDList *statContainerIDList)
{
	CallbackResult result;
	result.validData = CallbackResult::DATA_TYPE_LB_ROW;
	result.leaderboardRow.boardID = boardID;
	result.leaderboardRow.userID = userID;
	result.leaderboardRow.tag = tag;
	result.leaderboardRow.rowRank = rowRank;
	result.leaderboardRow.rowIndex = rowIndex;
	result.idList = statContainerIDList;

	if (!DispatchContextEventResult(context, result))
	{
		LOG("OnLiveContextEventDispatcher:%s:Could not find context %08x", __FUNCTION__, context);
		return false;
	}

	return true;

}
#endif

#define CASE_TO_STRING(x)	case x:		return (#x)

const char* OnLiveContextEventDispatcher::OnLiveDispatcherDataTypeToString(CallbackResult::dataType type)
{
	switch(type)
	{
		CASE_TO_STRING(CallbackResult::DATA_TYPE_STATUS_RESULT);
		CASE_TO_STRING(CallbackResult::DATA_TYPE_OLID);
		CASE_TO_STRING(CallbackResult::DATA_TYPE_LOGIN_STATE);
		CASE_TO_STRING(CallbackResult::DATA_TYPE_PRIVACY_STATUS);
		CASE_TO_STRING(CallbackResult::DATA_TYPE_DATA_LIST);
#ifdef USE_OVERLAY
		CASE_TO_STRING(CallbackResult::DATA_TYPE_BUTTONS);
#endif
		CASE_TO_STRING(CallbackResult::DATA_TYPE_ID_LIST_RESULT);
#ifdef USE_ACHIEVEMENT
		CASE_TO_STRING(CallbackResult::DATA_TYPE_ACHIEVEMENT);
		CASE_TO_STRING(CallbackResult::DATA_TYPE_POINTS);
#endif
#ifdef USE_VOICE
		CASE_TO_STRING(CallbackResult::DATA_TYPE_CHANNEL_EVENT);
		CASE_TO_STRING(CallbackResult::DATA_TYPE_PARTICIPANT_EVENT);
		CASE_TO_STRING(CallbackResult::DATA_TYPE_VOICE_MONITOR_EVENT);
#endif
#ifdef USE_LEADERBOARD
		CASE_TO_STRING(CallbackResult::DATA_TYPE_LB_ROW);
#endif
		default:								return "Unknown Dispatacher Data Type";
	}
}
