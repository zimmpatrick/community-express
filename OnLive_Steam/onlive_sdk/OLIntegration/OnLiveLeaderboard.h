///////////////////////////////////////////////////////////////////////////////
//
// OnLiveLeaderboard.h $Revision: 0 $
//
// SDK Integrated
//
///////////////////////////////////////////////////////////////////////////////
// 
// This class is used as a basic starting point for integrating OnLive Leaderboard into your
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

#ifndef __ONLIVE_LEADERBOARD_H__
#define __ONLIVE_LEADERBOARD_H__

#include "OLAPI.h"
#include "OLUser.h"
#include "OLLeaderboard.h"

#include "OnLiveContextEventDispatcher.h"

///////////////////////////////////////////////////////////////////////////////
//                              OnLive Stats                                 //
///////////////////////////////////////////////////////////////////////////////

class OnLiveStat
{
public:
	OnLiveStat();
	OnLiveStat(const OnLiveStat &other);

	virtual OnLiveStat & operator= (const OnLiveStat &other);

	virtual ~OnLiveStat();

	virtual olapi::LeaderboardStatType GetType() = 0;

	virtual olapi::OLStatus addFlags( const olapi::LeaderboardSetStatFlags new_flag);
	virtual olapi::LeaderboardSetStatFlags getFlags() const;

	virtual olapi::OLStatus setID( const olapi::OLID& statID);
	virtual olapi::OLID getID() const;

	virtual olapi::OLID getContainer();
	inline bool fail() 
	{return m_fatal;}

protected:
	OnLiveStat(const olapi::LeaderboardStatType type);
	OnLiveStat(const olapi::LeaderboardStatType type, const olapi::OLID& statID);
	OnLiveStat(const olapi::LeaderboardStatType type, const olapi::OLID& statID, const olapi::LeaderboardSetStatFlags flags);

	virtual olapi::OLStatus initContainer();
	virtual olapi::OLStatus setContainer(const olapi::OLID& externalContainerID);
	virtual olapi::OLStatus freeContainer();
	virtual olapi::OLStatus setStatType(const olapi::LeaderboardStatType type);
	
	bool m_fatal;
	olapi::OLID m_containerID;
	int * m_refcount;
};

class OnLiveIntStat : public OnLiveStat
{
public:
	OnLiveIntStat();
	OnLiveIntStat(const olapi::OLID& containerID);
	OnLiveIntStat(const olapi::OLID& statID, const __int64 value);
	OnLiveIntStat(const olapi::OLID& statID, const __int64 value, const olapi::LeaderboardSetStatFlags flags );

	virtual olapi::LeaderboardStatType GetType()
	{ return olapi::OL_LB_StatType_Integer; }

	inline olapi::OLStatus setValue( const __int64 value );
	inline __int64 getValue() const;
};

class OnLiveDoubleStat : public OnLiveStat
{
public:
	OnLiveDoubleStat();
	OnLiveDoubleStat(const olapi::OLID &containerID);
	OnLiveDoubleStat(const olapi::OLID &statID, const double value);
	OnLiveDoubleStat(const olapi::OLID &statID, const double value, const olapi::LeaderboardSetStatFlags flags );

	virtual olapi::LeaderboardStatType GetType()
	{ return olapi::OL_LB_StatType_FloatingPoint; }

	inline olapi::OLStatus setValue( const double value );
	inline double getValue() const;
};

///////////////////////////////////////////////////////////////////////////////
//                              OnLive Leaderboard                           //
///////////////////////////////////////////////////////////////////////////////

class OnLiveLeaderboard
{
public:
	static OnLiveLeaderboard& getInstance()
	{
		static OnLiveLeaderboard instance;
		return instance;
	}

	OnLiveLeaderboard();

	void init();
	void deinit();

	olapi::OLStatus SetStat( OnLiveStat *value,
							 void *pClassPtr,
							 OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus GetLeaderboardDefs( olapi::OLID boardID,
										void *pClassPtr,
										OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus GetLeaderboardStatDefs( olapi::OLID statID,
											void *pClassPtr,
											OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus GetLeaderboardRowsByIndex( olapi::OLID boardID,
												unsigned int startRow, 
												unsigned int endRow, 
												olapi::LeaderboardQueryFlags flags,
												void *pClassPtr,
												OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus GetLeaderboardRowsByUser( olapi::OLID boardID, 
											  olapi::OLID referenceUser,
											  unsigned int rowsAbove, 
											  unsigned int rowsBelow, 
											  olapi::LeaderboardQueryFlags flags,
											  void *pClassPtr,
											  OnLiveContextEventDispatcher::ContextEventCallback callback);


	olapi::OLStatus GetLeaderboardRowForUser(olapi::OLID boardID,
											 olapi::OLID userID,
											 void *pClassPtr,
											 OnLiveContextEventDispatcher::ContextEventCallback callback);

	olapi::OLStatus GetLeaderboardStatistic(olapi::OLID userID,
											olapi::OLID statID,
											void *pClassPtr,
											OnLiveContextEventDispatcher::ContextEventCallback callback);

private:
	olapi::OLLeaderboard * mLeaderboard;

	class LeaderboardEventHandler : public olapi::OLLeaderboardEventHandler
	{
	friend class OnLiveLeaderboard;

	public:
		LeaderboardEventHandler();

		// Inherited functions
		virtual olapi::OLStatus StatusResult(olapi::OLContext context, olapi::OLStatus status);
		virtual olapi::OLStatus ContainerIDResult(olapi::OLContext context, olapi::OLID id);
		virtual olapi::OLStatus IDListResult(olapi::OLContext context, olapi::OLIDList *idlist, olapi::OLID *containerlistid);
		virtual olapi::OLStatus RowRetrieved(olapi::OLContext context, 
											 olapi::OLID boardID, 
											 olapi::OLID userID, 
											 const char * tag, 
											 unsigned int rowRank, 
											 unsigned int rowIndex, 
											 olapi::OLIDList *statContainerIDList);
	};

	class LeaderboardEventCallback : public olapi::OLLeaderboardCallbacks
	{
	public:
		LeaderboardEventCallback();

		void EventWaiting();

	private:
		// No assignment operator
		LeaderboardEventCallback& operator=(LeaderboardEventCallback const&) {}
	};

	LeaderboardEventHandler * mLeaderboardEventHandler;
	LeaderboardEventCallback* mLeaderboardEventCallback;
};

#endif // __ONLIVE_LEADERBOARD_H__
