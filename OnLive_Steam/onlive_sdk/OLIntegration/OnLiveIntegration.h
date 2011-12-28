///////////////////////////////////////////////////////////////////////////////
//
// OnLiveIntegration.h $Revision: 87606 $
//
// SDK Integrated
//
///////////////////////////////////////////////////////////////////////////////
// 
// This class is used as a basic starting point for integrating OnLive into your
// game.  Most documentation to the classes defined herein are documented in the
// cpp file with minimal documentation in the header file.
// 
///////////////////////////////////////////////////////////////////////////////
// 
//           Copyright(c) 2009 - 2011 OnLive, Inc.  All Rights Reserved.
// 
// NOTICE TO USER:
// 
// This code contains confidential material proprietary to OnLive, Inc. Your access
// to and use of these confidential materials is subject to the terms and conditions
// of your confidentiality and SDK license agreements with OnLive. This document and
// information and ideas herein may not be disclosed, copied, reproduced or distributed
// to anyone outside your company without the prior written consent of OnLive.
// 
// You may not modify, reverse engineer, or otherwise attempt to use this code for
// purposes not specified in your SDK license agreement with OnLive. This material,
// including but not limited to documentation and related code, is protected by U.S.
// and international copyright laws and other intellectual property laws worldwide
// including, but not limited to, U.S. and international patents and patents pending.
// OnLive, MicroConsole, Brag Clips, Mova, Contour, the OnLive logo and the Mova logo
// are trademarks or registered trademarks of OnLive, Inc. The OnLive logo and the Mova
// logo are copyrights or registered copyrights of OnLive, Inc. All other trademarks
// and other intellectual property assets are the property of their respective owners
// and are used by permission. The furnishing of these materials does not give you any
// license to said intellectual property.
// 
// THIS SOFTWARE IS PROVIDED BY ONLIVE "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
// USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

#ifndef __ONLIVE_INTEGRATION_H__
#define __ONLIVE_INTEGRATION_H__

#include "OLAPI.h"
#include "OLApplication.h"
#include "OLService.h"
#include "OLUser.h"

#include "OnLiveContextEventDispatcher.h"

#pragma warning(disable: 4548)	// Disable warning C4548: expression before comma has no effect; expected expression with side-effect
#include <vector>

// NOTE: IN ORDER TO CHANGE THE DEFAULT BEHAVIOR OF ONLIVEINTEGRATION,
//   DEFINE ANY OF THESE VALUES BEFORE INCLUDING OnLiveIntegration.cpp/h

// SUPPORT_SUSPEND_SESSION needs to be define according to
// your game's requirements.  Failure to verify this will cause you game to fail certification.

#ifndef SUPPORT_SUSPEND_SESSION
	#define SUPPORT_SUSPEND_SESSION			(0)		// Turn this on to support suspend session
#endif

// The following is used to tell OnLive Integration to support Single-Session, Multi-Session or both.
// set SUPPORT_SESSION_TYPES to (SINGLE_SESSION), (MULTI_SESSION) or (SINGLE_SESSION | MULTI_SESSION).

#ifndef SUPPORT_SESSION_TYPES
	#define SINGLE_SESSION					(1)
	#define MULTI_SESSION					(2)
	#define SUPPORT_SESSION_TYPES			(SINGLE_SESSION | MULTI_SESSION)
#endif

#ifndef DONT_USE_LOG // This is defined in the samples because of their on screen logging
#undef LOG
#undef DEBUGLOG
#undef FATALOG
#undef INITLOG

#define LOG(...)		{ OnLive::getInstance().Log(olapi::OLLOG_INFO, __VA_ARGS__); }
#define DEBUGLOG(...)	{ OnLive::getInstance().Log(olapi::OLLOG_DEBUG, __VA_ARGS__); }
#define FATALLOG(...)	{ OnLive::getInstance().Log(olapi::OLLOG_FATAL, __VA_ARGS__); }
#define INITLOG()		{ OnLive::getInstance().SetLogLevel( olapi::OLLOG_DEBUG ); }
#endif

