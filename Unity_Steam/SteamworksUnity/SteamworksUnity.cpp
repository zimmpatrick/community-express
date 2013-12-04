// SteamworksUnity.cpp : Defines the exported functions for the DLL application.
// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

#ifdef _WIN32
#include "stdafx.h"
#endif

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
#ifdef _WIN32
	#ifdef STEAMWORKSUNITY_EXPORTS
		#define STEAMWORKSUNITY_API extern "C" __declspec(dllexport)
	#else
		#define STEAMWORKSUNITY_API extern "C" __declspec(dllimport)
	#endif
#elif defined(__APPLE__)
	#ifdef STEAMWORKSUNITY_EXPORTS
		#define STEAMWORKSUNITY_API extern "C" __attribute__((visibility("default")))
	#else
		#define STEAMWORKSUNITY_API extern "C"
	#endif

	#define strcpy_s(x,y,z)		strncpy(x,z,y); x[y-1]='\0';
	#define OutputDebugString	printf
	#define TEXT(x)				x
#endif

typedef uint64 (ZIMM_STDCALL *FPOnChallengeResponse)(uint64 challenge);

typedef void (ZIMM_STDCALL *FSteamAPIDebugTextHook)(int nSeverity, const char *pchDebugText);
typedef void (ZIMM_CDECL *FSteamAPIDebugTextHookCD)(int nSeverity, const char *pchDebugText);

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

#ifdef _WIN32
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
#endif

CSteamAPIContext context;
CSteamGameServerAPIContext serverContext;

STEAMWORKSUNITY_API bool SteamUnityAPI_Init(FPOnChallengeResponse pFPOnChallengeResponse,
                                            FPOnCallback pFPOnCallback)
{
#ifdef _WIN32
	/* hLicenseThread = CreateThread(
		NULL,				   // default security attributes
		0,					  // use default stack size  
		MyLicenseThreadFunction,	   // thread function name
		pFPOnChallengeResponse,		// argument to thread function 
		0,					  // use default creation flags 
		NULL);   // returns the thread identifier 
		*/
#endif
	
	SteamCallbacks::getInstance().delegateOnCallback = pFPOnCallback;
    
	if (SteamAPI_InitSafe())
	{
    	return context.Init();
	}

	return false;
}

// STEAMWORKSUNITY_API void SteamUnityAPI_Init(); (see below)
STEAMWORKSUNITY_API void SteamUnityAPI_Shutdown()
{
#if _WIN32
	if (hLicenseThread)
	{
		// TODO, clean this up to pass events, etc
		TerminateThread(hLicenseThread, 0);
		hLicenseThread = 0;
	}
#endif

	context.Clear();
    SteamAPI_Shutdown();
    
    SteamCallbacks::getInstance().delegateOnCallback = NULL;
}

FSteamAPIDebugTextHook hook = NULL;

//-----------------------------------------------------------------------------
// Purpose: callback hook for debug text emitted from the Steam API
//-----------------------------------------------------------------------------
extern "C" void ZIMM_CDECL CSteamAPIDebugTextHook( int nSeverity, const char *pchDebugText )
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

STEAMWORKSUNITY_API void SteamUnityAPI_WriteMiniDump(uint32 unExceptionCode, void* pExceptionInfo, uint32 unBuildID)
{
	SteamAPI_WriteMiniDump(unExceptionCode, pExceptionInfo, unBuildID);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SetMiniDumpComment(const char* pchComment)
{
	SteamAPI_SetMiniDumpComment(pchComment);
}

STEAMWORKSUNITY_API void SteamUnityAPI_RunCallbacks()
{
	return SteamAPI_RunCallbacks();
}

STEAMWORKSUNITY_API const char * SteamUnityAPI_GetPersonaNameByID(uint64 steamIDFriend)
{
	return context.SteamFriends()->GetFriendPersonaName( CSteamID(steamIDFriend) );
}

STEAMWORKSUNITY_API void* SteamUnityAPI_SteamApps()
{
	return context.SteamApps();
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamApps_GetCurrentGameLanguage(void* pSteamApps)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->GetCurrentGameLanguage();
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamApps_GetAvailableGameLanguages(void* pSteamApps)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->GetAvailableGameLanguages();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamApps_BIsSubscribedApp(void* pSteamApps, AppId_t iApp)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->BIsSubscribedApp(iApp);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamApps_BIsDlcInstalled(void* pSteamApps, AppId_t iApp)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->BIsDlcInstalled(iApp);
}

STEAMWORKSUNITY_API uint32 SteamUnityAPI_SteamApps_GetEarliestPurchaseUnixTime(void* pSteamApps, AppId_t iApp)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->GetEarliestPurchaseUnixTime(iApp);
}

STEAMWORKSUNITY_API int32 SteamUnityAPI_SteamApps_GetDLCCount(void* pSteamApps)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->GetDLCCount();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamApps_BGetDLCDataByIndex(void* pSteamApps, int32 iIndex, AppId_t* pAppID, bool* pbAvailable, char* pchName, int32 cchNameBufferSize)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->BGetDLCDataByIndex(iIndex, pAppID, pbAvailable, pchName, cchNameBufferSize);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamApps_InstallDLC(void* pSteamApps, AppId_t iApp)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	pISteamApps->InstallDLC(iApp);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamApps_UninstallDLC(void* pSteamApps, AppId_t iApp)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	pISteamApps->UninstallDLC(iApp);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamApps_GetCurrentBetaName(void* pSteamApps, char* pchName, int32 cchNameBufferSize)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->GetCurrentBetaName(pchName, cchNameBufferSize);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamApps_MarkContentCorrupt(void* pSteamApps, bool bMissingFilesOnly)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->MarkContentCorrupt(bMissingFilesOnly);
}

