///////////////////////////////////////////////////////////////////////////////
//
// OnLiveContextEventDispatcher.h $Revision: 57033 $
//
// SDK Integrated
//
///////////////////////////////////////////////////////////////////////////////
// 
// This class is used to catch and dispatch context-based event requests.
//
// This class is integrated into OLIntegration in order to allow an application
// to use context-based event callbacks without modifying OLIntegration code.
// When an application makes a call to OLIntegration that requires an event-based
// result, the application passes in a callback function that will receive the result.
// OLIntegration then registers this callback with the dispatcher using the context
// of the call.  When OLIntegration receives the event with the result, it calls
// the dispatcher which then resolves the callback and gives the result to the application.
//
// NOTE: The application can use either C-Style or C++-style callbacks.
//      If a C++-style callback is desired, then the app should create a static member
//      fuction for the callback and pass the this pointer as pClassPtr.  When the static member
//      function is called, pClassPtr can be cast to the appropriate this pointer to resolve
//      the callback.
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

#ifndef __ONLIVE_CONTEXT_EVENT_DISPATCHER_H__
#define __ONLIVE_CONTEXT_EVENT_DISPATCHER_H__

#include "OLAPI.h"
#include "OLTypes.h"
#include "OLApplication.h"
#include "OLService.h"
#include "OLUser.h"
#ifdef USE_OVERLAY
	#include "OLOverlay.h"
#endif
#ifdef USE_ACHIEVEMENT
	#include "OLAchievement.h"
#endif
#ifdef USE_VOICE
	#include "OLVoice.h"
#endif

#pragma warning(disable: 4548)	// Disable warning C4548: expression before comma has no effect; expected expression with side-effect


#include <map>

#ifdef USE_VOICE
	typedef struct VoiceChannelEvent
	{
		olapi::OLID	channelOwner;					// owner of the channel
		const char *channelURI;						// vivox uri
		const char *channelName;					// channel name
	} _VoiceChannelEvent;

	typedef struct VoiceParticipantEvent
	{
		olapi::OLID	participantID;					// if of user
		const char *participantTag;					// tag of user
		olapi::OLVoiceParticipantEventType type;	// event type
	} _VoiceParticipantEvent;

	typedef struct VoiceMonitorEvent
	{
		bool voiceDetected;							// True if someone is talking
		double energy;								// Direct energy value
		int smoothedEnergy;							// Smoothed enerygy value
	} _VoiceMonitorEvent;
#endif

///////////////////////////////////////////////////////////////////////////////
//                     OnLive Context Event Dispatcher                       //
///////////////////////////////////////////////////////////////////////////////
class OnLiveContextEventDispatcher
{
public:
	//Callback function definitions
	struct CallbackResult
	{
		olapi::OLStatus					status;
		olapi::OLContext				context;

		enum dataType
		{
			DATA_TYPE_STATUS_RESULT,		//valid variables: status result
			DATA_TYPE_OLID,					//valid variables: id
			DATA_TYPE_LOGIN_STATE,			//valid variables: loginState
			DATA_TYPE_PRIVACY_STATUS,		//valid variables: privacyStatus
			DATA_TYPE_DATA_LIST,			//valid variables: dataList
#ifdef USE_OVERLAY
			DATA_TYPE_BUTTONS,				//valid variables: buttons
#endif
			DATA_TYPE_ID_LIST_RESULT,		//valid variables: idList, containerListId
#ifdef USE_ACHIEVEMENT
			DATA_TYPE_ACHIEVEMENT,			//valid variables: achievementValue
			DATA_TYPE_POINTS,				//valid variables: points
#endif
#ifdef USE_VOICE
			DATA_TYPE_CHANNEL_EVENT,
			DATA_TYPE_PARTICIPANT_EVENT,
			DATA_TYPE_VOICE_MONITOR_EVENT,
#endif
#ifdef USE_LEADERBOARD
			DATA_TYPE_LB_ROW,			//valid variables: idList, leaderboardRow
#endif
		};

		dataType						validData;	// This denotes which data below is valid

		//The following variables are only valid if specified by validData
		olapi::OLID					id;
		olapi::OLLoginState			loginState;
		olapi::OLGameStatus			privacyStatus;
		olapi::OLDataList*			dataList;
		int							buttons;
		olapi::OLIDList				*idList;
		olapi::OLID					*containerListId;

#ifdef USE_ACHIEVEMENT
		unsigned int				points;

