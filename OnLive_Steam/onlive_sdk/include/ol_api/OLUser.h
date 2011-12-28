/*************************************************************************************
			Copyright(c)   2009 - 2011 OnLive, Inc.  All Rights Reserved.

NOTICE TO USER:

This code contains confidential material proprietary to OnLive, Inc. Your access to
and use of these confidential materials is subject to the terms and conditions of
your confidentiality and SDK license agreements with OnLive. This document and
information and ideas herein may not be disclosed, copied, reproduced or distributed
to anyone outside your company without the prior written consent of OnLive.

You may not modify, reverse engineer, or otherwise attempt to use this code for
purposes not specified in your SDK license agreement with OnLive. This material,
including but not limited to documentation and related code, is protected by U.S.
and international copyright laws and other intellectual property laws worldwide
including, but not limited to, U.S. and international patents and patents pending.
OnLive, MicroConsole, Brag Clips, Mova, Contour, the OnLive logo and the Mova logo
are trademarks or registered trademarks of OnLive, Inc. The OnLive logo and the Mova
logo are copyrights or registered copyrights of OnLive, Inc. All other trademarks
and other intellectual property assets are the property of their respective owners
and are used by permission. The furnishing of these materials does not give you any
license to said intellectual property.

THIS SOFTWARE IS PROVIDED BY ONLIVE "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.
*************************************************************************************/
//
// OnLive Service Platform API
// OLUser.h $Revision: 91436 $
//

/// \file OLUser.h

#ifndef OLUSER_H
#define OLUSER_H

#include <ol_api/OLAPI.h>

namespace olapi
{

	/// \addtogroup OLUSER_API_PRIVACY_STATUS
	/// 

	/*! \brief Returns true if the supplied game privacy flag is set.
		\param[in] olGameStatus		The status flagset from the \ref olapi::OLUserSessionEventHandler::GamePrivacyStatusResult
										event (requested with \ref olapi::OLUserSession::GetGamePrivacyStatus).
		\param[in] flag				An \ref olapi::OLGameStatus enum flag to test against the status value.
		\return true if the flag is set in the supplied olGameStatus.
	*/
	#define IsGamePrivacyFlagSet(olGameStatus, flag) ((olGameStatus) & (flag))


	///
	/// \brief These GameStatus privacy flags allow runtime control over Brag Clips and Game Spectating.
	///
	///	\see 
	/// - \ref OLUserSession::GetGamePrivacyStatus.
	/// - \ref OLUserSessionEventHandler::GamePrivacyStatusResult.
	///	- \ref OLUserSession::SetGamePrivacyStatus.
	///
	enum OLGameStatus
	{
		OL_GAME_PUBLIC						= 0,	///< Allow anyone to spectate and take Brag Clips.
		OL_GAME_SPECTATING_PRIVATE			= 1,	///< Disable spectating entirely.
		OL_GAME_SPECTATING_PRIVATE_AUDIO	= 2,	///< Disable spectating audio (video is available).
		OL_GAME_BRAG_CLIP_PRIVATE			= 4,	///< Disable Brag Clip captures entirely.
		OL_GAME_BRAG_CLIP_PRIVATE_AUDIO		= 8,	///< Disable Brag Clip audio capture (video is available).

		OL_GAME_STATUS_MASK = OL_GAME_SPECTATING_PRIVATE | OL_GAME_SPECTATING_PRIVATE_AUDIO | OL_GAME_BRAG_CLIP_PRIVATE | OL_GAME_BRAG_CLIP_PRIVATE_AUDIO,
	} ;

	/// 

	/// \addtogroup OLUSER_API_SESSION_STATUS_CONTAINER
	/// 

	/*! \brief The name of the UserSessionStatus container. */
	#define USERSESSIONSTATUSCONTAINERNAME	"UserSessionStatus"


	///
	/// \brief Identifies how the title was launched (as a Trial or Full/Rental PlayPass version).
	///
	/// \see \ref OLUSER_API_SESSION_STATUS_CONTAINER.
	///
	enum OLLicenseType
	{
		OL_LICENSE_TYPE_UNKNOWN = -1,				///< Unknown.
		OL_LICENSE_TYPE_GAME,						///< This title is owned by the user, it is a Full title license.
		// SDK Version 1.173 rename from OL_LICENSE_TYPE_DEMO
		OL_LICENSE_TYPE_TRIAL,						///< This title is a Trial.
		OL_LICENSE_TYPE_RENTAL,						///< This title is rented by the user, it is a Full title license.
		// SDK Version 1.107 addition
 		OL_LICENSE_TYPE_CHANNEL,					///< This title is part of a Channel PlayPack.
	};

	///
	/// \brief Used to index into the \ref olapi::UserSessionStatusContainerKeyNames array.
	///
	/// \note You should not use the deprecated keys with containers; the container values may not be legitimate.
	///
	/// \see \ref OLUSER_API_SESSION_STATUS_CONTAINER and \ref CONTAINERS_PAGE_USING_KEYS.
	///
	enum UserSessionStatusContainerKeys
	{
		OLKEY_RO_UserSessionStatusName,				///< string, read only, Name of the container (see \#define \ref USERSESSIONSTATUSCONTAINERNAME).
		OLKEY_RO_MasterSessionID,					///< OLID,   read only, Session ID for the user when connected to OnLive.
		OLKEY_RO_Tagname,							///< string, read only, The user's tag name (OnLive user name).  See \ref ONLIVE_TAG_PAGE.
		OLKEY_RO_UserID,							///< OLID,   read only, User ID of the current user.
		OLKEY_RO_Latency,							///< int,    read only, DEPRECATED - Latency between user and the OnLiveGameServer machine in milliseconds.
		OLKEY_RO_Bandwidth,							///< int,    read only, DEPRECATED - Approximate bandwidth of user's connection to OnLive.
		// SDK Version 1.011 addition
		OLKEY_RO_UserSessionID,						///< OLID,   read only, Session ID of the currently bound user; same as \ref OLUserSession::GetSessionID.
		// SDK Version 1.063 additions
		OLKEY_RO_LicenseType,						///< int, cast to \ref olapi::OLLicenseType enum; indicates how the game was launched (as a Trial or Full/Rental PlayPass version).
		// SDK Version 1.166 additions
		OLKEY_RO_UserPurchasedContentIDs,			///< a vector of IDs that indicate Content this user has purchased for this title
		OLKEY_RO_UserSessionStatusKeysMax			///< indicates the array size; not an index.
	};

	///
	/// \brief Array of keyName strings for the UserSessionStatus container, indexed by the \ref UserSessionStatusContainerKeys enum.
	///
	/// See \ref OLAPPSESSION_API_SESSION_STATUS_CONTAINER and \ref CONTAINERS_PAGE_USING_KEYS.
	///
	#ifndef NO_OLAPI_DLL_LINKAGE
	extern __declspec(dllimport) char *UserSessionStatusContainerKeyNames[OLKEY_RO_UserSessionStatusKeysMax];
	#endif

	/// 

	/// \addtogroup OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER
	/// 

