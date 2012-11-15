// SteamworksUnity.cpp : Defines the exported functions for the DLL application.
// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

#include "stdafx.h"
#include <stdlib.h>

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

typedef uint64 (__stdcall *FPOnChallengeResponse)(uint64 challenge);
typedef void (__stdcall *FSteamAPIDebugTextHook)(int nSeverity, const char *pchDebugText);
typedef void (__cdecl *FSteamAPIDebugTextHookCD)(int nSeverity, const char *pchDebugText);

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

HANDLE hLicenseThread = NULL;

DWORD WINAPI MyLicenseThreadFunction( LPVOID lpParam ) 
{ 
	FPOnChallengeResponse pFPOnChallengeResponse = static_cast<FPOnChallengeResponse>( lpParam );

	while (true)
	{
		uint32 value = (uint32)rand();
		uint64 valueSQ = value * value;

		uint64 res = (pFPOnChallengeResponse)( valueSQ );
		if (res != (uint64)value)
		{
			// this will do for now, many throw an Exception
			//  in the main thread.
			abort();
		}

		Sleep(1000 * value % 5);
	}
}

STEAMWORKSUNITY_API bool SteamUnityAPI_Init(FPOnChallengeResponse pFPOnChallengeResponse)
{
	/* hLicenseThread = CreateThread( 
		NULL,				   // default security attributes
		0,					  // use default stack size  
		MyLicenseThreadFunction,	   // thread function name
		pFPOnChallengeResponse,		// argument to thread function 
		0,					  // use default creation flags 
		NULL);   // returns the thread identifier 
		*/

	return SteamAPI_Init();
}

// STEAMWORKSUNITY_API void SteamUnityAPI_Init(); (see below)
STEAMWORKSUNITY_API void SteamUnityAPI_Shutdown()
{
	if (hLicenseThread)
	{
		// TODO, clean this up to pass events, etc
		TerminateThread(hLicenseThread, 0);
		hLicenseThread = 0;
	}

	return SteamAPI_Shutdown();
}

FSteamAPIDebugTextHook hook = NULL;

//-----------------------------------------------------------------------------
// Purpose: callback hook for debug text emitted from the Steam API
//-----------------------------------------------------------------------------
extern "C" void __cdecl CSteamAPIDebugTextHook( int nSeverity, const char *pchDebugText )
{
	// if you're running in the debugger, only warnings (nSeverity >= 1) will be sent
	// if you add -debug_steamapi to the command-line, a lot of extra informational messages will also be sent
	if (hook) hook(nSeverity, pchDebugText);

	if ( nSeverity >= 1 )
	{
		// place to set a breakpoint for catching API errors
		int x = 3;
		x = x;
	}
}

STEAMWORKSUNITY_API void SteamUnityAPI_SetWarningMessageHook(FSteamAPIDebugTextHook SteamAPIDebugTextHook)
{	
	// set our debug handler
	hook = SteamAPIDebugTextHook;

	SteamClient()->SetWarningMessageHook( CSteamAPIDebugTextHook );
}



STEAMWORKSUNITY_API void SteamUnityAPI_RunCallbacks()
{
	return SteamAPI_RunCallbacks();
}

STEAMWORKSUNITY_API const char * SteamUnityAPI_GetPersonaNameByID(uint64 steamIDFriend)
{
	return SteamFriends()->GetFriendPersonaName( CSteamID(steamIDFriend) );
}

STEAMWORKSUNITY_API void SteamUnityAPI_SetTransactionAuthorizationCallback(FPOnTransactionAuthorizationReceived fpOnTransactionAuthorizationReceived)
{
	SteamCallbacks::getInstance().delegateOnTransactionAuthorizationReceived = fpOnTransactionAuthorizationReceived;
}