		struct achievementValue
		{
			olapi::OLID					achievementID;

			olapi::AchievementType		achievementType;

			bool						bBoolVal;
			unsigned int				intVal;
			int							intPrevVal;
			olapi::OLID					oneOfSetVal;

		}							achievementValue;
#endif
#ifdef USE_VOICE
		VoiceChannelEvent channelEvent; 
		VoiceParticipantEvent participantEvent;
		VoiceMonitorEvent voiceMonitorEvent;
#endif

#ifdef USE_LEADERBOARD
		struct leaderboardRow
		{
			olapi::OLID boardID; 
			olapi::OLID userID;
			const char * tag;
			unsigned int rowRank;
			unsigned int rowIndex;
		}							leaderboardRow;
#endif

	};
	typedef void (*ContextEventCallback)(void *pClassPtr, CallbackResult);

public:

	static OnLiveContextEventDispatcher& getInstance()
	{
		static OnLiveContextEventDispatcher instance;
		return instance;
	}

	OnLiveContextEventDispatcher();

	// Call this to register a callback for a context-based event request
	bool RegisterContextEventCallback(olapi::OLContext context, void *pClassPtr, ContextEventCallback pCallbackFunction, bool reservedContext = false);
	// Call this to remove a context
	bool RemoveDispatchContext(olapi::OLContext context);
	// When the event is received from a context-based request, call this to dispatch the result to a previously-registered callback
	bool DispatchContextEventResult(olapi::OLContext context, CallbackResult result);

	//These are helper functions to dispatch specific result types
	bool DispatchApplicationLoginStateResult(olapi::OLContext context, olapi::OLLoginState loginState);
	bool DispatchUserGamePrivacyStatusResult(olapi::OLContext context, olapi::OLGameStatus privacyStatus);
#ifdef USE_OVERLAY
	bool DispatchMessageBoxResult(olapi::OLContext context, int buttons);
#endif
	bool DispatchDataListResult(olapi::OLContext context, olapi::OLDataList* dataList);
	bool DispatchContainerIDResult(olapi::OLContext context, olapi::OLID olid);
	bool DispatchStatusResult(olapi::OLContext context, olapi::OLStatus status);
	bool DispatchIDListResult(olapi::OLContext context, olapi::OLIDList *idList, olapi::OLID *containerListId);

#ifdef USE_ACHIEVEMENT
	bool DispatchAchievementUpdatedBoolResult(olapi::OLContext context, olapi::OLID achievementID);
	bool DispatchAchievementUpdatedIntResult(olapi::OLContext context, olapi::OLID achievementID, unsigned int value, int prevValue);
	bool DispatchAchievementUpdatedOneOfSetResult(olapi::OLContext context, olapi::OLID achievementID, olapi::OLID value);
	bool DispatchPointsResult(olapi::OLContext context, unsigned int points);
#endif

#ifdef USE_VOICE
	bool DispatchChannelJoinedEvent(olapi::OLContext context, olapi::OLID channelOwner, const char *channelURI, const char *channelName);
	bool DispatchChannelExitedEvent(olapi::OLContext context, olapi::OLID channelOwner, const char *channelURI, const char *channelName);
	bool DispatchParticipantEvent(olapi::OLContext context, olapi::OLID participantID, const char *participantTag, olapi::OLVoiceParticipantEventType type);
	bool DispatchVoiceMonitorEvent(olapi::OLContext context, bool voiceDetected, double energy, int smoothedEnergy);
#endif

#ifdef USE_LEADERBOARD
	bool DispatchLeaderboardRowRetrieved(olapi::OLContext context, olapi::OLID boardID, olapi::OLID userID, const char * tag, unsigned int rowRank, unsigned int rowIndex, olapi::OLIDList *statContainerIDList);
#endif

	static const char* OnLiveDispatcherDataTypeToString(CallbackResult::dataType type);

protected:
private:
	struct ContextEvent
	{
		void						*pClassPtr;			//NOTE: For class-based callbacks, this should be the this pointer
		ContextEventCallback		pCallback;			//NOTE: For class-based callbacks, this should be a static member that resolves the this pointer
		bool						reservedContext;	// This tells the dispatcher to keep the context after a dispatch
	};

	typedef std::map<olapi::OLContext, ContextEvent>	contextEventsMapType;
	contextEventsMapType								mContextEventsMap;
};

#endif // __ONLIVE_CONTEXT_EVENT_DISPATCHER_H__