	///
	/// \brief The platform the user is running OnLive from.
	///
	/// See the OLKEY_RO_Platform value in the UserSystemSettings Container.
	///
	enum OLClientPlatform
	{
		OL_CLIENT_MICROCONSOLE = 0,					///< Client is OnLive MicroConsole.
		OL_CLIENT_WINDOWS,							///< Client is Windows.
		OL_CLIENT_MAC,								///< Client is Mac.		
		// SDK Version 1.088 additions
		OL_CLIENT_IOS,								///< Client is IOS (iPad, etc.).
		// SDK Version 1.172 additions
		OL_CLIENT_ANDROID,							///< Client is Android (HTC, etc.).
		// SDK Version 1.183 additions
		OL_CLIENT_GOOGLETV,							///< Client is Google TV.
		OL_CLIENT_MAX
	};

	///
	/// \brief The number of audio channels and user supports.
	///
	/// See the OLKEY_RO_AudioConfiguration value in the UserSystemSettings Container.
	/// 
	///
	enum OLAudio
	{
		OL_AUDIO_2_0 = 0,					///< Audio channel 2.0.
		OL_AUDIO_5_1,						///< Audio channel 5.1.
		OL_AUDIO_7_1,						///< Audio channel 7.1.
		OL_AUDIO_MAX
	};

	/// \brief OLKEY_RO_Langauge value for English (US) - see \ref OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER
	#define OL_LANGUAGE_ENGLISH		"english"
	/// \brief OLKEY_RO_Langauge value for French (Standard) - see \ref OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER
	#define OL_LANGUAGE_FRENCH		"french"	
	/// \brief OLKEY_RO_Langauge value for Italian (Standard) - see \ref OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER
	#define OL_LANGUAGE_ITALIAN		"italian"
	/// \brief OLKEY_RO_Langauge value for German (Standard) - see \ref OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER
	#define OL_LANGUAGE_GERMAN		"german"	
	/// \brief OLKEY_RO_Langauge value for Spanish (Spain) - see \ref OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER
	#define OL_LANGUAGE_SPANISH		"spanish"
	/// \brief OLKEY_RO_Langauge value for Japanese - see \ref OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER
	#define OL_LANGUAGE_JAPANESE	"japanese"
	/*! \brief The name of the UserSystemSettings container.
	*/
	#define USERSYSTEMSETTINGSCONTAINERNAME	"UserSystemSettings"

	///
	/// \brief The rating system used in user's region.
	///
	/// See the OLKEY_RO_GameRatingSystem value in the UserSystemSettings Container.
	/// 
	///
	enum OLGameRatingSystem
	{
		OL_GAME_RATING_SYSTEM_UNKNOWN = -1,		///< Unknown.
		OL_GAME_RATING_SYSTEM_PEGI = 0,			///< PEGI rating.
		OL_GAME_RATING_SYSTEM_ESRB,				///< ESRB rating.
		OL_GAME_RATING_SYSTEM_MAX
	};

	///
	/// \brief The rating symbols in ESRB rating system.
	///
	/// See the OLKEY_RO_GameRatingType value in the UserSystemSettings Container.
	/// 
	///
	enum OLEsrbRating
	{
		OL_ESRB_NOT_A_GAME = -1,		///< The title is not a game.
		OL_ESRB_RESERVED = 0,			///< Reserved for internal usage.
		OL_ESRB_EARLY_CHILDHOOD = 1,	///< Stands for EC (Early Childhood).
		OL_ESRB_EVERYONE,				///< Stands for E (Everyone).
		OL_ESRB_EVERYONE_TEN_PLUS,		///< Stands for E10+ (Everyoen 10 and older).
		OL_ESRB_TEEN,					///< Stands for T (Teen).
		OL_ESRB_MATURE,					///< Stands for M (Mature).
		OL_ESRB_ADULTS_ONLY,			///< Stands for AO (Adults Only).
		OL_ESRB_RATING_PENDING,			///< Stands for RP (Rating Pending).
		OL_ESRB_UNRATED,				///< The title is not rated.
	};