STEAMWORKSUNITY_API void* SteamUnityAPI_SteamApps()
{
	return SteamApps();
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamApps_GetCurrentGameLanguage(void* pSteamApps)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->GetCurrentGameLanguage();
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

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_GetLobbyCreatedResult(SteamAPICall_t hSteamAPICall, LobbyCreated_t &CallbackData, unsigned char &bFailed)
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

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_GetLobbyListReceivedResult(SteamAPICall_t hSteamAPICall, LobbyMatchList_t &CallbackData, unsigned char &bFailed)
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

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_GetLobbyEnteredResult(SteamAPICall_t hSteamAPICall, LobbyEnter_t &CallbackData, unsigned char &bFailed)
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

STEAMWORKSUNITY_API uint32 SteamUnityAPI_SteamUtils_GetAppID()
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

STEAMWORKSUNITY_API void SteamUnityAPI_SteamUtils_GetImageSize(int32 iIconIndex, uint32 *pnWidth, uint32 *pnHeight)
{
	SteamUtils()->GetImageSize(iIconIndex, pnWidth, pnHeight);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamUtils_GetImageRGBA(int32 iIconIndex, uint8 *pData, int32 nDataSize)
{
	SteamUtils()->GetImageRGBA(iIconIndex, pData, nDataSize);
}

STEAMWORKSUNITY_API void* SteamUnityAPI_SteamRemoteStorage()
{
	return SteamRemoteStorage();
}

STEAMWORKSUNITY_API int32 SteamUnityAPI_SteamRemoteStorage_GetFileCount(void* pSteamRemoteStorage)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->GetFileCount();
}

STEAMWORKSUNITY_API const char * SteamUnityAPI_SteamRemoteStorage_GetFileNameAndSize(void* pSteamRemoteStorage, int iFile, int32 * pnFileSizeInBytes)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->GetFileNameAndSize(iFile, pnFileSizeInBytes);
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamRemoteStorage_FileRead(void* pSteamRemoteStorage, const char *pchFile, void *pvData, int32 cubDataToRead)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FileRead(pchFile, pvData, cubDataToRead);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_WriteFile(void* pSteamRemoteStorage, const char *pchFile, void *pvData, int32 cubData)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FileWrite(pchFile, pvData, cubData);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_ForgetFile(void* pSteamRemoteStorage, const char *pchFile)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FileForget(pchFile);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_DeleteFile(void* pSteamRemoteStorage, const char *pchFile)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FileDelete(pchFile);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_FileExists(void* pSteamRemoteStorage, const char *pchFile)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FileExists(pchFile);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_FilePersisted(void* pSteamRemoteStorage, const char *pchFile)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FilePersisted(pchFile);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_GetQuota(void* pSteamRemoteStorage, int32 *pTotalSpace, int32 *pAvailableSpace)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->GetQuota(pTotalSpace, pAvailableSpace);
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



STEAMWORKSUNITY_API void* SteamUnityAPI_SteamGameServer()
{
	return SteamGameServer();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServer_Init(uint32 unIP, uint16 usMasterServerPort, uint16 usPort, uint16 usQueryPort, EServerMode eServerMode, const char *pchGameVersion)
{
	// Initialize the SteamGameServer interface, we tell it some info about us, and we request support
	// for both Authentication (making sure users own games) and secure mode, VAC running in our game
	// and kicking users who are VAC banned
	if ( !SteamGameServer_Init( unIP, usMasterServerPort, usPort, usQueryPort, eServerMode, pchGameVersion ) )
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

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamGameServer_GetSteamID(void* pSteamGameServer)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	return pISteamGameServer->GetSteamID().ConvertToUint64();
}

STEAMWORKSUNITY_API uint32 SteamUnityAPI_SteamGameServer_GetPublicIP(void* pSteamGameServer)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	return pISteamGameServer->GetPublicIP();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServer_SendUserConnectAndAuthenticate(void* pSteamGameServer, uint32 unIPClient, uint32 *pAuthTicket, uint32 authTicketSize, uint64 &steamIDClient)
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

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamGameServer_CreateUnauthenticatedUserConnection(void* pSteamGameServer)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	return pISteamGameServer->CreateUnauthenticatedUserConnection().ConvertToUint64();
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_SendUserDisconnect(void* pSteamGameServer, uint64 steamIDClient)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	return pISteamGameServer->SendUserDisconnect(steamIDClient);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_UpdateServerStatus(void* pSteamGameServer, int32 iMaxClients, int32 iBots, const char* pchServerName, const char* pchSpectatorServerName, uint16 usSpectatorPort, const char* pchRegionName, const char* pchMapName, bool bPassworded)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	pISteamGameServer->SetMaxPlayerCount(iMaxClients);
	pISteamGameServer->SetBotPlayerCount(iBots);
	pISteamGameServer->SetServerName(pchServerName);
	pISteamGameServer->SetSpectatorServerName(pchSpectatorServerName);
	pISteamGameServer->SetSpectatorPort(usSpectatorPort);
	pISteamGameServer->SetRegion(pchRegionName);
	pISteamGameServer->SetMapName(pchMapName);
	pISteamGameServer->SetPasswordProtected(bPassworded);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServer_UpdateUserData(void* pSteamGameServer, uint64 steamIDUser, const char* pchPlayerName, uint32 uiScore)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	return pISteamGameServer->BUpdateUserData(steamIDUser, pchPlayerName, uiScore);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_Shutdown()
{
	// Notify Steam master server we are going offline
	SteamGameServer()->EnableHeartbeats( false );

	// Disconnect from the steam servers
	SteamGameServer()->LogOff();

	return SteamGameServer_Shutdown();
}


STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_SetBasicServerData(void* pSteamGameServer, bool bDedicated, const char* pchGameName, const char* pchGameDesc, const char * pchModDir)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );
	
	pISteamGameServer->SetModDir(pchModDir);
	pISteamGameServer->SetDedicatedServer(bDedicated);
	pISteamGameServer->SetProduct(pchGameName);
	pISteamGameServer->SetGameDescription(pchGameDesc);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_LogOnAnonymous(void* pSteamGameServer)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	pISteamGameServer->LogOnAnonymous();
	pISteamGameServer->EnableHeartbeats( true );
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_SetKeyValues(void* pSteamGameServer, char** pKeys, char** pValues, int32 iCount)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	pISteamGameServer->ClearAllKeyValues();

	for (int i = 0; i < iCount; i++)
		pISteamGameServer->SetKeyValue(pKeys[i], pValues[i]);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_SetGameTags(void* pSteamGameServer, char* pchTags)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	pISteamGameServer->SetGameTags(pchTags);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_SetGameData(void* pSteamGameServer, char* pchData)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	pISteamGameServer->SetGameData(pchData);
}


