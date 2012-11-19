// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

#pragma once

#include "steam/steam_api.h"

#ifdef _WIN32
	#define STDCALL		__stdcall
	#define CDECL		__cdecl
	#define CALLBACK	STD_CALL
#elif defined(__APPLE__)
	#define STDCALL		__attribute__((stdcall))
	#define CDECL		__attribute__((cdecl))
	#define CALLBACK	CDECL
#endif

typedef void (CALLBACK *FPOnAvatarReceived)(AvatarImageLoaded_t*);
typedef void (CALLBACK *FPOnUserStatsReceived)(UserStatsReceived_t*);
typedef void (CALLBACK *FPOnTransactionAuthorizationReceived)(MicroTxnAuthorizationResponse_t*);
typedef void (CALLBACK *FPOnGameServerClientApprove)(GSClientApprove_t*);
typedef void (CALLBACK *FPOnGameServerClientDeny)(GSClientDeny_t*);
typedef void (CALLBACK *FPOnGameServerClientKick)(GSClientKick_t*);
typedef void (CALLBACK *FPOnGameServerPolicyResponse)(GSPolicyResponse_t*);
typedef void (CALLBACK *FPOnLeaderboardRetrieved)(LeaderboardFindResult_t*);
typedef void (CALLBACK *FPOnLeaderboardEntriesRetrieved)(LeaderboardScoresDownloaded_t*);
typedef void (CALLBACK *FPOnServerResponded)(HServerListRequest,gameserveritem_t*);
typedef void (CALLBACK *FPOnServerListComplete)(HServerListRequest);
typedef void (CALLBACK *FPOnNetworkP2PSessionRequest)(P2PSessionRequest_t*);
typedef void (CALLBACK *FPOnNetworkP2PSessionConnectFailed)(P2PSessionConnectFail_t*);

class SteamCallbacks : public ISteamMatchmakingServerListResponse
{
public:
	SteamCallbacks() : AvatarReceivedCallback(this, &SteamCallbacks::OnAvatarReceived)
					 , UserStatsReceivedCallback(this, &SteamCallbacks::OnUserStatsReceived)
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

	STEAM_CALLBACK(SteamCallbacks, OnAvatarReceived, AvatarImageLoaded_t, AvatarReceivedCallback);
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

	FPOnAvatarReceived delegateOnAvatarReceived;
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
