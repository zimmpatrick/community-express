#pragma once

#include "steam/steam_api.h"

class SteamCallbacks
{
public:
	SteamCallbacks() : UserStatsReceivedCallback(this, &SteamCallbacks::OnUserStatsReceived)
	{
	}

	static SteamCallbacks& getInstance()
	{
		static SteamCallbacks instance;
		return instance;
	}

	STEAM_CALLBACK(SteamCallbacks, OnUserStatsReceived, UserStatsReceived_t, UserStatsReceivedCallback);

	void OnLeaderboardRetrieved(LeaderboardFindResult_t *CallbackData, bool bIOFailure);
	CCallResult<SteamCallbacks, LeaderboardFindResult_t> SteamCallResultFindLeaderboard;

	void (__stdcall *delegateOnUserStatsReceived)(UserStatsReceived_t*);
	void (__stdcall *delegateOnLeaderboardRetrievedCallback)(LeaderboardFindResult_t*);
};
