
#pragma once

#include "stdafx.h"
#include "steam/steam_gameserver.h"

namespace SteamAPI
{

class SteamMasterServerUpdater : public ISteamMasterServerUpdater
{
public:
	// Call this as often as you like to tell the master server updater whether or not
	// you want it to be active (default: off).
	virtual void SetActive( bool bActive )
	{
	}

	// You usually don't need to modify this.
	// Pass -1 to use the default value for iHeartbeatInterval.
	// Some mods change this.
	virtual void SetHeartbeatInterval( int iHeartbeatInterval )
	{
	}

// These are in GameSocketShare mode, where instead of ISteamMasterServerUpdater creating its own
// socket to talk to the master server on, it lets the game use its socket to forward messages
// back and forth. This prevents us from requiring server ops to open up yet another port
// in their firewalls.
//
// the IP address and port should be in host order, i.e 127.0.0.1 == 0x7f000001
	
	// These are used when you've elected to multiplex the game server's UDP socket
	// rather than having the master server updater use its own sockets.
	// 
	// Source games use this to simplify the job of the server admins, so they 
	// don't have to open up more ports on their firewalls.
	
	// Call this when a packet that starts with 0xFFFFFFFF comes in. That means
	// it's for us.
	virtual bool HandleIncomingPacket( const void *pData, int cbData, uint32 srcIP, uint16 srcPort )
	{
		return true;
	}

	// AFTER calling HandleIncomingPacket for any packets that came in that frame, call this.
	// This gets a packet that the master server updater needs to send out on UDP.
	// It returns the length of the packet it wants to send, or 0 if there are no more packets to send.
	// Call this each frame until it returns 0.
	virtual int GetNextOutgoingPacket( void *pOut, int cbMaxOut, uint32 *pNetAdr, uint16 *pPort )
	{
		return 0;
	}


// Functions to set various fields that are used to respond to queries.
	
	// Call this to set basic data that is passed to the server browser.
	virtual void SetBasicServerData(
		unsigned short nProtocolVersion,
		bool bDedicatedServer,
		const char *pRegionName,
		const char *pProductName,
		unsigned short nMaxReportedClients,
		bool bPasswordProtected,
		const char *pGameDescription )
	{
	}

	// Call this to clear the whole list of key/values that are sent in rules queries.
	virtual void ClearAllKeyValues()
	{
	}
	
	// Call this to add/update a key/value pair.
	virtual void SetKeyValue( const char *pKey, const char *pValue )
	{
	}


	// You can call this upon shutdown to clear out data stored for this game server and
	// to tell the master servers that this server is going away.
	virtual void NotifyShutdown()
	{
	}

	// Returns true if the master server has requested a restart.
	// Only returns true once per request.
	virtual bool WasRestartRequested()
	{
		return false;
	}

	// Force it to request a heartbeat from the master servers.
	virtual void ForceHeartbeat()
	{
	}
	
	// Manually edit and query the master server list.
	// It will provide name resolution and use the default master server port if none is provided.
	virtual bool AddMasterServer( const char *pServerAddress )
	{
		return true;
	}
	virtual bool RemoveMasterServer( const char *pServerAddress )
	{
		return true;
	}
	
	virtual int GetNumMasterServers()
	{
		return 0;
	}
	
	// Returns the # of bytes written to pOut.
	virtual int GetMasterServerAddress( int iServer, char *pOut, int outBufferSize )
	{
		return 0;
	}
};

}