	///
	/// \brief The content descriptors in ESRB rating system.
	///
	/// See the OLKEY_RO_GameRatingDescriptors value in the UserSystemSettings Container.
	/// 
	///
	typedef __int64 OLEsrbContentDescriptors;
	#define OL_ESRB_ALCOHOL_REFERENCE			0x0000000000000001LL	///< Alcohol Reference.
	#define OL_ESRB_ANIMATED_BLOOD				0x0000000000000002LL	///< Animated Blood.
	#define OL_ESRB_BLOOD						0x0000000000000004LL	///< Blood .
	#define OL_ESRB_BLOOD_AND_GORE				0x0000000000000008LL	///< Blood and Gore.
	#define OL_ESRB_CARTOON_VIOLENCE			0x0000000000000010LL	///< Cartoon Violence.
	#define OL_ESRB_COMIC_MISCHIEF				0x0000000000000020LL	///< Comic Mischief.
	#define OL_ESRB_CRUDE_HUMOR					0x0000000000000040LL	///< Crude Humor.
	#define OL_ESRB_DRUG_REFERENCE				0x0000000000000080LL	///< Drug Reference.
	#define OL_ESRB_FANTASY_VIOLENCE			0x0000000000000100LL	///< Fantasy Violence.
	#define OL_ESRB_INTENSE_VIOLENCE			0x0000000000000200LL	///< Intense Violence.
	#define OL_ESRB_LANGUAGE					0x0000000000000400LL	///< Language .
	#define OL_ESRB_LYRICS						0x0000000000000800LL	///< Lyrics .
	#define OL_ESRB_MATURE_HUMOR				0x0000000000001000LL	///< Mature Humor.
	#define OL_ESRB_NUDITY						0x0000000000002000LL	///< Nudity .
	#define OL_ESRB_PARTIAL_NUDITY				0x0000000000004000LL	///< Partial Nudity.
	#define OL_ESRB_REAL_GAMBLING				0x0000000000008000LL	///< Real Gambling.
	#define OL_ESRB_SEXUAL_CONTENT				0x0000000000010000LL	///< Sexual Content.
	#define OL_ESRB_SEXUAL_THEMES				0x0000000000020000LL	///< Sexual Themes.
	#define OL_ESRB_SEXUAL_VIOLENCE				0x0000000000040000LL	///< Sexual Violence.
	#define OL_ESRB_SIMULATED_GAMBLING			0x0000000000080000LL	///< Simulated Gambling.
	#define OL_ESRB_STRONG_LANGUAGE				0x0000000000100000LL	///< Strong Language.
	#define OL_ESRB_STRONG_LYRICS				0x0000000000200000LL	///< Strong Lyrics.
	#define OL_ESRB_STRONG_SEXUAL_CONTENT		0x0000000000400000LL	///< Strong Sexual Content.
	#define OL_ESRB_SUGGESTIVE_THEMES			0x0000000000800000LL	///< Suggestive Themes.
	#define OL_ESRB_TOBACCO_REFERENCE			0x0000000001000000LL	///< Tobacco Reference.
	#define OL_ESRB_USE_OF_DRUGS				0x0000000002000000LL	///< Use of Drugs.
	#define OL_ESRB_USE_OF_ALCOHOL				0x0000000004000000LL	///< Use of Alcohol.
	#define OL_ESRB_USE_OF_TOBACCO				0x0000000008000000LL	///< Use of Tobacco.
	#define OL_ESRB_VIOLENCE					0x0000000010000000LL	///< Violence .
	#define OL_ESRB_VIOLENT_REFERENCES			0x0000000020000000LL	///< Violent References.
	#define OL_ESRB_MILD_ALCOHOL_REFERENCE		0x0000000100000000LL	///< Mild Alcohol Reference.
	#define OL_ESRB_MILD_ANIMATED_BLOOD			0x0000000200000000LL	///< Mild Animated Blood.
	#define OL_ESRB_MILD_BLOOD					0x0000000400000000LL	///< Mild Blood.
	#define OL_ESRB_MILD_BLOOD_AND_GORE			0x0000000800000000LL	///< Mild Blood and Gore.
	#define OL_ESRB_MILD_CARTOON_VIOLENCE		0x0000001000000000LL	///< Mild Cartoon Violence.
	#define OL_ESRB_MILD_COMIC_MISCHIEF			0x0000002000000000LL	///< Mild Comic Mischief.
	#define OL_ESRB_MILD_CRUDE_HUMOR			0x0000004000000000LL	///< Mild Crude Humor.
	#define OL_ESRB_MILD_DRUG_REFERENCE			0x0000008000000000LL	///< Mild Drug Reference.
	#define OL_ESRB_MILD_FANTASY_VIOLENCE		0x0000010000000000LL	///< Mild Fantasy Violence.
	#define OL_ESRB_MILD_INTENSE_VIOLENCE		0x0000020000000000LL	///< Mild Intense Violence.
	#define OL_ESRB_MILD_LANGUAGE				0x0000040000000000LL	///< Mild Language.
	#define OL_ESRB_MILD_LYRICS					0x0000080000000000LL	///< Mild Lyrics.
	#define OL_ESRB_MILD_MATURE_HUMOR			0x0000100000000000LL	///< Mild Mature Humor.
	#define OL_ESRB_MILD_NUDITY					0x0000200000000000LL	///< Mild Nudity.
	#define OL_ESRB_MILD_PARTIAL_NUDITY			0x0000400000000000LL	///< Mild Partial Nudity.
	#define OL_ESRB_MILD_REAL_GAMBLING			0x0000800000000000LL	///< Mild Real Gambling.
	#define OL_ESRB_MILD_SEXUAL_CONTENT			0x0001000000000000LL	///< Mild Sexual Content.
	#define OL_ESRB_MILD_SEXUAL_THEMES			0x0002000000000000LL	///< Mild Sexual Themes.
	#define OL_ESRB_MILD_SEXUAL_VIOLENCE		0x0004000000000000LL	///< Mild Sexual Violence.
	#define OL_ESRB_MILD_SIMULATED_GAMBLING		0x0008000000000000LL	///< Mild Simulated Gambling.
	#define OL_ESRB_MILD_STRONG_LANGUAGE		0x0010000000000000LL	///< Mild Strong Language.
	#define OL_ESRB_MILD_STRONG_LYRICS			0x0020000000000000LL	///< Mild Strong Lyrics.
	#define OL_ESRB_MILD_STRONG_SEXUAL_CONTENT	0x0040000000000000LL	///< Mild Strong Sexual Content.
	#define OL_ESRB_MILD_SUGGESTIVE_THEMES		0x0080000000000000LL	///< Mild Suggestive Themes.
	#define OL_ESRB_MILD_TOBACCO_REFERENCE		0x0100000000000000LL	///< Mild Tobacco Reference.
	#define OL_ESRB_MILD_USE_OF_DRUGS			0x0200000000000000LL	///< Mild Use of Drugs.
	#define OL_ESRB_MILD_USE_OF_ALCOHOL			0x0400000000000000LL	///< Mild Use of Alcohol.
	#define OL_ESRB_MILD_USE_OF_TOBACCO			0x0800000000000000LL	///< Mild Use of Tobacco.
	#define OL_ESRB_MILD_VIOLENCE				0x1000000000000000LL	///< Mild Violence.
	#define OL_ESRB_MILD_VIOLENT_REFERENCES		0x2000000000000000LL	///< Mild Violent References.

	///
	/// \brief The rating symbols in PEGI rating system.
	///
	/// See the OLKEY_RO_GameRatingType value in the UserSystemSettings Container.
	/// 
	///
	enum OLPegiRating
	{
		OL_PEGI_NOT_A_GAME = -1,		///< The title is not a game.
		OL_PEGI_RESERVED = 0,			///< Reserved for internal usage.
		OL_PEGI_3 = 1,					///< Stands for PEGI 3.
		OL_PEGI_7,						///< Stands for PEGI 7.
		OL_PEGI_12,						///< Stands for PEGI 12.
		OL_PEGI_16,						///< Stands for PEGI 16.
		OL_PEGI_18,						///< Stands for PEGI 18.
		OL_PEGI_RATING_PENDING,			///< The title is awaiting rating.
		OL_PEGI_UNRATED,				///< The title is not rated.
	};

	///
	/// \brief The content descriptors in PEGI rating system.
	///
	/// See the OLKEY_RO_GameRatingDescriptors value in the UserSystemSettings Container.
	/// 
	///
	typedef __int64 OLPegiContentDescriptors;
	#define OL_PEGI_VIOLENCE					0x0000000000000001LL		///< Violence.
	#define OL_PEGI_BAD_LANGUAGE				0x0000000000000002LL		///< Bad Language.
	#define OL_PEGI_FEAR						0x0000000000000004LL		///< Fear.
	#define OL_PEGI_SEX							0x0000000000000008LL		///< Sex.
	#define OL_PEGI_DRUGS						0x0000000000000010LL		///< Drugs.
	#define OL_PEGI_GAMBLING					0x0000000000000020LL		///< Gambling.
	#define OL_PEGI_DISCRIMINATION				0x0000000000000040LL		///< Discrimination.
	#define OL_PEGI_ONLINE						0x0000000000000080LL		///< Online gameplay.