STEAMWORKSUNITY_API void* SteamUnityAPI_SteamFriends()
{
	return SteamFriends();
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetFriendCount(void* pSteamFriends)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendCount(k_EFriendFlagImmediate);
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamFriends_GetFriendByIndex(void* pSteamFriends, int iFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendByIndex(iFriend, k_EFriendFlagImmediate).ConvertToUint64();
}

STEAMWORKSUNITY_API const char * SteamUnityAPI_SteamFriends_GetFriendPersonaName(void* pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );
	
	return pISteamFriends->GetFriendPersonaName( CSteamID( steamIDFriend) );
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetFriendPersonaState(void* pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendPersonaState( CSteamID( steamIDFriend) );
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetClanCount(void* pSteamFriends)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetClanCount();
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamFriends_GetClanByIndex(void* pSteamFriends, int iClan)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetClanByIndex(iClan).ConvertToUint64();
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamFriends_GetClanName(void* pSteamFriends, uint64 steamIDClan)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetClanName(steamIDClan);
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamFriends_GetClanTag(void* pSteamFriends, uint64 steamIDClan)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetClanTag(steamIDClan);
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetFriendCountFromSource(void* pSteamFriends, uint64 steamIDSource)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendCountFromSource( CSteamID( steamIDSource) );
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamFriends_GetFriendFromSourceByIndex(void* pSteamFriends, uint64 steamIDSource, int iFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendFromSourceByIndex(steamIDSource, iFriend).ConvertToUint64();
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetSmallFriendAvatar(void* pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );
	
	return pISteamFriends->GetSmallFriendAvatar( CSteamID(steamIDFriend) );
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetMediumFriendAvatar(void* pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );
	
	return pISteamFriends->GetMediumFriendAvatar( CSteamID(steamIDFriend) );
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetLargeFriendAvatar(void* pSteamFriends, uint64 steamIDFriend, FPOnAvatarReceived fpOnAvatarReceived)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );
	
	SteamCallbacks::getInstance().delegateOnAvatarReceived = fpOnAvatarReceived;

	return pISteamFriends->GetLargeFriendAvatar( CSteamID(steamIDFriend) );
}

STEAMWORKSUNITY_API void* SteamUnityAPI_SteamUser()
{
	return SteamUser();
}

// returns the HSteamUser this interface represents
// this is only used internally by the API, and by a few select interfaces that support multi-user
STEAMWORKSUNITY_API int SteamUnityAPI_SteamUser_GetHSteamUser(void* pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->GetHSteamUser();
}

// returns true if the Steam client current has a live connection to the Steam servers. 
// If false, it means there is no active connection due to either a networking issue on the local machine, or the Steam server is down/busy.
// The Steam client will automatically be trying to recreate the connection as often as possible.
STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUser_BLoggedOn(void* pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->BLoggedOn();
}

// returns the CSteamID of the account currently logged into the Steam client
// a CSteamID is a unique identifier for an account, and used to differentiate users in all parts of the Steamworks API
STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamUser_GetSteamID(void* pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->GetSteamID().ConvertToUint64();
}

STEAMWORKSUNITY_API uint32 SteamUnityAPI_SteamUser_InitiateGameConnection(void* pSteamUser, void *pAuthTicket, int32 cbMaxTicketSize, uint64 steamIDGameServer, uint32 unIPServer, uint16 usPortServer, bool bSecure)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->InitiateGameConnection(pAuthTicket, cbMaxTicketSize, steamIDGameServer, unIPServer, usPortServer, bSecure);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamUser_TerminateGameConnection(void* pSteamUser, uint32 unIPServer, uint16 usPortServer)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->TerminateGameConnection(unIPServer, usPortServer);
}

