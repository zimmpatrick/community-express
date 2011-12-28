
#pragma once

#include "stdafx.h"

namespace SteamAPI
{

class SteamFriends : public ISteamFriends
{
public:
	virtual const char *GetPersonaName()
	{
		TRACE( TEXT("GetPersonaName()") );
		return "";
	}
	
	// sets the player name, stores it on the server and publishes the changes to all friends who are online
	virtual void SetPersonaName( const char *pchPersonaName )
	{
	}

	// gets the status of the current user
	virtual EPersonaState GetPersonaState()
	{
		return k_EPersonaStateOffline;
	}

	// friend iteration
	// takes a set of k_EFriendFlags, and returns the number of users the client knows about who meet that criteria
	// then GetFriendByIndex() can then be used to return the id's of each of those users
	virtual int GetFriendCount( int iFriendFlags )
	{
		return 0;
	}

	// returns the steamID of a user
	// iFriend is a index of range [0, GetFriendCount())
	// iFriendsFlags must be the same value as used in GetFriendCount()
	// the returned CSteamID can then be used by all the functions below to access details about the user
	virtual CSteamID GetFriendByIndex( int iFriend, int iFriendFlags )
	{
		return CSteamID();
	}

	// returns a relationship to a user
	virtual EFriendRelationship GetFriendRelationship( CSteamID steamIDFriend )
	{
		return k_EFriendRelationshipNone;
	}

	// returns the current status of the specified user
	// this will only be known by the local user if steamIDFriend is in their friends list; on the same game server; in a chat room or lobby; or in a small group with the local user
	virtual EPersonaState GetFriendPersonaState( CSteamID steamIDFriend )
	{
		return k_EPersonaStateOffline;
	}

	// returns the name another user - guaranteed to not be NULL.
	// same rules as GetFriendPersonaState() apply as to whether or not the user knowns the name of the other user
	// note that on first joining a lobby, chat room or game server the local user will not known the name of the other users automatically; that information will arrive asyncronously
	// 
	virtual const char *GetFriendPersonaName( CSteamID steamIDFriend )
	{
		return 0;
	}

	// gets the avatar of the current user, which is a handle to be used in IClientUtils::GetImageRGBA(), or 0 if none set
	virtual int GetFriendAvatar( CSteamID steamIDFriend, int eAvatarSize )
	{
		return 0;
	}
	// returns true if the friend is actually in a game, and fills in pFriendGameInfo with an extra details 
	virtual bool GetFriendGamePlayed( CSteamID steamIDFriend, FriendGameInfo_t *pFriendGameInfo )
	{
		return false;
	}
	// accesses old friends names - returns an empty string when their are no more items in the history
	virtual const char *GetFriendPersonaNameHistory( CSteamID steamIDFriend, int iPersonaName )
	{
		return 0;
	}

	// returns true if the specified user meets any of the criteria specified in iFriendFlags
	// iFriendFlags can be the union (binary or, |) of one or more k_EFriendFlags values
	virtual bool HasFriend( CSteamID steamIDFriend, int iFriendFlags )
	{
		return false;
	}

	// clan (group) iteration and access functions
	virtual int GetClanCount()
	{
		return 0;
	}
	virtual CSteamID GetClanByIndex( int iClan )
	{
		return CSteamID();
	}
	virtual const char *GetClanName( CSteamID steamIDClan )
	{
		return 0;
	}
	virtual const char *GetClanTag( CSteamID steamIDClan )
	{
		return 0;
	}

	// iterators for getting users in a chat room, lobby, game server or clan
	// note that large clans that cannot be iterated by the local user
	// steamIDSource can be the steamID of a group, game server, lobby or chat room
	virtual int GetFriendCountFromSource( CSteamID steamIDSource )
	{
		return 0;
	}

	virtual CSteamID GetFriendFromSourceByIndex( CSteamID steamIDSource, int iFriend )
	{
		return CSteamID();
	}

	// returns true if the local user can see that steamIDUser is a member or in steamIDSource
	virtual bool IsUserInSource( CSteamID steamIDUser, CSteamID steamIDSource )
	{
		return true;
	}

	// User is in a game pressing the talk button (will suppress the microphone for all voice comms from the Steam friends UI)
	virtual void SetInGameVoiceSpeaking( CSteamID steamIDUser, bool bSpeaking )
	{
	}

	// activates the game overlay, with an optional dialog to open 
	// valid options are "Friends", "Community", "Players", "Settings", "LobbyInvite", "OfficialGameGroup", "Stats", "Achievements"
	virtual void ActivateGameOverlay( const char *pchDialog )
	{
	}

	// activates game overlay to a specific place
	// valid options are
	//		"steamid" - opens the overlay web browser to the specified user or groups profile
	//		"chat" - opens a chat window to the specified user, or joins the group chat 
	//		"stats" - opens the overlay web browser to the specified user's stats
	//		"achievements" - opens the overlay web browser to the specified user's achievements
	virtual void ActivateGameOverlayToUser( const char *pchDialog, CSteamID steamID )
	{
	}

	// activates game overlay web browser directly to the specified URL
	// full address with protocol type is required, e.g. http://www.steamgames.com/
	virtual void ActivateGameOverlayToWebPage( const char *pchURL ) 
	{
	}

	// activates game overlay to store page for app
	virtual void ActivateGameOverlayToStore( AppId_t nAppID )
	{
	}