	///
	/// \brief Used to index into the \ref olapi::UserSystemSettingsContainerKeyNames array.
	///
	/// \note You should not use the deprecated keys with containers; the container values may not be legitimate.
	///
	/// \see
	/// - \ref OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER.
	/// - \ref CONTAINERS_PAGE_USING_KEYS.
	///
	enum UserSystemSettingsContainerKeys
	{
		OLKEY_RO_UserSystemSettingsName,			///< string,		read only,  container name; see \#define \ref USERSYSTEMSETTINGSCONTAINERNAME.
		OLKEY_RO_TVOutputMode,						///< string,		read only,  DEPRECATED - Current resolution of the console or PC. - Do not use.
		OLKEY_RO_Language,							///< string,		read only,  user's preferred language; see the language \#defines in OLUser.h.
		OLKEY_RW_AudioVolume,						///< float,			read write, 0.0 to 1.0; the master volume level.
		// undeprecated in SDK Version 1.201 //
		OLKEY_RO_GameServerCountryCode,				///< string,		read only,  the country code of the location where your title is running. This field is compliant with ISO 3166-1 alpha 2 standard. Currently OnLive supports \"US\"(United States) and \"GB\" (United Kingdom). More country will be added in the future.
		// SDK Version 1.009 additions
		OLKEY_RO_Platform,							///< int,			read only,  the user's platform.  Cast the int to an \ref OLClientPlatform enum.
		OLKEY_RO_Keyboard,							///< int,			read only,  cast to bool, true means Keyboard is connected.
		OLKEY_RO_Mouse,								///< int,			read only,  cast to bool, true means Mouse is connected.
		OLKEY_RO_Gamepad,							///< int,			read only,  number of gamepads connected.
		// SDK Version 1.023 additions
		OLKEY_RO_AudioConfiguration,				///< int,			read only,  cast to an \ref OLAudio enum; represents the user's audio channel support.
		// SDK Version 1.041 additions
		OLKEY_RO_VibrationSupported,				///< int,			read only,  cast to bool, true means vibration is supported in Gamepad driver.
		// SDK Version 1.088 additions
		OLKEY_RO_TouchScreen,						///< int,			read only,  cast to bool, true means touch screen is available on this platform.
		// SDK Version 1.183 additions			
		OLKEY_RO_Voice,								///< int,			read only,  cast to bool, true means voice is supported on this platform.
		// SDK Version 1.199 additions		
		OLKEY_RO_TouchPoints,						///< int,			read only,  number of touchpoints the touch device will support.
		// SDK Version 1.201 additions  //
		OLKEY_RO_GameRatingSystem,					///< int,			read only,  cast to an \ref OLGameRatingSystem enum; represents the game rating system used in user's region.
		//
		OLKEY_RO_GameRatingCategory,				///< int,			read only,  cast to an \ref OLEsrbRating or \ref OLPegiRating enum based on the value from \ref OLKEY_RO_GameRatingSystem; represents the game rating category.
		OLKEY_RO_GameRatingDescriptors,				///< bit field,		read only,  a combination of \ref OLEsrbContentDescriptors or \ref OLPegiContentDescriptors based on the value from \ref OLKEY_RO_GameRatingSystem; represents the game rating descriptors.

		OLKEY_RO_UserSystemSettingsKeysMax
	};

	///
	/// \brief Array of keyName strings for the UserSystemSettings container, indexed by the \ref UserSystemSettingsContainerKeys enum.
	///
	/// See \ref OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER and \ref CONTAINERS_PAGE_USING_KEYS.
	///
	#ifndef NO_OLAPI_DLL_LINKAGE
	extern __declspec(dllimport) char *UserSystemSettingsContainerKeyNames[OLKEY_RO_UserSystemSettingsKeysMax];
	#endif

	/// 


	///
	/// \brief DEPRECATED - Additional context info for a OLUserSessionEventHandler::LoadGameRequest or a
	///			OLUserSessionEventHandler::SaveGameRequest event.
	///
	/// Load and save game requests are deprecated and should be avoided by new titles.
	///
	enum OLUIRequestType
	{
		OL_UIREQUEST_SUSPEND,						///< This request is hidden, and used during shutdown after a SuspendSession responds OL_NOT_IMPLEMENTED.
		OL_UIREQUEST_HIDDEN,						///< DEPRECATED - The request is hidden and selection is automated.
		OL_UIREQUEST_AUTOMATIC,						///< DEPRECATED - Status of operation is shown, selection is automated.
	};


	class OLUserSessionEventHandler;
	class OLUserSessionEventWaitingCallback;

	///
	/// \ingroup OLUSER_API
	/// \class OLUserSession
	///	\brief The OLUserSession API is responsible for communicating user events, such as the state of a user,
	///     while the user is bound to your title. The OLUserSession API also gives you access 
	///		to the bound user's session status, system settings, and privacy settings and allows you to control 
	///		input timeouts and handle user events, such as pause and resume.
	///
	/// \par API Initialization
	///		The OLUserSession singleton is created and retrieved with \ref olapi::OLGetUser() and destroyed 
	///		with \ref olapi::OLStopUser().\n\n
	///		NOTE: OLUserSession depends on the OLService API and should be initialized after it. \n
	///		See \ref OLUSER_API_CREATE_AND_DESTROY and \ref SDK_SESSION_FLOW_PAGE for general info about 
	///		initializing the OnLive SDK APIs.
	///
	/// \par API Event and Callback Handling
	///		See \ref SDK_EVENTS_PAGE for an overview or \ref OLUSER_API_EVENT_HANDLING for specifics.
	///
	/// \par Getting Information about the user or user session
	///		The user session status container holds info about the user's session (user tag, userId,
	///		etc).  See \ref GetStatus and \ref OLUSER_API_SESSION_STATUS_CONTAINER.\n\n
	/// 
	///     The UserSystemSettings container has info about the user's preferences and hardware
	///		(language/locale, audio channels and volume, availability of mouse/keyboard/controller, etc).
	///		See \ref GetUserSystemSettings and \ref OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER.\n\n
	///
	/// \par Game Privacy Status Flags
	///		The Game Privacy Status Flags give you runtime control over users spectating your title
	///		or creating Brag Clips.  For example, if a 3rd party online system uses an email address
	///		as the username, you should disable Brag Clips and spectating while the email address is
	///		visible.  See \ref SetGamePrivacyStatus and \ref OLUSER_API_PRIVACY_STATUS.
	///
	///	\par Inactivity Timeouts
	///		The OnLive Service Platform (OSP) has two inactivity checks: a 5+ minute user inactivity timeout and a 15 second 
	///		video stall detection timeout. Each can be disabled temporarily if your title expects to be inactive. 
	///		See \ref SuspendInputTimeout / \ref SuspendVideoStallDetection and \ref OLUSER_API_INACTIVITY_TIMEOUTS.
	///
	class OLUserSession : public OLEventBase
	{
		public:

		/// \addtogroup OLUSER_API_EVENT_HANDLING
		/// 
		/// \name OLUserSession Callbacks & Event Handling
		/// 

		/// 
		/// \brief Sets the passed in callback handler object with the OLUserSession API.
		///
		/// \par Description:
		/// If you intend to handle callbacks for OLUserSession, you should set
		/// a callback handler object before you start the OnLive Services with \ref olapi::OLStartServices.
		/// Once services are started, callbacks can be dispatched by the SDK at any time.
		///
		/// \note OLUserSession stores a single callback handler object pointer; calling this method
		/// multiple times will replace the previous pointer.  There is no way to 'unregister'
		/// the pointer (you cannot set it to NULL).
		///
		///	\see - \ref EVENTS_PAGE_REGISTRATION for details.
		///
		///		\param[in]	callbackHandler	Your implementation of the \ref OLUserSessionEventWaitingCallback interface.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS			Normal operation; callback handlers can be dispatched on the supplied obj while
		///										the services are running.
		///		\retval OL_INVALID_PARAM	Error - The supplied callback handler obj pointer was NULL (or invalid);
		///										the callbackHandler obj has not been stored.
		///
		virtual OLStatus SetEventWaitingCallback(OLUserSessionEventWaitingCallback* callbackHandler) = 0;