#define DEFAULT_SCREEN_WIDTH	(1280)
#define DEFAULT_SCREEN_HEIGHT	(720)
#define DEFAULT_REFRESH_RATE	(60)

// Please refer to class OnLive at the bottom of this header file.

///////////////////////////////////////////////////////////////////////////////
//                              OnLive Service                               //
///////////////////////////////////////////////////////////////////////////////
class OnLiveService
{
public:

	static OnLiveService& getInstance()
	{
		static OnLiveService instance;
		return instance;
	}

	OnLiveService();

	// Initialize OLService
	void init();

	// Shutdown OLService
	void deinit();

	// Are we running from the test harness?
	bool isDbgMode();

	// Get the SDK version string
	const char *getSDKVersion();

	// Log
	void Log(olapi::OLLogLevel level, char* format, ...);

	// Set the Log Level
	olapi::OLStatus SetLogLevel(olapi::OLLogLevel level);
	olapi::OLStatus dbg_EnableSDKLogReception(bool bOnOff);

	// Send a heartbeat (for debug purposes only - usage breaks compliance)
	void SendHeartbeat(olapi::OLContext context);

	// Get a container name
	olapi::OLStatus GetContainerName(olapi::OLID containerID, int size, char *value);
	// Get a container value
	olapi::OLStatus GetContainerValue(olapi::OLID containerID, const char *identifier_in, int size, char *value);
	olapi::OLStatus GetContainerValueAsInt(olapi::OLID containerID, const char *identifier_in, __int64 *value);
	olapi::OLStatus GetContainerValueAsFloat(olapi::OLID containerID, const char *identifier_in, double *value);
	olapi::OLStatus GetContainerValueAsID(olapi::OLID containerID, const char *identifier_in, olapi::OLID *value, olapi::OLIDTypes type);

	// Create a container
	olapi::OLStatus CreateContainer(olapi::OLID *containerID);
	// Destroy a container
	olapi::OLStatus DestroyContainer(olapi::OLID containerID);

	// Set a container value
	olapi::OLStatus SetContainerValue(olapi::OLID containerID, char *identifier_in, char *value);
	olapi::OLStatus SetContainerValueFromInt(olapi::OLID containerID, const char *identifier_in, __int64 value);
	olapi::OLStatus SetContainerValueFromFloat(olapi::OLID containerID, const char *identifier_in, double value);
	olapi::OLStatus SetContainerValueFromID(olapi::OLID containerID, const char *identifier_in, olapi::OLID& value);

private:
	class ServiceEventHandler : public olapi::OLServiceEventHandler
	{
	public:
		ServiceEventHandler();

		// Inherited functions
		void dbg_IsTestHarness();
		olapi::OLStatus dbg_SDKLogReceived(char *buf);
		olapi::OLStatus SDKInfoContainerIDReceived(olapi::OLID containerID);
	
	public:
		bool mDbgMode;
		char mSDKVersion[OL_MAX_VALUE_STRING_SIZE];
	};

	class ServiceEventCallback : public olapi::OLServiceEventWaitingCallback
	{
	public:
		ServiceEventCallback();
		
		// Inherited functions
		void EventWaiting();

	private:
		// No assignment operator
		ServiceEventCallback& operator=(ServiceEventCallback const&) {}
	};

public:
	olapi::OLService* mService;

private:
	ServiceEventHandler* mServiceEventHandler;
	ServiceEventCallback* mServiceEventCallback;
};

///////////////////////////////////////////////////////////////////////////////
//                            OnLive Application                             //
///////////////////////////////////////////////////////////////////////////////
class OnLiveUser;

