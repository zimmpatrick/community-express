// Copyright (c) 2011-2012, Zimmdot, LLC
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
