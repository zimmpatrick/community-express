
#pragma once

#include "stdafx.h"
#include "steam/steam_gameserver.h"

namespace SteamAPI
{

class SteamGameServer : public ISteamGameServer
{
public:
	// connection functions
	virtual void LogOn() 
	{
		TRACE( TEXT( "LogOn" ) );
	}
	
	virtual void LogOff()
	{
		TRACE( TEXT( "LogOn" ) );
	}
	
	
	// status functions
	virtual bool BLoggedOn()
	{
		TRACE( TEXT( "BLoggedOn" ) );
		return true;
	}
	
	virtual bool BSecure()
	{
		TRACE( TEXT( "BSecure" ) );
		return true;
	} 

	virtual CSteamID GetSteamID()
	{
		TRACE( TEXT( "GetSteamID" ) );
		return CSteamID();
	} 

	// Handles receiving a new connection from a Steam user.  This call will ask the Steam
	// servers to validate the users identity, app ownership, and VAC status.  If the Steam servers 
	// are off-line, then it will validate the cached ticket itself which will validate app ownership 
	// and identity.  The AuthBlob here should be acquired on the game client using SteamUser()->InitiateGameConnection()
	// and must then be sent up to the game server for authentication.
	//
	// Return Value: returns true if the users ticket passes basic checks. pSteamIDUser will contain the Steam ID of this user. pSteamIDUser must NOT be NULL
	// If the call succeeds then you should expect a GSClientApprove_t or GSClientDeny_t callback which will tell you whether authentication
	// for the user has succeeded or failed (the steamid in the callback will match the one returned by this call)
	virtual bool SendUserConnectAndAuthenticate( uint32 unIPClient, const void *pvAuthBlob, uint32 cubAuthBlobSize, CSteamID *pSteamIDUser )
	{
		TRACE( TEXT( "SendUserConnectAndAuthenticate" ) );
		return true;
	}

	// Creates a fake user (ie, a bot) which will be listed as playing on the server, but skips validation.  
	// 
	// Return Value: Returns a SteamID for the user to be tracked with, you should call HandleUserDisconnect()
	// when this user leaves the server just like you would for a real user.
	virtual CSteamID CreateUnauthenticatedUserConnection()
	{
		TRACE( TEXT( "CreateUnauthenticatedUserConnection" ) );
		return CSteamID();
	}

	// Should be called whenever a user leaves our game server, this lets Steam internally
	// track which users are currently on which servers for the purposes of preventing a single
	// account being logged into multiple servers, showing who is currently on a server, etc.
	virtual void SendUserDisconnect( CSteamID steamIDUser )
	{
		TRACE( TEXT( "SendUserDisconnect" ) );
	}

	// Update the data to be displayed in the server browser and matchmaking interfaces for a user
	// currently connected to the server.  For regular users you must call this after you receive a
	// GSUserValidationSuccess callback.
	// 
	// Return Value: true if successful, false if failure (ie, steamIDUser wasn't for an active player)
	virtual bool BUpdateUserData( CSteamID steamIDUser, const char *pchPlayerName, uint32 uScore )
	{
		TRACE( TEXT( "BUpdateUserData" ) );
		return true;
	}

	// You shouldn't need to call this as it is called internally by SteamGameServer_Init() and can only be called once.
	//
	// To update the data in this call which may change during the servers lifetime see UpdateServerStatus() below.
	//
	// Input:	nGameAppID - The Steam assigned AppID for the game
	//			unServerFlags - Any applicable combination of flags (see k_unServerFlag____ constants below)
	//			unGameIP - The IP Address the server is listening for client connections on (might be INADDR_ANY)
	//			unGamePort - The port which the server is listening for client connections on
	//			unSpectatorPort - the port on which spectators can join to observe the server, 0 if spectating is not supported
	//			usQueryPort - The port which the ISteamMasterServerUpdater API should use in order to listen for matchmaking requests
	//			pchGameDir - A unique string identifier for your game
	//			pchVersion - The current version of the server as a string like 1.0.0.0
	//			bLanMode - Is this a LAN only server?
	//			
	// bugbug jmccaskey - figure out how to remove this from the API and only expose via SteamGameServer_Init... or make this actually used,
	// and stop calling it in SteamGameServer_Init()?
	virtual bool BSetServerType( uint32 unServerFlags, uint32 unGameIP, uint16 unGamePort, 
								uint16 unSpectatorPort, uint16 usQueryPort, const char *pchGameDir, const char *pchVersion, bool bLANMode )
	{
		TRACE( TEXT( "BSetServerType" ) );
		return true;
	}