	// Mark a target user as 'played with'. This is a client-side only feature that requires that the calling user is 
	// in game 
	virtual void SetPlayedWith( CSteamID steamIDUserPlayedWith )
	{
	}

	// activates game overlay to open the invite dialog. Invitations will be sent for the provided lobby.
	// You can also use ActivateGameOverlay( "LobbyInvite" ) to allow the user to create invitations for their current public lobby.
	virtual void ActivateGameOverlayInviteDialog( CSteamID steamIDLobby )
	{
	}

	// gets the small (32x32) avatar of the current user, which is a handle to be used in IClientUtils::GetImageRGBA(), or 0 if none set
	virtual int GetSmallFriendAvatar( CSteamID steamIDFriend )
	{
		return 0;
	}

	// gets the medium (64x64) avatar of the current user, which is a handle to be used in IClientUtils::GetImageRGBA(), or 0 if none set
	virtual int GetMediumFriendAvatar( CSteamID steamIDFriend )
	{
		return 0;
	}

	// gets the large (184x184) avatar of the current user, which is a handle to be used in IClientUtils::GetImageRGBA(), or 0 if none set
	// returns -1 if this image has yet to be loaded, in this case wait for a AvatarImageLoaded_t callback and then call this again
	virtual int GetLargeFriendAvatar( CSteamID steamIDFriend )
	{
		return 0;
	}

	// requests information about a user - persona name & avatar
	// if bRequireNameOnly is set, then the avatar of a user isn't downloaded 
	// - it's a lot slower to download avatars and churns the local cache, so if you don't need avatars, don't request them
	// if returns true, it means that data is being requested, and a PersonaStateChanged_t callback will be posted when it's retrieved
	// if returns false, it means that we already have all the details about that user, and functions can be called immediately
	virtual bool RequestUserInformation( CSteamID steamIDUser, bool bRequireNameOnly )
	{
		return false;
	}

	// requests information about a clan officer list
	// when complete, data is returned in ClanOfficerListResponse_t call result
	// this makes available the calls below
	// you can only ask about clans that a user is a member of
	// note that this won't download avatars automatically; if you get an officer,
	// and no avatar image is available, call RequestUserInformation( steamID, false ) to download the avatar
	virtual SteamAPICall_t RequestClanOfficerList( CSteamID steamIDClan )
	{
		return 0;
	}

	// iteration of clan officers - can only be done when a RequestClanOfficerList() call has completed
	
	// returns the steamID of the clan owner
	virtual CSteamID GetClanOwner( CSteamID steamIDClan )
	{
		return CSteamID();
	}
	// returns the number of officers in a clan (including the owner)
	virtual int GetClanOfficerCount( CSteamID steamIDClan ) 
	{
		return 0;
	}
	// returns the steamID of a clan officer, by index, of range [0,GetClanOfficerCount)
	virtual CSteamID GetClanOfficerByIndex( CSteamID steamIDClan, int iOfficer )
	{
		return CSteamID();
	}
	// if current user is chat restricted, he can't send or receive any text/voice chat messages.
	// the user can't see custom avatars. But the user can be online and send/recv game invites.
	// a chat restricted user can't add friends or join any groups.
	virtual uint32 GetUserRestrictions()
	{
		return 0;
	}

	// Rich Presence data is automatically shared between friends who are in the same game
	// Each user has a set of Key/Value pairs
	// Up to 20 different keys can be set
	// There are two magic keys:
	//		"status"  - a UTF-8 string that will show up in the 'view game info' dialog in the Steam friends list
	//		"connect" - a UTF-8 string that contains the command-line for how a friend can connect to a game
	// GetFriendRichPresence() returns an empty string "" if no value is set
	// SetRichPresence() to a NULL or an empty string deletes the key
	// You can iterate the current set of keys for a friend with GetFriendRichPresenceKeyCount()
	// and GetFriendRichPresenceKeyByIndex() (typically only used for debugging)
	virtual bool SetRichPresence( const char *pchKey, const char *pchValue )
	{
		return true;
	}
	virtual void ClearRichPresence()
	{
	}
	virtual const char *GetFriendRichPresence( CSteamID steamIDFriend, const char *pchKey )
	{
		return 0;
	};
	virtual int GetFriendRichPresenceKeyCount( CSteamID steamIDFriend )
	{
		return 0;
	}
	virtual const char *GetFriendRichPresenceKeyByIndex( CSteamID steamIDFriend, int iKey )
	{
		return 0;
	}

	// rich invite support
	// if the target accepts the invite, the pchConnectString gets added to the command-line for launching the game
	// if the game is already running, a GameRichPresenceJoinRequested_t callback is posted containing the connect string
	// invites can only be sent to friends
	virtual bool InviteUserToGame( CSteamID steamIDFriend, const char *pchConnectString )
	{
		return 0;
	}

	// recently-played-with friends iteration
	// this iterates the entire list of users recently played with, across games
	// GetFriendCoplayTime() returns as a unix time
	virtual int GetCoplayFriendCount()
	{
		return 0;
	}
	virtual CSteamID GetCoplayFriend( int iCoplayFriend )
	{
		return CSteamID();
	}
	virtual int GetFriendCoplayTime( CSteamID steamIDFriend )
	{
		return 0;
	}
	virtual AppId_t GetFriendCoplayGame( CSteamID steamIDFriend )
	{
		return 0;
	}
};

}
