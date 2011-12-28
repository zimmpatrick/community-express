
#pragma once

#include "stdafx.h"

namespace SteamAPI
{

class SteamApps : public ISteamApps
{
public:
	virtual bool BIsSubscribed()
	{
		TRACE( TEXT("BIsSubscribed()") );
		return true;
	}

	virtual bool BIsLowViolence()
	{
		TRACE( TEXT("BIsLowViolence()") );
		return false;
	}

	virtual bool BIsCybercafe()
	{
		TRACE( TEXT("BIsCybercafe()") );
		return false;
	}

	virtual bool BIsVACBanned()
	{
		TRACE( TEXT("BIsVACBanned()") );
		return false;
	}

	virtual const char *GetCurrentGameLanguage()
	{
		TRACE( TEXT("GetCurrentGameLanguage()") );
		return "EN";
	}

	virtual const char *GetAvailableGameLanguages()
	{
		TRACE( TEXT("GetAvailableGameLanguages()") );
		return "EN";
	}

	// only use this member if you need to check ownership of another game related to yours, a demo for example
	virtual bool BIsSubscribedApp( AppId_t appID )
	{
		TRACE( TEXT("BIsSubscribedApp(%d)"), appID );
		return true;
	}

	// Takes AppID of DLC and checks if the user owns the DLC & if the DLC is installed
	virtual bool BIsDlcInstalled( AppId_t appID )
	{
		TRACE( TEXT("BIsDlcInstalled(%d)"), appID );
		return false;
	}


	// returns the Unix time of the purchase of the app
	virtual uint32 GetEarliestPurchaseUnixTime( AppId_t nAppID )
	{
		return 0;
	}

	// Checks if the user is subscribed to the current app through a free weekend
	// This function will return false for users who have a retail or other type of license
	// Before using, please ask your Valve technical contact how to package and secure your free weekened
	virtual bool BIsSubscribedFromFreeWeekend()
	{
		return false;
	}

	// Returns the number of DLC pieces for the running app
	virtual int GetDLCCount()
	{
		return 0;
	}

	// Returns metadata for DLC by index, of range [0, GetDLCCount()]
	virtual bool BGetDLCDataByIndex( int iDLC, AppId_t *pAppID, bool *pbAvailable, char *pchName, int cchNameBufferSize )
	{
		return false;
	}

	// Install/Uninstall control for optional DLC
	virtual void InstallDLC( AppId_t nAppID )
	{
	}

	virtual void UninstallDLC( AppId_t nAppID )
	{
	}

};

}
