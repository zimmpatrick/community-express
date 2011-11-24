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

	void (__stdcall *delegateOnUserStatsReceived)(UserStatsReceived_t*);
};
