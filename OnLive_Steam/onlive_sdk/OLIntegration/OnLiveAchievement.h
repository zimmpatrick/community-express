///////////////////////////////////////////////////////////////////////////////
//
// OnLiveAchievement.h $Revision: 0 $
//
// SDK Integrated
//
///////////////////////////////////////////////////////////////////////////////
// 
// This class is used as a basic starting point for integrating OnLive Achievement into your
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

#ifndef __ONLIVE_ACHIEVEMENT_H__
#define __ONLIVE_ACHIEVEMENT_H__

#include "OLAPI.h"
#include "OLUser.h"
#include "OLAchievement.h"

#include "OnLiveContextEventDispatcher.h"

#include "TitleAchievementDefs.h"

#include <vector>

///////////////////////////////////////////////////////////////////////////////
//                              OnLive Achievement                           //
///////////////////////////////////////////////////////////////////////////////

class OnLiveAchievement
{
public:
	static OnLiveAchievement& getInstance()
	{
		static OnLiveAchievement instance;
		return instance;
	}

	OnLiveAchievement();

	void init();
	void deinit();

	olapi::OLStatus LookupAchievementID(olapi::TitleAchievements achievementIndex, olapi::OLID *achievementID);
	olapi::OLStatus LookupAchievementIndex(olapi::OLID achievementID, olapi::TitleAchievements *achievementIndex);
	olapi::OLStatus LookupAchievementOneOfSetID(olapi::TitleAchievements achievementIndex, olapi::TitleAchievements_OneOfSetItems achievementOneOfSetIndex, olapi::OLID *achievementOneOfSetID );
	olapi::OLStatus LookupAchievementOneOfSetID(olapi::OLID achievementID, olapi::TitleAchievements_OneOfSetItems achievementOneOfSetIndex, olapi::OLID *achievementOneOfSetID );
	olapi::OLStatus LookupAchievementOneOfSetIndex(olapi::TitleAchievements achievementIndex, olapi::OLID achievementOneOfSetID, olapi::TitleAchievements_OneOfSetItems *achievementOneOfSetIndex );
	olapi::OLStatus LookupAchievementOneOfSetIndex(olapi::OLID achievementID, olapi::OLID achievementOneOfSetID, olapi::TitleAchievements_OneOfSetItems *achievementOneOfSetIndex );
	
	olapi::OLStatus SetAchievementBool(olapi::TitleAchievements achievementIndex,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus SetAchievementInt(olapi::TitleAchievements achievementIndex,
		unsigned int value,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus SetAchievementOneOfSet(olapi::TitleAchievements achievementIndex,
		olapi::TitleAchievements_OneOfSetItems value,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus SetAchievementBool(olapi::OLID achievementID,
		olapi::OLID userID,
		olapi::OLID titleID,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus SetAchievementInt(olapi::OLID achievementID,
		unsigned int value,
		olapi::OLID userID,
		olapi::OLID titleID,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus SetAchievementOneOfSet(olapi::OLID achievementID,
		olapi::OLID value,
		olapi::OLID userID,
		olapi::OLID titleID,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus GetAchievementDefContainerIDList(olapi::OLID titleID,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus GetAchievementMediaIconContainerIDList(olapi::OLID titleID,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);
	olapi::OLStatus GetAchievementMediaVideoContainerIDList(olapi::OLID titleID,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus GetAchievementsContainerIDList(olapi::OLID userID,
		olapi::OLID titleID,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus GetPointsForTitle(olapi::OLID userID,
		olapi::OLID titleID,
		void *pClassPtr,
		OnLiveContextEventDispatcher::ContextEventCallback callback);

private:
	olapi::OLAchievement * mAchievement;

	class AchievementEventHandler : public olapi::OLAchievementEventHandler
	{
	friend class OnLiveAchievement;

	public:
		AchievementEventHandler();

		// Inherited functions
		virtual olapi::OLStatus StatusResult(olapi::OLContext context, olapi::OLStatus status);
		virtual olapi::OLStatus ContainerIDResult(olapi::OLContext context, olapi::OLID id);
		virtual olapi::OLStatus IDListResult(olapi::OLContext context, olapi::OLIDList *idlist, olapi::OLID *containerlistid);
		virtual olapi::OLStatus PointsResult(olapi::OLContext context, unsigned int points);
		virtual olapi::OLStatus AchievementUpdatedBool(olapi::OLContext context, olapi::OLID achievementID);
		virtual olapi::OLStatus AchievementUpdatedInt(olapi::OLContext context, olapi::OLID achievementID, unsigned int value, int prevValue);
		virtual olapi::OLStatus AchievementUpdatedOneOfSet(olapi::OLContext context, olapi::OLID achievementID, olapi::OLID value);

	private:
	};

	class AchievementEventCallback : public olapi::OLAchievementCallbacks
	{
	public:
		AchievementEventCallback();

		void EventWaiting();

	private:
		// No assignment operator
		AchievementEventCallback& operator=(AchievementEventCallback const&) {}
	};

	AchievementEventHandler * mAchievementEventHandler;
	AchievementEventCallback* mAchievementEventCallback;

	struct AchievementDefinition
	{
		olapi::OLID		ID;
		unsigned int	sequenceNum;

		std::vector<olapi::OLID> oneOfSetIDs;
	};
	static bool AchievementDefinitionCompare (const AchievementDefinition &a, const AchievementDefinition &b) { return (a.sequenceNum > b.sequenceNum); }
	std::vector<AchievementDefinition>	mAchievementDefs;

	void setupAchievementDefs();
};

#endif // __ONLIVE_ACHIEVEMENT_H__
