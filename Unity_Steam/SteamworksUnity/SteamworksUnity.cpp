// SteamworksUnity.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

// steam api header file
#include "steam/steam_api.h"

// Game server API code from Steam, lets us do authentication via Steam for who to allow to play,
// also lets us get ourselves listed in the Steam master server so the server browser can find us
#include "steam/steam_gameserver.h"

// Steam Callbacks class is used to connect Steam's responses to our API
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

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_IsAPICallCompleted(SteamAPICall_t hSteamAPICall, unsigned char &bFailed)
{
	bool result, failed;

	if (SteamGameServer())
	{
		result = SteamGameServerUtils()->IsAPICallCompleted(hSteamAPICall, &failed);
	}
	else
	{
		result = SteamUtils()->IsAPICallCompleted(hSteamAPICall, &failed);
	}

	bFailed = failed ? 1 : 0;

	return result;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_GetGameServerUserStatsReceivedResult(SteamAPICall_t hSteamAPICall, GSStatsReceived_t &CallbackData, unsigned char &bFailed)
{
	bool result, failed;

	if (SteamGameServer())
	{
		result = SteamGameServerUtils()->GetAPICallResult(hSteamAPICall, &CallbackData, sizeof(CallbackData), CallbackData.k_iCallback, &failed);
	}
	else
	{
		result = SteamUtils()->GetAPICallResult(hSteamAPICall, &CallbackData, sizeof(CallbackData), CallbackData.k_iCallback, &failed);
	}

	bFailed = failed ? 1 : 0;

	return result;
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamUtils_GetAppID()
{
	if (SteamGameServer())
	{
		return SteamGameServerUtils()->GetAppID();
	}
	else
	{
		return SteamUtils()->GetAppID();
	}
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
	return SteamGameServer();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServer_Init(uint32 unIP, uint16 usPort, uint16 usGamePort, uint16 usSpectatorPort, uint16 usMasterServerPort, EServerMode eServerMode, const char *pchGameDir, const char *pchGameVersion)
{
	// Initialize the SteamGameServer interface, we tell it some info about us, and we request support
	// for both Authentication (making sure users own games) and secure mode, VAC running in our game
	// and kicking users who are VAC banned
	if ( !SteamGameServer_Init( unIP, usPort, usGamePort, usSpectatorPort, usMasterServerPort, eServerMode, pchGameDir, pchGameVersion ) )
	{
		OutputDebugString( TEXT("SteamGameServer_Init call failed\n") );
		return false;
	}

	return true;
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_RunCallbacks()
{
	SteamGameServer_RunCallbacks();
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_SetCallbacks(FPOnGameServerClientApprove fpOnGameServerClientApprove, FPOnGameServerClientDeny fpOnGameServerClientDeny, FPOnGameServerClientKick fpOnGameServerClientKick, FPOnGameServerPolicyResponse fpOnGameServerPolicyResponse)
{
	SteamCallbacks::getInstance().delegateOnGameServerClientApprove = fpOnGameServerClientApprove;
	SteamCallbacks::getInstance().delegateOnGameServerClientDeny = fpOnGameServerClientDeny;
	SteamCallbacks::getInstance().delegateOnGameServerClientKick = fpOnGameServerClientKick;
	SteamCallbacks::getInstance().delegateOnGameServerPolicyResponse = fpOnGameServerPolicyResponse;
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamGameServer_GetSteamID(void * pSteamGameServer)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	return pISteamGameServer->GetSteamID().ConvertToUint64();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServer_SendUserConnectAndAuthenticate(void * pSteamGameServer, uint32 unIPClient, uint32 *pAuthTicket, uint32 authTicketSize, uint64 &steamIDClient)
{
	CSteamID clientSteamID;

	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	if (pISteamGameServer->SendUserConnectAndAuthenticate(unIPClient, pAuthTicket, authTicketSize, &clientSteamID))
	{
		steamIDClient = clientSteamID.ConvertToUint64();
		return true;
	}

	return false;
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_SendUserDisconnect(void * pSteamGameServer, uint64 steamIDClient)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	return pISteamGameServer->SendUserDisconnect(steamIDClient);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_Shutdown()
{
	return SteamGameServer_Shutdown();
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

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetClanCount(void * pSteamFriends)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetClanCount();
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamFriends_GetClanByIndex(void * pSteamFriends, int iClan)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetClanByIndex(iClan).ConvertToUint64();
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamFriends_GetClanName(void * pSteamFriends, uint64 steamIDClan)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetClanName(steamIDClan);
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamFriends_GetClanTag(void * pSteamFriends, uint64 steamIDClan)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetClanTag(steamIDClan);
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetFriendCountFromSource(void * pSteamFriends, uint64 steamIDSource)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendCountFromSource( CSteamID( steamIDSource) );
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamFriends_GetFriendFromSourceByIndex(void * pSteamFriends, uint64 steamIDSource, int iFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendFromSourceByIndex(steamIDSource, iFriend).ConvertToUint64();
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

STEAMWORKSUNITY_API uint32 SteamUnityAPI_SteamUser_InitiateGameConnection(void * pSteamUser, void *pAuthTicket, int32 cbMaxTicketSize, uint64 steamIDGameServer, uint32 unIPServer, uint16 usPortServer, bool bSecure)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->InitiateGameConnection(pAuthTicket, cbMaxTicketSize, steamIDGameServer, unIPServer, usPortServer, bSecure);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamUser_TerminateGameConnection(void * pSteamUser, uint32 unIPServer, uint16 usPortServer)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->TerminateGameConnection(unIPServer, usPortServer);
}

STEAMWORKSUNITY_API void * SteamUnityAPI_SteamUserStats()
{
	return SteamUserStats();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_RequestCurrentStats(void* pSteamUserStats, FPOnUserStatsReceived fpOnUserStatsReceived)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	SteamCallbacks::getInstance().delegateOnUserStatsReceived = fpOnUserStatsReceived;

	return pISteamUserStats->RequestCurrentStats();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_GetUserStatInt(void* pSteamUserStats, uint64 steamID, const char *pchName, int32 &nData)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetUserStat(steamID, pchName, &nData);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_GetUserStatFloat(void* pSteamUserStats, uint64 steamID, const char *pchName, float &fData)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetUserStat(steamID, pchName, &fData);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_SetStatInt(void* pSteamUserStats, const char *pchName, int32 nData)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->SetStat(pchName, nData);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_SetStatFloat(void* pSteamUserStats, const char *pchName, float fData)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->SetStat(pchName, fData);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_GetAchievement(void* pSteamUserStats, const char *pchName, bool &bAchieved)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	bool achieved;

	if (pISteamUserStats->GetAchievement(pchName, &achieved))
	{
		bAchieved = achieved ? 1 : 0;
		return true;
	}

	return false;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_GetUserAchievement(void* pSteamUserStats, uint64 steamIDUser, const char *pchName, unsigned char &bAchieved)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	bool achieved;

	if (pISteamUserStats->GetUserAchievement(steamIDUser, pchName, &achieved))
	{
		bAchieved = achieved ? 1 : 0;
		return true;
	}

	return false;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_SetAchievement(void* pSteamUserStats, const char *pchName)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->SetAchievement(pchName);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_StoreStats(void* pSteamUserStats)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->StoreStats();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_FindLeaderboard(void* pSteamUserStats, const char *pchLeaderboardName, FPOnLeaderboardRetrieved fpOnLeaderboardRetrieved)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	SteamAPICall_t result = pISteamUserStats->FindLeaderboard(pchLeaderboardName);

	if (result != 0)
	{
		SteamCallbacks::getInstance().delegateOnLeaderboardRetrieved = fpOnLeaderboardRetrieved;
		SteamCallbacks::getInstance().SteamCallResultFindLeaderboard.Set(result, &SteamCallbacks::getInstance(), &SteamCallbacks::OnLeaderboardRetrieved);
		return true;
	}

	return false;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_FindOrCreateLeaderboard(void* pSteamUserStats, const char *pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, ELeaderboardDisplayType eLeaderboardDisplayType, FPOnLeaderboardRetrieved fpOnLeaderboardRetrieved)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	SteamAPICall_t result = pISteamUserStats->FindOrCreateLeaderboard(pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);

	if (result != 0)
	{
		SteamCallbacks::getInstance().delegateOnLeaderboardRetrieved = fpOnLeaderboardRetrieved;
		SteamCallbacks::getInstance().SteamCallResultFindLeaderboard.Set(result, &SteamCallbacks::getInstance(), &SteamCallbacks::OnLeaderboardRetrieved);
		return true;
	}

	return false;
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamUserStats_GetLeaderboardName(void* pSteamUserStats, SteamLeaderboard_t hSteamLeaderboard)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetLeaderboardName(hSteamLeaderboard);
}

STEAMWORKSUNITY_API ELeaderboardSortMethod SteamUnityAPI_SteamUserStats_GetLeaderboardSortMethod(void* pSteamUserStats, SteamLeaderboard_t hSteamLeaderboard)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetLeaderboardSortMethod(hSteamLeaderboard);
}

STEAMWORKSUNITY_API ELeaderboardDisplayType SteamUnityAPI_SteamUserStats_GetLeaderboardDisplayType(void* pSteamUserStats, SteamLeaderboard_t hSteamLeaderboard)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetLeaderboardDisplayType(hSteamLeaderboard);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_UploadLeaderboardScore(void* pSteamUserStats, SteamLeaderboard_t hSteamLeaderboard, ELeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, int32 nScore, int32* pScoreDetails, int cScoreDetailCount)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->UploadLeaderboardScore(hSteamLeaderboard, eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailCount) != 0;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_RequestLeaderboardEntries(void* pSteamUserStats, SteamLeaderboard_t hSteamLeaderboard, ELeaderboardDataRequest eLeaderboardDataRequest, int nRangeStart, int nRangeEnd, FPOnLeaderboardEntriesRetrieved fpOnLeaderboardEntriesRetrieved)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	SteamAPICall_t result = pISteamUserStats->DownloadLeaderboardEntries(hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd);

	if (result != 0)
	{
		SteamCallbacks::getInstance().delegateOnLeaderboardEntriesRetrieved = fpOnLeaderboardEntriesRetrieved;
		SteamCallbacks::getInstance().SteamCallResultLeaderboardScoresDownloaded.Set(result, &SteamCallbacks::getInstance(), &SteamCallbacks::OnLeaderboardEntriesRetrieved);
		return true;
	}

	return false;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_GetDownloadedLeaderboardEntry(void* pSteamUserStats, SteamLeaderboardEntries_t hSteamLeaderboardEntries, int32 index, LeaderboardEntry_t &outLeaderboardEntry, int32* pDetails, int cDetailsMax)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries, index, &outLeaderboardEntry, pDetails, cDetailsMax);
}

STEAMWORKSUNITY_API void * SteamUnityAPI_SteamGameServerStats()
{
	return SteamGameServerStats();
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamGameServerStats_RequestUserStats(void* pSteamGameServerStats, uint64 steamIDUser)
{
	ISteamGameServerStats * pISteamGameServerStats = static_cast<ISteamGameServerStats*>( pSteamGameServerStats );

	return pISteamGameServerStats->RequestUserStats(steamIDUser);;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServerStats_GetUserStatInt(void* pSteamGameServerStats, uint64 steamIDUser, const char *pchName, int32 &nData)
{
	ISteamGameServerStats * pISteamGameServerStats = static_cast<ISteamGameServerStats*>( pSteamGameServerStats );

	return pISteamGameServerStats->GetUserStat(steamIDUser, pchName, &nData);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServerStats_GetUserStatFloat(void* pSteamGameServerStats, uint64 steamIDUser, const char *pchName, float &fData)
{
	ISteamGameServerStats * pISteamGameServerStats = static_cast<ISteamGameServerStats*>( pSteamGameServerStats );

	return pISteamGameServerStats->GetUserStat(steamIDUser, pchName, &fData);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServerStats_SetStatInt(void* pSteamGameServerStats, uint64 steamIDUser, const char *pchName, int32 nData)
{
	ISteamGameServerStats * pISteamGameServerStats = static_cast<ISteamGameServerStats*>( pSteamGameServerStats );

	return pISteamGameServerStats->SetUserStat(steamIDUser, pchName, nData);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServerStats_SetStatFloat(void* pSteamGameServerStats, uint64 steamIDUser, const char *pchName, float fData)
{
	ISteamGameServerStats * pISteamGameServerStats = static_cast<ISteamGameServerStats*>( pSteamGameServerStats );

	return pISteamGameServerStats->SetUserStat(steamIDUser, pchName, fData);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServerStats_GetUserAchievement(void* pSteamGameServerStats, uint64 steamIDUser, const char *pchName, unsigned char &bAchieved)
{
	ISteamGameServerStats * pISteamGameServerStats = static_cast<ISteamGameServerStats*>( pSteamGameServerStats );

	bool achieved;

	if (pISteamGameServerStats->GetUserAchievement(steamIDUser, pchName, &achieved))
	{
		bAchieved = achieved ? 1 : 0;
		return true;
	}

	return false;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServerStats_SetUserAchievement(void* pSteamGameServerStats, uint64 steamIDUser, const char *pchName)
{
	ISteamGameServerStats * pISteamGameServerStats = static_cast<ISteamGameServerStats*>( pSteamGameServerStats );

	return pISteamGameServerStats->SetUserAchievement(steamIDUser, pchName);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServerStats_StoreStats(void* pSteamGameServerStats, uint64 steamIDUser)
{
	ISteamGameServerStats * pISteamGameServerStats = static_cast<ISteamGameServerStats*>( pSteamGameServerStats );

	return pISteamGameServerStats->StoreUserStats(steamIDUser) != 0;
}

STEAMWORKSUNITY_API void * SteamUnityAPI_SteamMatchmaking()
{
	return SteamMatchmaking();
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetNumLobbyMembers(void* pSteamMatchmaking, uint64 steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetNumLobbyMembers(steamIDLobby);
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamFriends_GetLobbyMemberByIndex(void* pSteamMatchmaking, uint64 steamIDLobby, int iMember)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetLobbyMemberByIndex(steamIDLobby, iMember).ConvertToUint64();
}


// Steam Callbacks are used to bridge Steam's responses back to our app
void SteamCallbacks::OnUserStatsReceived(UserStatsReceived_t *pCallbackData)
{
	delegateOnUserStatsReceived(pCallbackData);
}

void SteamCallbacks::OnGameServerClientApprove(GSClientApprove_t *pCallbackData)
{
	delegateOnGameServerClientApprove(pCallbackData);
}

void SteamCallbacks::OnGameServerClientDeny(GSClientDeny_t *pCallbackData)
{
	delegateOnGameServerClientDeny(pCallbackData);
}

void SteamCallbacks::OnGameServerClientKick(GSClientKick_t *pCallbackData)
{
	delegateOnGameServerClientKick(pCallbackData);
}

void SteamCallbacks::OnGameServerPolicyResponse(GSPolicyResponse_t *pCallbackData)
{
	delegateOnGameServerPolicyResponse(pCallbackData);
}

void SteamCallbacks::OnLeaderboardRetrieved(LeaderboardFindResult_t *pCallbackData, bool bIOFailure)
{
	delegateOnLeaderboardRetrieved(pCallbackData);
}

void SteamCallbacks::OnLeaderboardEntriesRetrieved(LeaderboardScoresDownloaded_t *pCallbackData, bool bIOFailure )
{
	delegateOnLeaderboardEntriesRetrieved(pCallbackData);
}