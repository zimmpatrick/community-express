// SteamworksUnity.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

// steam api header file
#include "steam/steam_api.h"

// Game server API code from Steam, lets us do authentication via Steam for who to allow to play,
// also lets us get ourselves listed in the Steam master server so the server browser can find us
#include "steam/steam_gameserver.h"

// Steam Callbacks class is used to connect Steam's responses to our app
#include "SteamCallbacks.h"

// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the STEAMWORKSUNITY_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// STEAMWORKSUNITY_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef STEAMWORKSUNITY_EXPORTS
#define STEAMWORKSUNITY_API extern "C" __declspec(dllexport)
#else
#define STEAMWORKSUNITY_API extern "C" __declspec(dllimport)
#endif

// STEAMWORKSUNITY_API void SteamUnityAPI_Init(); (see below)
STEAMWORKSUNITY_API void SteamUnityAPI_Shutdown()
{
	return SteamAPI_Shutdown();
}

// checks if a local Steam client is running 
STEAMWORKSUNITY_API bool SteamUnityAPI_IsSteamRunning()
{
	return SteamAPI_IsSteamRunning();
}

// Detects if your executable was launched through the Steam client, and restarts your game through 
// the client if necessary. The Steam client will be started if it is not running.
//
// Returns: true if your executable was NOT launched through the Steam client. This function will
//		  then start your application through the client. Your current process should exit.
//
//		  false if your executable was started through the Steam client or a steam_appid.txt file
//		  is present in your game's directory (for development). Your current process should continue.
//
// NOTE: This function should be used only if you are using CEG or not using Steam's DRM. Once applied
//	   to your executable, Steam's DRM will handle restarting through Steam if necessary.
STEAMWORKSUNITY_API bool SteamUnityAPI_RestartAppIfNecessary( uint32 unOwnAppID )
{
	return SteamAPI_RestartAppIfNecessary( unOwnAppID );
}

STEAMWORKSUNITY_API bool SteamUnityAPI_Init()
{
	return SteamAPI_Init();
}

STEAMWORKSUNITY_API void SteamUnityAPI_RunCallbacks()
{
	return SteamAPI_RunCallbacks();
}

STEAMWORKSUNITY_API void * SteamUnityAPI_SteamRemoteStorage()
{
	return SteamRemoteStorage();
}

STEAMWORKSUNITY_API int32 SteamUnityAPI_SteamRemoteStorage_GetFileCount(void * pSteamRemoteStorage)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->GetFileCount();
}

STEAMWORKSUNITY_API const char * SteamUnityAPI_SteamRemoteStorage_GetFileNameAndSize(void * pSteamRemoteStorage, int iFile, int32 * pnFileSizeInBytes)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->GetFileNameAndSize(iFile, pnFileSizeInBytes);
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamRemoteStorage_FileRead(void * pSteamRemoteStorage, const char *pchFile, void *pvData, int32 cubDataToRead)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FileRead(pchFile, pvData, cubDataToRead);
}



// Current game server version
#define SPACEWAR_SERVER_VERSION "1.0.0.0"

// UDP port for the spacewar server to do authentication on (ie, talk to Steam on)
#define SPACEWAR_AUTHENTICATION_PORT 8766

// UDP port for the spacewar server to listen on
#define SPACEWAR_SERVER_PORT 27015

// UDP port for the master server updater to listen on
#define SPACEWAR_MASTER_SERVER_UPDATER_PORT 27016

// How long to wait for a response from the server before resending our connection attempt
#define SERVER_CONNECTION_RETRY_MILLISECONDS 350

// How long to wait for a client to send an update before we drop its connection server side
#define SERVER_TIMEOUT_MILLISECONDS 5000

// Maximum packet size in bytes
#define MAX_SPACEWAR_PACKET_SIZE 1024*512

// Maximum number of players who can join a server and play simultaneously
#define MAX_PLAYERS_PER_SERVER 4

// Time to pause wait after a round ends before starting a new one
#define MILLISECONDS_BETWEEN_ROUNDS 4000

// How long photon beams live before expiring
#define PHOTON_BEAM_LIFETIME_IN_TICKS 1750

// How fast can photon beams be fired?
#define PHOTON_BEAM_FIRE_INTERVAL_TICKS 250

// Amount of space needed for beams per ship
#define MAX_PHOTON_BEAMS_PER_SHIP PHOTON_BEAM_LIFETIME_IN_TICKS/PHOTON_BEAM_FIRE_INTERVAL_TICKS

// Time to timeout a connection attempt in
#define MILLISECONDS_CONNECTION_TIMEOUT 30000