	// Updates server status values which shows up in the server browser and matchmaking APIs
	virtual void UpdateServerStatus( int cPlayers, int cPlayersMax, int cBotPlayers, 
									 const char *pchServerName, const char *pSpectatorServerName, 
									 const char *pchMapName )
	{
		TRACE( TEXT( "UpdateServerStatus" ) );
	}

	// This can be called if spectator goes away or comes back (passing 0 means there is no spectator server now).
	virtual void UpdateSpectatorPort( uint16 unSpectatorPort )
	{
		TRACE( TEXT( "UpdateSpectatorPort" ) );
	}

	// Sets a string defining the "gametags" for this server, this is optional, but if it is set
	// it allows users to filter in the matchmaking/server-browser interfaces based on the value
	virtual void SetGameTags( const char *pchGameTags )
	{
		TRACE( TEXT( "SetGameTags" ) );
	}

	// Sets a string defining the 
	// Ask for the gameplay stats for the server. Results returned in a callback
	virtual void GetGameplayStats( ) 
	{
		TRACE( TEXT( "GetGameplayStats" ) );
	}

	// Gets the reputation score for the game server. This API also checks if the server or some
	// other server on the same IP is banned from the Steam master servers.
	virtual SteamAPICall_t GetServerReputation( )
	{
		TRACE( TEXT( "GetServerReputation" ) );
		return 0;
	}


	// Ask if a user in in the specified group, results returns async by GSUserGroupStatus_t
	// returns false if we're not connected to the steam servers and thus cannot ask
	virtual bool RequestUserGroupStatus( CSteamID steamIDUser, CSteamID steamIDGroup )
	{
		TRACE( TEXT( "RequestUserGroupStatus" ) );
		return true;
	}

	// Returns the public IP of the server according to Steam, useful when the server is 
	// behind NAT and you want to advertise its IP in a lobby for other clients to directly
	// connect to
	virtual uint32 GetPublicIP()
	{
		TRACE( TEXT( "GetPublicIP" ) );
		return 0;
	}

	// Sets a string defining the "gamedata" for this server, this is optional, but if it is set
	// it allows users to filter in the matchmaking/server-browser interfaces based on the value
	// don't set this unless it actually changes, its only uploaded to the master once (when
	// acknowledged)
	virtual void SetGameData( const char *pchGameData)
	{
		TRACE( TEXT( "SetGameData" ) );
	}
 
	// After receiving a user's authentication data, and passing it to SendUserConnectAndAuthenticate, use this function
	// to determine if the user owns downloadable content specified by the provided AppID.
	virtual EUserHasLicenseForAppResult UserHasLicenseForApp( CSteamID steamID, AppId_t appID )
	{
		TRACE( TEXT( "UserHasLicenseForApp" ) );
		return k_EUserHasLicenseResultHasLicense;
	}

	// New auth system APIs - do not mix with the old auth system APIs.
	// ----------------------------------------------------------------

	// Retrieve ticket to be sent to the entity who wishes to authenticate you ( using BeginAuthSession API ). 
	// pcbTicket retrieves the length of the actual ticket.
	virtual HAuthTicket GetAuthSessionTicket( void *pTicket, int cbMaxTicket, uint32 *pcbTicket )
	{
		TRACE( TEXT( "GetAuthSessionTicket" ) );
		return 0;
	}

	// Authenticate ticket ( from GetAuthSessionTicket ) from entity steamID to be sure it is valid and isnt reused
	// Registers for callbacks if the entity goes offline or cancels the ticket ( see ValidateAuthTicketResponse_t callback and EAuthSessionResponse )
	virtual EBeginAuthSessionResult BeginAuthSession( const void *pAuthTicket, int cbAuthTicket, CSteamID steamID )
	{
		TRACE( TEXT( "BeginAuthSession" ) );
		return k_EBeginAuthSessionResultOK;
	}

	// Stop tracking started by BeginAuthSession - called when no longer playing game with this entity
	virtual void EndAuthSession( CSteamID steamID )
	{
		TRACE( TEXT( "EndAuthSession" ) );
	}

	// Cancel auth ticket from GetAuthSessionTicket, called when no longer playing game with the entity you gave the ticket to
	virtual void CancelAuthTicket( HAuthTicket hAuthTicket )
	{
		TRACE( TEXT( "CancelAuthTicket" ) );
	}
};

}
