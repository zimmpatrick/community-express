///////////////////////////////////////////////////////////////////////////////
//
//  OnLiveAchievement.cpp $Revision: 0 $
//
///////////////////////////////////////////////////////////////////////////////
//
// This class is used as a starting point for integrating the OnLive Achievement SDK into
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

#include "OnLiveAchievement.h"
#include "OnLiveFunctions.h"
#include "OnLiveIntegration.h"

using namespace olapi;

///////////////////////////////////////////////////////////////////////////////
//                              OnLiveAchievement                            //
///////////////////////////////////////////////////////////////////////////////

OnLiveAchievement::OnLiveAchievement()
	: mAchievementEventHandler(NULL)
	, mAchievementEventCallback(NULL)
{

}

///////////////////////////////////////////////////////////////////////////////

void OnLiveAchievement::init()
{
	OLStatus status = olapi::OLGetAchievement(OLAPI_VERSION, &mAchievement);

	if (status == OL_SUCCESS)
	{
		mAchievementEventHandler = new AchievementEventHandler();
		mAchievement->SetEventHandler(mAchievementEventHandler);

		mAchievementEventCallback = new AchievementEventCallback();
		mAchievement->SetEventWaitingCallback(mAchievementEventCallback);

		setupAchievementDefs();

		LOG( "OnLiveAchievement::initialized.");

		return;
	}

	onlive_function::exit_game(true);
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveAchievement::deinit()
{
	if (mAchievement)
	{
		olapi::OLStopAchievement(mAchievement);
		LOG( "OnLiveAchievement::API stopped.");
	}

	if (mAchievementEventHandler)
	{
		delete mAchievementEventHandler;
	}

	if (mAchievementEventCallback)
	{
		delete mAchievementEventCallback;
	}

	mAchievementEventHandler = NULL;
	mAchievementEventCallback = NULL;

	LOG( "OnLiveAchievement::deinit.");
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::LookupAchievementID(olapi::TitleAchievements achievementIndex, olapi::OLID *achievementID)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	if(achievementIndex <= 0 || mAchievementDefs.size() <= (unsigned int) achievementIndex)
		return olapi::OL_INVALID_PARAM;

	if(!achievementID)
		return olapi::OL_INVALID_PARAM;

	*achievementID = mAchievementDefs[achievementIndex].ID;

	return olapi::OL_SUCCESS;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::LookupAchievementIndex(olapi::OLID achievementID, olapi::TitleAchievements *achievementIndex)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	if(!achievementIndex)
		return olapi::OL_INVALID_PARAM;

	for(int i = 0; i < (int) mAchievementDefs.size(); i++)
	{
		if(achievementID == mAchievementDefs[i].ID)
		{
			*achievementIndex = (olapi::TitleAchievements) i;
			return olapi::OL_SUCCESS;
		}
	}

	return olapi::OL_IDENTIFIER_NOT_FOUND;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::LookupAchievementOneOfSetID(olapi::TitleAchievements achievementIndex, olapi::TitleAchievements_OneOfSetItems achievementOneOfSetIndex, olapi::OLID *achievementOneOfSetID )
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	if(achievementIndex <= 0 || mAchievementDefs.size() <= (unsigned int) achievementIndex)
		return olapi::OL_INVALID_PARAM;

	if(!achievementOneOfSetID)
		return olapi::OL_INVALID_PARAM;

	AchievementDefinition achievementDefinition = mAchievementDefs[achievementIndex];
	if(achievementOneOfSetIndex <= 0 || achievementDefinition.oneOfSetIDs.size() <= (unsigned int) achievementOneOfSetIndex)
		return olapi::OL_INVALID_PARAM;

	*achievementOneOfSetID = achievementDefinition.oneOfSetIDs[achievementOneOfSetIndex];

	return olapi::OL_SUCCESS;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::LookupAchievementOneOfSetID(olapi::OLID achievementID, olapi::TitleAchievements_OneOfSetItems achievementOneOfSetIndex, olapi::OLID *achievementOneOfSetID )
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	if(!achievementOneOfSetID)
		return olapi::OL_INVALID_PARAM;

	olapi::TitleAchievements achievementIndex;
	olapi::OLStatus error = LookupAchievementIndex(achievementID, &achievementIndex);
	if(error != olapi::OL_SUCCESS)
		return error;

	return LookupAchievementOneOfSetID(achievementIndex, achievementOneOfSetIndex, achievementOneOfSetID);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::LookupAchievementOneOfSetIndex(olapi::TitleAchievements achievementIndex, olapi::OLID achievementOneOfSetID, olapi::TitleAchievements_OneOfSetItems *achievementOneOfSetIndex )
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	if(achievementIndex <= 0 || mAchievementDefs.size() <= (unsigned int) achievementIndex)
		return olapi::OL_INVALID_PARAM;

	if(!achievementOneOfSetIndex)
		return olapi::OL_INVALID_PARAM;

	AchievementDefinition achievementDefinition = mAchievementDefs[achievementIndex];

	for(int i = 0; i < (int) achievementDefinition.oneOfSetIDs.size(); i++)
	{
		if( achievementDefinition.oneOfSetIDs[i] == achievementOneOfSetID )
		{
			*achievementOneOfSetIndex = (olapi::TitleAchievements_OneOfSetItems) i;
			return olapi::OL_SUCCESS;
		}
	}

	return olapi::OL_IDENTIFIER_NOT_FOUND;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::LookupAchievementOneOfSetIndex(olapi::OLID achievementID, olapi::OLID achievementOneOfSetID, olapi::TitleAchievements_OneOfSetItems *achievementOneOfSetIndex )
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	if(!achievementOneOfSetIndex)
		return olapi::OL_INVALID_PARAM;

	olapi::TitleAchievements achievementIndex;
	olapi::OLStatus error = LookupAchievementIndex(achievementID, &achievementIndex);
	if(error != olapi::OL_SUCCESS)
		return error;

	return LookupAchievementOneOfSetIndex(achievementIndex, achievementOneOfSetID, achievementOneOfSetIndex);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::SetAchievementBool(olapi::TitleAchievements achievementIndex,
													  void *pClassPtr,
													  OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	olapi::OLStatus status;

	if(achievementIndex <= 0 || mAchievementDefs.size() <= (unsigned int) achievementIndex)
		return olapi::OL_INVALID_PARAM;

	olapi::OLID userID = OnLiveApplication::getInstance().getClientUserID();

	olapi::OLID titleID;
	status = OLGetTitleID(&titleID);
	if(status != olapi::OL_SUCCESS)
		return status;

	return SetAchievementBool(mAchievementDefs[achievementIndex].ID, userID, titleID, pClassPtr, callback);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::SetAchievementInt(olapi::TitleAchievements achievementIndex,
													 unsigned int value,
													 void *pClassPtr,
													 OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	if(achievementIndex <= 0 || mAchievementDefs.size() <= (unsigned int) achievementIndex)
		return olapi::OL_INVALID_PARAM;

	olapi::OLID userID = OnLiveApplication::getInstance().getClientUserID();

	olapi::OLID titleID;
	olapi::OLStatus status = OLGetTitleID(&titleID);
	if(status != olapi::OL_SUCCESS)
		return status;

	return SetAchievementInt(mAchievementDefs[achievementIndex].ID, value, userID, titleID, pClassPtr, callback);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::SetAchievementOneOfSet(olapi::TitleAchievements achievementIndex,
														  olapi::TitleAchievements_OneOfSetItems value,
														  void *pClassPtr,
														  OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	if(achievementIndex <= 0 || mAchievementDefs.size() <= (unsigned int) achievementIndex)
		return olapi::OL_INVALID_PARAM;

	if(value <= 0 || mAchievementDefs[achievementIndex].oneOfSetIDs.size() <= (unsigned int) value)
		return olapi::OL_INVALID_PARAM;

	olapi::OLID userID = OnLiveApplication::getInstance().getClientUserID();

	olapi::OLID titleID;
	olapi::OLStatus status = OLGetTitleID(&titleID);
	if(status != olapi::OL_SUCCESS)
		return status;

	return SetAchievementOneOfSet(mAchievementDefs[achievementIndex].ID, mAchievementDefs[achievementIndex].oneOfSetIDs[value], userID, titleID, pClassPtr, callback);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::SetAchievementBool(olapi::OLID achievementID,
													  olapi::OLID userID,
													  olapi::OLID titleID,
													  void *pClassPtr,
													  OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mAchievement->SetAchievementBool(context, achievementID, userID, titleID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::SetAchievementInt(olapi::OLID achievementID,
													 unsigned int value,
													 olapi::OLID userID,
													 olapi::OLID titleID,
													 void *pClassPtr,
													 OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mAchievement->SetAchievementInt(context, achievementID, value, userID, titleID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::SetAchievementOneOfSet(olapi::OLID achievementID,
														  olapi::OLID value,
														  olapi::OLID userID,
														  olapi::OLID titleID,
														  void *pClassPtr,
														  OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mAchievement->SetAchievementOneOfSet(context, achievementID, value, userID, titleID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::GetAchievementDefContainerIDList(olapi::OLID titleID,
																	void *pClassPtr,
																	OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mAchievement->GetAchievementDefContainerIDList(context, titleID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::GetAchievementMediaIconContainerIDList(olapi::OLID titleID,
																		  void *pClassPtr,
																		  OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mAchievement->GetAchievementMediaIconContainerIDList(context, titleID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::GetAchievementMediaVideoContainerIDList(olapi::OLID titleID,
																		   void *pClassPtr,
																		   OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mAchievement->GetAchievementMediaVideoContainerIDList(context, titleID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::GetAchievementsContainerIDList(olapi::OLID userID,
																  olapi::OLID titleID,
																  void *pClassPtr,
																  OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mAchievement->GetAchievementsContainerIDList(context, userID, titleID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::GetPointsForTitle(olapi::OLID userID,
													 olapi::OLID titleID,
													 void *pClassPtr,
													 OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mAchievement) return olapi::OL_SERVICES_NOT_STARTED;

	OLContext context = olapi::OLGetNextContext();
	OLStatus status = mAchievement->GetPointsForTitle(context, userID, titleID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

OnLiveAchievement::AchievementEventHandler::AchievementEventHandler() 
{
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::AchievementEventHandler::StatusResult(OLContext context, OLStatus status)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchStatusResult(context, status))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::AchievementEventHandler::ContainerIDResult(olapi::OLContext context, olapi::OLID id)
{
	UNUSED_PARAM(context);
	UNUSED_PARAM(id);
	return olapi::OL_NOT_IMPLEMENTED;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::AchievementEventHandler::IDListResult(olapi::OLContext context, olapi::OLIDList *idlist, olapi::OLID *containerlistid)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchIDListResult(context, idlist, containerlistid))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::AchievementEventHandler::PointsResult(OLContext context, unsigned int points)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchPointsResult(context, points))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::AchievementEventHandler::AchievementUpdatedBool(OLContext context, OLID achievementID)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchAchievementUpdatedBoolResult(context, achievementID))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::AchievementEventHandler::AchievementUpdatedInt(OLContext context, OLID achievementID, unsigned int value, int prevValue)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchAchievementUpdatedIntResult(context, achievementID, value, prevValue))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveAchievement::AchievementEventHandler::AchievementUpdatedOneOfSet(OLContext context, OLID achievementID, OLID value)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchAchievementUpdatedOneOfSetResult(context, achievementID, value))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

OnLiveAchievement::AchievementEventCallback::AchievementEventCallback()
{

}

///////////////////////////////////////////////////////////////////////////////

void OnLiveAchievement::AchievementEventCallback::EventWaiting()
{
	OnLiveAchievement::getInstance().mAchievement->HandleEvent(0, 0);
}

