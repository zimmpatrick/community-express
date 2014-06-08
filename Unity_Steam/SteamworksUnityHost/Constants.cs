/*
 * Community Express SDK
 * http://www.communityexpresssdk.com/
 *
 * Copyright (c) 2011-2014, Zimmdot, LLC
 * All rights reserved.
 *
 * Subject to terms and condition provided in LICENSE.txt
 * Dual licensed under a Commercial Development and LGPL licenses.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace CommunityExpressNS
{
    /// <summary>
    /// These are the namespace comments for CommunityExpressNS
    /// </summary>
    [CompilerGenerated]
    class NamespaceDoc
    {
    }

	/// <summary>
    ///  General result codes
	/// </summary>
	public enum EResult
	{
        /// success
		EResultOK = 1,
        /// generic failure
        EResultFail = 2,
        /// no/failed network connection
        EResultNoConnection = 3,
        // OBSOLETE - removed
        //EResultNoConnectionRetry = 4,				
        /// password/ticket is invalid
        EResultInvalidPassword = 5,
        /// same user logged in elsewhere
        EResultLoggedInElsewhere = 6,
        /// protocol version is incorrect
        EResultInvalidProtocolVer = 7,
        /// a parameter is incorrect
        EResultInvalidParam = 8,
        /// file was not found
        EResultFileNotFound = 9,
        /// called method busy - action not taken
        EResultBusy = 10,
        /// called object was in an invalid state
        EResultInvalidState = 11,
        /// name is invalid
        EResultInvalidName = 12,
        /// email is invalid
        EResultInvalidEmail = 13,
        /// name is not unique
        EResultDuplicateName = 14,
        /// access is denied
        EResultAccessDenied = 15,
        /// operation timed out
        EResultTimeout = 16,
        /// VAC2 banned
        EResultBanned = 17,
        /// account not found
        EResultAccountNotFound = 18,
        /// steamID is invalid
        EResultInvalidSteamID = 19,
        /// The requested service is currently unavailable
        EResultServiceUnavailable = 20,
        /// The user is not logged on
        EResultNotLoggedOn = 21,
        /// Request is pending (may be in process, or waiting on third party)
        EResultPending = 22,
        /// Encryption or Decryption failed
        EResultEncryptionFailure = 23,
        /// Insufficient privilege
        EResultInsufficientPrivilege = 24,
        /// Too much of a good thing
        EResultLimitExceeded = 25,
        /// Access has been revoked (used for revoked guest passes)
        EResultRevoked = 26,
        /// License/Guest pass the user is trying to access is expired
        EResultExpired = 27,
        /// Guest pass has already been redeemed by account, cannot be acked again
        EResultAlreadyRedeemed = 28,
        /// The request is a duplicate and the action has already occurred in the past, ignored this time
        EResultDuplicateRequest = 29,
        /// All the games in this guest pass redemption request are already owned by the user
        EResultAlreadyOwned = 30,
        /// IP address not found
        EResultIPNotFound = 31,						
		/// failed to write change to the data store
        EResultPersistFailed = 32,					
		/// failed to acquire access lock for this operation
        EResultLockingFailed = 33,					
		/// Logon session replaced
        EResultLogonSessionReplaced = 34,
		/// Failed to connect
        EResultConnectFailed = 35,
		/// Failed to 
        EResultHandshakeFailed = 36,
		/// In-out failure
        EResultIOFailure = 37,
		/// Remote disconnect
        EResultRemoteDisconnect = 38,
        /// failed to find the shopping cart requested
        EResultShoppingCartNotFound = 39,
        /// a user didn't allow it
        EResultBlocked = 40,
        /// target is ignoring sender
        EResultIgnored = 41,
        /// nothing matching the request found
        EResultNoMatch = 42,						
		/// user's account is disabled
        EResultAccountDisabled = 43,
        /// this service is not accepting content changes right now
        EResultServiceReadOnly = 44,
        /// account doesn't have value, so this feature isn't available
        EResultAccountNotFeatured = 45,
        /// allowed to take this action, but only because requester is admin
        EResultAdministratorOK = 46,
        /// A Version mismatch in content transmitted within the Steam protocol.
        EResultContentVersion = 47,
        /// The current CM can't service the user making a request, user should try another.
        EResultTryAnotherCM = 48,					
        /// You are already logged in elsewhere, this cached credential login has failed.
        EResultPasswordRequiredToKickSession = 49,	
        /// You are already logged in elsewhere, you must wait
        EResultAlreadyLoggedInElsewhere = 50,		
        /// Long running operation (content download) suspended/paused
        EResultSuspended = 51,						
        /// Operation canceled (typically by user: content download)
        EResultCancelled = 52,						
        /// Operation canceled because data is ill formed or unrecoverable
        EResultDataCorruption = 53,					
        /// Operation canceled - not enough disk space.
        EResultDiskFull = 54,						
        /// an remote call or IPC call failed
        EResultRemoteCallFailed = 55				
	};
    /// <summary>
    /// Codes for the mode the game server is in
    /// </summary>
	public enum EServerMode
	{
        /// Don't authenticate user logins and don't list on the server list
        eServerModeNoAuthentication = 1,
        /// Authenticate users, list on the server list, don't run VAC on clients that connect
        eServerModeAuthentication = 2,
        /// Authenticate users, list on the server list and VAC protect clients
        eServerModeAuthenticationAndSecure = 3, 
	};
	
	/// <summary>
    /// Error codes for use with the voice functions
	/// </summary>
	public enum EVoiceResult
	{
        /// Voice is usable
		EVoiceResultOK = 0,
        /// Voice is not on currently
		EVoiceResultNotInitialized = 1,
        /// Voice is not recording
		EVoiceResultNotRecording = 2,
        /// Voice has no data
		EVoiceResultNoData = 3,
        /// Voice buffer is too small
		EVoiceResultBufferTooSmall = 4,
        /// Voice data is corrupted
		EVoiceResultDataCorrupted = 5
	};

	/// <summary>
    /// Result codes to GSHandleClientDeny/Kick
	/// </summary>
	public enum EDenyReason
	{
        /// Invalid user
		EDenyInvalid = 0,
        /// Invalid game version
		EDenyInvalidVersion = 1,
        /// Generic denial
		EDenyGeneric = 2,
        /// User not logged on
		EDenyNotLoggedOn = 3,
        /// User does not have a game license
		EDenyNoLicense = 4,
        /// User failed anti-cheat check
		EDenyCheater = 5,
        /// User is logged in on another machine
		EDenyLoggedInElseWhere = 6,
        /// Unknown text
		EDenyUnknownText = 7,
        /// Incompatible anti-cheat code
		EDenyIncompatibleAnticheat = 8,
        /// Memory corruption
		EDenyMemoryCorruption = 9,
        /// Incompatible computer software
		EDenyIncompatibleSoftware = 10,
        /// Connection to steam lost
		EDenySteamConnectionLost = 11,
        /// Error in steam connection
		EDenySteamConnectionError = 12,
        /// Steam response taking too long
		EDenySteamResponseTimedOut = 13,
        /// Steam validtaion stalled
		EDenySteamValidationStalled = 14,
        /// Steam game owner left a guest user
		EDenySteamOwnerLeftGuestUser = 15
	};

	/// <summary>
    /// results from BeginAuthSession
	/// </summary>
	public enum EBeginAuthSessionResult
	{
        /// Ticket is valid for this game and this steamID.
		EBeginAuthSessionResultOK = 0,
        /// Ticket is not valid.
        EBeginAuthSessionResultInvalidTicket = 1,			
        /// A ticket has already been submitted for this steamID
        EBeginAuthSessionResultDuplicateRequest = 2,		
        /// Ticket is from an incompatible interface version
        EBeginAuthSessionResultInvalidVersion = 3,			
        /// Ticket is not for this game
        EBeginAuthSessionResultGameMismatch = 4,			
        /// Ticket has expired
        EBeginAuthSessionResultExpiredTicket = 5			
	};

	/// <summary>
    /// Callback values for callback ValidateAuthTicketResponse_t which is a response to BeginAuthSession
	/// </summary>
 
	public enum EAuthSessionResponse
	{
        /// Steam has verified the user is online, the ticket is valid and ticket has not been reused.
		EAuthSessionResponseOK = 0,
        /// The user in question is not connected to steam
        EAuthSessionResponseUserNotConnectedToSteam = 1,
        /// The license has expired.
        EAuthSessionResponseNoLicenseOrExpired = 2,
        /// The user is VAC banned for this game.
        EAuthSessionResponseVACBanned = 3,
        /// The user account has logged in elsewhere and the session containing the game instance has been disconnected.
        EAuthSessionResponseLoggedInElseWhere = 4,
        /// VAC has been unable to perform anti-cheat checks on this user
        EAuthSessionResponseVACCheckTimedOut = 5,
        /// The ticket has been canceled by the issuer
        EAuthSessionResponseAuthTicketCanceled = 6,
        /// This ticket has already been used, it is not valid.
        EAuthSessionResponseAuthTicketInvalidAlreadyUsed = 7,
        /// This ticket is not from a user instance currently connected to steam.
        EAuthSessionResponseAuthTicketInvalid = 8				
	};

	/// <summary>
    /// results from UserHasLicenseForApp
	/// </summary>
 
	public enum EUserHasLicenseResult
	{
        /// User has a license for specified app
        EUserHasLicenseResultHasLicense = 0,
        /// User does not have a license for the specified app
        EUserHasLicenseResultDoesNotHaveLicense = 1,
        /// User has not been authenticated
        EUserHasLicenseResultNoAuth = 2							
	};

	/// <summary>
    /// Steam universes.  Each universe is a self-contained Steam instance.
	/// </summary>
 
	public enum EUniverse
	{
        /// Invalid universe
		EUniverseInvalid = 0,
        /// Public universe
		EUniversePublic = 1,
        /// Beta universe
		EUniverseBeta = 2,
        /// Internal universe
		EUniverseInternal = 3,
        /// Development universe
		EUniverseDev = 4,
        /// RC universe
		EUniverseRC = 5
	};
	
	/// <summary>
    /// Steam account types
	/// </summary>
	public enum EAccountType
	{
        /// <summary>Invalid account</summary>
		EAccountTypeInvalid = 0,
        /// single user account
        EAccountTypeIndividual = 1,		
        /// multiseat (e.g. cybercafe) account
        EAccountTypeMultiseat = 2,		
        /// game server account
        EAccountTypeGameServer = 3,		
        /// anonymous game server account
        EAccountTypeAnonGameServer = 4,	
        /// pending
        EAccountTypePending = 5,		
        /// content server
        EAccountTypeContentServer = 6,	
		/// Clan account
        EAccountTypeClan = 7,
		/// Chat account
        EAccountTypeChat = 8,
        /// Fake SteamID for local PSN account on PS3 or Live account on 360, etc.
        EAccountTypeConsoleUser = 9,	
		/// Anonymous user
        EAccountTypeAnonUser = 10
	};
}
