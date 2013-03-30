// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
	// General result codes
	public enum EResult
	{
		EResultOK	= 1,							// success
		EResultFail = 2,							// generic failure 
		EResultNoConnection = 3,					// no/failed network connection
	//	EResultNoConnectionRetry = 4,				// OBSOLETE - removed
		EResultInvalidPassword = 5,					// password/ticket is invalid
		EResultLoggedInElsewhere = 6,				// same user logged in elsewhere
		EResultInvalidProtocolVer = 7,				// protocol version is incorrect
		EResultInvalidParam = 8,					// a parameter is incorrect
		EResultFileNotFound = 9,					// file was not found
		EResultBusy = 10,							// called method busy - action not taken
		EResultInvalidState = 11,					// called object was in an invalid state
		EResultInvalidName = 12,					// name is invalid
		EResultInvalidEmail = 13,					// email is invalid
		EResultDuplicateName = 14,					// name is not unique
		EResultAccessDenied = 15,					// access is denied
		EResultTimeout = 16,						// operation timed out
		EResultBanned = 17,							// VAC2 banned
		EResultAccountNotFound = 18,				// account not found
		EResultInvalidSteamID = 19,					// steamID is invalid
		EResultServiceUnavailable = 20,				// The requested service is currently unavailable
		EResultNotLoggedOn = 21,					// The user is not logged on
		EResultPending = 22,						// Request is pending (may be in process, or waiting on third party)
		EResultEncryptionFailure = 23,				// Encryption or Decryption failed
		EResultInsufficientPrivilege = 24,			// Insufficient privilege
		EResultLimitExceeded = 25,					// Too much of a good thing
		EResultRevoked = 26,						// Access has been revoked (used for revoked guest passes)
		EResultExpired = 27,						// License/Guest pass the user is trying to access is expired
		EResultAlreadyRedeemed = 28,				// Guest pass has already been redeemed by account, cannot be acked again
		EResultDuplicateRequest = 29,				// The request is a duplicate and the action has already occurred in the past, ignored this time
		EResultAlreadyOwned = 30,					// All the games in this guest pass redemption request are already owned by the user
		EResultIPNotFound = 31,						// IP address not found
		EResultPersistFailed = 32,					// failed to write change to the data store
		EResultLockingFailed = 33,					// failed to acquire access lock for this operation
		EResultLogonSessionReplaced = 34,
		EResultConnectFailed = 35,
		EResultHandshakeFailed = 36,
		EResultIOFailure = 37,
		EResultRemoteDisconnect = 38,
		EResultShoppingCartNotFound = 39,			// failed to find the shopping cart requested
		EResultBlocked = 40,						// a user didn't allow it
		EResultIgnored = 41,						// target is ignoring sender
		EResultNoMatch = 42,						// nothing matching the request found
		EResultAccountDisabled = 43,
		EResultServiceReadOnly = 44,				// this service is not accepting content changes right now
		EResultAccountNotFeatured = 45,				// account doesn't have value, so this feature isn't available
		EResultAdministratorOK = 46,				// allowed to take this action, but only because requester is admin
		EResultContentVersion = 47,					// A Version mismatch in content transmitted within the Steam protocol.
		EResultTryAnotherCM = 48,					// The current CM can't service the user making a request, user should try another.
		EResultPasswordRequiredToKickSession = 49,	// You are already logged in elsewhere, this cached credential login has failed.
		EResultAlreadyLoggedInElsewhere = 50,		// You are already logged in elsewhere, you must wait
		EResultSuspended = 51,						// Long running operation (content download) suspended/paused
		EResultCancelled = 52,						// Operation canceled (typically by user: content download)
		EResultDataCorruption = 53,					// Operation canceled because data is ill formed or unrecoverable
		EResultDiskFull = 54,						// Operation canceled - not enough disk space.
		EResultRemoteCallFailed = 55				// an remote call or IPC call failed
	};

	public enum EServerMode
	{
		eServerModeNoAuthentication = 1, // Don't authenticate user logins and don't list on the server list
		eServerModeAuthentication = 2, // Authenticate users, list on the server list, don't run VAC on clients that connect
		eServerModeAuthenticationAndSecure = 3, // Authenticate users, list on the server list and VAC protect clients
	};
	
	// Error codes for use with the voice functions
	public enum EVoiceResult
	{
		EVoiceResultOK = 0,
		EVoiceResultNotInitialized = 1,
		EVoiceResultNotRecording = 2,
		EVoiceResultNoData = 3,
		EVoiceResultBufferTooSmall = 4,
		EVoiceResultDataCorrupted = 5
	};

	// Result codes to GSHandleClientDeny/Kick
	public enum EDenyReason
	{
		EDenyInvalid = 0,
		EDenyInvalidVersion = 1,
		EDenyGeneric = 2,
		EDenyNotLoggedOn = 3,
		EDenyNoLicense = 4,
		EDenyCheater = 5,
		EDenyLoggedInElseWhere = 6,
		EDenyUnknownText = 7,
		EDenyIncompatibleAnticheat = 8,
		EDenyMemoryCorruption = 9,
		EDenyIncompatibleSoftware = 10,
		EDenySteamConnectionLost = 11,
		EDenySteamConnectionError = 12,
		EDenySteamResponseTimedOut = 13,
		EDenySteamValidationStalled = 14,
		EDenySteamOwnerLeftGuestUser = 15
	};

	// results from BeginAuthSession
	public enum EBeginAuthSessionResult
	{
		EBeginAuthSessionResultOK = 0,						// Ticket is valid for this game and this steamID.
		EBeginAuthSessionResultInvalidTicket = 1,			// Ticket is not valid.
		EBeginAuthSessionResultDuplicateRequest = 2,		// A ticket has already been submitted for this steamID
		EBeginAuthSessionResultInvalidVersion = 3,			// Ticket is from an incompatible interface version
		EBeginAuthSessionResultGameMismatch = 4,			// Ticket is not for this game
		EBeginAuthSessionResultExpiredTicket = 5			// Ticket has expired
	};

	// Callback values for callback ValidateAuthTicketResponse_t which is a response to BeginAuthSession
	public enum EAuthSessionResponse
	{
		EAuthSessionResponseOK = 0,								// Steam has verified the user is online, the ticket is valid and ticket has not been reused.
		EAuthSessionResponseUserNotConnectedToSteam = 1,		// The user in question is not connected to steam
		EAuthSessionResponseNoLicenseOrExpired = 2,				// The license has expired.
		EAuthSessionResponseVACBanned = 3,						// The user is VAC banned for this game.
		EAuthSessionResponseLoggedInElseWhere = 4,				// The user account has logged in elsewhere and the session containing the game instance has been disconnected.
		EAuthSessionResponseVACCheckTimedOut = 5,				// VAC has been unable to perform anti-cheat checks on this user
		EAuthSessionResponseAuthTicketCanceled = 6,				// The ticket has been canceled by the issuer
		EAuthSessionResponseAuthTicketInvalidAlreadyUsed = 7,	// This ticket has already been used, it is not valid.
		EAuthSessionResponseAuthTicketInvalid = 8				// This ticket is not from a user instance currently connected to steam.
	};

	// results from UserHasLicenseForApp
	public enum EUserHasLicenseResult
	{
		EUserHasLicenseResultHasLicense = 0,					// User has a license for specified app
		EUserHasLicenseResultDoesNotHaveLicense = 1,			// User does not have a license for the specified app
		EUserHasLicenseResultNoAuth = 2							// User has not been authenticated
	};

	// Steam universes.  Each universe is a self-contained Steam instance.
	public enum EUniverse
	{
		EUniverseInvalid = 0,
		EUniversePublic = 1,
		EUniverseBeta = 2,
		EUniverseInternal = 3,
		EUniverseDev = 4,
		EUniverseRC = 5
	};
	
	// Steam account types
	public enum EAccountType
	{
		EAccountTypeInvalid = 0,
		EAccountTypeIndividual = 1,		// single user account
		EAccountTypeMultiseat = 2,		// multiseat (e.g. cybercafe) account
		EAccountTypeGameServer = 3,		// game server account
		EAccountTypeAnonGameServer = 4,	// anonymous game server account
		EAccountTypePending = 5,		// pending
		EAccountTypeContentServer = 6,	// content server
		EAccountTypeClan = 7,
		EAccountTypeChat = 8,
		EAccountTypeAnonUser = 10
	};
}
