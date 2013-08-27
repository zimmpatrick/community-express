// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

#pragma once

#include "steam/steam_api.h"

#ifdef _WIN32
	#define ZIMM_STDCALL	__stdcall
	#define ZIMM_CDECL		__cdecl
	#define ZIMM_CALLBACK	ZIMM_STDCALL
#elif defined(__APPLE__)
	#define ZIMM_STDCALL	__attribute__((stdcall))
	#define ZIMM_CDECL		__attribute__((cdecl))
	#define ZIMM_CALLBACK	ZIMM_CDECL
#endif



typedef void (ZIMM_CALLBACK *FPOnCallback)(int k_iCallback, void *pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall);

typedef void (ZIMM_CALLBACK *FPOnServerResponded)(HServerListRequest,gameserveritem_t*);
typedef void (ZIMM_CALLBACK *FPOnServerListComplete)(HServerListRequest);


#define STEAM_CALLBACK_CESDK1( thisclass, func, param, var ) var(this, &thisclass::func)
#define STEAM_CALLBACK_CESDK2( thisclass, func, param, var ) CCallback< thisclass, param, false > var; void func( param *pParam ) { delegateOnCallback(param::k_iCallback, pParam, false, k_uAPICallInvalid);  }
#define STEAM_GAMESERVER_CALLBACK_CESDK2( thisclass, func, param, var ) CCallback< thisclass, param, true > var; void func( param *pParam ) { delegateOnCallback(param::k_iCallback, pParam, false, k_uAPICallInvalid);  }