// How many times a second does the server send world updates to clients
#define SERVER_UPDATE_SEND_RATE 60

// How many times a second do we send our updated client state to the server
#define CLIENT_UPDATE_SEND_RATE 30

// How fast does the server internally run at?
#define MAX_CLIENT_AND_SERVER_FPS 86



STEAMWORKSUNITY_API void * SteamUnityAPI_SteamGameServer()
{
	char *pchGameDir = "killingfloor";
	uint32 unIP = 0;
	uint16 usSpectatorPort = 0; // We don't support specators in our game
	uint16 usMasterServerUpdaterPort = SPACEWAR_MASTER_SERVER_UPDATER_PORT;

#ifdef USE_GS_AUTH_API
	EServerMode eMode = eServerModeAuthenticationAndSecure;
#else
	// Don't let Steam do authentication
	EServerMode eMode = eServerModeNoAuthentication;
#endif
	// Initialize the SteamGameServer interface, we tell it some info about us, and we request support
	// for both Authentication (making sure users own games) and secure mode, VAC running in our game
	// and kicking users who are VAC banned
	if ( !SteamGameServer_Init( unIP, SPACEWAR_AUTHENTICATION_PORT, SPACEWAR_SERVER_PORT, usSpectatorPort, usMasterServerUpdaterPort, eMode, pchGameDir, SPACEWAR_SERVER_VERSION ) )
	{
		OutputDebugString( TEXT("SteamGameServer_Init call failed\n") );
	}

	return SteamGameServer();
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_Shutdown()
{
	return SteamGameServer_Shutdown();
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamGameServer_GetSteamID(void * pSteamGameServer)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	return pISteamGameServer->GetSteamID().ConvertToUint64();
}


STEAMWORKSUNITY_API void * SteamUnityAPI_SteamFriends()
{
	return SteamFriends();
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetFriendCount(void * pSteamFriends)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendCount(k_EFriendFlagImmediate);
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamFriends_GetFriendByIndex(void * pSteamFriends, int iFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendByIndex(iFriend, k_EFriendFlagImmediate).ConvertToUint64();
}

STEAMWORKSUNITY_API const char * SteamUnityAPI_SteamFriends_GetFriendPersonaName(void * pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );
	
	return pISteamFriends->GetFriendPersonaName( CSteamID( steamIDFriend) );
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetFriendPersonaState(void * pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );
	
	return pISteamFriends->GetFriendPersonaState( CSteamID( steamIDFriend) );
}

STEAMWORKSUNITY_API void * SteamUnityAPI_SteamUser()
{
	return SteamUser();
}

// returns the HSteamUser this interface represents
// this is only used internally by the API, and by a few select interfaces that support multi-user
STEAMWORKSUNITY_API int SteamUnityAPI_SteamUser_GetHSteamUser(void * pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->GetHSteamUser();
}

// returns true if the Steam client current has a live connection to the Steam servers. 
// If false, it means there is no active connection due to either a networking issue on the local machine, or the Steam server is down/busy.
// The Steam client will automatically be trying to recreate the connection as often as possible.
STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUser_BLoggedOn(void * pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->BLoggedOn();
}

// returns the CSteamID of the account currently logged into the Steam client
// a CSteamID is a unique identifier for an account, and used to differentiate users in all parts of the Steamworks API
STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamUser_GetSteamID(void * pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->GetSteamID().ConvertToUint64();
}


STEAMWORKSUNITY_API void * SteamUnityAPI_SteamUserStats()
{
	return SteamUserStats();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_RequestCurrentStats(void* pSteamUserStats, void (__stdcall *OnUserStatsReceivedCallback)(UserStatsReceived_t*))
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	SteamCallbacks::getInstance().delegateOnUserStatsReceived = OnUserStatsReceivedCallback;

	return pISteamUserStats->RequestCurrentStats();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_GetUserStatInt(void* pSteamUserStats, uint64 steamID, const char *statName, int &statValue)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetUserStat(steamID, statName, &statValue);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_GetUserStatFloat(void* pSteamUserStats, uint64 steamID, const char *statName, float &statValue)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetUserStat(steamID, statName, &statValue);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_SetStatInt(void* pSteamUserStats, const char *statName, int statValue)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->SetStat(statName, statValue);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_SetStatFloat(void* pSteamUserStats, const char *statName, float statValue)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->SetStat(statName, statValue);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_StoreStats(void* pSteamUserStats)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->StoreStats();
}


// Steam Callbacks are used to bridge Steam's responses back to our app
void SteamCallbacks::OnUserStatsReceived(UserStatsReceived_t *CallbackData)
{
	delegateOnUserStatsReceived(CallbackData);
}
