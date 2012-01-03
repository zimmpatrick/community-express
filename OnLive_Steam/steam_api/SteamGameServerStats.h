
#pragma once

#include "stdafx.h"
#include "steam/steam_gameserver.h"

namespace SteamAPI
{

class SteamGameServerStats : public ISteamGameServerStats
{
public:
	// downloads stats for the user
	// returns a GSStatsReceived_t callback when completed
	// if the user has no stats, GSStatsReceived_t.m_eResult will be set to k_EResultFail
	// these stats will only be auto-updated for clients playing on the server. For other
	// users you'll need to call RequestUserStats() again to refresh any data
	virtual SteamAPICall_t RequestUserStats( CSteamID steamIDUser )
	{
		return 0;
	}

	// requests stat information for a user, usable after a successful call to RequestUserStats()
	virtual bool GetUserStat( CSteamID steamIDUser, const char *pchName, int32 *pData )
	{
		return true;
	}
	virtual bool GetUserStat( CSteamID steamIDUser, const char *pchName, float *pData )
	{
		return true;
	}
	virtual bool GetUserAchievement( CSteamID steamIDUser, const char *pchName, bool *pbAchieved ) 
	{
		return true;
	}

	// Set / update stats and achievements. 
	// Note: These updates will work only on stats game servers are allowed to edit and only for 
	// game servers that have been declared as officially controlled by the game creators. 
	// Set the IP range of your official servers on the Steamworks page
	virtual bool SetUserStat( CSteamID steamIDUser, const char *pchName, int32 nData )
	{
		return true;
	}
	virtual bool SetUserStat( CSteamID steamIDUser, const char *pchName, float fData )
	{
		return true;
	}
	virtual bool UpdateUserAvgRateStat( CSteamID steamIDUser, const char *pchName, float flCountThisSession, double dSessionLength )
	{
		return true;
	}

	virtual bool SetUserAchievement( CSteamID steamIDUser, const char *pchName )
	{
		return true;
	}
	virtual bool ClearUserAchievement( CSteamID steamIDUser, const char *pchName )
	{
		return true;
	}

	// Store the current data on the server, will get a GSStatsStored_t callback when set.
	//
	// If the callback has a result of k_EResultInvalidParam, one or more stats 
	// uploaded has been rejected, either because they broke constraints
	// or were out of date. In this case the server sends back updated values.
	// The stats should be re-iterated to keep in sync.
	virtual SteamAPICall_t StoreUserStats( CSteamID steamIDUser )
	{
		return 0;
	}
};

}