class SteamCallbacks : public ISteamMatchmakingServerListResponse
{
public:					
						// k_iSteamUserCallbacks
	SteamCallbacks() :	STEAM_CALLBACK_CESDK1(SteamCallbacks, OnSteamServersConnected, SteamServersConnected_t, SteamServersConnected)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnSteamServerConnectFailure, SteamServerConnectFailure_t, SteamServerConnectFailure)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnSteamServersDisconnected, SteamServersDisconnected_t, SteamServersDisconnected)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnClientGameServerDeny, ClientGameServerDeny_t, ClientGameServerDeny)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnIPCFailure, IPCFailure_t, IPCFailure)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnValidateAuthTicketResponse, ValidateAuthTicketResponse_t, ValidateAuthTicketResponse)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnMicroTxnAuthorizationResponse, MicroTxnAuthorizationResponse_t, MicroTxnAuthorizationResponse)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnEncryptedAppTicketResponse, EncryptedAppTicketResponse_t, EncryptedAppTicketResponse)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGetAuthSessionTicketResponse, GetAuthSessionTicketResponse_t, GetAuthSessionTicketResponse)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGameWebCallback, GameWebCallback_t, GameWebCallback)
		
		// k_iSteamFriendsCallbacks
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGameOverlayActivated, GameOverlayActivated_t, GameOverlayActivated)

		// k_iSteamUtilsCallbacks
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnIPCountry, IPCountry_t, IPCountry)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnLowBatteryPower, LowBatteryPower_t, LowBatteryPower)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnSteamAPICallCompleted, SteamAPICallCompleted_t, SteamAPICallCompleted)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnSteamShutdown, SteamShutdown_t, SteamShutdown)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnCheckFileSignature, CheckFileSignature_t, CheckFileSignature)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGamepadTextInputDismissed, GamepadTextInputDismissed_t, GamepadTextInputDismissed)
		
		// k_iSteamUserStatsCallbacks
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnUserStatsReceived, UserStatsReceived_t, UserStatsReceived)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnUserStatsStored, UserStatsStored_t, UserStatsStored)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnUserAchievementStored, UserAchievementStored_t, UserAchievementStored)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnLeaderboardFindResult, LeaderboardFindResult_t, LeaderboardFindResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnLeaderboardScoresDownloaded, LeaderboardScoresDownloaded_t, LeaderboardScoresDownloaded)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnLeaderboardScoreUploaded, LeaderboardScoreUploaded_t, LeaderboardScoreUploaded)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnNumberOfCurrentPlayers, NumberOfCurrentPlayers_t, NumberOfCurrentPlayers)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnUserStatsUnloaded, UserStatsUnloaded_t, UserStatsUnloaded)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnUserAchievementIconFetched, UserAchievementIconFetched_t, UserAchievementIconFetched)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGlobalAchievementPercentagesReady, GlobalAchievementPercentagesReady_t, GlobalAchievementPercentagesReady)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnLeaderboardUGCSet, LeaderboardUGCSet_t, LeaderboardUGCSet)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGlobalStatsReceived, GlobalStatsReceived_t, GlobalStatsReceived)

		// k_iClientRemoteStorageCallbacks
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageAppSyncedClient, RemoteStorageAppSyncedClient_t, RemoteStorageAppSyncedClient)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageAppSyncedServer, RemoteStorageAppSyncedServer_t, RemoteStorageAppSyncedServer)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageAppSyncProgress, RemoteStorageAppSyncProgress_t, RemoteStorageAppSyncProgress)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageAppSyncStatusCheck, RemoteStorageAppSyncStatusCheck_t, RemoteStorageAppSyncStatusCheck)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageConflictResolution, RemoteStorageConflictResolution_t, RemoteStorageConflictResolution)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageFileShareResult, RemoteStorageFileShareResult_t, RemoteStorageFileShareResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStoragePublishFileResult, RemoteStoragePublishFileResult_t, RemoteStoragePublishFileResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageDeletePublishedFileResult, RemoteStorageDeletePublishedFileResult_t, RemoteStorageDeletePublishedFileResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageEnumerateUserPublishedFilesResult, RemoteStorageEnumerateUserPublishedFilesResult_t, RemoteStorageEnumerateUserPublishedFilesResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageSubscribePublishedFileResult, RemoteStorageSubscribePublishedFileResult_t, RemoteStorageSubscribePublishedFileResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageEnumerateUserSubscribedFilesResult, RemoteStorageEnumerateUserSubscribedFilesResult_t, RemoteStorageEnumerateUserSubscribedFilesResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageUnsubscribePublishedFileResult, RemoteStorageUnsubscribePublishedFileResult_t, RemoteStorageUnsubscribePublishedFileResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageUpdatePublishedFileResult, RemoteStorageUpdatePublishedFileResult_t, RemoteStorageUpdatePublishedFileResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageDownloadUGCResult, RemoteStorageDownloadUGCResult_t, RemoteStorageDownloadUGCResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageGetPublishedFileDetailsResult, RemoteStorageGetPublishedFileDetailsResult_t, RemoteStorageGetPublishedFileDetailsResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageEnumerateWorkshopFilesResult, RemoteStorageEnumerateWorkshopFilesResult_t, RemoteStorageEnumerateWorkshopFilesResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageGetPublishedItemVoteDetailsResult, RemoteStorageGetPublishedItemVoteDetailsResult_t, RemoteStorageGetPublishedItemVoteDetailsResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStoragePublishedFileSubscribed, RemoteStoragePublishedFileSubscribed_t, RemoteStoragePublishedFileSubscribed)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStoragePublishedFileUnsubscribed, RemoteStoragePublishedFileUnsubscribed_t, RemoteStoragePublishedFileUnsubscribed)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStoragePublishedFileDeleted, RemoteStoragePublishedFileDeleted_t, RemoteStoragePublishedFileDeleted)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageUpdateUserPublishedItemVoteResult, RemoteStorageUpdateUserPublishedItemVoteResult_t, RemoteStorageUpdateUserPublishedItemVoteResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageUserVoteDetails, RemoteStorageUserVoteDetails_t, RemoteStorageUserVoteDetails)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageEnumerateUserSharedWorkshopFilesResult, RemoteStorageEnumerateUserSharedWorkshopFilesResult_t, RemoteStorageEnumerateUserSharedWorkshopFilesResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageSetUserPublishedFileActionResult, RemoteStorageSetUserPublishedFileActionResult_t, RemoteStorageSetUserPublishedFileActionResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStorageEnumeratePublishedFilesByUserActionResult, RemoteStorageEnumeratePublishedFilesByUserActionResult_t, RemoteStorageEnumeratePublishedFilesByUserActionResult)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnRemoteStoragePublishFileProgress, RemoteStoragePublishFileProgress_t, RemoteStoragePublishFileProgress)

		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnAvatarReceived, AvatarImageLoaded_t, AvatarReceivedCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnTransactionAuthorizationReceived, MicroTxnAuthorizationResponse_t, TransactionAuthorizationReceivedCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnLobbyDataUpdated, LobbyDataUpdate_t, LobbyDataUpdatedCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnLobbyChatUpdated, LobbyChatUpdate_t, LobbyChatUpdatedCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnLobbyChatMessage, LobbyChatMsg_t, LobbyChatMessageCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnLobbyGameCreated, LobbyGameCreated_t, LobbyGameCreatedCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGameServerClientApprove, GSClientApprove_t, GameServerClientApproveCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGameServerClientDeny, GSClientDeny_t, GameServerClientDenyCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGameServerClientKick, GSClientKick_t, GameServerClientKickCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGameServerPolicyResponse, GSPolicyResponse_t, GameServerPolicyResponseCallback)
	{
		delegateOnServerResponded = 0;
		delegateOnServerListComplete = 0;
		delegateOnCallback = 0;
	}

	static SteamCallbacks& getInstance()
	{
		static SteamCallbacks instance;
		return instance;
	}

	//
	// ISteamMatchmakingServerListResponse Implementation
	//
	// Server has responded ok with updated data
	virtual void ServerResponded(HServerListRequest hRequest, int iServer);
	// Server has failed to respond
	virtual void ServerFailedToRespond(HServerListRequest hRequest, int iServer);
	// A list refresh you had initiated is now 100% completed
	virtual void RefreshComplete(HServerListRequest hRequest, EMatchMakingServerResponse response);

	// k_iSteamUserCallbacks
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnSteamServersConnected, SteamServersConnected_t, SteamServersConnected);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnSteamServerConnectFailure, SteamServerConnectFailure_t, SteamServerConnectFailure);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnSteamServersDisconnected, SteamServersDisconnected_t, SteamServersDisconnected);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnClientGameServerDeny, ClientGameServerDeny_t, ClientGameServerDeny);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnIPCFailure, IPCFailure_t, IPCFailure);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnValidateAuthTicketResponse, ValidateAuthTicketResponse_t, ValidateAuthTicketResponse);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnMicroTxnAuthorizationResponse, MicroTxnAuthorizationResponse_t, MicroTxnAuthorizationResponse);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnEncryptedAppTicketResponse, EncryptedAppTicketResponse_t, EncryptedAppTicketResponse);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnGetAuthSessionTicketResponse, GetAuthSessionTicketResponse_t, GetAuthSessionTicketResponse);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnGameWebCallback, GameWebCallback_t, GameWebCallback);

	// k_iSteamFriendsCallbacks
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnGameOverlayActivated, GameOverlayActivated_t, GameOverlayActivated);

	// k_iSteamUtilsCallbacks
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnIPCountry, IPCountry_t, IPCountry);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLowBatteryPower, LowBatteryPower_t, LowBatteryPower);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnSteamAPICallCompleted, SteamAPICallCompleted_t, SteamAPICallCompleted);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnSteamShutdown, SteamShutdown_t, SteamShutdown);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnCheckFileSignature, CheckFileSignature_t, CheckFileSignature);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnGamepadTextInputDismissed, GamepadTextInputDismissed_t, GamepadTextInputDismissed);

	// k_iSteamUserStatsCallbacks
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnUserStatsReceived, UserStatsReceived_t, UserStatsReceived);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnUserStatsStored, UserStatsStored_t, UserStatsStored);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnUserAchievementStored, UserAchievementStored_t, UserAchievementStored);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLeaderboardFindResult, LeaderboardFindResult_t, LeaderboardFindResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLeaderboardScoresDownloaded, LeaderboardScoresDownloaded_t, LeaderboardScoresDownloaded);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLeaderboardScoreUploaded, LeaderboardScoreUploaded_t, LeaderboardScoreUploaded);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnNumberOfCurrentPlayers, NumberOfCurrentPlayers_t, NumberOfCurrentPlayers);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnUserStatsUnloaded, UserStatsUnloaded_t, UserStatsUnloaded);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnUserAchievementIconFetched, UserAchievementIconFetched_t, UserAchievementIconFetched);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnGlobalAchievementPercentagesReady, GlobalAchievementPercentagesReady_t, GlobalAchievementPercentagesReady);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLeaderboardUGCSet, LeaderboardUGCSet_t, LeaderboardUGCSet);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnGlobalStatsReceived, GlobalStatsReceived_t, GlobalStatsReceived);

	// k_iClientRemoteStorageCallbacks
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageAppSyncedClient, RemoteStorageAppSyncedClient_t, RemoteStorageAppSyncedClient);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageAppSyncedServer, RemoteStorageAppSyncedServer_t, RemoteStorageAppSyncedServer);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageAppSyncProgress, RemoteStorageAppSyncProgress_t, RemoteStorageAppSyncProgress);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageAppSyncStatusCheck, RemoteStorageAppSyncStatusCheck_t, RemoteStorageAppSyncStatusCheck);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageConflictResolution, RemoteStorageConflictResolution_t, RemoteStorageConflictResolution);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageFileShareResult, RemoteStorageFileShareResult_t, RemoteStorageFileShareResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStoragePublishFileResult, RemoteStoragePublishFileResult_t, RemoteStoragePublishFileResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageDeletePublishedFileResult, RemoteStorageDeletePublishedFileResult_t, RemoteStorageDeletePublishedFileResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageEnumerateUserPublishedFilesResult, RemoteStorageEnumerateUserPublishedFilesResult_t, RemoteStorageEnumerateUserPublishedFilesResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageSubscribePublishedFileResult, RemoteStorageSubscribePublishedFileResult_t, RemoteStorageSubscribePublishedFileResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageEnumerateUserSubscribedFilesResult, RemoteStorageEnumerateUserSubscribedFilesResult_t, RemoteStorageEnumerateUserSubscribedFilesResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageUnsubscribePublishedFileResult, RemoteStorageUnsubscribePublishedFileResult_t, RemoteStorageUnsubscribePublishedFileResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageUpdatePublishedFileResult, RemoteStorageUpdatePublishedFileResult_t, RemoteStorageUpdatePublishedFileResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageDownloadUGCResult, RemoteStorageDownloadUGCResult_t, RemoteStorageDownloadUGCResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageGetPublishedFileDetailsResult, RemoteStorageGetPublishedFileDetailsResult_t, RemoteStorageGetPublishedFileDetailsResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageEnumerateWorkshopFilesResult, RemoteStorageEnumerateWorkshopFilesResult_t, RemoteStorageEnumerateWorkshopFilesResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageGetPublishedItemVoteDetailsResult, RemoteStorageGetPublishedItemVoteDetailsResult_t, RemoteStorageGetPublishedItemVoteDetailsResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStoragePublishedFileSubscribed, RemoteStoragePublishedFileSubscribed_t, RemoteStoragePublishedFileSubscribed);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStoragePublishedFileUnsubscribed, RemoteStoragePublishedFileUnsubscribed_t, RemoteStoragePublishedFileUnsubscribed);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStoragePublishedFileDeleted, RemoteStoragePublishedFileDeleted_t, RemoteStoragePublishedFileDeleted);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageUpdateUserPublishedItemVoteResult, RemoteStorageUpdateUserPublishedItemVoteResult_t, RemoteStorageUpdateUserPublishedItemVoteResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageUserVoteDetails, RemoteStorageUserVoteDetails_t, RemoteStorageUserVoteDetails);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageEnumerateUserSharedWorkshopFilesResult, RemoteStorageEnumerateUserSharedWorkshopFilesResult_t, RemoteStorageEnumerateUserSharedWorkshopFilesResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageSetUserPublishedFileActionResult, RemoteStorageSetUserPublishedFileActionResult_t, RemoteStorageSetUserPublishedFileActionResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStorageEnumeratePublishedFilesByUserActionResult, RemoteStorageEnumeratePublishedFilesByUserActionResult_t, RemoteStorageEnumeratePublishedFilesByUserActionResult);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnRemoteStoragePublishFileProgress, RemoteStoragePublishFileProgress_t, RemoteStoragePublishFileProgress);


	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnAvatarReceived, AvatarImageLoaded_t, AvatarReceivedCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnTransactionAuthorizationReceived, MicroTxnAuthorizationResponse_t, TransactionAuthorizationReceivedCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLobbyDataUpdated, LobbyDataUpdate_t, LobbyDataUpdatedCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLobbyChatUpdated, LobbyChatUpdate_t, LobbyChatUpdatedCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLobbyChatMessage, LobbyChatMsg_t, LobbyChatMessageCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLobbyGameCreated, LobbyGameCreated_t, LobbyGameCreatedCallback);

	STEAM_GAMESERVER_CALLBACK_CESDK2(SteamCallbacks, OnGameServerClientApprove, GSClientApprove_t, GameServerClientApproveCallback);
	STEAM_GAMESERVER_CALLBACK_CESDK2(SteamCallbacks, OnGameServerClientDeny, GSClientDeny_t, GameServerClientDenyCallback);
	STEAM_GAMESERVER_CALLBACK_CESDK2(SteamCallbacks, OnGameServerClientKick, GSClientKick_t, GameServerClientKickCallback);
	STEAM_GAMESERVER_CALLBACK_CESDK2(SteamCallbacks, OnGameServerPolicyResponse, GSPolicyResponse_t, GameServerPolicyResponseCallback);

	FPOnServerResponded delegateOnServerResponded;
	FPOnServerListComplete delegateOnServerListComplete;

	FPOnCallback delegateOnCallback;
};
