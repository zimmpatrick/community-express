///////////////////////////////////////////////////////////////////////////////
//
//  OnLiveLeaderboard.cpp $Revision: 0 $
//
///////////////////////////////////////////////////////////////////////////////
//
// This class is used as a starting point for integrating the OnLive Leaderboard SDK into
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

#include "OnLiveLeaderboard.h"
#include "OnLiveFunctions.h"
#include "OnLiveIntegration.h"
#include "OLLeaderboard.h"

using namespace olapi;

///////////////////////////////////////////////////////////////////////////////
//                                OnLiveStat                                 //
///////////////////////////////////////////////////////////////////////////////

OnLiveStat::OnLiveStat() : m_fatal(false), m_refcount(NULL), m_containerID(NULL) 
{
	m_refcount = new int(-1);
}

OnLiveStat::OnLiveStat(const LeaderboardStatType type) : m_fatal(false)
{
	initContainer();
	setStatType(type);
}

OnLiveStat::OnLiveStat(const LeaderboardStatType type, const OLID& statID) : m_fatal(false)
{
	initContainer();
	setStatType(type);
	setID(statID);
}

OnLiveStat::OnLiveStat(const LeaderboardStatType type, const OLID& statID, const LeaderboardSetStatFlags flags) : m_fatal(false)
{
	initContainer();
	setStatType(type);
	setID(statID);
	addFlags(flags);
}

///////////////////////////////////////////////////////////////////////////////

OnLiveStat::OnLiveStat(const OnLiveStat &other)
{
	if(*(other.m_refcount) > 0)
	{
		++(*(other.m_refcount));
	}
	m_refcount = other.m_refcount;
	m_containerID = other.m_containerID;
	m_fatal = other.m_fatal;
}

OnLiveStat & OnLiveStat::operator= (const OnLiveStat &other)
{
	if (this == &other)
		return *this;
	--(*m_refcount);
	if( *m_refcount == 0)
	{
		freeContainer();
	}
	if(*(other.m_refcount) > 0)
	{
		++(*(other.m_refcount));
	}
	m_refcount = other.m_refcount;
	m_containerID = other.m_containerID;
	m_fatal = other.m_fatal;
	return *this;
}

OnLiveStat::~OnLiveStat()
{
	--(*m_refcount);
	if( *m_refcount == 0)
	{
		freeContainer();
	}
}

OLStatus OnLiveStat::freeContainer()
{
	OLStatus st = OL_SUCCESS;
	if ( !!m_containerID )
	{
		st = OnLiveService::getInstance().DestroyContainer(m_containerID);
		if ( st != OL_SUCCESS )
		{
			LOG( "OnLiveStat::~OnLiveStat(): Failed to delete container with error: ( %s )", OLStatusToString(st) );
			m_fatal = true;
		}
		else
		{
			m_containerID = OLID(NULL);
		}
	}
	delete m_refcount;
	m_refcount = new int(-1);
	return st;
}