class OnLiveApplication
{
public:
	// Basic session state machine.
	enum State
	{
		STATE_BOOTUP,				// Initial state
		STATE_STARTING,				// Sent OL_LOGIN_READY_TO_START
		STATE_STARTED,				// Received Start() event
		STATE_BINDING,				// Sent OL_LOGIN_READY_TO_BIND
		STATE_READY_FOR_CLIENT,		// Received BindUser() event
		STATE_RUNNING,				// Sent OL_LOGIN_READY_FOR_CLIENT, running state is automatic after send
		STATE_UNBINDING,			// Sent OL_LOGIN_READY_TO_UNBIND
		STATE_UNBOUND,				// Received UnbindUser() event
		STATE_STOPPING,				// Sent OL_LOGIN_READY_TO_STOP
		STATE_STOPPED,				// Received Stop() event
		STATE_EXITING,				// Received ExitReceived callback
	};

	static OnLiveApplication& getInstance()
	{
		static OnLiveApplication instance;
		return instance;
	}

	OnLiveApplication();

	// Initialize OLApplication
	void init();

	// Shutdown OLApplication
	void deinit();

	// Statrus functions
	bool isMultiSessionMode();

	bool isBooting();
	bool isStarting();
	bool isStarted();
	bool isBound();
	bool isPlaying();
	bool isUnbound();
	bool isExiting();
	bool isTimeToExit();
	bool isRestoredSession();
	bool isSuspendedSession();

	// Flag to signal game is saving/busy and can not exit
	void setGameSavingData(bool issaving);
	bool getGameSavingData();

	// Clears the saving data from any value to false.
	void clearGameSavingData();

	// Debug functions
	void setWindowedMode(bool windowedmode);
	void setVSyncMode(bool windowedmode);

	// Asynchronous indicator of needing to change resolution.
	bool didResolutionChange(bool reset = true);

	// Get the resolution settings.
	int getResX();
	int getResY();
	int getRefreshRate();
	bool getWindowedMode();
	bool getVSyncMode();


	olapi::OLID getClientUserID();
	olapi::OLID getUserSessionID();
	olapi::OLLicenseType getLicenseType();
	const char *getUserTag();

	// Start and stop the game
	bool start(bool wait = true, DWORD timeout = OL_INFINITE);
	bool stop(bool wait = true, olapi::OLExitCode exit_code = olapi::OL_EXITCODE_NORMAL, DWORD timeout = OL_INFINITE);
	bool exit_onlive(olapi::OLExitCode exit_code = olapi::OL_EXITCODE_NORMAL);

	// Try to bind a user
	bool bind(bool wait = true, bool send_ready_for_client = true, DWORD timeout = OL_INFINITE);

	// Unbind a user
	bool unbind(bool wait = true, DWORD timeout = OL_INFINITE);

	// Enter running mode (gameplay mode)
	void ready_for_client();

	// Onlive events
	olapi::OLStatus GetLoginState(void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback pCallbackFunction);
	olapi::OLStatus GetStatusContainer(void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback pCallbackFunction);

private:

	class ApplicationEventHandler : public olapi::OLAppSessionEventHandler
	{
	public:
		ApplicationEventHandler();

		// Inherited functions
		olapi::OLStatus BindUser(bool restoreSession, olapi::OLID identityID, olapi::OLID containerID);
		olapi::OLStatus ChangeResolution(olapi::OLResolution resolution, bool is_windowed_mode, bool is_vsync_on);
		olapi::OLStatus ContainerIDResult(olapi::OLContext context, olapi::OLID id);
		olapi::OLStatus IDListResult(olapi::OLContext context, olapi::OLIDList *idlist);
		olapi::OLStatus LoginStateResult(olapi::OLContext context, olapi::OLLoginState state);
		olapi::OLStatus Start(olapi::OLStartMode mode);
		olapi::OLStatus StatusResult(olapi::OLContext context, olapi::OLStatus status);
		olapi::OLStatus Stop();
		olapi::OLStatus SuspendSession();
		olapi::OLStatus UnbindUser(olapi::OLID identityID);