		///
		/// \brief Sets the passed in event handler object with the OLUserSession API.
		///
		/// \par Description:
		/// Once you have set the OLUserSession API's event handler, calling one of the OLUserSessionEventWaitingCallback methods
		///		may dispatch events on your object (assuming events are available to process).
		///		
		///
		/// \note OLUserSession stores a single event handler pointer; calling this method
		/// multiple times will replace the previous pointer.  There is no way to 'unregister'
		/// the pointer (you cannot set it to NULL).  
		///
		///	\see 
		/// - \ref EVENTS_PAGE_HANDLING_OVERVIEW.
		/// - \ref EVENTS_PAGE_REGISTRATION for details.
		///
		///		\param	eventHandler	Your implementation of the OLUserSessionEventHandler interface.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS			Normal operation; events will be dispatched on the supplied obj.
		///		\retval OL_INVALID_PARAM	Error - The supplied event handler pointer was NULL (or invalid);
		///										the event handler has not been set.
		///
		virtual OLStatus SetEventHandler(OLUserSessionEventHandler* eventHandler) = 0;




		///
		/// \brief Sets the passed in event handler and attempts to handle a single event. 
		/// \note Prefer to use the simpler
		///  overload \ref HandleEvent(unsigned int).
		///
		/// \par Description:
		/// This method acts as a combination of \ref SetEventHandler() and \ref HandleEvent(unsigned int).
		/// If the supplied event handler is not NULL, we set the event handler to the supplied value then try to handle a single event.
		/// If the supplied event handler is NULL, we try to handle a single event using the previously set handler.
		///
		/// \see 
		/// - \ref SDK_EVENTS_PAGE 
		///
		///		\param[in]	eventHandler	If NULL, we use the last set event handler.
		///									Otherwise, the API's event handler is set to this handler.
		///
		///		\param[in]	waitTimeMs	The max amount of time to wait for an event in milliseconds.
		///										Use 0 to return immediately if there is no event waiting (non-blocking).
		///										Otherwise, this call may block for up to waitTimeMs if no event is available.
		///										You can use \ref OL_INFINITE to wait forever for the next event.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS			An event was dispatched.
		///		\retval OL_INVALID_PARAM	The supplied handler was NULL, but the API does not have an event handler set.
		///		\retval OL_EVENT_TIMEOUT	No events were available for the duration of the wait.
		///
		virtual OLStatus HandleEvent(OLUserSessionEventHandler* eventHandler, unsigned int waitTimeMs) = 0;




		///
		/// \brief Attempts to dispatch a single event on this API's registered event handler object.
		///
		/// \par Description:
		/// You must register an OLUserSessionEventHandler instance with \ref OLUserSession::SetEventHandler() before
		///		calling HandleEvent().
		///
		/// \see - \ref SDK_EVENTS_PAGE 
		///
		///		\param[in]	waitTimeMs	The max amount of time to wait for an event in milliseconds.
		///										Use 0 to return immediately if there is no event waiting (non-blocking).
		///										Otherwise, this call may block for up to waitTimeMs if no event is available.
		///										You can use \ref OL_INFINITE to wait forever for the next event.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS			An event was dispatched.
		///		\retval OL_INVALID_PARAM	The API does not have an event handler stored (see \ref OLUserSession::SetEventHandler).
		///		\retval OL_EVENT_TIMEOUT	No events were available for the duration of the wait.
		///
		virtual OLStatus HandleEvent(unsigned int waitTimeMs) = 0;

		/// 
		
		/// 

		/// \addtogroup OLUSER_API_INACTIVITY_TIMEOUTS
		/// 
		/// \name Controlling Inactivity Timeouts
		/// 

		///
		/// \brief Temporarily disables the OnLive Service Platform (OSP)'s user inactivity timeout for up to 5 minutes at a time.
		///
		/// \par Description:
		/// If a user is inactive (no keyboard/mouse/controller input) for more than 5 minutes,
		/// the OSP may disconnect the user and start the \ref SDK_SESSION_FLOW_SECTION_UNGRACEFUL_EXIT.
		/// The inactivity timeout is configurable within the OSP, but will never be less than 5 minutes.
		/// 
		/// \note If you expect the user to be inactive for more than 5 minutes (ex: during 
		///	end credits or a long cut-scene), use this method to disable the inactivity timeout
		/// for up to 5 minutes at a time.  When the user is expected to be active again, 
		/// call \ref ResumeInputTimeout() to re-enable the inactivity timeout.
		///
		/// \responseEventList
		///		The OSP automatically enables the timer after 5 minutes and sends an unsolicited StatusResult 
		///		event letting you know:
		///
		///		\ospEvent OLUserSessionEventHandler::StatusResult
		///			\eventDesc	An unsolicited event with olStatus \ref OL_INPUTTIMER_REACTIVATED
		///						means the OSP has re-enabled the inactivity timer.  If you still
		///						expect the user to be inactive, call SuspendInputTimeout again.
		///						This cycle can be repeated indefinitely.
		///
		///						This unsolicited event has a single status code value:
		///							\statusResult OL_INPUTTIMER_REACTIVATED
		///
		///
		///	\responseEventListEnd
		///
		/// \note The actual inactivity timeout used by OnLive may be longer than 5 minutes, 
		///		but we guarantee that it will never be less than 5 minutes.  Assume that it is 5 minutes
		///		and your users will not see OnLive's UI warning that they will be disconnected due to inactivity.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS					Success, the input timeout is disabled.
		///		\retval OL_FAIL						Unable to perform the request. (e.g. OSP's user inactivity timeout is already disabled).
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus SuspendInputTimeout() = 0;




		///
		/// \brief Re-enables the OnLive Service Platform's 5+ minute user inactivity timeout.
		/// \par Description:
		/// If a user is inactive (no keyboard/mouse/controller input) for more than 5+ minutes,
		/// the OnLive Service Platform (OSP) will disconnect the user and start the \ref SDK_SESSION_FLOW_SECTION_UNGRACEFUL_EXIT.
		///
		/// \see \ref SuspendInputTimeout()
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS					Success, the input timeout is re-enabled.
		///		\retval OL_FAIL						Unable to perform the request. (e.g. OSP's user inactivity timeout is not disabled).
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus ResumeInputTimeout() = 0;




		///
		/// \brief Temporarily disables the OSP's 15 second video stall detection.
		///
		/// \par Description:
		/// The OnLive Service Platform (OSP) expects your title to render frames while a user is bound to your title.
		/// If the OSP does not detect a frame render for 15 seconds, it may 
		/// disconnect the user and start the \ref SDK_SESSION_FLOW_SECTION_UNGRACEFUL_EXIT.
		/// \par
		/// If you expect your title to stop rendering, you should disable video stall detection
		/// with this method and re-enable it once you are rendering again (see \ref ResumeVideoStallDetection).
		///
		/// \note Currently, the OSP detects stalls by tracking buffer swaps (i.e.: the d3d Present/PresentEx 
		/// call, or equivalent). In the future, however, the OSP will be able to track framebuffer differences.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS					Success, the video stall detection is disabled.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus SuspendVideoStallDetection() = 0;




