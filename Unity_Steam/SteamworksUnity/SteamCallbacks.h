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
typedef void (ZIMM_CALLBACK *FPOnAvatarReceived)(AvatarImageLoaded_t*);
typedef void (ZIMM_CALLBACK *FPOnUserStatsReceived)(UserStatsReceived_t*);
typedef void (ZIMM_CALLBACK *FPOnTransactionAuthorizationReceived)(MicroTxnAuthorizationResponse_t*);
typedef void (ZIMM_CALLBACK *FPOnGameServerClientApprove)(GSClientApprove_t*);
typedef void (ZIMM_CALLBACK *FPOnGameServerClientDeny)(GSClientDeny_t*);
typedef void (ZIMM_CALLBACK *FPOnGameServerClientKick)(GSClientKick_t*);
typedef void (ZIMM_CALLBACK *FPOnGameServerPolicyResponse)(GSPolicyResponse_t*);
typedef void (ZIMM_CALLBACK *FPOnLeaderboardRetrieved)(LeaderboardFindResult_t*);
typedef void (ZIMM_CALLBACK *FPOnLeaderboardEntriesRetrieved)(LeaderboardScoresDownloaded_t*);
typedef void (ZIMM_CALLBACK *FPOnServerResponded)(HServerListRequest,gameserveritem_t*);
typedef void (ZIMM_CALLBACK *FPOnServerListComplete)(HServerListRequest);
typedef void (ZIMM_CALLBACK *FPOnNetworkP2PSessionRequest)(P2PSessionRequest_t*);
typedef void (ZIMM_CALLBACK *FPOnNetworkP2PSessionConnectFailed)(P2PSessionConnectFail_t*);
typedef void (ZIMM_CALLBACK *FPOnGamepadTextInputDismissed)(GamepadTextInputDismissed_t*);
typedef void (ZIMM_CALLBACK *FPOnLobbyDataUpdated)(LobbyDataUpdate_t*);
typedef void (ZIMM_CALLBACK *FPOnLobbyChatUpdated)(LobbyChatUpdate_t*);
typedef void (ZIMM_CALLBACK *FPOnLobbyChatMessage)(LobbyChatMsg_t*);
typedef void (ZIMM_CALLBACK *FPOnLobbyGameCreated)(LobbyGameCreated_t*);
typedef void (ZIMM_CALLBACK *FPOnGetAuthSessionTicketResponse)(GetAuthSessionTicketResponse_t*);


#define STEAM_CALLBACK_CESDK1( thisclass, func, param, var ) var(this, &thisclass::func)
#define STEAM_CALLBACK_CESDK2( thisclass, func, param, var ) CCallback< thisclass, param, false > var; void func( param *pParam ) { delegateOnCallback(param::k_iCallback, pParam, false, k_uAPICallInvalid);  }
#define STEAM_GAMESERVER_CALLBACK_CESDK2( thisclass, func, param, var ) CCallback< thisclass, param, true > var; void func( param *pParam ) { delegateOnCallback(param::k_iCallback, pParam, false, k_uAPICallInvalid);  }