	public:
		bool mMultiSessionMode;
		int mScreenHeight;
		int mScreenWidth;
		int mRefreshRate;
		bool mWindowedMode;
		bool mVSyncMode;
		bool mResolutionChanged;
		int mGameIsSaving;
		olapi::OLID mClientUserID;
		olapi::OLID mUserSessionID;
		olapi::OLLicenseType mLicenseType;
		char mUserTag[OL_MAX_VALUE_STRING_SIZE];
		bool mRestoreSession;
		bool mSuspendSession;
	};

	class ApplicationEventCallback : public olapi::OLAppSessionCallbacks
	{
	public:
		ApplicationEventCallback();
		
		// Inherited functions
		void EventWaiting();
		void ExitReceived();

	private:
		// No assignment operator
		ApplicationEventCallback& operator=(ApplicationEventCallback const&) {}
	};

private:

	olapi::OLAppSession* mApplication;
	ApplicationEventHandler* mApplicationEventHandler;
	ApplicationEventCallback* mApplicationEventCallback;
};

///////////////////////////////////////////////////////////////////////////////
//                                  OnLive User                              //
///////////////////////////////////////////////////////////////////////////////

class OnLiveUser
{
friend class OnLive;

public:

	static OnLiveUser& getInstance()
	{
		static OnLiveUser instance;
		return instance;
	}

	OnLiveUser();

	// Initialize OLUser
	void init();

	// Shutdown OLUser
	void deinit();

	enum PauseState
	{
		PAUSE_STATE_RUNNING,		// Received Resume() (or never paused)
		PAUSE_STATE_PAUSED,			// Received Pause(false)
	};

	// Initiates a request for user system settings.  Returns false if the
	// response hasn't been received yet, and true if it has.  Keep invoking
	// this function every frame until it returns true.
	bool obtainSystemSettings();

	// Get the current pause state.
	PauseState getPauseState();

	// Set and get game privacy status
	olapi::OLStatus SetGamePrivacyStatus(olapi::OLGameStatus privacyStatus);
	olapi::OLStatus GetGamePrivacyStatus(void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback pCallbackFunction);

	//Suspend/Resume Input Timeout
	olapi::OLStatus SuspendInputTimeout();
	olapi::OLStatus ResumeInputTimeout();

	olapi::OLStatus GetStatusContainer(void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback pCallbackFunction);
	olapi::OLStatus GetUserSystemSettings(void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback pCallbackFunction);

	// Get various user system settings.
	const char *getLanguage();
	bool getKeyboardAndMouse();
	bool getVibrationSupported();
	olapi::OLAudio getAudioConfiguration();

#ifdef USE_CONTENT
	std::vector<olapi::OLID> getContentIDs() { return m_content_id_list; }
	static void CALLBACK unpackUserPurchasedContentIDs(const char* value, unsigned int classPtr);
#endif

private:
	class UserEventHandler : public olapi::OLUserSessionEventHandler
	{
	public:
		UserEventHandler();

		// Inherited functions
		olapi::OLStatus ContainerIDResult(olapi::OLContext context, olapi::OLID id);
		olapi::OLStatus GamePrivacyStatusResult(olapi::OLContext context, olapi::OLGameStatus privacyStatus);
		olapi::OLStatus IDListResult(olapi::OLContext context, olapi::OLIDList *idlist);
		olapi::OLStatus Pause();
		olapi::OLStatus Resume();
		olapi::OLStatus StatusResult(olapi::OLContext context, olapi::OLStatus status);

	public:
		PauseState mPause;
		char mLanguage[OL_MAX_VALUE_STRING_SIZE];
		bool mKeyboard;
		bool mMouse;
		olapi::OLAudio mAudioConfiguration;
		bool mVibrationSupported;
		bool mHaveSystemSettings;
		olapi::OLContext mSystemSettingsContext;
	};

	class UserEventCallback : public olapi::OLUserSessionEventWaitingCallback
	{
	public:
		UserEventCallback();