		///
		/// \brief Re-enables the OnLive Service Platform (OSP)'s 15 second video stall detection.
		///
		/// \par Description:
		/// The OSP expects your title to render frames while a user is bound to your title.
		/// If the OSP does not detect a frame render for 15 seconds, it may 
		/// disconnect the user and start the \ref SDK_SESSION_FLOW_SECTION_UNGRACEFUL_EXIT.
		///
		/// \see 
		/// \ref SuspendVideoStallDetection().
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS			Success, the input timeout is re-enabled.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus ResumeVideoStallDetection() = 0;

		/// 

		/// 


		///
		/// \ingroup OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER
		/// \brief Requests the UserSystemSettings container from the OnLive Service Platform (OSP).  The OSP responds
		///		with either a ContainerIDResult() event on success or a StatusResult event on failure.
		///
		/// \par Description:
		/// The UserSystemSettings container holds a user's settings and preferences; for example:
		///		language and volume, available input hardware (mouse/keyboard/controller), etc
		///		(see \ref olapi::UserSystemSettingsContainerKeys).
		/// 
		///	\note The title can use GetUserSystemSettings() to read the user's platform type and assist in defaulting to the correct layout; this includes
		///     the use and availability of specific keys, such as the Windows key, Menu key, and Command key.
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent OLUserSessionEventHandler::ContainerIDResult
		///			\eventDesc	Returns the ID of the UserSystemSettings Container.
		///						Note: The SDK Test Harness does not populate this container
		///						(you will get a container with empty or default values).
		///
		///		\ospEvent OLUserSessionEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR	Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the container.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context		Used to pair this request with its event response (see \ref olapi::OLGetNextContext()).
		///
		///	\return	Returns an OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult() or StatusResult() event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus GetUserSystemSettings(OLContext context) = 0;




		///
		/// \ingroup OLUSER_API_USER_SESSION_ID
		/// \brief Requests the current user session's ID from the OnLive Service Platform (OSP).  The OSP responds
		///		with either an IDListResult() event on success or a StatusResult() event on failure.
		///
		/// \par Description:
		/// The user session ID is not normally needed by SDK users, but it is used internally for tracking session metrics.
		/// This is the same value as in the UserSessionStatus container (see \ref olapi::OLKEY_RO_UserSessionID).
		///
		/// \responseEventList
		///		The SDK responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent OLUserSessionEventHandler::IDListResult
		///			\eventDesc	Returns the ID of the UserSessionStatus Container.
		///						If the SessionID is already cached on the client, the GetSessionID call can 
		///						trigger the event dispatch immediately as a side effect.
		///
		///		\ospEvent OLUserSessionEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates that the OSP could not fulfill this request
		///						 due to one of the following errors:
		///
		///				\statusResult OL_SESSION_NOT_STARTED	The user session was invalid by the time the OSP got the request.
		///
		/// \responseEventListEnd
		/// 
		///		\param[in]	context		Used to pair this request with its event response (see \ref olapi::OLGetNextContext()).
		///
		///	\return	Returns an OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect an IDListResult or StatusResult Event response matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session.
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus GetSessionID(OLContext context) = 0;




		///
		/// \ingroup OLUSER_API_SESSION_STATUS_CONTAINER
		/// \brief Requests the current UserSessionStatus container from the OnLive Service Platform (OSP).  The OSP responds
		///		with either a ContainerIdResult() event on success or a StatusResult() event on failure.
		///
		/// \par Description:
		/// The UserSessionStatus container provides information about the currently bound user.
		/// For example, the user's tag (username), ID, and their license type for this title.
		/// \see 
		/// \ref olapi::UserSessionStatusContainerKeys.
		///
		///	\note This is the same container you receive in the \ref olapi::OLAppSessionEventHandler::BindUser
		///	event.
		///
		/// \responseEventList
		///		The SDK responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent OLUserSessionEventHandler::ContainerIDResult
		///			\eventDesc	Provides the ContainerID of the UserSessionStatus container.
		///						See \ref OLUSER_API_SESSION_STATUS_CONTAINER.
		///
		///		\ospEvent OLUserSessionEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates that the OSP could not fulfill this request
		///						 due to one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR	Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the container.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context		Used to pair this request with its event response (see \ref olapi::OLGetNextContext()).
		///
		///	\return	Returns an OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult() or StatusResult() event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session.
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus GetStatus(OLContext context) = 0;



		/// \name Game Privacy Status
		/// 

		///
		/// \ingroup OLUSER_API_PRIVACY_STATUS
		/// \brief Sets the title's privacy status flags (control the ability of users to spectate
		///		or anyone to capture Brag Clips of the title).
		///
		/// \par Description:
		/// There are 3 tiers of privacy settings:\n
		///   - Title Settings (specified in your title's submission form)\n
		///   - User Settings (the user's preferences, edited in the OnLive Overlay Menu)\n
		///   - Runtime Settings (calling this method)
		/// \par
		/// The overall privacy values are these three settings or'd together.  This means your title
		/// can become more private than default at runtime, but can never override either the title-wide
		/// settings or the bound user's privacy preferences.
		/// \par
		/// A title's default runtime privacy status is \ref OL_GAME_PUBLIC, allowing anyone to spectate
		/// or capture a Brag Clip.  You can only change the privacy flags while a user is bound to your title.
		/// When a user unbinds from your title, the privacy flags are reset back to OL_GAME_PUBLIC.
		///
		/// \note The OLGameStatus value should be treated as an integer flagset (it will contain the set flags or'd together,
		///		rather than a discrete value from the OLGameStatus enum).
		///
		/// \see \ref OLUSER_API_PRIVACY_STATUS.
		///
		///		\param[in]	privacyStatus	The new privacy status flag(s).  Set multiple flags by with bitwise-or
		///									then cast the result back to an OLGameStatus.
		///
		///	\return	Returns an OLStatus code.
		///		\retval OL_SUCCESS					New privacy settings sent to the OnLive Service Platform (OSP).
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus SetGamePrivacyStatus(OLGameStatus privacyStatus) = 0;

		/// 

		/// \name Game Privacy Status
		/// 


		///
		/// \ingroup OLUSER_API_PRIVACY_STATUS
		/// \brief Retrieve the title's current runtime privacy status flags from the OnLive Service Platform (OSP); the result is
		///		returned in an \ref OLUserSessionEventHandler::GamePrivacyStatusResult Event.
		/// \par Description:
		///	You can only retrieve the title's privacy status while a user is bound.  These bits reflect only the
		/// title's runtime privacy settings; the OSP sets the total privacy settings by merging the title-wide
		/// privacy bits specified for your title with the bound user's privacy settings and the game's
		/// runtime settings. 
		///
		/// \see 
		/// - \ref OLUSER_API_PRIVACY_STATUS.
		/// - \ref SetGamePrivacyStatus.
		///		\param[in]	context		Used to pair this request with the response event (see \ref olapi::OLGetNextContext()).
		///
		///	\return	Returns an OLStatus code.
		///		\retval OL_SUCCESS					Request sent to the OSP.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus GetGamePrivacyStatus(OLContext context) = 0;