class SteamCallbacks : public ISteamMatchmakingServerListResponse
{
public:
	SteamCallbacks() : STEAM_CALLBACK_CESDK1(SteamCallbacks, OnSteamServersConnected, SteamServersConnected_t, SteamServersConnected)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnSteamServerConnectFailure, SteamServerConnectFailure_t, SteamServerConnectFailure)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnSteamServersDisconnected, SteamServersDisconnected_t, SteamServersDisconnected)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnClientGameServerDeny, ClientGameServerDeny_t, ClientGameServerDeny)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnIPCFailure, IPCFailure_t, IPCFailure)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnValidateAuthTicketResponse, ValidateAuthTicketResponse_t, ValidateAuthTicketResponse)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnMicroTxnAuthorizationResponse, MicroTxnAuthorizationResponse_t, MicroTxnAuthorizationResponse)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnEncryptedAppTicketResponse, EncryptedAppTicketResponse_t, EncryptedAppTicketResponse)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGetAuthSessionTicketResponse, GetAuthSessionTicketResponse_t, GetAuthSessionTicketResponse)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGameWebCallback, GameWebCallback_t, GameWebCallback)
		
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnIPCountry, IPCountry_t, IPCountry)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnLowBatteryPower, LowBatteryPower_t, LowBatteryPower)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnSteamAPICallCompleted, SteamAPICallCompleted_t, SteamAPICallCompleted)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnSteamShutdown, SteamShutdown_t, SteamShutdown)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnCheckFileSignature, CheckFileSignature_t, CheckFileSignature)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnGamepadTextInputDismissed, GamepadTextInputDismissed_t, GamepadTextInputDismissed)

		,  STEAM_CALLBACK_CESDK1(SteamCallbacks, OnAvatarReceived, AvatarImageLoaded_t, AvatarReceivedCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnUserStatsReceived, UserStatsReceived_t, UserStatsReceivedCallback)
		, STEAM_CALLBACK_CESDK1(SteamCallbacks, OnUserStatsStored, UserStatsStored_t, UserStatsStoredCallback)
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
		delegateOnLeaderboardRetrieved = 0;
		delegateOnLeaderboardEntriesRetrieved = 0;
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

	// k_iSteamUtilsCallbacks
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnIPCountry, IPCountry_t, IPCountry);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLowBatteryPower, LowBatteryPower_t, LowBatteryPower);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnSteamAPICallCompleted, SteamAPICallCompleted_t, SteamAPICallCompleted);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnSteamShutdown, SteamShutdown_t, SteamShutdown);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnCheckFileSignature, CheckFileSignature_t, CheckFileSignature);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnGamepadTextInputDismissed, GamepadTextInputDismissed_t, GamepadTextInputDismissed);


	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnAvatarReceived, AvatarImageLoaded_t, AvatarReceivedCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnUserStatsReceived, UserStatsReceived_t, UserStatsReceivedCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnUserStatsStored, UserStatsStored_t, UserStatsStoredCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnTransactionAuthorizationReceived, MicroTxnAuthorizationResponse_t, TransactionAuthorizationReceivedCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLobbyDataUpdated, LobbyDataUpdate_t, LobbyDataUpdatedCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLobbyChatUpdated, LobbyChatUpdate_t, LobbyChatUpdatedCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLobbyChatMessage, LobbyChatMsg_t, LobbyChatMessageCallback);
	STEAM_CALLBACK_CESDK2(SteamCallbacks, OnLobbyGameCreated, LobbyGameCreated_t, LobbyGameCreatedCallback);

	STEAM_GAMESERVER_CALLBACK_CESDK2(SteamCallbacks, OnGameServerClientApprove, GSClientApprove_t, GameServerClientApproveCallback);
	STEAM_GAMESERVER_CALLBACK_CESDK2(SteamCallbacks, OnGameServerClientDeny, GSClientDeny_t, GameServerClientDenyCallback);
	STEAM_GAMESERVER_CALLBACK_CESDK2(SteamCallbacks, OnGameServerClientKick, GSClientKick_t, GameServerClientKickCallback);
	STEAM_GAMESERVER_CALLBACK_CESDK2(SteamCallbacks, OnGameServerPolicyResponse, GSPolicyResponse_t, GameServerPolicyResponseCallback);

	void OnLeaderboardRetrieved(LeaderboardFindResult_t *pCallbackData, bool bIOFailure);
	CCallResult<SteamCallbacks, LeaderboardFindResult_t> SteamCallResultFindLeaderboard;

	void OnLeaderboardEntriesRetrieved(LeaderboardScoresDownloaded_t *pCallbackData, bool bIOFailure);
	CCallResult<SteamCallbacks, LeaderboardScoresDownloaded_t> SteamCallResultLeaderboardScoresDownloaded;

	FPOnLeaderboardRetrieved delegateOnLeaderboardRetrieved;
	FPOnLeaderboardEntriesRetrieved delegateOnLeaderboardEntriesRetrieved;
	FPOnServerResponded delegateOnServerResponded;
	FPOnServerListComplete delegateOnServerListComplete;

	FPOnCallback delegateOnCallback;
};
