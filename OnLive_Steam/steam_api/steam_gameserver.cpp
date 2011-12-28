
#include "stdafx.h"
#include "steam/steam_gameserver.h"

S_API bool SteamGameServer_Init( uint32 unIP, uint16 usPort, uint16 usGamePort, uint16 usSpectatorPort, uint16 usQueryPort, EServerMode eServerMode, const char *pchGameDir, const char *pchVersionString )
{
	TRACE( TEXT("SteamGameServer_Init(%d,%d,%d,%d,%d,%d,'%s','%s')"), 
		unIP, usPort, usGamePort, usSpectatorPort, usQueryPort, (int)eServerMode, pchGameDir, pchVersionString );
	return true;
}

S_API void SteamGameServer_Shutdown()
{
	TRACE( TEXT("SteamGameServer_Shutdown()") );
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
	return 0;
}

S_API ISteamUtils *SteamGameServerUtils()
{
	TRACE( TEXT("SteamGameServerUtils()") );
	return 0;
}

S_API ISteamMasterServerUpdater *SteamMasterServerUpdater()
{
	TRACE( TEXT("SteamMasterServerUpdater()") );
	return 0;
}

S_API ISteamNetworking *SteamGameServerNetworking()
{
	TRACE( TEXT("SteamGameServerNetworking()") );
	return 0;
}

S_API ISteamGameServerStats *SteamGameServerStats()
{
	TRACE( TEXT("SteamGameServerStats()") );
	return 0;
}