		// Inherited functions
		void EventWaiting();

	private:
		// No assignment operator
		UserEventCallback& operator=(UserEventCallback const&) {}
	};

private:

#ifdef USE_CONTENT
	std::vector<olapi::OLID> m_content_id_list;
#endif

	olapi::OLUserSession* mUser;
	UserEventHandler* mUserEventHandler;
	UserEventCallback* mUserEventCallback;
};

///////////////////////////////////////////////////////////////////////////////
//                                  OnLive                                   //
///////////////////////////////////////////////////////////////////////////////

// Contains all the OnLive APIs

class OnLive
{
public:
	static OnLive& getInstance()
	{
		static OnLive instance;
		return instance;
	}

	void init();

	// Are we running from the test harness
	bool isDbgMode();

	// Is the game paused
	OnLiveUser::PauseState getPauseState();

	// Is the game in multi-session mode
	bool isMultiSessionMode();

	// Is it time to exit, ExitReceived has been triggered.
	bool isTimeToExit();

	// Is a user Playing()
	bool isPlaying();

	// Get various user system settings.
	const char *getLanguage();
	bool getKeyboardAndMouse();
	olapi::OLAudio getAudioConfiguration();

	// This used in your main loop(s), if you have a loop for front
	// end and game this belongs in both.
	bool runFrame();


	// Called to begin interfacing with OSP. If wait is true then this call will block
	// until a start is complete.
	bool start(bool wait = true, DWORD timeout = OL_INFINITE);

	// Tell OnLive that we would like to stop.  If wait is true then this call will block
	// until a stop is received then call Exit()
	bool stop(bool wait = true, olapi::OLExitCode exit_code = olapi::OL_EXITCODE_NORMAL, DWORD timeout = OL_INFINITE);

	// Called when ready to bind a user. If wait is true then this call will block
	// until a user is bound, then call ready_for_client().
	bool bind(bool wait = true, bool send_ready_for_client = true, DWORD timeout = OL_INFINITE);

	// This is used to initiate an unbind.  If wait is true then this call will block
	// until the user is unbound.
	bool OnLive::unbind(bool wait = true, DWORD timeout = OL_INFINITE);

	// If you sare saving data set this to true, set to false when done
	void setGameSavingData(bool issaving);

	bool getGameSavingData();

	// If you are running a debug build in windowed mode, set this to true.
	void setWindowedMode(bool windowedmode);

	// These are used to retrieve resolution information outside of your
	// main loop.  In the main loop your will receive a change_ressolution
	// if the resolution changes.
	bool getWindowedMode();
	int getResX();
	int getResY();
	int getRefreshRate();

	// This is used to retrieve a pointer to the users tag name.
	const char *getUserTag();

	// This is used to retrieve the SDK version, this is a debug only function
	const char *getSDKVersion();

	// This is a tradition log function, logging is permitted on online in
	// please refer to the onlive documentation for release requirements.
	// Basic requirements are that log level INFO and above should be
	// very minimal very important information.  Log spamming is not aloud
	// at any level above DEBUG, if detected your game will fail certification.
	void Log(olapi::OLLogLevel level, char* format, ...);

	// Set the log level
	olapi::OLStatus SetLogLevel(olapi::OLLogLevel level);
	olapi::OLStatus dbg_EnableSDKLogReception(bool bOnOff);

	// Actual startup.  This initializes and starts the class up.
	void startup();

	// Actual shutdown.  This should only be used after a stop command before
	// you exit your app.
	void shutdown();

	// Called when a user quits the game, this will perform an unbind() in
	// multi-session or a stop() in single session.
	void userExit();

	OnLiveApplication::State mState;

private:
	bool mBindComplete;

public:
	bool mUnsolicitedExit;

private:
	OnLive();
	OnLive(const OnLive&);
	OnLive& operator=(const OnLive&);
	~OnLive();
};


#endif // __ONLIVE_INTEGRATION_H__