STEAMWORKSUNITY_API uint32 SteamUnityAPI_SteamApps_GetAppInstallDir(void* pSteamApps, AppId_t iApp, char* pchDirectory, int32 cchDirectoryBufferSize)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->GetAppInstallDir(iApp, pchDirectory, cchDirectoryBufferSize);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamApps_BIsSubscribed(void* pSteamApps)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->BIsSubscribed();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamApps_BIsSubscribedFromFreeWeekend(void* pSteamApps)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->BIsSubscribedFromFreeWeekend();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamApps_BIsLowViolence(void* pSteamApps)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->BIsLowViolence();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamApps_BIsCybercafe(void* pSteamApps)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->BIsCybercafe();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamApps_BIsVACBanned(void* pSteamApps)
{
	ISteamApps * pISteamApps = static_cast<ISteamApps*>( pSteamApps );

	return pISteamApps->BIsVACBanned();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_IsAPICallCompleted(SteamAPICall_t hSteamAPICall, unsigned char &bFailed)
{
	bool result, failed;

	if (serverContext.SteamGameServer())
	{
		result = serverContext.SteamGameServerUtils()->IsAPICallCompleted(hSteamAPICall, &failed);
	}
	else
	{
		result = context.SteamUtils()->IsAPICallCompleted(hSteamAPICall, &failed);
	}

	bFailed = failed ? 1 : 0;

	return result;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_GetAPICallResult(SteamAPICall_t hSteamAPICall, void *pCallback, int cubCallback, int iCallbackExpected, unsigned char &bFailed)
{
	bool result, failed;

	if (serverContext.SteamGameServer())
	{
		result = serverContext.SteamGameServerUtils()->GetAPICallResult(hSteamAPICall, pCallback, cubCallback, iCallbackExpected, &failed);
	}
	else
	{
		result = context.SteamUtils()->GetAPICallResult(hSteamAPICall, pCallback, cubCallback, iCallbackExpected, &failed);
	}

	bFailed = failed ? 1 : 0;

	return result;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_GetGameServerUserStatsReceivedResult(SteamAPICall_t hSteamAPICall, GSStatsReceived_t &CallbackData, unsigned char &bFailed)
{
	bool result, failed;

	if (serverContext.SteamGameServer())
	{
		result = serverContext.SteamGameServerUtils()->GetAPICallResult(hSteamAPICall, &CallbackData, sizeof(CallbackData), CallbackData.k_iCallback, &failed);
	}
	else
	{
		result = context.SteamUtils()->GetAPICallResult(hSteamAPICall, &CallbackData, sizeof(CallbackData), CallbackData.k_iCallback, &failed);
	}

	bFailed = failed ? 1 : 0;

	return result;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_GetLobbyCreatedResult(SteamAPICall_t hSteamAPICall, LobbyCreated_t &CallbackData, unsigned char &bFailed)
{
	bool result, failed;

	if (serverContext.SteamGameServer())
	{
		result = serverContext.SteamGameServerUtils()->GetAPICallResult(hSteamAPICall, &CallbackData, sizeof(CallbackData), CallbackData.k_iCallback, &failed);
	}
	else
	{
		result = context.SteamUtils()->GetAPICallResult(hSteamAPICall, &CallbackData, sizeof(CallbackData), CallbackData.k_iCallback, &failed);
	}

	bFailed = failed ? 1 : 0;

	return result;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_GetLobbyListReceivedResult(SteamAPICall_t hSteamAPICall, LobbyMatchList_t &CallbackData, unsigned char &bFailed)
{
	bool result, failed;

	if (serverContext.SteamGameServer())
	{
		result = serverContext.SteamGameServerUtils()->GetAPICallResult(hSteamAPICall, &CallbackData, sizeof(CallbackData), CallbackData.k_iCallback, &failed);
	}
	else
	{
		result = context.SteamUtils()->GetAPICallResult(hSteamAPICall, &CallbackData, sizeof(CallbackData), CallbackData.k_iCallback, &failed);
	}

	bFailed = failed ? 1 : 0;

	return result;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_GetLobbyEnteredResult(SteamAPICall_t hSteamAPICall, LobbyEnter_t &CallbackData, unsigned char &bFailed)
{
	bool result, failed;

	if (serverContext.SteamGameServer())
	{
		result = serverContext.SteamGameServerUtils()->GetAPICallResult(hSteamAPICall, &CallbackData, sizeof(CallbackData), CallbackData.k_iCallback, &failed);
	}
	else
	{
		result = context.SteamUtils()->GetAPICallResult(hSteamAPICall, &CallbackData, sizeof(CallbackData), CallbackData.k_iCallback, &failed);
	}

	bFailed = failed ? 1 : 0;

	return result;
}

STEAMWORKSUNITY_API AppId_t SteamUnityAPI_SteamUtils_GetAppID()
{
	if (serverContext.SteamGameServer())
	{
		return serverContext.SteamGameServerUtils()->GetAppID();
	}
	else
	{
		return context.SteamUtils()->GetAppID();
	}
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamUtils_GetImageSize(int32 iIconIndex, uint32 *pnWidth, uint32 *pnHeight)
{
	context.SteamUtils()->GetImageSize(iIconIndex, pnWidth, pnHeight);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamUtils_GetImageRGBA(int32 iIconIndex, uint8 *pData, int32 nDataSize)
{
	context.SteamUtils()->GetImageRGBA(iIconIndex, pData, nDataSize);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_ShowGamepadTextInput(EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, const char *pchDescription, uint32 unCharMax)
{
	return context.SteamUtils()->ShowGamepadTextInput(eInputMode, eLineInputMode, pchDescription, unCharMax);
}

STEAMWORKSUNITY_API uint32 SteamUnityAPI_SteamUtils_GetEnteredGamepadTextLength()
{
	return context.SteamUtils()->GetEnteredGamepadTextLength();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUtils_GetEnteredGamepadTextInput(char* pchText, int32 cchText)
{
	return context.SteamUtils()->GetEnteredGamepadTextInput(pchText, cchText);
}

STEAMWORKSUNITY_API void* SteamUnityAPI_SteamRemoteStorage()
{
	return context.SteamRemoteStorage();
}

STEAMWORKSUNITY_API int32 SteamUnityAPI_SteamRemoteStorage_GetFileCount(void* pSteamRemoteStorage)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->GetFileCount();
}

STEAMWORKSUNITY_API int32 SteamUnityAPI_SteamRemoteStorage_GetFileSize(void* pSteamRemoteStorage, const char *pchFile)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->GetFileSize(pchFile);
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

STEAMWORKSUNITY_API UGCFileWriteStreamHandle_t SteamUnityAPI_SteamRemoteStorage_FileWriteStreamOpen(void* pSteamRemoteStorage, const char *pchFile)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FileWriteStreamOpen(pchFile);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_FileWriteStreamWriteChunk(void* pSteamRemoteStorage, UGCFileWriteStreamHandle_t writeHandle, void *pvData, int32 cubData)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FileWriteStreamWriteChunk(writeHandle, pvData, cubData);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_FileWriteStreamClose(void* pSteamRemoteStorage, UGCFileWriteStreamHandle_t writeHandle)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FileWriteStreamClose(writeHandle);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_FileWriteStreamCancel(void* pSteamRemoteStorage, UGCFileWriteStreamHandle_t writeHandle)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->FileWriteStreamCancel(writeHandle);
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

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_FileShare(void* pSteamRemoteStorage, const char *pchFile)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );
	
	SteamAPICall_t hAPICall = pISteamRemoteStorage->FileShare(pchFile);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageFileShareResult, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_GetQuota(void* pSteamRemoteStorage, int32 *pTotalSpace, int32 *pAvailableSpace)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->GetQuota(pTotalSpace, pAvailableSpace);
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_EnumerateUserSubscribedFiles(void* pSteamRemoteStorage, uint32 unStartIndex)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->EnumerateUserSubscribedFiles( unStartIndex );
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageEnumerateUserSubscribedFilesResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_EnumerateUserPublishedFiles(void* pSteamRemoteStorage, uint32 unStartIndex)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->EnumerateUserPublishedFiles( unStartIndex );
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageEnumerateUserPublishedFilesResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_SubscribePublishedFile(void* pSteamRemoteStorage, uint64 unPublishedFileId)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->SubscribePublishedFile( unPublishedFileId );
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageSubscribePublishedFileResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_UnsubscribePublishedFile(void* pSteamRemoteStorage, uint64 unPublishedFileId)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->UnsubscribePublishedFile( unPublishedFileId );
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageUnsubscribePublishedFileResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_GetPublishedItemVoteDetails(void* pSteamRemoteStorage, uint64 unPublishedFileId)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->GetPublishedItemVoteDetails( unPublishedFileId );
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageGetPublishedItemVoteDetailsResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_UpdateUserPublishedItemVote(void* pSteamRemoteStorage, uint64 unPublishedFileId, bool bVoteUp)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->UpdateUserPublishedItemVote( unPublishedFileId, bVoteUp );
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageSubscribePublishedFileResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_GetUserPublishedItemVoteDetails(void* pSteamRemoteStorage, uint64 unPublishedFileId)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->GetUserPublishedItemVoteDetails( unPublishedFileId);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageSubscribePublishedFileResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_EnumerateUserSharedWorkshopFiles(void* pSteamRemoteStorage, uint64 steamId, uint32 unStartIndex, SteamParamStringArray_t requiredTags, SteamParamStringArray_t excludedTags )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->EnumerateUserSharedWorkshopFiles( CSteamID( steamId ), unStartIndex, &requiredTags, &excludedTags);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageEnumerateUserSharedWorkshopFilesResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_PublishVideo(void* pSteamRemoteStorage,  EWorkshopVideoProvider eVideoProvider, const char *pchVideoAccount, const char *pchVideoIdentifier, const char *pchPreviewFile, AppId_t nConsumerAppId, const char *pchTitle, const char *pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, SteamParamStringArray_t *pTags )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->PublishVideo( eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageSubscribePublishedFileResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_SetUserPublishedFileAction(void* pSteamRemoteStorage, PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->SetUserPublishedFileAction( unPublishedFileId, eAction);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageSubscribePublishedFileResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedFilesByUserAction(void* pSteamRemoteStorage, EWorkshopFileAction eAction, uint32 unStartIndex )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->EnumeratePublishedFilesByUserAction( eAction, unStartIndex);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageSubscribePublishedFileResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_UGCDownloadToLocation(void* pSteamRemoteStorage, UGCHandle_t hContent, const char *pchLocation, uint32 unPriority )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->UGCDownloadToLocation( hContent, pchLocation, unPriority);
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_CommitPublishedFileUpdate(void* pSteamRemoteStorage, PublishedFileUpdateHandle_t updateHandle )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->CommitPublishedFileUpdate(updateHandle);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageUpdatePublishedFileResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_GetPublishedFileDetails(void* pSteamRemoteStorage, PublishedFileId_t unPublishedFileId )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->GetPublishedFileDetails(unPublishedFileId);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageGetPublishedFileDetailsResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_DeletePublishedFile(void* pSteamRemoteStorage, PublishedFileId_t unPublishedFileId )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->DeletePublishedFile(unPublishedFileId);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageSubscribePublishedFileResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_PublishWorkshopFile(void* pSteamRemoteStorage, const char *pchFile, const char *pchPreviewFile, AppId_t nConsumerAppId, const char *pchTitle, const char *pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, SteamParamStringArray_t pTags, EWorkshopFileType eWorkshopFileType )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->PublishWorkshopFile(pchFile, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, &pTags, eWorkshopFileType);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStoragePublishFileResult, hAPICall );
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStoragePublishFileProgress, hAPICall );
	return hAPICall;
}

STEAMWORKSUNITY_API PublishedFileUpdateHandle_t SteamUnityAPI_SteamRemoteStorage_CreatePublishedFileUpdateRequest(void* pSteamRemoteStorage, PublishedFileUpdateHandle_t updateHandle )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	PublishedFileUpdateHandle_t hAPICall = pISteamRemoteStorage->CreatePublishedFileUpdateRequest(updateHandle);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageSubscribePublishedFileResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileFile(void* pSteamRemoteStorage, PublishedFileUpdateHandle_t updateHandle, const char *pchFile )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->UpdatePublishedFileFile(updateHandle, pchFile);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFilePreviewFile(void* pSteamRemoteStorage, PublishedFileUpdateHandle_t updateHandle, const char *pchPreviewFile )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->UpdatePublishedFilePreviewFile(updateHandle, pchPreviewFile);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileTitle(void* pSteamRemoteStorage, PublishedFileUpdateHandle_t updateHandle, const char *pchTitle )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->UpdatePublishedFileTitle(updateHandle, pchTitle);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileDescription(void* pSteamRemoteStorage, PublishedFileUpdateHandle_t updateHandle, const char *pchDescription )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->UpdatePublishedFileDescription(updateHandle, pchDescription);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileVisibility(void* pSteamRemoteStorage, PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->UpdatePublishedFileVisibility(updateHandle, eVisibility);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileTags(void* pSteamRemoteStorage, PublishedFileUpdateHandle_t updateHandle, SteamParamStringArray_t pTags )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	return pISteamRemoteStorage->UpdatePublishedFileTags(updateHandle, &pTags);
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedWorkshopFiles(void* pSteamRemoteStorage, EWorkshopEnumerationType eEnumerationType, uint32 unStartIndex, uint32 unCount, uint32 unDays, SteamParamStringArray_t tags, SteamParamStringArray_t userTags)
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->EnumeratePublishedWorkshopFiles(eEnumerationType, unStartIndex, unCount, unDays, &tags, &userTags);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageEnumerateWorkshopFilesResult, hAPICall );
	
	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamRemoteStorage_UGCDownload(void* pSteamRemoteStorage, UGCHandle_t hContent, uint32 unPriority )
{
	ISteamRemoteStorage * pISteamRemoteStorage = static_cast<ISteamRemoteStorage*>( pSteamRemoteStorage );

	SteamAPICall_t hAPICall = pISteamRemoteStorage->UGCDownload(hContent, unPriority);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().RemoteStorageDownloadUGCResult, hAPICall );
	
	return hAPICall;
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
	return serverContext.SteamGameServer();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamGameServer_Init(uint32 unIP, uint16 usMasterServerPort, uint16 usPort, uint16 usQueryPort, EServerMode eServerMode, const char *pchGameVersion)
{
	// Initialize the SteamGameServer interface, we tell it some info about us, and we request support
	// for both Authentication (making sure users own games) and secure mode, VAC running in our game
	// and kicking users who are VAC banned
	if ( !SteamGameServer_InitSafe( unIP, usMasterServerPort, usPort, usQueryPort, eServerMode, pchGameVersion ) )
	{
		OutputDebugString(TEXT("SteamGameServer_Init call failed\n") );
		return false;
	}

	return serverContext.Init();
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_RunCallbacks()
{
	SteamGameServer_RunCallbacks();
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
	serverContext.SteamGameServer()->EnableHeartbeats( false );

	// Disconnect from the steam servers
	serverContext.SteamGameServer()->LogOff();
	
	serverContext.Clear();
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

STEAMWORKSUNITY_API void SteamUnityAPI_SteamGameServer_GetGameplayStats(void* pSteamGameServer)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	pISteamGameServer->GetGameplayStats();
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamGameServer_GetServerReputation(void* pSteamGameServer)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	SteamAPICall_t hAPICall = pISteamGameServer->GetServerReputation();
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().GSReputation, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamGameServer_AssociateWithClan(void* pSteamGameServer, uint64 steamIDOfClan)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	SteamAPICall_t hAPICall = pISteamGameServer->AssociateWithClan(steamIDOfClan);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().AssociateWithClanResult, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamGameServer_ComputeNewPlayerCompatibility(void* pSteamGameServer, uint64 steamIDNewPlayer)
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	SteamAPICall_t hAPICall = pISteamGameServer->ComputeNewPlayerCompatibility(steamIDNewPlayer);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().ComputeNewPlayerCompatibilityResult, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamGameServer_RequestUserGroupStatus(void* pSteamGameServer, uint64 steamIDUser, uint64 steamIDGroup )
{
	ISteamGameServer * pISteamGameServer = static_cast<ISteamGameServer*>( pSteamGameServer );

	SteamAPICall_t hAPICall = pISteamGameServer->RequestUserGroupStatus(steamIDUser, steamIDGroup);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().GSClientGroupStatus, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API void* SteamUnityAPI_SteamFriends()
{
	return context.SteamFriends();
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetFriendCount(void* pSteamFriends, int iFriendFlags)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendCount(iFriendFlags);
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamFriends_GetFriendByIndex(void* pSteamFriends, int iFriend, int iFriendFlags)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendByIndex(iFriend, iFriendFlags).ConvertToUint64();
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

STEAMWORKSUNITY_API int SteamUnityAPI_SteamFriends_GetLargeFriendAvatar(void* pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );
	
	return pISteamFriends->GetLargeFriendAvatar( CSteamID(steamIDFriend) );
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamFriends_RequestFriendRichPresence(void* pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );
	
	pISteamFriends->RequestFriendRichPresence( CSteamID(steamIDFriend) );
}

STEAMWORKSUNITY_API AppId_t SteamUnityAPI_SteamFriends_GetFriendCoplayGame(void* pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return (AppId_t)pISteamFriends->GetFriendCoplayGame( CSteamID(steamIDFriend) );
}

STEAMWORKSUNITY_API FriendGameInfo_t SteamUnityAPI_SteamFriends_GetFriendGamePlayed(void* pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	FriendGameInfo_t friendGameInfo;
	pISteamFriends->GetFriendGamePlayed( CSteamID(steamIDFriend), &friendGameInfo);
	return friendGameInfo;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamFriends_RequestClanOfficerList(void* pSteamFriends, uint64 steamIDClan)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	SteamAPICall_t hAPICall = pISteamFriends->RequestClanOfficerList(steamIDClan);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().ClanOfficerListResponse, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamFriends_SetRichPresence(void* pSteamFriends, const char* pchKey, const char* pchValue)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	bool bSuccess = pISteamFriends->SetRichPresence(pchKey, pchValue);
	return bSuccess;
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamFriends_ClearRichPresence(void* pSteamFriends)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	pISteamFriends->ClearRichPresence();
}

STEAMWORKSUNITY_API const char * SteamUnityAPI_GetFriendRichPresence(void* pSteamFriends, uint64 steamIDFriend, const char *pchKey)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendRichPresence( CSteamID(steamIDFriend), pchKey);
}

STEAMWORKSUNITY_API int SteamUnityAPI_GetFriendRichPresenceKeyCount(void* pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendRichPresenceKeyCount( CSteamID(steamIDFriend));
}

STEAMWORKSUNITY_API const char * SteamUnityAPI_GetFriendRichPresenceKeyByIndex(void* pSteamFriends, uint64 steamIDFriend, int pchKeyIndex)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	return pISteamFriends->GetFriendRichPresenceKeyByIndex( CSteamID(steamIDFriend), pchKeyIndex);
}

STEAMWORKSUNITY_API void SteamUnityAPI_RequestFriendRichPresence(void* pSteamFriends, uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	pISteamFriends->RequestFriendRichPresence( CSteamID(steamIDFriend));
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamFriends_InviteUserToGame(void* pSteamFriends,  uint64 steamIDFriend, const char *pchConnectString)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	bool bSuccess = pISteamFriends->InviteUserToGame( CSteamID(steamIDFriend), pchConnectString);
	return bSuccess;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamFriends_DownloadClanActivityCounts(void* pSteamFriends,  uint64 psteamIDClans[], int cClansToRequest)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	CSteamID * aSteamIDClans = new CSteamID[cClansToRequest];
	for(int i=0;i<cClansToRequest;i++)
	{
		aSteamIDClans[i].SetFromUint64(psteamIDClans[i]);
	}

	SteamAPICall_t hAPICall = pISteamFriends->DownloadClanActivityCounts(aSteamIDClans, cClansToRequest);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().DownloadClanActivityCountsResult, hAPICall );

	delete[] aSteamIDClans;
	aSteamIDClans = 0;

	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamFriends_GetFollowerCount(void* pSteamFriends,  uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	SteamAPICall_t hAPICall = pISteamFriends->GetFollowerCount(steamIDFriend);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().FriendsGetFollowerCount, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamFriends_IsFollowing(void* pSteamFriends,  uint64 steamIDFriend)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	SteamAPICall_t hAPICall = pISteamFriends->IsFollowing(steamIDFriend);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().FriendsIsFollowing, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamFriends_EnumerateFollowingList(void* pSteamFriends,  uint32 startIndex)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	SteamAPICall_t hAPICall = pISteamFriends->EnumerateFollowingList(startIndex);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().FriendsEnumerateFollowingList, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamFriends_ActivateGameOverlay(void* pSteamFriends, const char* pchDialog)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	pISteamFriends->ActivateGameOverlay(pchDialog);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamFriends_ActivateGameOverlayToUser(void* pSteamFriends, const char* pchDialog, uint64 steamIDUser)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	pISteamFriends->ActivateGameOverlayToUser(pchDialog, steamIDUser);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamFriends_ActivateGameOverlayToWebPage(void* pSteamFriends, const char* pchURL)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	pISteamFriends->ActivateGameOverlayToWebPage(pchURL);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamFriends_ActivateGameOverlayToStore(void* pSteamFriends, AppId_t iApp, EOverlayToStoreFlag eFlag)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	pISteamFriends->ActivateGameOverlayToStore(iApp, eFlag);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamFriends_ActivateGameOverlayInviteDialog(void* pSteamFriends, uint64 steamIDLobby)
{
	ISteamFriends * pISteamFriends = static_cast<ISteamFriends*>( pSteamFriends );

	pISteamFriends->ActivateGameOverlayInviteDialog(steamIDLobby);
}

STEAMWORKSUNITY_API void* SteamUnityAPI_SteamUser()
{
	return context.SteamUser();
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

STEAMWORKSUNITY_API EUserHasLicenseForAppResult SteamUnityAPI_SteamUser_UserHasLicenseForApp(void* pSteamUser, uint64 steamID, AppId_t iApp)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->UserHasLicenseForApp(steamID, iApp);
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

STEAMWORKSUNITY_API HAuthTicket SteamUnityAPI_SteamUser_GetAuthSessionTicket(void* pSteamUser, void* pTicket, int iMaxTicket, uint32 *pTicketSize)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );
	
	return pISteamUser->GetAuthSessionTicket(pTicket, iMaxTicket, pTicketSize);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamUser_CancelAuthTicket(void* pSteamUser, HAuthTicket hAuthTicket)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );
	
	return pISteamUser->CancelAuthTicket(hAuthTicket);
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamUser_GetPlayerSteamLevel(void* pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->GetPlayerSteamLevel();
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamUser_GetGameBadgeLevel(void* pSteamUser, int nSeries, bool bFoil)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->GetGameBadgeLevel(nSeries, bFoil);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamUser_StartVoiceRecording(void* pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	pISteamUser->StartVoiceRecording();
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamUser_StopVoiceRecording(void* pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	pISteamUser->StopVoiceRecording();
}
	
STEAMWORKSUNITY_API int SteamUnityAPI_SteamUser_GetAvailableVoice(void* pSteamUser, uint32 *pcbCompressed, uint32 *pcbUncompressed, uint32 nUncompressedVoiceDesiredSampleRate)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->GetAvailableVoice(pcbCompressed, pcbUncompressed, nUncompressedVoiceDesiredSampleRate);
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamUser_GetVoice(void* pSteamUser, bool bWantCompressed, void *pDestBuffer, uint32 cbDestBufferSize, uint32 *nBytesWritten, bool bWantUncompressed, void *pUncompressedDestBuffer, uint32 cbUncompressedDestBufferSize, uint32 *nUncompressBytesWritten, uint32 nUncompressedVoiceDesiredSampleRate )
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, bWantUncompressed, pUncompressedDestBuffer, cbUncompressedDestBufferSize, nUncompressBytesWritten, nUncompressedVoiceDesiredSampleRate);
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamUser_DecompressVoice(void* pSteamUser, const void *pCompressed, uint32 cbCompressed, void *pDestBuffer, uint32 cbDestBufferSize, uint32 *nBytesWritten, uint32 nDesiredSampleRate )
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, nDesiredSampleRate);
}

STEAMWORKSUNITY_API uint32 SteamUnityAPI_SteamUser_GetVoiceOptimalSampleRate(void* pSteamUser)
{
	ISteamUser * pISteamUser = static_cast<ISteamUser*>( pSteamUser );

	return pISteamUser->GetVoiceOptimalSampleRate();
}

STEAMWORKSUNITY_API void* SteamUnityAPI_SteamUserStats()
{
	return context.SteamUserStats();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_RequestCurrentStats(void* pSteamUserStats)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

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

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_IndicateAchievementProgress(void* pSteamUserStats, const char *pchName, uint32 nCurProgress, uint32 nMaxProgress )
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->IndicateAchievementProgress(pchName, nCurProgress, nMaxProgress);
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamUserStats_FindLeaderboard(void* pSteamUserStats, const char *pchLeaderboardName)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	SteamAPICall_t hAPICall = pISteamUserStats->FindLeaderboard(pchLeaderboardName);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().LeaderboardFindResult, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamUserStats_FindOrCreateLeaderboard(void* pSteamUserStats, const char *pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, ELeaderboardDisplayType eLeaderboardDisplayType)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	SteamAPICall_t hAPICall = pISteamUserStats->FindOrCreateLeaderboard(pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().LeaderboardFindResult, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamUserStats_GetLeaderboardName(void* pSteamUserStats, SteamLeaderboard_t hSteamLeaderboard)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetLeaderboardName(hSteamLeaderboard);
}

STEAMWORKSUNITY_API int SteamUnityAPI_SteamUserStats_GetLeaderboardEntryCount(void* pSteamUserStats, SteamLeaderboard_t hSteamLeaderboard)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetLeaderboardEntryCount(hSteamLeaderboard);
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

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamUserStats_RequestLeaderboardEntries(void* pSteamUserStats, SteamLeaderboard_t hSteamLeaderboard, ELeaderboardDataRequest eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	SteamAPICall_t hAPICall = pISteamUserStats->DownloadLeaderboardEntries(hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().LeaderboardScoresDownloaded, hAPICall );

	return hAPICall;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_GetDownloadedLeaderboardEntry(void* pSteamUserStats, SteamLeaderboardEntries_t hSteamLeaderboardEntries, int32 index, LeaderboardEntry_t* outLeaderboardEntry, int32* pDetails, int cDetailsMax)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries, index, outLeaderboardEntry, pDetails, cDetailsMax);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamUserStats_ResetAllStats(void* pSteamUserStats, bool bAchievementsToo)
{
	ISteamUserStats * pISteamUserStats = static_cast<ISteamUserStats*>( pSteamUserStats );

	return pISteamUserStats->ResetAllStats(bAchievementsToo);
}


STEAMWORKSUNITY_API void* SteamUnityAPI_SteamGameServerStats()
{
	return serverContext.SteamGameServerStats();
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamGameServerStats_RequestUserStats(void* pSteamGameServerStats, uint64 steamIDUser)
{
	ISteamGameServerStats * pISteamGameServerStats = static_cast<ISteamGameServerStats*>( pSteamGameServerStats );

	SteamAPICall_t hAPICall = pISteamGameServerStats->RequestUserStats(steamIDUser);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().UserStatsReceived, hAPICall );

	return hAPICall;
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
	return context.SteamMatchmaking();
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamMatchmaking_CreateLobby(void* pSteamMatchmaking, ELobbyType eLobbyType, int32 cMaxMembers)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	SteamAPICall_t hAPICall = pISteamMatchmaking->CreateLobby(eLobbyType, cMaxMembers);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().LobbyGameCreatedCallback, hAPICall );

	return hAPICall;
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

	SteamAPICall_t hAPICall = pISteamMatchmaking->RequestLobbyList();

	return hAPICall;
}

STEAMWORKSUNITY_API SteamAPICall_t SteamUnityAPI_SteamMatchmaking_JoinLobby(void* pSteamMatchmaking, uint64 steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );
	
	SteamAPICall_t hAPICall = pISteamMatchmaking->JoinLobby(steamIDLobby);
	SteamAPI_RegisterCallResult( &SteamCallbacks::getInstance().LobbyEnterCallback, hAPICall );

	return hAPICall;

}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_InviteUserToLobby(void* pSteamMatchmaking, uint64 steamIDLobby, uint64 steamID)
{
		ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

		return pISteamMatchmaking->InviteUserToLobby(steamIDLobby, steamID);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_LeaveLobby(void* pSteamMatchmaking, uint64 steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->LeaveLobby(steamIDLobby);
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamMatchmaking_GetLobbyByIndex(void* pSteamMatchmaking, int iLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetLobbyByIndex(iLobby).ConvertToUint64();
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

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_RequestLobbyData(void* pSteamMatchmaking, uint64 steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->RequestLobbyData(steamIDLobby);
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamMatchmaking_GetLobbyData(void* pSteamMatchmaking, uint64 steamIDLobby, const char* pchKey)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetLobbyData(steamIDLobby, pchKey);
}

STEAMWORKSUNITY_API int32 SteamUnityAPI_SteamMatchmaking_GetLobbyDataCount(void* pSteamMatchmaking, uint64 steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetLobbyDataCount(steamIDLobby);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_GetLobbyDataByIndex(void* pSteamMatchmaking, uint64 steamIDLobby, int32 iDataIndex, char* pchKey, int32 cchKeyBufferSize, char* pchValue, int32 cchValueBufferSize)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetLobbyDataByIndex(steamIDLobby, iDataIndex, pchKey, cchKeyBufferSize, pchValue, cchValueBufferSize);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_SetLobbyData(void* pSteamMatchmaking, uint64 steamIDLobby, const char* pchKey, const char* pchValue)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->SetLobbyData(steamIDLobby, pchKey, pchValue);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_DeleteLobbyData(void* pSteamMatchmaking, uint64 steamIDLobby, const char* pchKey)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->DeleteLobbyData(steamIDLobby, pchKey);
}

STEAMWORKSUNITY_API const char* SteamUnityAPI_SteamMatchmaking_GetLobbyMemberData(void* pSteamMatchmaking, uint64 steamIDLobby, uint64 steamIDUser, const char *pchKey)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetLobbyMemberData(steamIDLobby, steamIDUser, pchKey);
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_SetLobbyMemberData(void* pSteamMatchmaking, uint64 steamIDLobby, const char *pchKey, const char *pchValue)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	pISteamMatchmaking->SetLobbyMemberData(steamIDLobby, pchKey, pchValue);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_SendLobbyChatMsg(void* pSteamMatchmaking, uint64 steamIDLobby, void* pData, int32 iDataLength)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->SendLobbyChatMsg(steamIDLobby, pData, iDataLength);
}

STEAMWORKSUNITY_API int32 SteamUnityAPI_SteamMatchmaking_GetLobbyChatEntry(void* pSteamMatchmaking, uint64 steamIDLobby, int32 iChatID, uint64* steamID, uint8* pData, int32 iDataLength, EChatEntryType* chatEntryType)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	CSteamID csteamID;
	int32 ret = pISteamMatchmaking->GetLobbyChatEntry(steamIDLobby, iChatID, &csteamID, pData, iDataLength, chatEntryType);
	*steamID = csteamID.ConvertToUint64();

	return ret;
}

STEAMWORKSUNITY_API void SteamUnityAPI_SteamMatchmaking_SetLobbyGameServer(void* pSteamMatchmaking, CSteamID steamIDLobby, uint32 unGameServerIP, uint16 unGameServerPort, uint64 steamIDGameServer)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	pISteamMatchmaking->SetLobbyGameServer(steamIDLobby, unGameServerIP, unGameServerPort, steamIDGameServer);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_GetLobbyGameServer(void* pSteamMatchmaking, uint64 steamIDLobby, uint32 *punGameServerIP, uint16 *punGameServerPort, uint64 *psteamIDGameServer)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	CSteamID csteamID;
	bool ret = pISteamMatchmaking->GetLobbyGameServer(steamIDLobby, punGameServerIP, punGameServerPort, &csteamID);
	*psteamIDGameServer = csteamID.ConvertToUint64();

	return ret;
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_SetLobbyMemberLimit(void* pSteamMatchmaking, uint64 steamIDLobby, int32 iMaxMembers)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->SetLobbyMemberLimit(steamIDLobby, iMaxMembers);
}

STEAMWORKSUNITY_API int32 SteamUnityAPI_SteamMatchmaking_GetLobbyMemberLimit(void* pSteamMatchmaking, uint64 steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetLobbyMemberLimit(steamIDLobby);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_SetLobbyType(void* pSteamMatchmaking, uint64 steamIDLobby, ELobbyType eLobbyType)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->SetLobbyType(steamIDLobby, eLobbyType);
}
		
STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_SetLobbyJoinable(void* pSteamMatchmaking, uint64 steamIDLobby, bool bLobbyJoinable)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->SetLobbyJoinable(steamIDLobby, bLobbyJoinable);
}

STEAMWORKSUNITY_API uint64 SteamUnityAPI_SteamMatchmaking_GetLobbyOwner(void* pSteamMatchmaking, uint64 steamIDLobby)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->GetLobbyOwner(steamIDLobby).ConvertToUint64();
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_SetLobbyOwner(void* pSteamMatchmaking, uint64 steamIDLobby, uint64 steamIDNewOwner)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->SetLobbyOwner(steamIDLobby, steamIDNewOwner);
}

STEAMWORKSUNITY_API bool SteamUnityAPI_SteamMatchmaking_SetLinkedLobby(void* pSteamMatchmaking, uint64 steamIDLobby, uint64 steamIDLobbyDependent)
{
	ISteamMatchmaking * pISteamMatchmaking = static_cast<ISteamMatchmaking*>( pSteamMatchmaking );

	return pISteamMatchmaking->SetLinkedLobby(steamIDLobby, steamIDLobbyDependent);
}


MatchMakingKeyValuePair_t* g_pKeyValuePairs = NULL;

STEAMWORKSUNITY_API void* SteamUnityAPI_SteamMatchmakingServers()
{
	return context.SteamMatchmakingServers();
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
			strcpy_s(g_pKeyValuePairs[i].m_szKey, 256, pKeys[i]);
			strcpy_s(g_pKeyValuePairs[i].m_szValue, 256, pValues[i]);
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
			strcpy_s(g_pKeyValuePairs[i].m_szKey, 256, pKeys[i]);
			strcpy_s(g_pKeyValuePairs[i].m_szValue, 256, pValues[i]);
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
			strcpy_s(g_pKeyValuePairs[i].m_szKey, 256, pKeys[i]);
			strcpy_s(g_pKeyValuePairs[i].m_szValue, 256, pValues[i]);
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
			strcpy_s(g_pKeyValuePairs[i].m_szKey, 256, pKeys[i]);
			strcpy_s(g_pKeyValuePairs[i].m_szValue, 256, pValues[i]);
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
			strcpy_s(g_pKeyValuePairs[i].m_szKey, 256, pKeys[i]);
			strcpy_s(g_pKeyValuePairs[i].m_szValue, 256, pValues[i]);
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

	pISteamMatchmakingServers->ReleaseRequest(hRequest);
}


STEAMWORKSUNITY_API void* SteamUnityAPI_SteamNetworking()
{
	return context.SteamNetworking();
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

void SteamCallbacks::ServerResponded(HServerListRequest hRequest, int iServer)
{
	gameserveritem_t* callbackData = context.SteamMatchmakingServers()->GetServerDetails(hRequest, iServer);

	delegateOnServerResponded(hRequest, callbackData);
}

void SteamCallbacks::ServerFailedToRespond(HServerListRequest hRequest, int iServer)
{
	 gameserveritem_t* callbackData = context.SteamMatchmakingServers()->GetServerDetails(hRequest, iServer);
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