STEAMWORKSUNITY_API EUserHasLicenseForAppResult SteamUnityAPI_SteamUser_UserHasLicenseForApp(void* pSteamUser, uint64 steamID, AppId_t appID)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->UserHasLicenseForApp(steamID, appID);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUser_BIsBehindNAT(void* pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->BIsBehindNAT();
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamUser_AdvertiseGame(void* pSteamUser, uint64 steamIDGameServer, uint32 unServerIP, uint16 usPort)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->AdvertiseGame(steamIDGameServer, unServerIP, usPort);
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamUser_RequestEncryptedAppTicket(void* pSteamUser, void* pDataToInclude, int32 iDataLength)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->RequestEncryptedAppTicket(pDataToInclude, iDataLength);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUser_GetEncryptedAppTicket(void* pSteamUser, void* pTicket, int iMaxTicket, uint32 *pTicketSize)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->GetEncryptedAppTicket(pTicket, iMaxTicket, pTicketSize);
}


STEAMWORKSUNITY_API void* SteamUnityAPI_SteamUserStats()
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

STEAMWORKSUNITY_API const char * SteamUnityAPI_SteamUserStats_GetAchievementDisplayAttribute(void* pSteamUserStats, const char *pchName, const char *pchAttribute)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetAchievementDisplayAttribute(pchName, pchAttribute);
}

STEAMWORKSUNITY_API int32 SteamUnityAPI_SteamUserStats_GetAchievementIcon(void* pSteamUserStats, const char *pchName)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetAchievementIcon(pchName);
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

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_GetDownloadedLeaderboardEntry(void* pSteamUserStats, SteamLeaderboardEntries_t hSteamLeaderboardEntries, int32 index, LeaderboardEntry_t* outLeaderboardEntry, int32* pDetails, int cDetailsMax)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries, index, outLeaderboardEntry, pDetails, cDetailsMax);
}


