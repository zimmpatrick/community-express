
#pragma once

#include "stdafx.h"

namespace SteamAPI
{

class SteamUtils : public ISteamUtils
{
public:
	virtual uint32 GetSecondsSinceAppActive()
	{
		TRACE( TEXT("GetSecondsSinceAppActive()") );
		return 0;
	}

	virtual uint32 GetSecondsSinceComputerActive()
	{
		TRACE( TEXT("GetSecondsSinceComputerActive()") );
		return 0;
	}

	// the universe this client is connecting to
	virtual EUniverse GetConnectedUniverse()
	{
		TRACE( TEXT("GetConnectedUniverse()") );
		return k_EUniversePublic;
	}

	// Steam server time - in PST, number of seconds since January 1, 1970 (i.e unix time)
	virtual uint32 GetServerRealTime()
	{
		TRACE( TEXT("GetServerRealTime()") );
		return 0;
	}

	// returns the 2 digit ISO 3166-1-alpha-2 format country code this client is running in (as looked up via an IP-to-location database)
	// e.g "US" or "UK".
	virtual const char *GetIPCountry()
	{
		TRACE( TEXT("GetIPCountry()") );
		return "US";
	}

	// returns true if the image exists, and valid sizes were filled out
	virtual bool GetImageSize( int iImage, uint32 *pnWidth, uint32 *pnHeight )
	{
		TRACE( TEXT("GetImageSize(%d,%x,%x)"), iImage, pnWidth, pnHeight );
		return false;
	}

	// returns true if the image exists, and the buffer was successfully filled out
	// results are returned in RGBA format
	// the destination buffer size should be 4 * height * width * sizeof(char)
	virtual bool GetImageRGBA( int iImage, uint8 *pubDest, int nDestBufferSize )
	{
		TRACE( TEXT("GetImageRGBA(%d,%x,%d)"), iImage, pubDest, nDestBufferSize );
		return false;
	}

	// returns the IP of the reporting server for valve - currently only used in Source engine games
	virtual bool GetCSERIPPort( uint32 *unIP, uint16 *usPort )
	{
		TRACE( TEXT("GetCSERIPPort(%x,%x)"), unIP, usPort );
		return false;
	}

	// return the amount of battery power left in the current system in % [0..100], 255 for being on AC power
	virtual uint8 GetCurrentBatteryPower() 
	{
		TRACE( TEXT("GetCurrentBatteryPower()") );
		return 255;
	}

	// returns the appID of the current process
	virtual uint32 GetAppID()
	{
		TRACE( TEXT("GetAppID()") );
		return 0;
	}

	// Sets the position where the overlay instance for the currently calling game should show notifications.
	// This position is per-game and if this function is called from outside of a game context it will do nothing.
	virtual void SetOverlayNotificationPosition( ENotificationPosition eNotificationPosition )
	{
		TRACE( TEXT("SetOverlayNotificationPosition(%d)"), (int)eNotificationPosition );
	}

	// API asynchronous call results
	// can be used directly, but more commonly used via the callback dispatch API (see steam_api.h)
	virtual bool IsAPICallCompleted( SteamAPICall_t hSteamAPICall, bool *pbFailed )
	{
		TRACE( TEXT("IsAPICallCompleted(%d,%x)"), (int)hSteamAPICall, pbFailed );
		return false;
	}

	virtual ESteamAPICallFailure GetAPICallFailureReason( SteamAPICall_t hSteamAPICall )
	{
		TRACE( TEXT("GetAPICallFailureReason(%d)"), (int)hSteamAPICall );
		return k_ESteamAPICallFailureNone;
	}

	virtual bool GetAPICallResult( SteamAPICall_t hSteamAPICall, void *pCallback, int cubCallback, int iCallbackExpected, bool *pbFailed )
	{
		TRACE( TEXT("GetAPICallResult(%d,%x,%d,%d,%x)"), (int)hSteamAPICall, pCallback, cubCallback, iCallbackExpected, pbFailed );
		return false;
	}

	// this needs to be called every frame to process matchmaking results
	// redundant if you're already calling SteamAPI_RunCallbacks()
	virtual void RunFrame()
	{
		TRACE( TEXT("RunFrame()") );
	}

	// returns the number of IPC calls made since the last time this function was called
	// Used for perf debugging so you can understand how many IPC calls your game makes per frame
	// Every IPC call is at minimum a thread context switch if not a process one so you want to rate
	// control how often you do them.
	virtual uint32 GetIPCCallCount()
	{
		TRACE( TEXT("GetIPCCallCount()") );
		return 0;
	}

	// API warning handling
	// 'int' is the severity; 0 for msg, 1 for warning
	// 'const char *' is the text of the message
	// callbacks will occur directly after the API function is called that generated the warning or message
	virtual void SetWarningMessageHook( SteamAPIWarningMessageHook_t pFunction )
	{
		TRACE( TEXT("SetWarningMessageHook(%x)"), pFunction );
	}

	// Returns true if the overlay is running & the user can access it. The overlay process could take a few seconds to
	// start & hook the game process, so this function will initially return false while the overlay is loading.
	virtual bool IsOverlayEnabled()
	{
		TRACE( TEXT("IsOverlayEnabled()") );
		return 0;
	}

	// Normally this call is unneeded if your game has a constantly running frame loop that calls the 
	// D3D Present API, or OGL SwapBuffers API every frame.
	//
	// However, if you have a game that only refreshes the screen on an event driven basis then that can break 
	// the overlay, as it uses your Present/SwapBuffers calls to drive it's internal frame loop and it may also
	// need to Present() to the screen any time an even needing a notification happens or when the overlay is
	// brought up over the game by a user.  You can use this API to ask the overlay if it currently need a present
	// in that case, and then you can check for this periodically (roughly 33hz is desirable) and make sure you
	// refresh the screen with Present or SwapBuffers to allow the overlay to do it's work.
	virtual bool BOverlayNeedsPresent()
	{
		TRACE( TEXT("BOverlayNeedsPresent()") );
		return false;
	}

	// Asynchronous call to check if file is signed, result is returned in CheckFileSignature_t
	virtual SteamAPICall_t CheckFileSignature( const char *szFileName )
	{
		TRACE( TEXT("CheckFileSignature('%s')"), szFileName );
		return 0;
	}

};

}