OLStatus OnLiveStat::setContainer(const OLID& externalContainerID)
{
	--(*m_refcount);
	if( *m_refcount == 0)
	{
		freeContainer();
	}
	m_containerID = externalContainerID;
	return OL_SUCCESS;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveStat::initContainer()
{
	if( !!m_containerID )
		return OL_SUCCESS;
	m_containerID = OLID(NULL);
	m_refcount = NULL;
	OLStatus st = OnLiveService::getInstance().CreateContainer(&m_containerID);
	if ( st != OL_SUCCESS )
	{
		LOG( "OnLiveStat::initContainer() : Unable to create a container ( %s )", OLStatusToString(st) );
		m_fatal = true;
	}
	m_refcount = new int(1);
	return st;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveStat::addFlags( const LeaderboardSetStatFlags new_flags)
{
	if(m_fatal)
		return OL_INTERNAL_ERROR;
	LeaderboardSetStatFlags total_flags = (LeaderboardSetStatFlags) (new_flags | getFlags());
	OLStatus  st = OnLiveService::getInstance().SetContainerValueFromInt( m_containerID, LeaderboardStatContainerKeyNames[OLKEY_RO_LB_StatSetFlags], total_flags);
	if ( st != OL_SUCCESS )
	{
		LOG( "OnLiveStat::addFlags() : Unable to add flags %u. (%s)", new_flags, OLStatusToString(st) );
		m_fatal = true;
	}
	return st;
}

///////////////////////////////////////////////////////////////////////////////

LeaderboardSetStatFlags OnLiveStat::getFlags() const
{
	if(m_fatal)
		return OL_LB_SetStatFlags_None;
	__int64 flags = 0;
	OLStatus st = OnLiveService::getInstance().GetContainerValueAsInt( m_containerID, LeaderboardStatContainerKeyNames[OLKEY_RO_LB_StatSetFlags], &flags );
	if ( st != OL_SUCCESS && st != OL_IDENTIFIER_NOT_FOUND )
	{
		LOG( "OnLiveStat::getFlags() Error: %s)", OLStatusToString(st) );
	}
	return (LeaderboardSetStatFlags) flags;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveStat::setID( const OLID& statID)
{
	if(m_fatal)
		return OL_INTERNAL_ERROR;
	OLStatus st = OnLiveService::getInstance().SetContainerValueFromID( m_containerID, LeaderboardStatContainerKeyNames[OLKEY_RO_LB_StatID], const_cast<OLID&>(statID));
	if ( st != OL_SUCCESS )
	{
		char statid_string[OLID_STRING_SIZE];
		LOG( "OnLiveStat::setID() : Unable to set id to %s. (%s)", (const_cast<OLID&>(statID)).getIDString(statid_string), OLStatusToString(st) );
		m_fatal = true;
	}
	return st;
}

///////////////////////////////////////////////////////////////////////////////

OLID OnLiveStat::getID() const
{
	if(m_fatal)
		return OLID(NULL);
	OLID statID(NULL);
	OLStatus st = OnLiveService::getInstance().GetContainerValueAsID( m_containerID, LeaderboardStatContainerKeyNames[OLKEY_RO_LB_StatID], &statID, OL_STAT_ID );
	if ( st != OL_SUCCESS && st != OL_IDENTIFIER_NOT_FOUND )
	{
		LOG( "OnLiveStat::getID() Error: %s)", OLStatusToString(st) );
	}
	return statID;
}

OLStatus OnLiveStat::setStatType(const LeaderboardStatType type)
{
	if(m_fatal)
		return OL_INTERNAL_ERROR;
	OLStatus st = OnLiveService::getInstance().SetContainerValueFromInt( m_containerID, LeaderboardStatContainerKeyNames[OLKEY_RO_LB_StatType], type);
	if ( st != OL_SUCCESS )
	{
		LOG( "OnLiveStat::setStatType() : Unable to set type to %d. (%s)", type, OLStatusToString(st) );
		m_fatal = true;
	}
	return st;
}

///////////////////////////////////////////////////////////////////////////////

OLID OnLiveStat::getContainer()
{
	return m_containerID;
}

///////////////////////////////////////////////////////////////////////////////
//                                OnLiveIntStat                              //
///////////////////////////////////////////////////////////////////////////////

OnLiveIntStat::OnLiveIntStat() 
	: OnLiveStat(OL_LB_StatType_Integer) 
{}

OnLiveIntStat::OnLiveIntStat(const OLID& containerID) 
	: OnLiveStat() 
{
	setContainer(containerID);
}

OnLiveIntStat::OnLiveIntStat(const OLID& statID, const __int64 value) 
	: OnLiveStat(OL_LB_StatType_Integer, statID)
{
	setValue(value);
}

OnLiveIntStat::OnLiveIntStat(const OLID& statID, const __int64 value, const LeaderboardSetStatFlags flags ) 
	: OnLiveStat(OL_LB_StatType_Integer, statID, flags)
{
	setValue(value);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveIntStat::setValue( const __int64 value )
{
	if(m_fatal)
		return OL_INTERNAL_ERROR;
	OLStatus  st = OnLiveService::getInstance().SetContainerValueFromInt( m_containerID, LeaderboardStatContainerKeyNames[OLKEY_RO_LB_StatValue], value);
	if ( st != OL_SUCCESS )
	{
		LOG( "OnLiveIntStat::setValue() : Unable to set value %llu. (%s)", value, OLStatusToString(st) );
		m_fatal = true;
	}
	return st;
}

///////////////////////////////////////////////////////////////////////////////

__int64 OnLiveIntStat::getValue() const
{
	if(m_fatal)
		return 0;
	__int64 value;
	OLStatus st = OnLiveService::getInstance().GetContainerValueAsInt( m_containerID, LeaderboardStatContainerKeyNames[OLKEY_RO_LB_StatValue], &value );
	if ( st != OL_SUCCESS && st != OL_IDENTIFIER_NOT_FOUND )
	{
		LOG( "OnLiveIntStat::getValue() Error: %s)", OLStatusToString(st) );
	}
	return value;
}

///////////////////////////////////////////////////////////////////////////////
//                              OnLiveDoubleStat                             //
///////////////////////////////////////////////////////////////////////////////

OnLiveDoubleStat::OnLiveDoubleStat() 
	: OnLiveStat(OL_LB_StatType_FloatingPoint) 
{}

OnLiveDoubleStat::OnLiveDoubleStat(const OLID& containerID) 
	: OnLiveStat() 
{
	setContainer(containerID);
}

OnLiveDoubleStat::OnLiveDoubleStat(const OLID& statID, const double value) 
	: OnLiveStat(OL_LB_StatType_FloatingPoint, statID)
{
	setValue(value);
}

OnLiveDoubleStat::OnLiveDoubleStat(const OLID& statID, const double value, const LeaderboardSetStatFlags flags ) 
	: OnLiveStat(OL_LB_StatType_FloatingPoint, statID, flags)
{
	setValue(value);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveDoubleStat::setValue( const double value )
{
	if(m_fatal)
		return OL_INTERNAL_ERROR;
	OLStatus  st = OnLiveService::getInstance().SetContainerValueFromFloat( m_containerID, LeaderboardStatContainerKeyNames[OLKEY_RO_LB_StatValue], value);
	if ( st != OL_SUCCESS )
	{
		LOG( "OnLiveDoubleStat::setValue() : Unable to set value %f. (%s)", value, OLStatusToString(st) );
		m_fatal = true;
	}
	return st;
}

///////////////////////////////////////////////////////////////////////////////

double OnLiveDoubleStat::getValue() const
{
	if(m_fatal)
		return 0;
	double value;
	OLStatus st = OnLiveService::getInstance().GetContainerValueAsFloat( m_containerID, LeaderboardStatContainerKeyNames[OLKEY_RO_LB_StatValue], &value );
	if ( st != OL_SUCCESS && st != OL_IDENTIFIER_NOT_FOUND )
	{
		LOG( "OnLiveDoubleStat::getValue() Error: %s)", OLStatusToString(st) );
	}
	return value;
}

///////////////////////////////////////////////////////////////////////////////
//                              OnLiveLeaderboard                            //
///////////////////////////////////////////////////////////////////////////////

OnLiveLeaderboard::OnLiveLeaderboard()
	: mLeaderboardEventHandler(NULL)
	, mLeaderboardEventCallback(NULL)
{

}

///////////////////////////////////////////////////////////////////////////////

void OnLiveLeaderboard::init()
{
	OLStatus status = OLGetLeaderboard(OLAPI_VERSION, &mLeaderboard);

	if (status == OL_SUCCESS)
	{
		mLeaderboardEventHandler = new LeaderboardEventHandler();
		mLeaderboard->SetEventHandler(mLeaderboardEventHandler);

		mLeaderboardEventCallback = new LeaderboardEventCallback();
		mLeaderboard->SetEventWaitingCallback(mLeaderboardEventCallback);

		LOG( "OnLiveLeaderboard::initialized.");

		return;
	}

	onlive_function::exit_game(true);
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveLeaderboard::deinit()
{
	if (mLeaderboard)
	{
		OLStopLeaderboard(mLeaderboard);
		LOG( "OnLiveLeaderboard::API stopped.");
	}

	if (mLeaderboardEventHandler)
	{
		delete mLeaderboardEventHandler;
	}

	if (mLeaderboardEventCallback)
	{
		delete mLeaderboardEventCallback;
	}

	mLeaderboardEventHandler = NULL;
	mLeaderboardEventCallback = NULL;

	LOG( "OnLiveLeaderboard::deinit.");
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::SetStat(OnLiveStat *stat,
									void *pClassPtr,
									OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mLeaderboard) return OL_SERVICES_NOT_STARTED;

	OLContext context = OLGetNextContext();
	OLStatus status = mLeaderboard->SetLeaderboardStatistic(context, stat->getContainer());

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);
	return status;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::GetLeaderboardDefs(OLID boardID,
													  void *pClassPtr,
													  OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mLeaderboard) return OL_SERVICES_NOT_STARTED;

	OLContext context = OLGetNextContext();
	OLStatus status = mLeaderboard->GetLeaderboardDefs(context, OLID(NULL), boardID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::GetLeaderboardStatDefs(OLID statID,
														  void *pClassPtr,
														  OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mLeaderboard) return OL_SERVICES_NOT_STARTED;

	OLContext context = OLGetNextContext();
	OLStatus status = mLeaderboard->GetLeaderboardStatDefs(context, OLID(NULL), statID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::GetLeaderboardRowsByIndex(OLID boardID,
															 unsigned int startRow,
															 unsigned int endRow,
															 LeaderboardQueryFlags flags,
															 void *pClassPtr,
															 OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mLeaderboard) return OL_SERVICES_NOT_STARTED;

	OLContext context = OLReserveNextContext(olapi::OL_DURATION_AUTO);
	OLStatus status = mLeaderboard->GetLeaderboardRowsByIndex(context, OLID(NULL), boardID, startRow, endRow, flags, NULL);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback, true);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::GetLeaderboardRowsByUser(OLID boardID, 
															OLID referenceUser,
															unsigned int rowsAbove,
															unsigned int rowsBelow, 
															LeaderboardQueryFlags flags,
															void *pClassPtr,
															OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mLeaderboard) return OL_SERVICES_NOT_STARTED;

	OLContext context = OLReserveNextContext(olapi::OL_DURATION_AUTO);
	OLStatus status = mLeaderboard->GetLeaderboardRowsByUser(context, OLID(NULL), boardID, referenceUser, rowsAbove, rowsBelow, flags, NULL);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback, true);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::GetLeaderboardRowForUser(OLID boardID,
															OLID userID,
															void *pClassPtr,
															OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mLeaderboard) return OL_SERVICES_NOT_STARTED;

	OLContext context = OLReserveNextContext(olapi::OL_DURATION_AUTO);
	OLStatus status = mLeaderboard->GetLeaderboardRowForUser(context, OLID(NULL), boardID, userID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback, true);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::GetLeaderboardStatistic(OLID userID, 
														   OLID statID,
														   void *pClassPtr,
														   OnLiveContextEventDispatcher::ContextEventCallback callback)
{
	if (!mLeaderboard) return OL_SERVICES_NOT_STARTED;

	OLContext context = OLGetNextContext();
	OLStatus status = mLeaderboard->GetLeaderboardStatistic(context, OLID(NULL), userID, statID);

	if (status == OL_SUCCESS)
		OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, callback);

	return status;
}

///////////////////////////////////////////////////////////////////////////////

OnLiveLeaderboard::LeaderboardEventHandler::LeaderboardEventHandler() 
{
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::LeaderboardEventHandler::StatusResult(OLContext context, OLStatus status)
{
	bool success = OnLiveContextEventDispatcher::getInstance().DispatchStatusResult(context, status);
	OLReleaseContext(context);
	OnLiveContextEventDispatcher::getInstance().RemoveDispatchContext(context);
	if (success)
		return OL_SUCCESS;
	else
		return OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::LeaderboardEventHandler::ContainerIDResult(OLContext context, OLID id)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchContainerIDResult(context, id))
		return OL_SUCCESS;
	else
		return OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::LeaderboardEventHandler::IDListResult(OLContext context, OLIDList *idlist, OLID *containerlistid)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchIDListResult(context, idlist, containerlistid))
		return OL_SUCCESS;
	else
		return OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveLeaderboard::LeaderboardEventHandler::RowRetrieved(OLContext context,
																  OLID boardID,
																  OLID userID,
																  const char * tag,
																  unsigned int rowRank,
																  unsigned int rowIndex,
																  OLIDList *statContainerIDList)
{
	bool success = OnLiveContextEventDispatcher::getInstance().DispatchLeaderboardRowRetrieved(context,boardID,userID,tag,rowRank,rowIndex,statContainerIDList);
	if( statContainerIDList == NULL )
	{
		//then this is the last row and we can free the context.
		OLReleaseContext(context);
		OnLiveContextEventDispatcher::getInstance().RemoveDispatchContext(context);
	}
	if( success )
		return OL_SUCCESS;
	else
		return OL_FAIL;
	

}

///////////////////////////////////////////////////////////////////////////////

OnLiveLeaderboard::LeaderboardEventCallback::LeaderboardEventCallback()
{

}

///////////////////////////////////////////////////////////////////////////////

void OnLiveLeaderboard::LeaderboardEventCallback::EventWaiting()
{
	OnLiveLeaderboard::getInstance().mLeaderboard->HandleEvent(0);
}

