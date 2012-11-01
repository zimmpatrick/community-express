#pragma once

#include "steam/steam_api.h"

typedef void (__stdcall *FPOnUserStatsReceived)(UserStatsReceived_t*);
typedef void (__stdcall *FPOnTransactionAuthorizationReceived)(MicroTxnAuthorizationResponse_t*);
typedef void (__stdcall *FPOnGameServerClientApprove)(GSClientApprove_t*);
typedef void (__stdcall *FPOnGameServerClientDeny)(GSClientDeny_t*);
typedef void (__stdcall *FPOnGameServerClientKick)(GSClientKick_t*);
typedef void (__stdcall *FPOnGameServerPolicyResponse)(GSPolicyResponse_t*);
typedef void (__stdcall *FPOnLeaderboardRetrieved)(LeaderboardFindResult_t*);
typedef void (__stdcall *FPOnLeaderboardEntriesRetrieved)(LeaderboardScoresDownloaded_t*);
typedef void (__stdcall *FPOnServerResponded)(HServerListRequest,gameserveritem_t*);
typedef void (__stdcall *FPOnServerListComplete)(HServerListRequest);
typedef void (__stdcall *FPOnNetworkP2PSessionRequest)(P2PSessionRequest_t*);
typedef void (__stdcall *FPOnNetworkP2PSessionConnectFailed)(P2PSessionConnectFail_t*);

class SteamCallbacks : public ISteamMatchmakingServerListResponse
{
public:
	SteamCallbacks() : UserStatsReceivedCallback(this, &SteamCallbacks::OnUserStatsReceived)
					 , TransactionAuthorizationReceivedCallback(this, &SteamCallbacks::OnTransactionAuthorizationReceived)
					 , GameServerClientApproveCallback(this, &SteamCallbacks::OnGameServerClientApprove)
					 , GameServerClientDenyCallback(this, &SteamCallbacks::OnGameServerClientDeny)
					 , GameServerClientKickCallback(this, &SteamCallbacks::OnGameServerClientKick)
					 , GameServerPolicyResponseCallback(this, &SteamCallbacks::OnGameServerPolicyResponse)
	{
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

	STEAM_CALLBACK(SteamCallbacks, OnUserStatsReceived, UserStatsReceived_t, UserStatsReceivedCallback);
	STEAM_CALLBACK(SteamCallbacks, OnTransactionAuthorizationReceived, MicroTxnAuthorizationResponse_t, TransactionAuthorizationReceivedCallback);

	STEAM_GAMESERVER_CALLBACK(SteamCallbacks, OnGameServerClientApprove, GSClientApprove_t, GameServerClientApproveCallback);
	STEAM_GAMESERVER_CALLBACK(SteamCallbacks, OnGameServerClientDeny, GSClientDeny_t, GameServerClientDenyCallback);
	STEAM_GAMESERVER_CALLBACK(SteamCallbacks, OnGameServerClientKick, GSClientKick_t, GameServerClientKickCallback);
	STEAM_GAMESERVER_CALLBACK(SteamCallbacks, OnGameServerPolicyResponse, GSPolicyResponse_t, GameServerPolicyResponseCallback);

	void OnLeaderboardRetrieved(LeaderboardFindResult_t *pCallbackData, bool bIOFailure);
	CCallResult<SteamCallbacks, LeaderboardFindResult_t> SteamCallResultFindLeaderboard;

	void OnLeaderboardEntriesRetrieved(LeaderboardScoresDownloaded_t *pCallbackData, bool bIOFailure);
	CCallResult<SteamCallbacks, LeaderboardScoresDownloaded_t> SteamCallResultLeaderboardScoresDownloaded;

	FPOnUserStatsReceived delegateOnUserStatsReceived;
	FPOnTransactionAuthorizationReceived delegateOnTransactionAuthorizationReceived;
	FPOnGameServerClientApprove delegateOnGameServerClientApprove;
	FPOnGameServerClientDeny delegateOnGameServerClientDeny;
	FPOnGameServerClientKick delegateOnGameServerClientKick;
	FPOnGameServerPolicyResponse delegateOnGameServerPolicyResponse;
	FPOnLeaderboardRetrieved delegateOnLeaderboardRetrieved;
	FPOnLeaderboardEntriesRetrieved delegateOnLeaderboardEntriesRetrieved;
	FPOnServerResponded delegateOnServerResponded;
	FPOnServerListComplete delegateOnServerListComplete;
	FPOnNetworkP2PSessionRequest delegateOnNetworkP2PSessionRequest;
	FPOnNetworkP2PSessionConnectFailed delegateOnNetworkP2PSessionConnectFailed;
};