STEAMWORKSUNITY_API void* SteamUnityAPI_SteamGameServerStats()
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


STEAMWORKSUNITY_API void* SteamUnityAPI_SteamMatchmaking()
{
	return SteamMatchmaking();
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamMatchmaking_CreateLobby(void* pSteamMatchmaking, ELobbyType eLobbyType, int32 cMaxMembers)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->CreateLobby(eLobbyType, cMaxMembers);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListStringFilter(void* pSteamMatchmaking, const char *pchKeyToMatch, const char *pchValueToMatch, ELobbyComparison eComparisonType)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->AddRequestLobbyListStringFilter(pchKeyToMatch, pchValueToMatch, eComparisonType);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListNumericalFilter(void* pSteamMatchmaking, const char *pchKeyToMatch, int nValueToMatch, ELobbyComparison eComparisonType)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->AddRequestLobbyListNumericalFilter(pchKeyToMatch, nValueToMatch, eComparisonType);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListNearValueFilter(void* pSteamMatchmaking, const char *pchKeyToMatch, int nValueToBeCloseTo)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->AddRequestLobbyListNearValueFilter(pchKeyToMatch, nValueToBeCloseTo);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(void* pSteamMatchmaking, int nSlotsAvailable)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->AddRequestLobbyListFilterSlotsAvailable(nSlotsAvailable);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListDistanceFilter(void* pSteamMatchmaking, ELobbyDistanceFilter eLobbyDistanceFilter)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->AddRequestLobbyListDistanceFilter(eLobbyDistanceFilter);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListResultCountFilter(void* pSteamMatchmaking, int cMaxResults)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->AddRequestLobbyListResultCountFilter(cMaxResults);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(void* pSteamMatchmaking, CSteamID steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->AddRequestLobbyListCompatibleMembersFilter(steamIDLobby);
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamMatchmaking_RequestLobbyList(void* pSteamMatchmaking)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->RequestLobbyList();
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamMatchmaking_GetLobbyByIndex(void* pSteamMatchmaking, int iLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetLobbyByIndex(iLobby).ConvertToUint64();
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamMatchmaking_JoinLobby(void* pSteamMatchmaking, uint64 steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->JoinLobby(steamIDLobby);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_LeaveLobby(void* pSteamMatchmaking, uint64 steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->LeaveLobby(steamIDLobby);
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamMatchmaking_GetNumLobbyMembers(void* pSteamMatchmaking, uint64 steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetNumLobbyMembers(steamIDLobby);
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamMatchmaking_GetLobbyMemberByIndex(void* pSteamMatchmaking, uint64 steamIDLobby, int iMember)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetLobbyMemberByIndex(steamIDLobby, iMember).ConvertToUint64();
}


MatchMakingKeyValuePair_t* g_pKeyValuePairs = NULL;

STEAMWORKSUNITY_API void* SteamUnityAPI_SteamMatchmakingServers()
{
	return SteamMatchmakingServers();
}

STEAMWORKSUNITY_API HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestInternetServerList(void* pSteamMatchmakingServers, AppId_t iApp, char** pKeys, char** pValues, uint32 uiKeyValueCount, FPOnServerResponded fpOnServerResponded, FPOnServerListComplete fpOnServerListComplete)
{
	ISteamMatchmakingServers * pISteamMatchmakingServers = static_cast<ISteamMatchmakingServers*>( pSteamMatchmakingServers );

	SteamCallbacks::getInstance().delegateOnServerResponded = fpOnServerResponded;
	SteamCallbacks::getInstance().delegateOnServerListComplete = fpOnServerListComplete;

	if (uiKeyValueCount > 0)
	{
		g_pKeyValuePairs = new MatchMakingKeyValuePair_t[uiKeyValueCount];
		for (uint32 i = 0; i < uiKeyValueCount; i++)
		{
			strcpy_s<256>(g_pKeyValuePairs[i].m_szKey, pKeys[i]);
			strcpy_s<256>(g_pKeyValuePairs[i].m_szValue, pValues[i]);
		}

		return pISteamMatchmakingServers->RequestInternetServerList(iApp, &g_pKeyValuePairs, uiKeyValueCount, &SteamCallbacks::getInstance());
	}

	return pISteamMatchmakingServers->RequestInternetServerList(iApp, NULL, 0, &SteamCallbacks::getInstance());
}

STEAMWORKSUNITY_API HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestLANServerList(void* pSteamMatchmakingServers, AppId_t iApp, FPOnServerResponded fpOnServerResponded, FPOnServerListComplete fpOnServerListComplete)
{
	ISteamMatchmakingServers * pISteamMatchmakingServers = static_cast<ISteamMatchmakingServers*>( pSteamMatchmakingServers );

	SteamCallbacks::getInstance().delegateOnServerResponded = fpOnServerResponded;
	SteamCallbacks::getInstance().delegateOnServerListComplete = fpOnServerListComplete;

	return pISteamMatchmakingServers->RequestLANServerList(iApp, &SteamCallbacks::getInstance());
}

STEAMWORKSUNITY_API HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestSpectatorServerList(void* pSteamMatchmakingServers, AppId_t iApp, char** pKeys, char** pValues, uint32 uiKeyValueCount, FPOnServerResponded fpOnServerResponded, FPOnServerListComplete fpOnServerListComplete)
{
	ISteamMatchmakingServers * pISteamMatchmakingServers = static_cast<ISteamMatchmakingServers*>( pSteamMatchmakingServers );

	SteamCallbacks::getInstance().delegateOnServerResponded = fpOnServerResponded;
	SteamCallbacks::getInstance().delegateOnServerListComplete = fpOnServerListComplete;

	if (uiKeyValueCount > 0)
	{
		g_pKeyValuePairs = new MatchMakingKeyValuePair_t[uiKeyValueCount];
		for (uint32 i = 0; i < uiKeyValueCount; i++)
		{
			strcpy_s<256>(g_pKeyValuePairs[i].m_szKey, pKeys[i]);
			strcpy_s<256>(g_pKeyValuePairs[i].m_szValue, pValues[i]);
		}

		return pISteamMatchmakingServers->RequestSpectatorServerList(iApp, &g_pKeyValuePairs, uiKeyValueCount, &SteamCallbacks::getInstance());
	}

	return pISteamMatchmakingServers->RequestSpectatorServerList(iApp, NULL, 0, &SteamCallbacks::getInstance());
}

STEAMWORKSUNITY_API HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestHistoryServerList(void* pSteamMatchmakingServers, AppId_t iApp, char** pKeys, char** pValues, uint32 uiKeyValueCount, FPOnServerResponded fpOnServerResponded, FPOnServerListComplete fpOnServerListComplete)
{
	ISteamMatchmakingServers * pISteamMatchmakingServers = static_cast<ISteamMatchmakingServers*>( pSteamMatchmakingServers );

	SteamCallbacks::getInstance().delegateOnServerResponded = fpOnServerResponded;
	SteamCallbacks::getInstance().delegateOnServerListComplete = fpOnServerListComplete;

	if (uiKeyValueCount > 0)
	{
		g_pKeyValuePairs = new MatchMakingKeyValuePair_t[uiKeyValueCount];
		for (uint32 i = 0; i < uiKeyValueCount; i++)
		{
			strcpy_s<256>(g_pKeyValuePairs[i].m_szKey, pKeys[i]);
			strcpy_s<256>(g_pKeyValuePairs[i].m_szValue, pValues[i]);
		}

		return pISteamMatchmakingServers->RequestHistoryServerList(iApp, &g_pKeyValuePairs, uiKeyValueCount, &SteamCallbacks::getInstance());
	}

	return pISteamMatchmakingServers->RequestHistoryServerList(iApp, NULL, 0, &SteamCallbacks::getInstance());
}

STEAMWORKSUNITY_API HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestFavoriteServerList(void* pSteamMatchmakingServers, AppId_t iApp, char** pKeys, char** pValues, uint32 uiKeyValueCount, FPOnServerResponded fpOnServerResponded, FPOnServerListComplete fpOnServerListComplete)
{
	ISteamMatchmakingServers * pISteamMatchmakingServers = static_cast<ISteamMatchmakingServers*>( pSteamMatchmakingServers );

	SteamCallbacks::getInstance().delegateOnServerResponded = fpOnServerResponded;
	SteamCallbacks::getInstance().delegateOnServerListComplete = fpOnServerListComplete;

	if (uiKeyValueCount > 0)
	{
		g_pKeyValuePairs = new MatchMakingKeyValuePair_t[uiKeyValueCount];
		for (uint32 i = 0; i < uiKeyValueCount; i++)
		{
			strcpy_s<256>(g_pKeyValuePairs[i].m_szKey, pKeys[i]);
			strcpy_s<256>(g_pKeyValuePairs[i].m_szValue, pValues[i]);
		}

		return pISteamMatchmakingServers->RequestFavoritesServerList(iApp, &g_pKeyValuePairs, uiKeyValueCount, &SteamCallbacks::getInstance());
	}

	return pISteamMatchmakingServers->RequestFavoritesServerList(iApp, NULL, 0, &SteamCallbacks::getInstance());
}

STEAMWORKSUNITY_API HServerListRequest SteamUnityAPI_SteamMatchmakingServers_RequestFriendServerList(void* pSteamMatchmakingServers, AppId_t iApp, char** pKeys, char** pValues, uint32 uiKeyValueCount, FPOnServerResponded fpOnServerResponded, FPOnServerListComplete fpOnServerListComplete)
{
	ISteamMatchmakingServers * pISteamMatchmakingServers = static_cast<ISteamMatchmakingServers*>( pSteamMatchmakingServers );

	SteamCallbacks::getInstance().delegateOnServerResponded = fpOnServerResponded;
	SteamCallbacks::getInstance().delegateOnServerListComplete = fpOnServerListComplete;

	if (uiKeyValueCount > 0)
	{
		g_pKeyValuePairs = new MatchMakingKeyValuePair_t[uiKeyValueCount];
		for (uint32 i = 0; i < uiKeyValueCount; i++)
		{
			strcpy_s<256>(g_pKeyValuePairs[i].m_szKey, pKeys[i]);
			strcpy_s<256>(g_pKeyValuePairs[i].m_szValue, pValues[i]);
		}

		return pISteamMatchmakingServers->RequestFriendsServerList(iApp, &g_pKeyValuePairs, uiKeyValueCount, &SteamCallbacks::getInstance());
	}

	return pISteamMatchmakingServers->RequestFriendsServerList(iApp, NULL, 0, &SteamCallbacks::getInstance());
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmakingServers_ReleaseRequest(void* pSteamMatchmakingServers, HServerListRequest hRequest)
{
	ISteamMatchmakingServers * pISteamMatchmakingServers = static_cast<ISteamMatchmakingServers*>( pSteamMatchmakingServers );

	if (g_pKeyValuePairs)
	{
		delete [] g_pKeyValuePairs;
		g_pKeyValuePairs = NULL;
	}

	return pISteamMatchmakingServers->ReleaseRequest(hRequest);
}


STEAMWORKSUNITY_API void* SteamUnityAPI_SteamNetworking()
{
	return SteamNetworking();
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamNetworking_SetCallbacks(FPOnNetworkP2PSessionRequest fpOnP2PSessionRequest, FPOnNetworkP2PSessionConnectFailed fpOnP2PSessionConnectFailed)
{
	SteamCallbacks::getInstance().delegateOnNetworkP2PSessionRequest = fpOnP2PSessionRequest;
	SteamCallbacks::getInstance().delegateOnNetworkP2PSessionConnectFailed = fpOnP2PSessionConnectFailed;
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamNetworking_AllowP2PPacketRelay(void* pSteamNetworking, bool bAllow)
{
	ISteamNetworking * pISteamNetworking = static_cast<ISteamNetworking*>( pSteamNetworking );

	pISteamNetworking->AllowP2PPacketRelay(bAllow);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamNetworking_AcceptP2PSessionWithUser(void* pSteamNetworking, uint64 steamID)
{
	ISteamNetworking * pISteamNetworking = static_cast<ISteamNetworking*>( pSteamNetworking );

	pISteamNetworking->AcceptP2PSessionWithUser(steamID);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamNetworking_SendP2PPacket(void* pSteamNetworking, uint64 steamID, void *pData, uint32 uiDataLength, uint8 eP2PSendType, int nChannel)
{
	ISteamNetworking * pISteamNetworking = static_cast<ISteamNetworking*>( pSteamNetworking );

	return pISteamNetworking->SendP2PPacket(steamID, pData, uiDataLength, (EP2PSend)eP2PSendType, nChannel);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamNetworking_IsP2PPacketAvailable(void* pSteamNetworking, uint32 *pPacketSize, int32 nChannel)
{
	ISteamNetworking * pISteamNetworking = static_cast<ISteamNetworking*>( pSteamNetworking );

	return pISteamNetworking->IsP2PPacketAvailable(pPacketSize, nChannel);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamNetworking_ReadP2PPacket(void* pSteamNetworking, void* data, uint32 uiPacketSize, uint64 *pSteamID, int32 nChannel)
{
	ISteamNetworking * pISteamNetworking = static_cast<ISteamNetworking*>( pSteamNetworking );

	CSteamID steamID;
	
	if (pISteamNetworking->ReadP2PPacket(data, uiPacketSize, &uiPacketSize, &steamID, nChannel))
	{
		*pSteamID = steamID.ConvertToUint64();
		return true;
	}

	return false;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamNetworking_CloseP2PSession(void* pSteamNetworking, uint64 steamID)
{
	ISteamNetworking * pISteamNetworking = static_cast<ISteamNetworking*>( pSteamNetworking );

	return pISteamNetworking->CloseP2PSessionWithUser(steamID);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamNetworking_CloseP2PChannel(void* pSteamNetworking, uint64 steamID, int32 nChannel)
{
	ISteamNetworking * pISteamNetworking = static_cast<ISteamNetworking*>( pSteamNetworking );

	return pISteamNetworking->CloseP2PChannelWithUser(steamID, nChannel);
}

		
// Steam Callbacks are used to bridge Steam's responses back to our app
void SteamCallbacks::OnAvatarReceived(AvatarImageLoaded_t *pCallbackData)
{
	delegateOnAvatarReceived(pCallbackData);
}

void SteamCallbacks::OnUserStatsReceived(UserStatsReceived_t *pCallbackData)
{
	delegateOnUserStatsReceived(pCallbackData);
}

void SteamCallbacks::OnTransactionAuthorizationReceived(MicroTxnAuthorizationResponse_t *pCallbackData)
{
	delegateOnTransactionAuthorizationReceived(pCallbackData);
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

void SteamCallbacks::OnLeaderboardEntriesRetrieved(LeaderboardScoresDownloaded_t *pCallbackData, bool bIOFailure)
{
	delegateOnLeaderboardEntriesRetrieved(pCallbackData);
}

void SteamCallbacks::ServerResponded(HServerListRequest hRequest, int iServer)
{
	gameserveritem_t* callbackData = SteamMatchmakingServers()->GetServerDetails(hRequest, iServer);

	delegateOnServerResponded(hRequest, callbackData);
}

void SteamCallbacks::ServerFailedToRespond(HServerListRequest hRequest, int iServer)
{
	gameserveritem_t* callbackData = SteamMatchmakingServers()->GetServerDetails(hRequest, iServer);

	printf("");
}

void SteamCallbacks::RefreshComplete(HServerListRequest hRequest, EMatchMakingServerResponse response)
{
	if (g_pKeyValuePairs)
	{
		delete [] g_pKeyValuePairs;
		g_pKeyValuePairs = NULL;
	}

	delegateOnServerListComplete(hRequest);
}