		/// 

	};

	///
	/// \ingroup OLUSER_API OLUSER_API_EVENT_HANDLING
	/// \class OLUserSessionEventHandler
	/// \brief This is the OLUserSession API's event interface; your OLUserSessionEventHandler() must implement this interface.
	///		Each method represents an asynchronous event returned by the SDK.
	///
	/// See \ref EVENTS_PAGE_CONCEPTS for an overview of event handling.
	///
	/// \par Pause/Resume Events
	///		Gameplay must be paused while the OnLive Overlay is displayed; the \ref OLUserSessionEventHandler::Pause
	///		and \ref OLUserSessionEventHandler::Resume events let you know when to do so.
	///
	///
	class OLUserSessionEventHandler
	{
		public:


		///
		/// \brief Event returning an OLStatus code result; this typically means an error was
		///		encountered while the OnLive Service Platform (OSP) was processing an async request.
		///
		/// \par Example:
		/// If the OnLive Service Platform (OSP) ends a user session at the same time as a request  for the 
		/// user session's status, the SDK will return OL_SUCCESS (because the SDK is 
		/// unaware of the OSP ending the user session at the time the request was made). 
		/// However, by the time the request for the user session's status gets to the OSP, 
		/// the user session has ended, and the request is invalid. The OSP dispatches a StatusResult 
		/// event with OL_SESSION_NOT_STARTED.
		///
		/// \see \ref EVENTS_PAGE_ASYNC_COMM.
		///
		///		\param[in] context		The matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in] status		An OLStatus code indicating the result of your request (typically an error).
		///
		///	\return An OLStatus code indicating if you have handled the event.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///
		virtual OLStatus StatusResult(OLContext context, OLStatus status) = 0;




		///
		/// \brief Event returning a list of OnLive IDs.
		///
		/// \par Description:
		/// The actual IDs in the list will depend on the type of request you made.
		/// For example, if this event is in response to a \ref olapi::OLUserSession::GetSessionID request,
		/// the list will contain a single application session ID.
		///
		/// \see \ref OLIDList
		///
		///		\param[in] context		The matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in] idList		The list of IDs; the list is invalidated when this method returns, so save off any needed data.
		///
		///	\return Returns an OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///
		virtual OLStatus IDListResult(OLContext context, OLIDList *idList) = 0;




		///
		/// \brief Event returning a containerID result; indicates that a request Successfully resulted
		///		in a container.
		///
		/// \note If you return OL_SUCCESS, the container will have its normal
		///		session flow (which varies by container type).  Any other return value causes
		///		the SDK to destroy the container immediately after this method returns.
		///
		/// \see \ref SDK_CONTAINERS_PAGE. 
		///
		///		\param[in] context		The matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in] containerID	The requested container's ID.  The container's type will depend on what you requested.
		///
		///	\return Returns an OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled; the container is valid until destroyed.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored (and the container will be destroyed immediately).
		///
		virtual OLStatus ContainerIDResult(OLContext context, OLID containerID) = 0;




		///
		/// \ingroup OLUSER_API_PAUSE_RESUME_EVENTS
		/// \brief Event sent when the game should pause gameplay (when the user brings up the
		///			OnLive Overlay or is having significant network problems).
		///	\par Description:
		///	Pausing should affect gameplay and any movies or cut scenes including bumper trailers or videos shown outside of a 
		/// game level.  All audio, including streaming or continuous audio, should pause while the title is in the pause state and continue 
		/// playing from the same point after a \ref Resume() event; pausing should not halt rendering.  Your title will not be able to receive 
		/// input from the user while it is paused.  The OnLive Service Platform (OSP) will send a Resume() event when gameplay should resume.
		/// \par 
		/// The title must process OnLive messages while paused and be able to resume automatically on reception of a Stop() event,  
		/// UnbindUser() event, or any conflicting OnLive event.
		/// 
		/// \note
		/// - Please be careful to ensure that the title's in-game pause state is retained across OnLive Pause/Resume events.  E.g., an end 
		/// user hits the start button to bring up the in-game pause menu, then presses the home button to bring up the OnLive Overlay.  Displaying 
		/// the overlay will trigger a Pause() event, and closing the overlay will send a Resume() event. The title's in-game pause menu should 
		/// remain in pause mode after the title handles the OnLive Resume() event.
		/// - It is very important that multiplayer titles handle pause properly - a pause event
		///	should not interrupt the title's network processing or disconnect the title's users.  
		///
		/// \par Exceptions
		/// When a Pause() event is received in an active multiplayer title, instead of pausing gameplay and audio, the title 
		/// must mute or duck the game audio by 50% (ducking is preferred).  
		/// \par
		/// The title can choose to pause or not pause during non-interactive loading sequences that do not 
		/// present the user with important information.  If the title does not pause, it must pause once the loading sequence is complete unless a 
		/// Resume() event has been received. This is a better user experience by overlapping the load sequence with the users time in the overlay.
		///
		///	\return Returns an OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled; gameplay is paused.
		///		\retval OL_FAIL					The event was ignored; gameplay is already paused.  The OSP will not 
		///											shutdown the game.
		///
		virtual OLStatus Pause() = 0;




		///
		/// \ingroup OLUSER_API_PAUSE_RESUME_EVENTS
		/// \brief Event sent when the game should restore gameplay to its previous state (the state the game was
		///		in when it received the \ref Pause event).
		///
		/// \par Description
		/// This is typically sent when a user closes the OnLive Overlay or when a user with network connectivity
		/// problems recovers.
		///
		/// \note The title must be careful to handle a case where it receives an unsolicited Resume() event (a sent Resume() event without first 
		/// being paused or without being in a user session).
		///
		///	\return Returns an OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled; the gameplay state is restored.
		///		\retval OL_FAIL					The event was ignored. gameplay is not paused. The OnLive Service Platform (OSP) will not 
		///											shutdown the game.
		///
		virtual OLStatus Resume() = 0;




		///
		/// \ingroup OLUSER_API_PRIVACY_STATUS
		/// \brief Event containing the title's current privacy status flags; (triggered by \ref OLUserSession::GetGamePrivacyStatus).
		/// \par Description:
		/// Event from the OnLive Service Platform containing the results of a title's request for its own privacy
		/// status flags. Privacy status is the five <I>OLGameStatus</I> bit flags, or'ed together.
		/// \note The OLGameStatus value should be treated as an integer flagset (it will contain the set flags or'd together,
		///		rather than a discrete value from the OLGameStatus enum).
		///
		///		\param[in] context		The matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param	privacyStatus	The current game privacy status flagset.
		///
		///	\return	Returns an OLStatus code.
		///		\retval OL_SUCCESS					Event successfully handled.
		///		\retval OL_NOT_IMPLEMENTED			Event ignored intentionally.
		///		\retval OL_FAIL						The event was not handled due to an unexpected error.
		///
		virtual OLStatus GamePrivacyStatusResult( OLContext context, OLGameStatus privacyStatus) = 0;
	};

