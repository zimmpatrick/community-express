// steam_api.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include "SteamUtils.h"
#include "SteamRemoteStorage.h"
#include "SteamApps.h"
#include "SteamClient.h"
#include "SteamUser.h"
#include "SteamMatchmaking.h"
#include "SteamFriends.h"
#include "SteamUserStats.h"
#include "SteamNetworking.h"
#include "SteamMatchmakingServers.h"

SteamAPI::SteamUtils gUtils;
SteamAPI::SteamRemoteStorage gRemoteStorage;
SteamAPI::SteamApps gApps;
SteamAPI::SteamClient gClient;
SteamAPI::SteamUser gUser;
SteamAPI::SteamMatchmaking gMatchmaking;
SteamAPI::SteamFriends gFriends;
SteamAPI::SteamUserStats gUserStats;
SteamAPI::SteamNetworking gNetworking;
SteamAPI::SteamMatchmakingServers gMatchmakingServers;

// #include "OnLiveIntegration.h"

S_API void SteamAPI_Shutdown()
{
	TRACE( TEXT("SteamAPI_Shutdown()") );
}

S_API bool SteamAPI_IsSteamRunning()
{
	TRACE( TEXT("SteamAPI_IsSteamRunning()") );
	return true;
}

S_API bool SteamAPI_RestartAppIfNecessary( uint32 unOwnAppID )
{
	TRACE( TEXT("SteamAPI_RestartAppIfNecessary(%d)"), unOwnAppID );
	return false;
}

S_API bool SteamAPI_Init()
{
	TRACE( TEXT("SteamAPI_Init()") );

#if 0
	OnLive::getInstance().startup();

	OnLive::getInstance().setWindowedMode(true);

	if (OnLive::getInstance().start(false))
	{
		OnLive::getInstance().bind();
	}
#endif
	
	return true;
}

S_API void SteamAPI_RunCallbacks()
{
	TRACE( TEXT("SteamAPI_RunCallbacks()") );

#if 0
	OnLive::getInstance().runFrame();
#endif
}

S_API void SteamAPI_RegisterCallback( class CCallbackBase *pCallback, int iCallback )
{
	TRACE( TEXT("SteamAPI_RegisterCallback(%x,%d)"), pCallback, iCallback );
}

S_API void SteamAPI_UnregisterCallback( class CCallbackBase *pCallback )
{
	TRACE( TEXT("SteamAPI_UnregisterCallback(%x)"), pCallback );
}

S_API void SteamAPI_RegisterCallResult( class CCallbackBase *pCallback, SteamAPICall_t hAPICall )
{
	TRACE( TEXT("SteamAPI_RegisterCallResult(%x,%d)"), pCallback, hAPICall );
}

S_API void SteamAPI_UnregisterCallResult( class CCallbackBase *pCallback, SteamAPICall_t hAPICall )
{
	TRACE( TEXT("SteamAPI_UnregisterCallResult(%x,%d)"), pCallback, hAPICall );
}

S_API ISteamUser *SteamUser()
{
	TRACE( TEXT("SteamUser()") );
	return &gUser;
}

S_API ISteamFriends *SteamFriends()
{
	TRACE( TEXT("SteamFriends()") );
	return &gFriends;
}

S_API ISteamUtils *SteamUtils()
{
	TRACE( TEXT("SteamUtils()") );
	return &gUtils;
}

S_API ISteamMatchmaking *SteamMatchmaking()
{
	TRACE( TEXT("SteamMatchmaking()") );
	return &gMatchmaking;
}

S_API ISteamUserStats *SteamUserStats()
{
	TRACE( TEXT("SteamUserStats()") );
	return &gUserStats;
}

S_API ISteamApps *SteamApps()
{
	TRACE( TEXT("SteamApps()") );
	return &gApps;
}

S_API ISteamNetworking *SteamNetworking()
{
	TRACE( TEXT("SteamNetworking()") );
	return &gNetworking;
}

S_API ISteamMatchmakingServers *SteamMatchmakingServers()
{
	TRACE( TEXT("SteamMatchmakingServers()") );
	return &gMatchmakingServers;
}

S_API ISteamRemoteStorage *SteamRemoteStorage()
{
	TRACE( TEXT("SteamRemoteStorage()") );
	return &gRemoteStorage;
}

S_API ISteamClient *SteamClient()
{
	TRACE( TEXT("SteamClient()") );
	return &gClient;
}

// crash dump recording functions
S_API void SteamAPI_WriteMiniDump( uint32 uStructuredExceptionCode, void* pvExceptionInfo, uint32 uBuildID )
{
	TRACE( TEXT("SteamAPI_WriteMiniDump(%d,%x,%d)"), uStructuredExceptionCode, pvExceptionInfo, uBuildID );
}

S_API void SteamAPI_SetMiniDumpComment( const char *pchMsg )
{
	TRACE( TEXT("SteamAPI_SetMiniDumpComment('%s')"), pchMsg );
}
