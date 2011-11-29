#pragma once

#include "steam/steam_api.h"

typedef void (__stdcall *FPOnUserStatsReceived)(UserStatsReceived_t*);
typedef void (__stdcall *FPOnGameServerClientApprove)(GSClientApprove_t*);
typedef void (__stdcall *FPOnGameServerClientDeny)(GSClientDeny_t*);
typedef void (__stdcall *FPOnGameServerClientKick)(GSClientKick_t*);
typedef void (__stdcall *FPOnGameServerPolicyResponse)(GSPolicyResponse_t*);
typedef void (__stdcall *FPOnLeaderboardRetrieved)(LeaderboardFindResult_t*);
typedef void (__stdcall *FPOnLeaderboardEntriesRetrieved)(LeaderboardScoresDownloaded_t*);

class SteamCallbacks
{
public:
	SteamCallbacks() : UserStatsReceivedCallback(this, &SteamCallbacks::OnUserStatsReceived)
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

	STEAM_CALLBACK(SteamCallbacks, OnUserStatsReceived, UserStatsReceived_t, UserStatsReceivedCallback);

	STEAM_GAMESERVER_CALLBACK(SteamCallbacks, OnGameServerClientApprove, GSClientApprove_t, GameServerClientApproveCallback);
	STEAM_GAMESERVER_CALLBACK(SteamCallbacks, OnGameServerClientDeny, GSClientDeny_t, GameServerClientDenyCallback);
	STEAM_GAMESERVER_CALLBACK(SteamCallbacks, OnGameServerClientKick, GSClientKick_t, GameServerClientKickCallback);
	STEAM_GAMESERVER_CALLBACK(SteamCallbacks, OnGameServerPolicyResponse, GSPolicyResponse_t, GameServerPolicyResponseCallback);

	void OnLeaderboardRetrieved(LeaderboardFindResult_t *pCallbackData, bool bIOFailure);
	CCallResult<SteamCallbacks, LeaderboardFindResult_t> SteamCallResultFindLeaderboard;

	void OnLeaderboardEntriesRetrieved(LeaderboardScoresDownloaded_t *pCallbackData, bool bIOFailure);
	CCallResult<SteamCallbacks, LeaderboardScoresDownloaded_t> SteamCallResultLeaderboardScoresDownloaded;

	FPOnUserStatsReceived delegateOnUserStatsReceived;
	FPOnGameServerClientApprove delegateOnGameServerClientApprove;
	FPOnGameServerClientDeny delegateOnGameServerClientDeny;
	FPOnGameServerClientKick delegateOnGameServerClientKick;
	FPOnGameServerPolicyResponse delegateOnGameServerPolicyResponse;
	FPOnLeaderboardRetrieved delegateOnLeaderboardRetrieved;
	FPOnLeaderboardEntriesRetrieved delegateOnLeaderboardEntriesRetrieved;
};