	///
	/// \ingroup OLUSER_API_EVENT_HANDLING OLUSER_API
	/// \class OLUserSessionEventWaitingCallback
	/// \brief This is the OLUserSession API's callback interface; your title's OLUserSession callbackHandler must implement this interface.
	///
	/// Each method in this class represents an OLUserSession callback.  If you choose to handle OLUserSession callbacks,
	/// store the passed in callbackHandler using \ref OLUserSession::SetEventWaitingCallback.  Callback methods
	/// may be invoked at any time once the OnLive Services are running (see \ref OLSERVICE_API_CONNECTION_DISCONNECTION).
	///
	/// \note Callback methods are always invoked from an internal OnLive SDK thread, so all callback implementations must be thread-safe.
	///
	/// See \ref EVENTS_PAGE_CONCEPTS for details.
	///
	class OLUserSessionEventWaitingCallback
	{
		public:


		///
		/// \brief Callback dispatched when an OLUserSession event is waiting to be handled.
		///
		/// \par Description:
		/// If your title handles events from callbacks, call \ref OLUserSession::HandleEvent(unsigned int) to dispatch the waiting event.
		///	If you handle events by polling, you can ignore this event.
		///
		/// \note The EventWaiting callback must be thread-safe and must complete in less than two milliseconds.
		///
		/// \see - \ref EVENTS_PAGE_HANDLING_OVERVIEW.
		///
		virtual void EventWaiting() = 0;
	};

	/// \addtogroup OLUSER_API_CREATE_AND_DESTROY
	/// 


	///
	/// \brief Gets the OLUserSession API singleton (the instance is created if necessary).
	///
	/// \par Description:
	///	The OLUserSession pointer can be stored until the API is destroyed with \ref olapi::OLStopUser().
	///
	/// \see \ref OLUSER_API_CREATE_AND_DESTROY.
	///
	///		\param[in] version			The API version string (for class compatibility), use \ref OLAPI_VERSION.
	///
	///		\param[out] olUserSession	The supplied pointer is set to the OLUserSession instance on success.
	///									The pointer is not modified on failure.
	///
	///	\return Returns an OLStatus code.
	///		\retval	OL_SUCCESS			Success; the olUserSession pointer is set to the singleton.
	///		\retval OL_INVALID_PARAM	The supplied version string or olUserSession pointer is NULL.
	///		\retval OL_INVALID_VERSION	The API version requested is not supported. This is a fatal 
	///										error and your title should exit.
	///		\retval OL_INTERNAL_ERROR	A general internal or system error.
	///		\retval OL_OUT_OF_MEMORY	The SDK could not alloc memory for the singleton.
	///		\retval OL_SERVICES_ALREADY_STARTED
	///									This API was initialized after \ref olapi::OLStartServices() was called.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLGetUser(const char* version, OLUserSession** olUserSession);

	///
	/// \brief Destroys the OLUserSession API singleton.
	///
	/// \par Description:
	/// The OnLive Services should be stopped before you destroy an API instance. 
	///
	/// \see 
	/// - \ref OLUSER_API_CREATE_AND_DESTROY 	
	/// - \ref olapi::OLStopServices()
	///
	///		\param[in] olUserSession		Pointer to the OLUserSession instance to destroy.
	///
	///	\return Returns an OLStatus code.
	///		\retval	OL_SUCCESS			Success; the olUserSession was destroyed.
	///		\retval OL_INVALID_PARAM	The supplied olUserSession pointer was NULL.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLStopUser(OLUserSession* olUserSession);

	/// 


/*!

\addtogroup OLUSER_API
	\brief The OLUserSession API is responsible for communicating user events while the user is bound to your title;
			this includes the user's state, session status, system settings, and privacy settings. This API also allows you to 
			control inactivity timeouts, such as pause and resume.

\addtogroup OLUSER_API_SESSION_STATUS_CONTAINER
	\brief The UserSessionStatus container holds information about the currently bound user,
		including the username, userID, and userSessionId, and more. 

		Also, see \ref SDK_CONTAINERS_PAGE.

\addtogroup OLUSER_API_USER_SESSION_ID
	\brief The user session ID is not normally needed by SDK users; it is used internally for tracking session metrics. 
	
	
\addtogroup OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER
	\brief The UserSystemSettings container holds a user's settings and preferences, such as language and locale info, audio
		hardware and volume, available input hardware (mouse/keyboard/controller, controller vibration), etc.
		
		Also, see \ref SDK_CONTAINERS_PAGE.

\addtogroup OLUSER_API_PAUSE_RESUME_EVENTS
	\brief The OnLive Service Platform sends pause and resume events when gameplay should stop and be restored.  Typically,
		this is when a user shows or hides the OnLive Overlay MenuSystem, or if a user is having severe
		network connectivity problems. 
		

\addtogroup OLUSER_API_PRIVACY_STATUS
	\brief A title's privacy status flags allow runtime control over spectating and Brag Clips. 
	
	A title may need to suppress spectating or Brag Clips at certain times; see the examples described below:

	<ul>
		<li> User Privacy - Suppress spectating & Brag Clips while the user logs into a 3rd party
			online system (with their email address onscreen).
		<li> Legal Issues - Some titles need to suppress audio while spectating or capturing Brag Clips
			due to music licensing issues.
		<li> Gameplay - You may want to suppress Brag Clips of your end sequence or at other times.
	</ul>

	Note: The title's privacy status can be overridden by either the title's configuration (the settings
	specified when you submitted your title to certification), or by the bound user's privacy settings.


\addtogroup OLUSER_API_INACTIVITY_TIMEOUTS
	\brief The OnLive Service Platform has two inactivity checks: a 5+ minute user inactivity timeout and a 15 second video stall
		detection timeout.  Each can be disabled temporarily if your title expects to be inactive.
	The actual user inactivity timeout duration may vary, but it will always be >= 5 minute.  If your title
	expects the user to be inactive (no input) for 5 minutes or longer, you should disable the inactivity timeout.
	If either timeout expires, the OnLive Service Platform will disconnect the title's bound user and start the 
	\ref SDK_SESSION_FLOW_SECTION_UNGRACEFUL_EXIT.
	

\addtogroup OLUSER_API_CREATE_AND_DESTROY
	\brief The OLUserSession singleton is created and retrieved with \ref olapi::OLGetUser() and destroyed with \ref olapi::OLStopUser(). Do 
	not destroy any of the OnLive APIs (or their handlers) until after you have stopped the OnLive Services. 
	
		Also, see: 
				- \ref SDK_SESSION_FLOW_PAGE.

				- \ref OLSERVICE_API_CONNECTION_DISCONNECTION.

\addtogroup OLUSER_API_EVENT_HANDLING
	\brief Callbacks and events are used for asynchronous communication to and from the OnLive Service Platform. 
	
	Also, see \ref SDK_EVENTS_PAGE.

*/

}; // namespace olapi

#endif // OLUSERSESSION_H
