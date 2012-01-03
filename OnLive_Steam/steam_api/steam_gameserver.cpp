
#include "stdafx.h"
#include "SteamGameServer.h"
#include "SteamGameServerStats.h"
#include "SteamMasterServerUpdater.h"

SteamAPI::SteamGameServer * gGameServer;
SteamAPI::SteamGameServerStats gGameServerStats;
SteamAPI::SteamMasterServerUpdater gMasterServerUpdater;

S_API bool SteamGameServer_Init( uint32 unIP, uint16 usPort, uint16 usGamePort, uint16 usSpectatorPort, uint16 usQueryPort, EServerMode eServerMode, const char *pchGameDir, const char *pchVersionString )
{
	TRACE( TEXT("SteamGameServer_Init(%d,%d,%d,%d,%d,%d,'%s','%s')"), 
		unIP, usPort, usGamePort, usSpectatorPort, usQueryPort, (int)eServerMode, pchGameDir, pchVersionString );
	gGameServer = new SteamAPI::SteamGameServer();
	return true;
}

S_API void SteamGameServer_Shutdown()
{
	TRACE( TEXT("SteamGameServer_Shutdown()") );
	delete gGameServer;
	gGameServer = 0;
}

S_API void SteamGameServer_RunCallbacks()
{
	TRACE( TEXT("SteamGameServer_RunCallbacks()") );
}

S_API bool SteamGameServer_BSecure()
{
	TRACE( TEXT("SteamGameServer_BSecure()") );
	return true;
}

S_API uint64 SteamGameServer_GetSteamID()
{
	TRACE( TEXT("SteamGameServer_GetSteamID()") );
	return 0;
}

S_API ISteamGameServer *SteamGameServer()
{
	TRACE( TEXT("SteamGameServer()") );
	return gGameServer;
}

S_API ISteamUtils *SteamGameServerUtils()
{
	TRACE( TEXT("SteamGameServerUtils()") );
	return SteamUtils();
}

S_API ISteamMasterServerUpdater *SteamMasterServerUpdater()
{
	TRACE( TEXT("SteamMasterServerUpdater()") );
	return &gMasterServerUpdater;
}

S_API ISteamNetworking *SteamGameServerNetworking()
{
	TRACE( TEXT("SteamGameServerNetworking()") );
	return SteamNetworking();
}

S_API ISteamGameServerStats *SteamGameServerStats()
{
	TRACE( TEXT("SteamGameServerStats()") );
	return &gGameServerStats;
}
