
#pragma once

#include "stdafx.h"

namespace SteamAPI
{

class SteamMatchmakingServers : public ISteamMatchmakingServers
{
public:// Request a new list of servers of a particular type.  These calls each correspond to one of the EMatchMakingType values.
	// Each call allocates a new asynchronous request object.
	// Request object must be released by calling ReleaseRequest( hServerListRequest )
	virtual HServerListRequest RequestInternetServerList( AppId_t iApp, MatchMakingKeyValuePair_t **ppchFilters, uint32 nFilters, ISteamMatchmakingServerListResponse *pRequestServersResponse ) 
	{
		return 0;
	}

	virtual HServerListRequest RequestLANServerList( AppId_t iApp, ISteamMatchmakingServerListResponse *pRequestServersResponse ) 
	{
		return 0;
	}

	virtual HServerListRequest RequestFriendsServerList( AppId_t iApp, MatchMakingKeyValuePair_t **ppchFilters, uint32 nFilters, ISteamMatchmakingServerListResponse *pRequestServersResponse )
	{
		return 0;
	}

	virtual HServerListRequest RequestFavoritesServerList( AppId_t iApp, MatchMakingKeyValuePair_t **ppchFilters, uint32 nFilters, ISteamMatchmakingServerListResponse *pRequestServersResponse )
	{
		return 0;
	}

	virtual HServerListRequest RequestHistoryServerList( AppId_t iApp, MatchMakingKeyValuePair_t **ppchFilters, uint32 nFilters, ISteamMatchmakingServerListResponse *pRequestServersResponse )
	{
		return 0;
	}

	virtual HServerListRequest RequestSpectatorServerList( AppId_t iApp, MatchMakingKeyValuePair_t **ppchFilters, uint32 nFilters, ISteamMatchmakingServerListResponse *pRequestServersResponse )
	{
		return 0;
	}


	// Releases the asynchronous request object and cancels any pending query on it if there's a pending query in progress.
	// RefreshComplete callback is not posted when request is released.
	virtual void ReleaseRequest( HServerListRequest hServerListRequest )
	{
	}


	/* the filters that are available in the ppchFilters params are:

		"map"		- map the server is running, as set in the dedicated server api
		"dedicated" - reports bDedicated from the API
		"secure"	- VAC-enabled
		"full"		- not full
		"empty"		- not empty
		"noplayers" - is empty
		"proxy"		- a relay server

	*/

	// Get details on a given server in the list, you can get the valid range of index
	// values by calling GetServerCount().  You will also receive index values in 
	// ISteamMatchmakingServerListResponse::ServerResponded() callbacks
	virtual gameserveritem_t *GetServerDetails( HServerListRequest hRequest, int iServer )
	{
		return 0;
	}
 

	// Cancel an request which is operation on the given list type.  You should call this to cancel
	// any in-progress requests before destructing a callback object that may have been passed 
	// to one of the above list request calls.  Not doing so may result in a crash when a callback
	// occurs on the destructed object.
	// Canceling a query does not release the allocated request handle.
	// The request handle must be released using ReleaseRequest( hRequest )
	virtual void CancelQuery( HServerListRequest hRequest )
	{
	}

	// Ping every server in your list again but don't update the list of servers
	// Query callback installed when the server list was requested will be used
	// again to post notifications and RefreshComplete, so the callback must remain
	// valid until another RefreshComplete is called on it or the request
	// is released with ReleaseRequest( hRequest )
	virtual void RefreshQuery( HServerListRequest hRequest )
	{
	}


	// Returns true if the list is currently refreshing its server list
	virtual bool IsRefreshing( HServerListRequest hRequest )
	{
		return false;
	}

	// How many servers in the given list, GetServerDetails above takes 0... GetServerCount() - 1
	virtual int GetServerCount( HServerListRequest hRequest )
	{
		return 0;
	}

	// Refresh a single server inside of a query (rather than all the servers )
	virtual void RefreshServer( HServerListRequest hRequest, int iServer )
	{
	}


	//-----------------------------------------------------------------------------
	// Queries to individual servers directly via IP/Port
	//-----------------------------------------------------------------------------

	// Request updated ping time and other details from a single server
	virtual HServerQuery PingServer( uint32 unIP, uint16 usPort, ISteamMatchmakingPingResponse *pRequestServersResponse )
	{
		return 0;
	} 

	// Request the list of players currently playing on a server
	virtual HServerQuery PlayerDetails( uint32 unIP, uint16 usPort, ISteamMatchmakingPlayersResponse *pRequestServersResponse )
	{
		return 0;
	}

	// Request the list of rules that the server is running (See ISteamMasterServerUpdater->SetKeyValue() to set the rules server side)
	virtual HServerQuery ServerRules( uint32 unIP, uint16 usPort, ISteamMatchmakingRulesResponse *pRequestServersResponse )
	{
		return 0;
	}

	// Cancel an outstanding Ping/Players/Rules query from above.  You should call this to cancel
	// any in-progress requests before destructing a callback object that may have been passed 
	// to one of the above calls to avoid crashing when callbacks occur.
	virtual void CancelServerQuery( HServerQuery hServerQuery )
	{
	} 
};

}
