///////////////////////////////////////////////////////////////////////////////
//
//  OnLiveIntegration.cpp $Revision: 91386 $
//
///////////////////////////////////////////////////////////////////////////////
//
// This class is used as a starting point for integrating the OnLive SDK into
// your game.
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

#include "OnLiveIntegration.h"
#include "OnLiveFunctions.h"

#ifdef USE_OVERLAY
#include "OnLiveOverlay.h"
#endif

#ifdef USE_ACHIEVEMENT
#include "OnLiveAchievement.h"
#endif

#ifdef USE_VOICE
#ifdef USE_SIMPLE_VOICE
#include "OnLiveSimpleVoice.h"
#else
#include "OnLiveVoice.h"
#endif
#endif

#ifdef USE_CONTENT
	#include "OnLiveContent.h"
#endif

#ifdef USE_LEADERBOARD
#include "OnLiveLeaderboard.h"
#endif

using namespace olapi;

//forward declaration
static bool PumpWindowsMessages(DWORD curtime = 0, DWORD timeout = OL_INFINITE);

///////////////////////////////////////////////////////////////////////////////
//                              OnLiveService                                //
///////////////////////////////////////////////////////////////////////////////

OnLiveService::OnLiveService() 
	: mService(NULL)
{
	// The OnLive Service class provides basic communication to/from OnLive.  In
	// addition it provides some debugging features and access to common/shared
	// objects such as containers.
}

void OnLiveService::init()
{
	// Initialization of the OnLive Service class uses the SDK device OLAPI_VERSION
	// to let the SDK know which version your game was compiled against.  This is
	// used in the SDK versioning system to provide the most up to date, bug fixed
	// version of the Service API.

	// Has init already been called?
	if( !mService )
	{
		olapi::OLStatus status = OLGetService(OLAPI_VERSION, &mService);

		if (status == OL_SUCCESS)
		{
			mServiceEventHandler = new ServiceEventHandler();
			mService->SetEventHandler(mServiceEventHandler);

			mServiceEventCallback = new ServiceEventCallback();
			mService->SetEventWaitingCallback(mServiceEventCallback);

			LOG( "OnLiveService::initialized.");

			return;
		}
	}

	// The following code needs to exit the game gracefully
	onlive_function::exit_game(true);
}


///////////////////////////////////////////////////////////////////////////////

bool OnLiveService::isDbgMode()
{
	// Use this API function to find out if your game is running in debug mode
	// under the SDK Test Harness.  It is a good practice to check for debug mode
	// and default to Windowed mode while running under the SDK Test Harness in
	// all builds except the final, submission build.

	if (!mService) return false;

	return mServiceEventHandler->mDbgMode;
}

///////////////////////////////////////////////////////////////////////////////

const char *OnLiveService::getSDKVersion()
{
	// This API function is used to retirve a pointer to the SDK version string.
	if (!mService) return NULL;

	return mServiceEventHandler->mSDKVersion;
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveService::deinit()
{
	// Use this function to stop the OnLive Service API.  All other OnLive API's
	// depend on this API.  This API needs to be shutdown last, or the shutdown
	// function OLStopServices() needs to be used before deconstruction.

	if (mService)
	{
		LOG( "OnLiveService::API stopping.");
		OLStopService(mService);
	}

	if (mServiceEventHandler)
	{
		delete mServiceEventHandler;
	}

	if (mServiceEventCallback)
	{
		delete mServiceEventCallback;
	}

	mService = NULL;
	mServiceEventHandler = NULL;
	mServiceEventCallback = NULL;
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveService::Log(olapi::OLLogLevel level, char* format, ...)
{
	if (mService)
	{
		// This API function is the interface to writing to the SDK Log.  The SDK
		// log is retrievable from a running game in both the SDK Test Harness and
		// on the OnLive service.  In the SDK Test Harness, logs are written to c:\ahulog.


		//format the string into a local buffer
		char buf[1024];
		int n = 0;
		try
		{
			va_list argument_list;
			va_start(argument_list, format);
			n = vsnprintf(buf, sizeof(buf), format, argument_list);
			va_end(argument_list);
		}
		catch(...)
		{
			n = _snprintf(buf, sizeof(buf), "LOG EXCEPTION ERROR: '%s'...", format);

			int i = 0;
			while (buf[i]) { if (buf[i]=='%') buf[i]='~'; i++; }
		}

		//If the string exceeds the buffer, then truncate it
		if (n > sizeof(buf) || n <= -1)
		{
			static const char trunc_message[] = "|<<TRUNCATED>>";
			strcpy(buf + sizeof(buf) - sizeof(trunc_message), trunc_message);
		}

		//Pass the message on
		mService->Log( level, "%s", buf );
	}
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveService::SetLogLevel(olapi::OLLogLevel level)
{
	// This API function is the interface to setting the log level, which is used
	// to filter the logging information captured in the SDK logs.  It is only
	// available to you in debug mode.

	// Usage of this function in a released game will cause compliance failure.

	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->dbg_SetSDKLogLevel(level);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveService::dbg_EnableSDKLogReception(bool bOnOff)
{
	// This API function is the interface to turning logs on or off, which is used
	// to filter the logging information captured in the SDK logs.  It is only
	// available to you in debug mode.

	// Usage of this function in a released game will cause compliance failure.

	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->dbg_EnableSDKLogReception( bOnOff );
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveService::SendHeartbeat(olapi::OLContext context)
{
	if (!mService) return;

	mService->dbg_SendHeartbeat( context );
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::GetContainerName(OLID containerID, int size, char *value)
{
	// This API function is the interface to getting data out of a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->GetContainerName(containerID, size, value);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::GetContainerValue(OLID containerID, const char *identifier_in, int size, char *value)
{
	// This API function is the interface to getting data out of a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->GetContainerValue(containerID, identifier_in, size, value);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::GetContainerValueAsInt(OLID containerID, const char *identifier_in, __int64 *value)
{
	// This API function is the interface to getting data out of a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->GetContainerValueAsInt(containerID, identifier_in, value);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::GetContainerValueAsFloat(OLID containerID, const char *identifier_in, double *value)
{
	// This API function is the interface to getting data out of a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->GetContainerValueAsFloat(containerID, identifier_in, value);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::GetContainerValueAsID(OLID containerID, const char *identifier_in, olapi::OLID *value, olapi::OLIDTypes type)
{
	// This API function is the interface to getting data out of a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->GetContainerValueAsID(containerID, identifier_in, value, type);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::CreateContainer(OLID *containerID)
{
	// This API function is the interface to setting data out into a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->CreateContainer(containerID);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::DestroyContainer(OLID containerID)
{
	// This API function is the interface to setting data out into a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->DestroyContainer(containerID);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::SetContainerValue(OLID containerID, char *identifier_in, char *value)
{
	// This API function is the interface to setting data out into a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->SetContainerValue(containerID, identifier_in, value);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::SetContainerValueFromInt(OLID containerID, const char *identifier_in, __int64 value)
{
	// This API function is the interface to setting data out into a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->SetContainerValueFromInt(containerID, identifier_in, value);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::SetContainerValueFromFloat(OLID containerID, const char *identifier_in, double value)
{
	// This API function is the interface to setting data out into a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->SetContainerValueFromFloat(containerID, identifier_in, value);
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::SetContainerValueFromID(OLID containerID, const char *identifier_in, olapi::OLID& value)
{
	// This API function is the interface to setting data out into a container.
	if (!mService) return olapi::OL_SERVICES_NOT_STARTED;

	return mService->SetContainerValueFromID(containerID, identifier_in, value);
}

/////////////////////////////ServiceEventHandler///////////////////////////////

OnLiveService::ServiceEventHandler::ServiceEventHandler()
	: mDbgMode(false)
{
	mSDKVersion[0] = '\0';
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveService::ServiceEventHandler::dbg_IsTestHarness()
{
	// This event notifies you that your game is running under the SDK Test
	// Harness.  The event is received from the OnLive SDK when services are started.
	// This allows your code to be more friendly to the test environment.

	// One example of this is that you can run in Windowed, instead of Full Screen,
	// mode when running under the OnLive Test Harness.  This makes debugging easier.

	// Usage of this function in a released game will cause compliance failure.

	mDbgMode = true;
};

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::ServiceEventHandler::dbg_SDKLogReceived(char *buf)
{
	// This is a debugging event you can use to retrieve SDK logs in real time
	// and output them.  This is useful if you would like to integrate the SDK
	// logging system into your logging system during development.

	// NOTE: Usage of this function in a released game will cause compliance failure.

	return onlive_function::log_received(buf);
};

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveService::ServiceEventHandler::SDKInfoContainerIDReceived(OLID containerID)
{
	// This is a debugging event that receives the container ID for the SDK Info
	// container.  The SDK info container holds the version of the SDK to which you
	// actually connected.

	if (containerID == olapi::OL_CONTAINER_ID)
	{
		char val[OL_MAX_VALUE_STRING_SIZE];
		olapi::OLStatus error;

		error = OnLiveService::getInstance().mService->GetContainerValue( containerID, olapi::SDKInfoContainerKeyNames[olapi::OLKEY_RO_OLAPIVersion], OL_MAX_VALUE_STRING_SIZE, val );
		DEBUGLOG("OnLiveService::SDKInfoContainerIDReceived()");
		if(error == olapi::OL_SUCCESS)
		{
			strcpy(mSDKVersion,val);
			LOG( "OnLiveService::SDKInfo - APPHost API OLAPI_VERSION : %s", mSDKVersion);
		}
		else
		{
			LOG( "SDKInfo - APPHost API OLAPI_VERSION : ERROR: %d", error);
		}

		if ( OnLiveService::getInstance().mService->DestroyContainer( containerID ) != olapi::OL_SUCCESS )
		{
			LOG( "DESTROY CONTAINER FAILED" );
		}

		return OL_SUCCESS;
	}

	return OL_FAIL;
};

/////////////////////////////ServiceEventCallback//////////////////////////////

OnLiveService::ServiceEventCallback::ServiceEventCallback()
{

}

///////////////////////////////////////////////////////////////////////////////

void OnLiveService::ServiceEventCallback::EventWaiting()
{
	// This callback notifies you that there is an event waiting to be handled.
	// Calling HandleEvent() immediately means that you would like to receive events
	// as they are received in the SDK.

	// All code in your event handlers MUST be thread safe if you choose to handle
	// events immediately.

	OnLiveService::getInstance().mService->HandleEvent(0, 0);
}

///////////////////////////////////////////////////////////////////////////////
//                            OnLiveApplication                              //
///////////////////////////////////////////////////////////////////////////////

OnLiveApplication::OnLiveApplication()
{
	// The OnLive Application class provides functions to control application
	// session flow.  Its functions are used to start, stop, change resolution,
	// bind, and unbind users.
}

void OnLiveApplication::init()
{
	// Initialization of the OnLive Application class uses the SDK Device OLAPI_VERSION
	// to let the SDK know which version you compiled against.  This is used in
	// the SDK versioning system to provide the most up to date bug fixed version
	// of this API.

	olapi::OLStatus status = OLGetApplication(OLAPI_VERSION, &mApplication);

	if (status == OL_SUCCESS)
	{
		mApplicationEventHandler = new ApplicationEventHandler();
		mApplication->SetEventHandler(mApplicationEventHandler);

		mApplicationEventCallback = new ApplicationEventCallback();
		mApplication->SetEventWaitingCallback(mApplicationEventCallback);

		LOG( "OnLiveApplication::initialized.");

		return;
	}

	// The following code is needed to exit the game gracefully and immediately.
	onlive_function::exit_game(true);
}


///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::bind(bool wait, bool send_ready_for_client, DWORD timeout)
{
	// This API function tells the OnLive platform that the game is ready to
	// receive a bind user event.

	// Currently this function is called every runFrame loop and sends the
	// READY_TO_BIND state whenever a user is not bound.

	// If you are going to call bind() outside of the main loop and not rely
	// on runFrame() to call bind() then you must be ready_for_client() within
	// the timeout period

	// The bind user event is used to notify your game that the user has been
	// bound and is ready to play.  Upon receipt of this event you will have received
	// an event to change resolution to the client-specified resolution, and the
	// O:\PROFILE directory will be mounted and ready to use.

	// Make sure the game is in a state where it can start a bind.

	if (!mApplication) return false;

	DWORD curtime = GetTickCount(); // counter for wait timer

	if (OnLive::getInstance().mState == STATE_STARTED || (OnLive::getInstance().mState == STATE_UNBOUND && isMultiSessionMode()))
	{
		//reset wait timer
		curtime = GetTickCount();

		while (wait && getGameSavingData())
		{
			if (PumpWindowsMessages( curtime, timeout ))
			{
				onlive_function::wait_aborted( onlive_function::WAIT_ABORT_WM_QUIT );
				return false;
			}
			if (isExiting())
			{
				onlive_function::wait_aborted( onlive_function::WAIT_ABORT_EXIT_RECEIVED );
				return false;
			}
			if (GetTickCount() - curtime > timeout)
			{
				onlive_function::wait_aborted( onlive_function::WAIT_ABORT_TIMEOUT );
				return false;
			}

			Sleep(0);
		}

		if (!getGameSavingData())
		{
			mApplication->SetLoginState(OL_LOGIN_READY_TO_BIND);
			mApplicationEventHandler->mClientUserID = OLID_INVALID;
			mApplicationEventHandler->mUserSessionID = OLID_INVALID;
			mApplicationEventHandler->mLicenseType = OL_LICENSE_TYPE_UNKNOWN;
			mApplicationEventHandler->mUserTag[0] = '\0';
			OnLive::getInstance().mState = STATE_BINDING;

			if (wait)
			{
				//reset wait timer
				curtime = GetTickCount();

				while (wait && OnLive::getInstance().mState != STATE_READY_FOR_CLIENT)
				{
					if (PumpWindowsMessages( curtime, timeout ))
					{
						onlive_function::wait_aborted( onlive_function::WAIT_ABORT_WM_QUIT );
						return false;
					}
					if (isExiting())
					{
						onlive_function::wait_aborted( onlive_function::WAIT_ABORT_EXIT_RECEIVED );
						return false;
					}
					if (GetTickCount() - curtime > timeout)
					{
						onlive_function::wait_aborted( onlive_function::WAIT_ABORT_TIMEOUT );
						return false;
					}

					Sleep(0);
				}

				if (didResolutionChange())
				{
					// If the resolution changed, the ChangeResolution() event happens before the BindUser() event, and
					// before the state gets set to STATE_READY_FOR_CLIENT and before any call to the game function
					// resume_game() or bind_user().

					onlive_function::change_resolution(getResX(), getResY(), getRefreshRate(), getWindowedMode(), getVSyncMode());
				}

				//reset wait timer
				curtime = GetTickCount();

				while (wait && !OnLiveUser::getInstance().obtainSystemSettings())
				{
					if (PumpWindowsMessages( curtime, timeout ))
					{
						onlive_function::wait_aborted( onlive_function::WAIT_ABORT_WM_QUIT );
						return false;
					}
					if (isExiting())
					{
						onlive_function::wait_aborted( onlive_function::WAIT_ABORT_EXIT_RECEIVED );
						return false;
					}
					if (GetTickCount() - curtime > timeout)
					{
						onlive_function::wait_aborted( onlive_function::WAIT_ABORT_TIMEOUT );
						return false;
					}

					Sleep(0);
				}

				#if SUPPORT_SUSPEND_SESSION == 1
				if (isRestoredSession())
				{
					onlive_function::resume_game();
				}
				else
				#endif
				{
					onlive_function::bind_user();
				}

				if( send_ready_for_client )
				{
					// If we are binding outside of the main loop then automatically Attach the Client if requested
					ready_for_client();
				}
			}

			return true;
		}
	}

	return false;
}

void OnLiveApplication::ready_for_client()
{
	// This API functions tells the system that we are ready for the client.
	// Up to this point the client can not see or interact with the game.

	// This must be called within 15 seconds of receiving the ChangeResolution
	// event during a bind process.  This time restriction will be configurable
	// per game in a future release.

	if (!mApplication) return;

	if (OnLive::getInstance().mState == STATE_READY_FOR_CLIENT)
	{
		// Tell OnLive we're ready for gameplay
		mApplication->SetLoginState(OL_LOGIN_READY_FOR_CLIENT);
		OnLive::getInstance().mState = STATE_RUNNING;
	}
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveApplication::GetLoginState(void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback pCallbackFunction)
{
	if (!mApplication) return olapi::OL_SERVICES_NOT_STARTED;

	olapi::OLContext context = OLGetNextContext();
	OLStatus error = mApplication->GetLoginState(context);
	if(error != OL_SUCCESS)
	{
		LOG("OLApplication::getLoginState error %d", error);
		return error;
	}
	else
	{
		// Register this call with the OnLiveContextDispatcher
		bool bHandled = OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, pCallbackFunction);
		if(!bHandled)
		{
			LOG("OLApplication::getLoginState context event dispatch unhandled");
			return OL_FAIL;
		}
		else
		{
			return OL_SUCCESS;
		}
	}
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveApplication::GetStatusContainer(void * pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback pCallbackFunction)
{
	if (!mApplication) return olapi::OL_SERVICES_NOT_STARTED;

	olapi::OLContext context = OLGetNextContext();
	OLStatus error = mApplication->GetStatus( context );
	if(error != OL_SUCCESS)
	{
		LOG("OLApplication::getStatusContainer error %d", error);
		return error;
	}
	else
	{
		// Register this call with the OnLiveContextDispatcher
		bool bHandled = OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, pCallbackFunction);
		if(!bHandled)
		{
			LOG("OLApplication::getStatusContainer context event dispatch unhandled");
			return OL_FAIL;
		}
		else
		{
			return OL_SUCCESS;
		}
	}
}


///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isExiting()
{
	if (!mApplication) return true;

	// This API function tells you if you are in an exiting state.  An exiting
	// state is one in which the game is waiting for the OnLive platform
	// to tell you to exit the game.

	return (OnLive::getInstance().mState >= STATE_STOPPING);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isTimeToExit()
{
	if (!mApplication) return true;

	// This API function is used to tell the game that it is time to exit your
	// main loop or shutdown the game.  There are other methods of controlling
	// this functionality- this is just one of them.

	return  (OnLive::getInstance().mState == STATE_EXITING);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isMultiSessionMode()
{
	// This API function tells you if the system has launch your game in multi-
	// session mode.  Multi-session mode is a mode where, at the end of unbinding
	// the user, instead of exiting, your app returns to the beginning of the
	// front end and waits for another user.

	if (!mApplication) return false;

	return mApplicationEventHandler->mMultiSessionMode;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isBooting()
{
	// This API function tells the game that it is still in a bootup state
	// and should not be rendering anything.  start() has not been called.

	if (!mApplication) return false;

	return (OnLive::getInstance().mState < STATE_STARTING);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isStarting()
{
	// This API function tells the game that it is still in a startup state
	// and should not be rendering anything.

	if (!mApplication) return false;

	return (OnLive::getInstance().mState < STATE_STARTED);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isStarted()
{
	// This API function tells you that the system has told you game to start,
	// but you have not set READY_TO_BIND.

	if (!mApplication) return false;

	return (OnLive::getInstance().mState == STATE_STARTED);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isUnbound()
{
	// This API function tells you that you have completely unbound and are
	// either in single session and need to READY_TO_STOP or in multi-session
	// and need to READY_TO_BIND.

	if (!mApplication) return false;

	return (OnLive::getInstance().mState == STATE_UNBOUND);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isBound()
{
	// This API function tells you that you are bound and are need to 
	// READY_FOR_CLIENT

	if (!mApplication) return false;

	return (OnLive::getInstance().mState == STATE_READY_FOR_CLIENT);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isPlaying()
{
	// This API function tells you that you have completely bound and are
	// playing the game.

	if (!mApplication) return false;

	return (OnLive::getInstance().mState == STATE_RUNNING);
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveApplication::setWindowedMode(bool windowedmode)
{
	// This API function is a flag used to tell your game if it should run
	// in windowed mode.  You should set this if you are running a debug
	// build of the game.  If you are not running in test harness this
	// flag will not be set.

	// Your game should then use getWindowedMode() to determine if it is
	// okay to run in a window.

	if (!mApplication) return;

	if (OnLive::getInstance().isDbgMode())
	{
		mApplicationEventHandler->mWindowedMode = windowedmode;
	}
	else
	{
//		mApplicationEventHandler->mWindowedMode = false;
	}
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::getWindowedMode()
{
	// This API function is used internally to determine if
	// the game should be running in windowed mode or full screen.
	if (!mApplication) return false;

	return mApplicationEventHandler->mWindowedMode;
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveApplication::setVSyncMode(bool vsync)
{
	// This API function is a flag used to tell your game if it should run
	// in windowed mode.  You should set this if you are running a debug
	// build of the game.  If you are not running in test harness this
	// flag will not be set.

	// Your game should then use getWindowedMode() to determine if it is
	// okay to run in a window.

	if (!mApplication) return;

	if (OnLive::getInstance().isDbgMode())
	{
		mApplicationEventHandler->mVSyncMode = vsync;
	}
	else
	{
//		mApplicationEventHandler->mVSyncMode = vsync;
	}
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::getVSyncMode()
{
	// This API function is used internally to determine if
	// the game should be running in windowed mode or full screen.
	if (!mApplication) return false;

	return mApplicationEventHandler->mVSyncMode;
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveApplication::clearGameSavingData()
{
	// This API function is used to tell OnLive that the game is finished
	// Saving
	if (!mApplication) return;

	mApplicationEventHandler->mGameIsSaving = 0;
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveApplication::setGameSavingData(bool issaving)
{
	// This API function is used to tell OnLive that the game is busy
	// saving data.
	// This does not protect against certain threading circumstances,
	// it will however keep from going negative.
	if (!mApplication) return;

	if (issaving)
		mApplicationEventHandler->mGameIsSaving++;
	else
	{
		mApplicationEventHandler->mGameIsSaving--;
		if (mApplicationEventHandler->mGameIsSaving <= 0)
			mApplicationEventHandler->mGameIsSaving = 0;
	}
}

bool OnLiveApplication::getGameSavingData()
{
	if (!mApplication) return false;

	return mApplicationEventHandler->mGameIsSaving > 0;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::didResolutionChange(bool reset)
{
	// This API function is an accessor function used internally to determine if
	// the resolution was changed.  The resolution can change before Start(),
	// and before BindUser().

	if (!mApplication) return false;

	bool retval = mApplicationEventHandler->mResolutionChanged;

	// If reset is true, this API expects that you have handled the change in
	// resolution.

	if (reset)
	{
		mApplicationEventHandler->mResolutionChanged = false;
	}

	return retval;
}

///////////////////////////////////////////////////////////////////////////////

int OnLiveApplication::getResX()
{
	// This API function is an accessor function used internally to get the
	// desired screen width.
	if (!mApplication) return 0;

	return mApplicationEventHandler->mScreenWidth;
}

///////////////////////////////////////////////////////////////////////////////

int OnLiveApplication::getResY()
{
	// This API function is an accessor function used internally to get the
	// desired screen height.
	if (!mApplication) return 0;

	return mApplicationEventHandler->mScreenHeight;
}

///////////////////////////////////////////////////////////////////////////////

int OnLiveApplication::getRefreshRate()
{
	// This API function is an accessor function used internally to get the
	// desired refresh rate (in Hz).
	if (!mApplication) return 0;

	return mApplicationEventHandler->mRefreshRate;
}


///////////////////////////////////////////////////////////////////////////////

olapi::OLID OnLiveApplication::getClientUserID()
{
	// This API function is an accessor function used to get the client user id
	if (!mApplication) return olapi::OLID(0);

	return mApplicationEventHandler->mClientUserID;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLID OnLiveApplication::getUserSessionID()
{
	// This API function is an accessor function used to get the users session id
	if (!mApplication) return olapi::OLID(0);

	return mApplicationEventHandler->mUserSessionID;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLLicenseType OnLiveApplication::getLicenseType()
{
	// This API function is an accessor function used to get the license type

	if (!mApplication) return olapi::OL_LICENSE_TYPE_UNKNOWN;

	return mApplicationEventHandler->mLicenseType;
}

///////////////////////////////////////////////////////////////////////////////

const char *OnLiveApplication::getUserTag()
{
	// This API function is an accessor function used to get the users tag
	// name.
	if (!mApplication) return NULL;

	return mApplicationEventHandler->mUserTag;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isRestoredSession()
{
	// This API function is an accessor function used to determine if this is a
	// restored session.

	if (!mApplication) return false;

	return mApplicationEventHandler->mRestoreSession;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::isSuspendedSession()
{
	// This API function is an accessor function used to determine if this is a
	// suspended session.

	if (!mApplication) return false;

	return mApplicationEventHandler->mSuspendSession;
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveApplication::deinit()
{
	// This is used to stop the OnLive Application API.

	if (mApplication)
	{
		OLStopApplication(mApplication);
		LOG( "OnLiveApplication::API stopped.");
	}

	if (mApplicationEventHandler)
	{
		delete mApplicationEventHandler;
	}

	if (mApplicationEventCallback)
	{
		delete mApplicationEventCallback;
	}

	mApplication = NULL;
	mApplicationEventHandler = NULL;
	mApplicationEventCallback = NULL;
	LOG( "OnLiveApplication::deinit.");
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::start(bool wait, DWORD timeout)
{
	// This API function tells OnLive that the game is READY_TO_START and waits
	// for OnLive to respond, telling you to continue the startup process.

	// This should be called from your game before you render any graphics, but
	// after you have loaded all non-user specific data.

	if (!mApplication) return false;

	DWORD curtime = GetTickCount(); // counter for wait timer

	if (OnLive::getInstance().mState == STATE_BOOTUP)
	{
		// Tell OnLive we're ready to start
		mApplication->SetLoginState(OL_LOGIN_READY_TO_START);
		OnLive::getInstance().mState = STATE_STARTING;

		//reset wait timer
		curtime = GetTickCount();

		// Wait for OnLive to tell us to start
		while (wait && OnLive::getInstance().mState != STATE_STARTED)
		{
			if (PumpWindowsMessages( curtime, timeout ))
			{
				onlive_function::wait_aborted( onlive_function::WAIT_ABORT_WM_QUIT );
				return false;
			}
			if (isExiting())
			{
				onlive_function::wait_aborted( onlive_function::WAIT_ABORT_EXIT_RECEIVED );
				return false;
			}
			if (GetTickCount() - curtime > timeout)
			{
				onlive_function::wait_aborted( onlive_function::WAIT_ABORT_TIMEOUT );
				return false;
			}

			Sleep(0);
		}

		return true;
	}

	return false;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::stop(bool wait, OLExitCode exit_code, DWORD timeout)
{
	if (!mApplication) return false;

	bool retval = false;

	// This API function tells OnLive that the game would like to stop, waits
	// for OnLive to acknowledge that you can stop, and then issues an exit
	// code and waits for the SDK to acknowledge that it has received the exit
	// code.

	DWORD curtime = GetTickCount(); // counter for wait timer

	if (!isExiting())
	{
		//reset wait timer
		curtime = GetTickCount();

		while (wait && getGameSavingData())
		{
			if (PumpWindowsMessages( curtime, timeout ))
			{
				onlive_function::wait_aborted( onlive_function::WAIT_ABORT_WM_QUIT );
				return false;
			}
			if (isExiting())
			{
				onlive_function::wait_aborted( onlive_function::WAIT_ABORT_EXIT_RECEIVED );
				return false;
			}
			if (GetTickCount() - curtime > timeout)
			{
				onlive_function::wait_aborted( onlive_function::WAIT_ABORT_TIMEOUT );
				return false;
			}

			Sleep(0);
		}

		if (!getGameSavingData())
		{
			// Tell OnLive we'd like to stop
			mApplication->SetLoginState(OL_LOGIN_READY_TO_STOP);
			mApplicationEventHandler->mUserTag[0] = '\0';
			OnLive::getInstance().mState = STATE_STOPPING;

			// Wait for OnLive to tell us it's okay to stop
			while (wait && OnLive::getInstance().mState < STATE_STOPPED)
			{
				if (PumpWindowsMessages( curtime, timeout ))
				{
					onlive_function::wait_aborted( onlive_function::WAIT_ABORT_WM_QUIT );
					return false;
				}

				Sleep(0);
			}

			retval = true;
		}
		else
		{
			return false;
		}
	}

	if (OnLive::getInstance().mState == STATE_STOPPED)
	{
		return exit_onlive( exit_code );
	}

	return retval;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::exit_onlive(olapi::OLExitCode exit_code)
{
	if (!mApplication) return true;

	// Tell OnLive our exit code.
	mApplication->Exit(exit_code);

	OnLive::getInstance().mUnsolicitedExit = false;

	// Wait for confirmation, this is mandatory.
	while (OnLive::getInstance().mState != STATE_EXITING)
	{
		if (PumpWindowsMessages())
		{
			onlive_function::wait_aborted( onlive_function::WAIT_ABORT_WM_QUIT );
			return false;
		}

		Sleep(0);
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveApplication::unbind(bool wait, DWORD timeout)
{
	if (!mApplication) return true;

	// This API function tells OnLive that it would like to unbind the current
	// user.

	DWORD curtime = GetTickCount(); // counter for wait timer

	if (isPlaying() || isBound())
	{
		// Tell OnLive we're ready to unbind user
		mApplication->SetLoginState(OL_LOGIN_READY_TO_UNBIND);

		if (wait)
		{
			//reset wait timer
			curtime = GetTickCount();

			// Wait for OnLive to unbind
			while (wait && !isUnbound())
			{
				if (PumpWindowsMessages( curtime, timeout ))
				{
					onlive_function::wait_aborted( onlive_function::WAIT_ABORT_WM_QUIT );
					return false;
				}
				if (isExiting())
				{
					onlive_function::wait_aborted( onlive_function::WAIT_ABORT_EXIT_RECEIVED );
					return false;
				}
				if (GetTickCount() - curtime > timeout)
				{
					onlive_function::wait_aborted( onlive_function::WAIT_ABORT_TIMEOUT );
					return false;
				}

				Sleep(0);
			}

			#if SUPPORT_SUSPEND_SESSION == 1
			if (isSuspendedSession())
			{
				onlive_function::suspend_game();
			}
			else
			#endif
			{
				onlive_function::unbind_user();
			}

			if (!isMultiSessionMode())
			{
				stop(true); // Single session, just exit.
			}
			else
			{
				bind(false); // Multi-session, prepare for another user.
			}
		}

		return true;
	}

	return false;
}

///////////////////////////ApplicationEventHandler/////////////////////////////

OnLiveApplication::ApplicationEventHandler::ApplicationEventHandler()
	: mMultiSessionMode(false)
	, mScreenWidth(DEFAULT_SCREEN_WIDTH)
	, mScreenHeight(DEFAULT_SCREEN_HEIGHT)
	, mRefreshRate(DEFAULT_REFRESH_RATE)
	, mVSyncMode(false)
	, mWindowedMode(false)
	, mResolutionChanged(false)
	, mGameIsSaving(0)
	, mRestoreSession(false)
	, mSuspendSession(false)
{
	mClientUserID = OLID_INVALID;
	mUserSessionID = OLID_INVALID;
	mLicenseType = OL_LICENSE_TYPE_UNKNOWN;
	mUserTag[0] = '\0';
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveApplication::ApplicationEventHandler::BindUser(bool restoreSession, OLID identityID, OLID containerID)
{
	UNUSED_PARAM(containerID);
	UNUSED_PARAM(identityID);
	UNUSED_PARAM(restoreSession);

	// This SDK Event notifies us that a user is trying to bind.  It contains
	// a session restore flag, the users OLID identity and a container ID used
	// to access per user data (name, language preferences, etc).

	// The session restore flag indicates that there is session data from a
	// previous run of the game that timed out.

	if (OnLive::getInstance().mState == STATE_BINDING)
	{
		char val[OL_MAX_VALUE_STRING_SIZE];

		mClientUserID = identityID;

		// Get the User's tag name.
		OLStatus error = OnLiveService::getInstance().mService->GetContainerValue( containerID, UserSessionStatusContainerKeyNames[OLKEY_RO_Tagname], OL_MAX_VALUE_STRING_SIZE, val );

		if (error == olapi::OL_SUCCESS)
		{
			strcpy(mUserTag, val);
		}
		else
		{
			LOG( "%s: ERROR: %d", UserSessionStatusContainerKeyNames[OLKEY_RO_Tagname], error);
		}

		olapi::OLID user_session_id = olapi::OLID();
		error = OnLiveService::getInstance().mService->GetContainerValueAsID( containerID, UserSessionStatusContainerKeyNames[olapi::OLKEY_RO_UserSessionID], &user_session_id, olapi::OL_SESSION_ID );

		if (error == olapi::OL_SUCCESS)
		{
			char idstring[OLID_STRING_SIZE];
			LOG( "%s: %s", UserSessionStatusContainerKeyNames[olapi::OLKEY_RO_UserSessionID], user_session_id.getIDString(idstring) );

			mUserSessionID = user_session_id;
		}
		else
		{
			LOG( "%s: ERROR: %d", UserSessionStatusContainerKeyNames[olapi::OLKEY_RO_UserSessionID], error);
		}

		error = OnLiveService::getInstance().mService->GetContainerValue( containerID, UserSessionStatusContainerKeyNames[olapi::OLKEY_RO_LicenseType], OL_MAX_VALUE_STRING_SIZE, val );

		if( error == olapi::OL_SUCCESS )
		{
			mLicenseType = (OLLicenseType)atoi(val);
		}
		else
		{
			LOG( "%s: ERROR: %d", UserSessionStatusContainerKeyNames[olapi::OLKEY_RO_LicenseType], error );
		}


#ifdef USE_CONTENT
		error = olapi::OLGetContainerVectorValue( containerID, UserSessionStatusContainerKeyNames[olapi::OLKEY_RO_UserPurchasedContentIDs], OL_MAX_VALUE_DESCRIPTION_SIZE, OnLiveUser::unpackUserPurchasedContentIDs, (unsigned int)&OnLiveUser::getInstance());

		if( error != olapi::OL_SUCCESS )
		{
			LOG( "%s: ERROR: %d", UserSessionStatusContainerKeyNames[olapi::OLKEY_RO_UserPurchasedContentIDs], error );
		}
#endif

		// don't delete the user session container if OLContent (TCC 16.0.1) or OLLauncher (TCC15.0.2) API is used
#ifndef USE_CONTENT
		if ( OnLiveService::getInstance().mService->DestroyContainer( containerID ) != olapi::OL_SUCCESS )
		{
			LOG( "DESTROY CONTAINER FAILED" );
		}
#endif
		mRestoreSession = restoreSession;
		OnLive::getInstance().mState = STATE_READY_FOR_CLIENT;
		return OL_SUCCESS;
	}
	else if (OnLive::getInstance().mState == STATE_READY_FOR_CLIENT || OnLive::getInstance().mState == STATE_RUNNING)
	{
		return OL_ALREADY_BOUND;
	}
	else
	{
		return OL_FAIL;
	}
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveApplication::ApplicationEventHandler::ChangeResolution(OLResolution resolution, bool is_windowed_mode, bool is_vsync_on)
{
	// This SDK Event notifies your game of the resolution required to either launch or
	// bind a user to the system.

	// Comment out individual lines in this function to control resolution support in your game.

	int screenWidth = mScreenWidth;
	int screenHeight = mScreenHeight;
	int refreshRate = mRefreshRate;
	bool windowedMode = mWindowedMode;
	bool vsyncMode = mVSyncMode;

	mWindowedMode = is_windowed_mode;
	mVSyncMode = is_vsync_on;

	// Standard aspect ratios : 16:9, 16:10, 4:3 5:4
	switch (resolution)
	{
		case OLR_VGA_640X480:			mScreenWidth = 640;		mScreenHeight = 480;	mRefreshRate = 60;	break;
		case OLR_SVGA_800X600:			mScreenWidth = 800;		mScreenHeight = 600;	mRefreshRate = 60;	break;
		case OLR_XGA_1024X768:			mScreenWidth = 1024;	mScreenHeight = 768;	mRefreshRate = 60;	break;
		case OLR_XGAPLUS_1152X864:		mScreenWidth = 1152;	mScreenHeight = 864;	mRefreshRate = 60;	break;
		case OLR_SXGA_1280X1024:		mScreenWidth = 1280;	mScreenHeight = 1024;	mRefreshRate = 60;	break;
		case OLR_SXGAPLUS_1400X1050:	mScreenWidth = 1400;	mScreenHeight = 1050;	mRefreshRate = 60;	break;
		case OLR_UXGA_1600X1200:		mScreenWidth = 1600;	mScreenHeight = 1200;	mRefreshRate = 60;	break;
		case OLR_QXGA_2048X1536:		mScreenWidth = 2048;	mScreenHeight = 1536;	mRefreshRate = 60;	break;
		case OLR_WSXGA_1440X900:		mScreenWidth = 1440;	mScreenHeight = 900;	mRefreshRate = 60;	break;
		case OLR_WSXGAPLUS_1680X1050:	mScreenWidth = 1680;	mScreenHeight = 1050;	mRefreshRate = 60;	break;
		case OLR_WUXGA_1920X1200:		mScreenWidth = 1920;	mScreenHeight = 1200;	mRefreshRate = 60;	break;
		case OLR_WQXGA_2560X1600:		mScreenWidth = 2560;	mScreenHeight = 1600;	mRefreshRate = 60;	break;

		case OLR_SDTV_NTSC_480I:		mScreenWidth = 720;		mScreenHeight = 480;	mRefreshRate = 60;	break;
		case OLR_SDTV_NTSC_480P:		mScreenWidth = 720;		mScreenHeight = 480;	mRefreshRate = 60;	break;

		case OLR_SDTV_PAL_576I:			mScreenWidth = 720;		mScreenHeight = 576;	mRefreshRate = 50;	break;
		case OLR_SDTV_PAL_576P:			mScreenWidth = 720;		mScreenHeight = 576;	mRefreshRate = 50;	break;
		case OLR_SDTV_PAL_576I60:		mScreenWidth = 720;		mScreenHeight = 576;	mRefreshRate = 60;	break;
		case OLR_SDTV_PAL_576P60:		mScreenWidth = 720;		mScreenHeight = 576;	mRefreshRate = 60;	break;

		case OLR_HDTV_720P:				mScreenWidth = 1280;	mScreenHeight = 720;	mRefreshRate = 60;	break;

		case OLR_HDTV_1080I_1280:		mScreenWidth = 1280;	mScreenHeight = 1080;	mRefreshRate = 60;	break;
		case OLR_HDTV_1080P_1280:		mScreenWidth = 1280;	mScreenHeight = 1080;	mRefreshRate = 60;	break;

		case OLR_HDTV_1080I_1440:		mScreenWidth = 1440;	mScreenHeight = 1080;	mRefreshRate = 60;	break;
		case OLR_HDTV_1080P_1440:		mScreenWidth = 1440;	mScreenHeight = 1080;	mRefreshRate = 60;	break;

		case OLR_HDTV_1080P:			mScreenWidth = 1920;	mScreenHeight = 1080;	mRefreshRate = 60;	break;

		case OLR_CUST_1024x576:			mScreenWidth = 1024;	mScreenHeight = 576;	mRefreshRate = 60;	break;
		case OLR_CUST_800X450:			mScreenWidth = 800;		mScreenHeight = 450;	mRefreshRate = 60;	break;
		case OLR_CUST_640X360:			mScreenWidth = 640;		mScreenHeight = 360;	mRefreshRate = 60;	break;
		case OLR_CUST_480X270:			mScreenWidth = 480;		mScreenHeight = 270;	mRefreshRate = 60;	break;

		case OLR_CUST_1600X900:			mScreenWidth = 1600;	mScreenHeight = 900;	mRefreshRate = 60;	break;
		case OLR_CUST_1440X810:			mScreenWidth = 1440;	mScreenHeight = 810;	mRefreshRate = 60;	break;

		default:
		return OL_RESOLUTION_INVALID;
	}

	if( !onlive_function::is_resolution_supported( mScreenWidth, mScreenHeight, mRefreshRate, mWindowedMode, mVSyncMode ) )
	{
		return OL_RESOLUTION_INVALID;
	}

	if (screenWidth != mScreenWidth || screenHeight != mScreenHeight || refreshRate != mRefreshRate || windowedMode != mWindowedMode || vsyncMode != mVSyncMode)
	{
		mResolutionChanged = true;
	}

	return OL_SUCCESS;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveApplication::ApplicationEventHandler::ContainerIDResult(OLContext context, OLID id)
{
#ifdef SIMULATE_STATUS_RESULT_ERRORS
	// 10% of the time, do an error instead
	if( rand() % 10 < 1 )
	{
		LOG( "ContainerIDResult - Simulating a StatusResult() error return" );

		return StatusResult( context, olapi::OL_INTERNAL_ERROR );
	}
#endif

	bool status = OnLiveContextEventDispatcher::getInstance().DispatchContainerIDResult(context, id);

	if (status == false)
	{
		OnLiveService::getInstance().mService->DestroyContainer(id);
		return olapi::OL_FAIL;
	}
	else
		return olapi::OL_SUCCESS;
}

OLStatus OnLiveApplication::ApplicationEventHandler::IDListResult(OLContext context, OLIDList *idlist)
{
	UNUSED_PARAM(context);
	UNUSED_PARAM(idlist);

	// This SDK event is not supported in this code at this time.

	return OL_NOT_IMPLEMENTED;
}

OLStatus OnLiveApplication::ApplicationEventHandler::LoginStateResult(OLContext context, OLLoginState state)
{
	bool bHandled = OnLiveContextEventDispatcher::getInstance().DispatchApplicationLoginStateResult(context, state);

	if(!bHandled)
	{
		LOG( "OnLiveApplication:ApplicationEventHandler:LoginStateResult() context event dispatch failed" );
	}

	return OL_SUCCESS;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveApplication::ApplicationEventHandler::Start(OLStartMode mode)
{
	// This SDK Event notifies the game that is it okay to proceed with startup.

	// We accept the start command in either multi-session or single-session.
	// If the game is configured in the metadata as a single-session game, it
	// will never receive a multi-session start event.

	if (OnLive::getInstance().mState >= STATE_STARTED)
	{
		return OL_ALREADY_STARTED;
	}

	switch (mode)
	{
		case OL_MULTI_SESSION:
		{
			#if (SUPPORT_SESSION_TYPES == (SINGLE_SESSION | MULTI_SESSION)) || SUPPORT_SESSION_TYPES == MULTI_SESSION
			mMultiSessionMode = true;
			OnLive::getInstance().mState = STATE_STARTED;
			return OL_SUCCESS;
			#endif
		} break;

		case OL_SINGLE_SESSION:
		{
			#if (SUPPORT_SESSION_TYPES == (SINGLE_SESSION | MULTI_SESSION)) || SUPPORT_SESSION_TYPES == SINGLE_SESSION
			mMultiSessionMode = false;
			OnLive::getInstance().mState = STATE_STARTED;
			return OL_SUCCESS;
			#endif
		} break;
	}

	return OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveApplication::ApplicationEventHandler::StatusResult(OLContext context, OLStatus status)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchStatusResult(context, status))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveApplication::ApplicationEventHandler::Stop()
{
	// This SDK event notifies the game that it needs to be stopped.  This
	// event can be initiated from the OnLive service or from the game.  From
	// the game it is initiated with a READY_TO_STOP login state.

	OnLive::getInstance().mState = STATE_STOPPED;
	return OL_SUCCESS;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveApplication::ApplicationEventHandler::SuspendSession()
{
	// This event is used to ask the game to hibernate so that it can later be
	// continued from exactly where the user left off.  This event is issued if
	// a user times out due to inactivity (15 minute timeout).

	// After this event is called the game will assume that it is equivilent to
	// a Stop or Unbind user (depending on session mode).  The game should respond
	// with a READY TO STOP or a READY TO BIND.

	// Recommended Behavior:
	// Always return OL_SUCCESS for this event.  This indicates that you've handled the event
	// and does not obligate you to save data.  If you support suspending, implement suspend_game() and save 
	// the data to the O:\\SESSION dir (and/or save the game to the O:\\PROFILE dir).
	//
	// NOTE: to properly suspend/resume, you must write at least a dummy file to the O:\\SESSION folder; the
	// file write is what notifies the OSP to flag the user's next login for (possible) restoration.

	#if SUPPORT_SUSPEND_SESSION == 1
		// Set this flag to tell us to perform a suspend or savegame
		mSuspendSession = true;
	#else
		mSuspendSession = false;
	#endif

	// tell OnLive we are going to suspend
	OnLive::getInstance().mState = OnLiveApplication::STATE_UNBOUND;
	return OL_SUCCESS;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveApplication::ApplicationEventHandler::UnbindUser(OLID identityID)
{
	UNUSED_PARAM(identityID);

	// This SDK event notifies the game that the current user has been unbound.
	// This event will happen when a user exits a multi-session game.  Upon receipt
	// of this even the game should return to an initialized state and ready itself
	// to bind another user.

	mClientUserID = OLID_INVALID;
	mUserSessionID = OLID_INVALID;
	mLicenseType = OL_LICENSE_TYPE_UNKNOWN;
	mUserTag[0] = '\0';
	OnLive::getInstance().mState = STATE_UNBOUND;
	return OL_SUCCESS;
}

///////////////////////////ApplicationEventCallback////////////////////////////

OnLiveApplication::ApplicationEventCallback::ApplicationEventCallback()
{}

///////////////////////////////////////////////////////////////////////////////

void OnLiveApplication::ApplicationEventCallback::EventWaiting()
{
	// This callback notifies you that there is an event waiting to be handled.
	// Calling HandleEvent() immediately means that you would like to receive events
	// as they are received in the SDK.

	// All code in your event handlers MUST be thread safe if you choose to handle
	// events immediately.

	OnLiveApplication::getInstance().mApplication->HandleEvent(0, 0);
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveApplication::ApplicationEventCallback::ExitReceived()
{
	// This SDK callback function notifies us that the SDK and OnLive have
	// receive the Exit(...) information and the game may now exit.

	OnLive::getInstance().mState = STATE_EXITING;
}

///////////////////////////////////////////////////////////////////////////////
//                                OnLiveUser                                 //
///////////////////////////////////////////////////////////////////////////////

OnLiveUser::OnLiveUser()
{
	// The OnLive User class provides functions to control the session flow while
	// a user it attached to the game.
}

void OnLiveUser::init()
{
	// Initialization of the OnLive User class uses the SDK Device OLAPI_VERSION
	// to let the SDK know what version you were compiled against.  This is used in
	// the SDK versioning system to provide the most up to date bug fixed version
	// of this API.

	olapi::OLStatus status = OLGetUser(OLAPI_VERSION, &mUser);

	if (status == OL_SUCCESS)
	{
		mUserEventHandler = new UserEventHandler();
		mUser->SetEventHandler(mUserEventHandler);

		mUserEventCallback = new UserEventCallback();
		mUser->SetEventWaitingCallback(mUserEventCallback);

		LOG( "OnLiveUser::initialized.");

		return;
	}

	// The following code is needed to exit the game gracefully and immediately.
	onlive_function::exit_game(true);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveUser::obtainSystemSettings()
{
	if (!mUser) return false;

	// This API function is used to obtain UserSystemSettings.

	if ( mUserEventHandler->mSystemSettingsContext == OL_INVALID_CONTEXT )
	{
		mUserEventHandler->mHaveSystemSettings = false;
		mUserEventHandler->mSystemSettingsContext = OLGetNextContext();
		OLStatus error = mUser->GetUserSystemSettings(mUserEventHandler->mSystemSettingsContext);
		if (error != OL_SUCCESS)
		{
			LOG("OLUserSession::GetSystemSettings error %d", error);
			mUserEventHandler->mSystemSettingsContext = OL_INVALID_CONTEXT;
		}
		return false;
	}
	else if (!mUserEventHandler->mHaveSystemSettings)
	{
		return false;
	}
	else
	{
		mUserEventHandler->mSystemSettingsContext = OL_INVALID_CONTEXT;
		return true;
	}
}

///////////////////////////////////////////////////////////////////////////////

OnLiveUser::PauseState OnLiveUser::getPauseState()
{
	if (!mUser) return PAUSE_STATE_RUNNING;

	// This API function is used to tell game if and how it should be paused.

	return mUserEventHandler->mPause;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveUser::SetGamePrivacyStatus(olapi::OLGameStatus privacyStatus)
{
	if (!mUser) return olapi::OL_SERVICES_NOT_STARTED;

	return mUser->SetGamePrivacyStatus(privacyStatus);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveUser::GetGamePrivacyStatus(void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback pCallbackFunction)
{
	if (!mUser) return olapi::OL_SERVICES_NOT_STARTED;

	olapi::OLContext context = OLGetNextContext();
	OLStatus error = mUser->GetGamePrivacyStatus(context);
	if(error != OL_SUCCESS)
	{
		LOG("OnLiveUser:GetGamePrivacyStatus error %d", error);
		return error;
	}
	else
	{
		// Register this call with the OnLiveContextDispatcher
		bool bHandled = OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, pCallbackFunction);
		if(!bHandled)
		{
			LOG("OnLiveUser:GetGamePrivacyStatus context event dispatch unhandled");
			return OL_FAIL;
		}
		else
		{
			return OL_SUCCESS;
		}
	}
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveUser::SuspendInputTimeout()
{
	if (!mUser) return olapi::OL_SERVICES_NOT_STARTED;

	return mUser->SuspendInputTimeout();
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveUser::ResumeInputTimeout()
{
	if (!mUser) return olapi::OL_SERVICES_NOT_STARTED;

	return mUser->ResumeInputTimeout();
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveUser::GetStatusContainer(void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback pCallbackFunction)
{
	if (!mUser) return olapi::OL_SERVICES_NOT_STARTED;

	olapi::OLContext context = OLGetNextContext();
	OLStatus error = mUser->GetStatus(context);
	if(error != OL_SUCCESS)
	{
		LOG("OnLiveUser::GetStatusContainer error %d", error);
		return error;
	}
	else
	{
		// Register this call with the OnLiveContextDispatcher
		bool bHandled = OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, pCallbackFunction);
		if(!bHandled)
		{
			LOG("OnLiveUser::GetStatusContainer context event dispatch unhandled");
			return OL_FAIL;
		}
		else
		{
			return OL_SUCCESS;
		}
	}
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLiveUser::GetUserSystemSettings(void *pClassPtr, OnLiveContextEventDispatcher::ContextEventCallback pCallbackFunction)
{
	if (!mUser) return olapi::OL_SERVICES_NOT_STARTED;

	olapi::OLContext context = OLGetNextContext();
	OLStatus error = mUser->GetUserSystemSettings(context);
	if(error != OL_SUCCESS)
	{
		LOG("OnLiveUser::GetUserSystemSettings error %d", error);
		return error;
	}
	else
	{
		// Register this call with the OnLiveContextDispatcher
		bool bHandled = OnLiveContextEventDispatcher::getInstance().RegisterContextEventCallback(context, pClassPtr, pCallbackFunction);
		if (!bHandled)
		{
			LOG("OnLiveUser::GetUserSystemSettings context event dispatch unhandled");
			return OL_FAIL;
		}
		else
		{
			return OL_SUCCESS;
		}
	}
}

///////////////////////////////////////////////////////////////////////////////

const char *OnLiveUser::getLanguage()
{
	// This API function is used to tell game the current user's language.

	if (!mUser) return NULL;

	return mUserEventHandler->mLanguage;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveUser::getKeyboardAndMouse()
{
	// This API function is used to tell game if a keyboard and mouse are attached

	if (!mUser) return false;

	return mUserEventHandler->mKeyboard && mUserEventHandler->mMouse;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLiveUser::getVibrationSupported()
{
	if (!mUser) return false;

	return mUserEventHandler->mVibrationSupported;
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLAudio OnLiveUser::getAudioConfiguration()
{
	// This API function is used to tell the game the user's audio configuration

	if (!mUser) return olapi::OL_AUDIO_2_0;

	return mUserEventHandler->mAudioConfiguration;
}

///////////////////////////////////////////////////////////////////////////////

void OnLiveUser::deinit()
{
	// This is used to stop the OnLive User API.

	if (mUser)
	{
		olapi::OLStopUser(mUser);
		LOG( "OnLiveUser::API stopped.");
	}

	if (mUserEventHandler)
	{
		delete mUserEventHandler;
	}

	if (mUserEventCallback)
	{
		delete mUserEventCallback;
	}

	mUser = NULL;
	mUserEventHandler = NULL;
	mUserEventCallback = NULL;
	LOG( "OnLiveUser::deinit.");
}

///////////////////////////////////////////////////////////////////////////////
#ifdef USE_CONTENT
void CALLBACK OnLiveUser::unpackUserPurchasedContentIDs(const char* value, unsigned int classPtr)
{
	((OnLiveUser*)classPtr)->m_content_id_list.push_back(olapi::OLID(value, olapi::OL_CONTENT_ID));
}
#endif


///////////////////////////////UserEventHandler////////////////////////////////

OnLiveUser::UserEventHandler::UserEventHandler()
	: mPause(PAUSE_STATE_RUNNING)
	, mKeyboard(false)
	, mMouse(false)
	, mAudioConfiguration(olapi::OL_AUDIO_2_0)
	, mVibrationSupported(false)
	, mHaveSystemSettings(false)
	, mSystemSettingsContext(OL_INVALID_CONTEXT)
{
	mLanguage[0] = '\0';
}


///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveUser::UserEventHandler::ContainerIDResult(OLContext context, OLID id)
{
#ifdef SIMULATE_STATUS_RESULT_ERRORS
	// 10% of the time, do an error instead
	if( rand() % 10 < 1 )
	{
		LOG( "ContainerIDResult - Simulating a StatusResult() error return" );

		return StatusResult( context, olapi::OL_INTERNAL_ERROR );
	}
#endif

	if (id != OL_CONTAINER_ID)
	{
		return OL_FAIL;
	}
	else if (context != mSystemSettingsContext)
	{
		if (!OnLiveContextEventDispatcher::getInstance().DispatchContainerIDResult(context, id))
		{
			LOG("Dropping container with unmatched context %u", context);
			OnLiveService::getInstance().mService->DestroyContainer(id);
			return OL_FAIL;
		}
	}
	else if (context == mSystemSettingsContext)
	{
		OLStatus error = OnLiveService::getInstance().mService->GetContainerValue(id, UserSystemSettingsContainerKeyNames[OLKEY_RO_Language], OL_MAX_VALUE_STRING_SIZE, mLanguage);
		if (error != OL_SUCCESS)
		{
			LOG("Failed to get UserSystemSettings.Language: %d", error);
			mLanguage[0] = '\0';
		}

		__int64 val;
		error = OnLiveService::getInstance().mService->GetContainerValueAsInt(id, UserSystemSettingsContainerKeyNames[OLKEY_RO_Keyboard], &val);
		if (error != OL_SUCCESS)
		{
			LOG("Failed to get UserSystemSettings.Keyboard: %d", error);
			val = 1;
		}
		mKeyboard = (val != 0);

		error = OnLiveService::getInstance().mService->GetContainerValueAsInt(id, UserSystemSettingsContainerKeyNames[OLKEY_RO_Mouse], &val);
		if (error != OL_SUCCESS)
		{
			LOG("Failed to get UserSystemSettings.Mouse: %d", error);
			val = 1;
		}
		mMouse = (val != 0);

		error = OnLiveService::getInstance().mService->GetContainerValueAsInt(id, UserSystemSettingsContainerKeyNames[OLKEY_RO_AudioConfiguration], &val);
		if (error != OL_SUCCESS)
		{
			LOG("Failed to get UserSystemSettings.AudioConfiguration: %d", error);
			val = OL_AUDIO_2_0;
		}
		mAudioConfiguration = (OLAudio) val;

		error = OnLiveService::getInstance().mService->GetContainerValueAsInt(id, UserSystemSettingsContainerKeyNames[OLKEY_RO_VibrationSupported], &val);
		if (error != OL_SUCCESS)
		{
			LOG("Failed to get UserSystemSettings.VibrationSupported: %d", error);
			val = 1;
		}
		mVibrationSupported = (val != 0);

		mHaveSystemSettings = true;
	}

	OnLiveService::getInstance().mService->DestroyContainer(id);
	return OL_SUCCESS;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveUser::UserEventHandler::GamePrivacyStatusResult(OLContext context, OLGameStatus privacyStatus)
{
	bool bHandled = OnLiveContextEventDispatcher::getInstance().DispatchUserGamePrivacyStatusResult(context, privacyStatus);

	if(!bHandled)
	{
		LOG( "OnLiveApplication:ApplicationEventHandler:GamePrivacyStatusResult() context event dispatch failed" );
	}

	return OL_SUCCESS;
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveUser::UserEventHandler::IDListResult(OLContext context, OLIDList *idlist)
{
	UNUSED_PARAM(context);
	UNUSED_PARAM(idlist);

	// This SDK event is not supported in this code at this time.

	return OL_NOT_IMPLEMENTED;
}


///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveUser::UserEventHandler::Pause()
{
	// Pause/Resume is used when the user enters/exits the in-game OnLive Overlay
	// menu, and may be used after the connection has been broken to/from the
	// client and is being restored.
	
	// During the pause state, the user could see part of the screen and the
	// publisher might want it animated.

	// In neither case is the user able to manipulate the paused game, so any
	// additional UI is not needed.

	// While paused, any event that would require the game to resume before
	// executing must automatically resume the game. See Resume() for more details.

	if (mPause == PAUSE_STATE_RUNNING)
	{
		mPause = PAUSE_STATE_PAUSED;
		onlive_function::pause(mPause); // thread safe call required
		return OL_SUCCESS;
	}
	else
	{
		// We're already paused or frozen.
		return OL_FAIL;
	}
}

///////////////////////////////////////////////////////////////////////////////

OLStatus OnLiveUser::UserEventHandler::Resume()
{
	// Pause/Resume is used when the user enters/exits the in-game OnLive Overlay
	// menu, and may be used after the connection has been broken to/from the
	// client and is being restored.

	// Some game engines may require the game to be in an unpaused state before executing
	// UI code.  The game must treat the resume event as an immediate operation so
	// that all operations to follow will be in a resumed or unpaused state.

	// All UI events from OnLive will issue a resume event before they are issued.

	if (mPause != PAUSE_STATE_RUNNING)
	{
		onlive_function::pause(PAUSE_STATE_RUNNING); // thread safe call required
		mPause = PAUSE_STATE_RUNNING;
		return OL_SUCCESS;
	}
	else
	{
		return OL_FAIL;
	}
}

///////////////////////////////////////////////////////////////////////////////


OLStatus OnLiveUser::UserEventHandler::StatusResult(OLContext context, OLStatus status)
{
	if (OnLiveContextEventDispatcher::getInstance().DispatchStatusResult(context, status))
		return olapi::OL_SUCCESS;
	else
		return olapi::OL_FAIL;
}

///////////////////////////////UserEventCallback///////////////////////////////

OnLiveUser::UserEventCallback::UserEventCallback()
{}

///////////////////////////////////////////////////////////////////////////////

void OnLiveUser::UserEventCallback::EventWaiting()
{
	// This callback notifies you that there is an event waiting to be handled,
	// calling HandleEvent immediately means that you would like to receive events
	// as they are received in the SDK.

	// All code in your event handlers MUST be thread safe it you choose to handle
	// events immediately.

	OnLiveUser::getInstance().mUser->HandleEvent(0, 0);
}

///////////////////////////////////////////////////////////////////////////////
//                                  OnLive                                   //
///////////////////////////////////////////////////////////////////////////////

// This class is expected to be single threaded and controlled from the game's
// main loop thread.

// TODO: Create a better-thread safe OnLive class.

///////////////////////////////////////////////////////////////////////////////

OnLive::OnLive()
	: mState(OnLiveApplication::STATE_BOOTUP)
	, mBindComplete(false)
	, mUnsolicitedExit(true)
{
}

///////////////////////////////////////////////////////////////////////////////

OnLive::~OnLive()
{
}
///////////////////////////////////////////////////////////////////////////////

void OnLive::init()
{
	OnLiveService::getInstance().init();
	OnLiveApplication::getInstance().init();
	OnLiveUser::getInstance().init();
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::isDbgMode()
{
	// This is an accessor function for the OnLive Service class.

	return OnLiveService::getInstance().isDbgMode();
}

///////////////////////////////////////////////////////////////////////////////

const char *OnLive::getSDKVersion()
{
	// This is an accessor function for the OnLive Service class.

	return OnLiveService::getInstance().getSDKVersion();
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::isMultiSessionMode()
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().isMultiSessionMode();
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::isTimeToExit()
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().isTimeToExit();
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::isPlaying()
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().isPlaying();
}

///////////////////////////////////////////////////////////////////////////////

OnLiveUser::PauseState OnLive::getPauseState()
{
	// This is an accessor function for the OnLive User class.

	return OnLiveUser::getInstance().getPauseState();
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::start(bool wait, DWORD timeout)
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().start(wait, timeout);
}

///////////////////////////////////////////////////////////////////////////////

void OnLive::startup()
{
	OnLive::getInstance().init();

#ifdef USE_OVERLAY
	OnLiveOverlay::getInstance().init();
#endif

#ifdef USE_ACHIEVEMENT
	OnLiveAchievement::getInstance().init();
#endif

#ifdef USE_VOICE
#ifdef USE_SIMPLE_VOICE
	SimpleVoiceChannel::getSingleChannelInstance().init();
#else
	OnLiveVoice::getInstance().init();
#endif
#endif

#ifdef USE_CONTENT
	OnLiveContent::getInstance().init();
#endif

#ifdef USE_LEADERBOARD
	OnLiveLeaderboard::getInstance().init();
#endif

	OLStartServices();

#if DEBUG
	OnLive::getInstance().SetLogLevel( olapi::OLLOG_DEBUG );
#endif
}

///////////////////////////////////////////////////////////////////////////////

void OnLive::setWindowedMode(bool windowedmode)
{
	// This is an accessor function for the OnLive Application class.

	OnLiveApplication::getInstance().setWindowedMode(windowedmode);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::getWindowedMode()
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().getWindowedMode();
}

///////////////////////////////////////////////////////////////////////////////

void OnLive::setGameSavingData(bool issaving)
{
	// This is an accessor function for the OnLive Application class.

	// Used to stall READY TO BIND, READY TO STOP
	OnLiveApplication::getInstance().setGameSavingData(issaving);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::getGameSavingData()
{
	return OnLiveApplication::getInstance().getGameSavingData();
}

///////////////////////////////////////////////////////////////////////////////

int OnLive::getResX()
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().getResX();
}

///////////////////////////////////////////////////////////////////////////////

int OnLive::getResY()
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().getResY();
}

///////////////////////////////////////////////////////////////////////////////

int OnLive::getRefreshRate()
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().getRefreshRate();
}

///////////////////////////////////////////////////////////////////////////////

const char *OnLive::getUserTag()
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().getUserTag();
}

///////////////////////////////////////////////////////////////////////////////

void OnLive::Log(olapi::OLLogLevel level, char* format, ...)
{
	//format the string into a local buffer
	char buf[1024];
	int n = 0;
	try
	{
		va_list argument_list;
		va_start(argument_list, format);
		n = vsnprintf(buf, sizeof(buf), format, argument_list);
		va_end(argument_list);
	}
	catch(...)
	{
		n = _snprintf(buf, sizeof(buf), "LOG EXCEPTION ERROR: '%s'...", format);

		int i = 0;
		while (buf[i]) { if (buf[i]=='%') buf[i]='~'; i++; }
	}


	//If the string exceeds the buffer, then truncate it
	if (n > sizeof(buf) || n <= -1)
	{
		static const char trunc_message[] = "|<<TRUNCATED>>";
		strcpy(buf + sizeof(buf) - sizeof(trunc_message), trunc_message);
	}

	//Pass the message on
	OnLiveService::getInstance().Log( level, "%s", buf );
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLive::SetLogLevel(olapi::OLLogLevel level)
{
	// This is an accessor function for the OnLive Service class.

	return OnLiveService::getInstance().SetLogLevel(level);
}

///////////////////////////////////////////////////////////////////////////////

olapi::OLStatus OnLive::dbg_EnableSDKLogReception(bool bOnOff)
{
	// This is an accessor function for the OnLive Service class.
	return OnLiveService::getInstance().dbg_EnableSDKLogReception( bOnOff );
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::bind(bool wait, bool send_ready_for_client, DWORD timeout)
{
	// This is an accessor function for the OnLive Application class.
	// We record whether the bind/ready for client is occuring in two stages to
	// avoid calling onlive_function::bind_user() twice.

	bool success = OnLiveApplication::getInstance().bind(wait, send_ready_for_client, timeout);
	mBindComplete = success && wait && !send_ready_for_client;
	return success;
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::stop(bool wait, olapi::OLExitCode exit_code, DWORD timeout)
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().stop(wait, exit_code, timeout);
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::unbind(bool wait, DWORD timeout)
{
	// This is an accessor function for the OnLive Application class.

	return OnLiveApplication::getInstance().unbind(wait, timeout);
}

///////////////////////////////////////////////////////////////////////////////

const char *OnLive::getLanguage()
{
	// This is an accessor function for the OnLive User class

	return OnLiveUser::getInstance().getLanguage();
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::getKeyboardAndMouse()
{
	// This is an accessor function for the OnLive User class

	return OnLiveUser::getInstance().getKeyboardAndMouse();
}

///////////////////////////////////////////////////////////////////////////////

OLAudio OnLive::getAudioConfiguration()
{
	// This is an accessor function for the OnLive User class

	return OnLiveUser::getInstance().getAudioConfiguration();
}

///////////////////////////////////////////////////////////////////////////////

bool OnLive::runFrame()
{
	// This API function is used to manage the OnLive

	if (OnLiveApplication::getInstance().isTimeToExit())
	{
		static bool exit_called = false;

		// If we have completely exited the game and are still in the main loop
		// call exit_game().
		if(!exit_called)
		{
			shutdown();
			onlive_function::exit_game(mUnsolicitedExit);
			exit_called = true;
		}
	}
	else if (OnLiveApplication::getInstance().isExiting())
	{
		// If we're exiting, make sure Exit() gets called to continue the
		// shutdown procedure.
		OnLiveApplication::getInstance().stop();
	}
	else if (OnLiveApplication::getInstance().isBooting())
	{
		// We have not started up for some reason, so start()
		OnLiveApplication::getInstance().start(false);
	}
	else if (OnLiveApplication::getInstance().isUnbound())
	{
		if (!getGameSavingData())
		{
			#if SUPPORT_SUSPEND_SESSION == 1 
			if (OnLiveApplication::getInstance().isSuspendedSession())
			{
				onlive_function::suspend_game();
			}
			else
			#endif
			{
				onlive_function::unbind_user();
			}

#ifdef USE_CONTENT
			//Clear any purchased content ids from the previous user
			OnLiveUser::getInstance().m_content_id_list.clear();
#endif

			if (!isMultiSessionMode())
			{
				// Single session, just exit.
				OnLiveApplication::getInstance().stop(true); // this has to be true because of isExiting above, to be fixed.
			}
			else
			{
				// Multi-session, prepare for another user.
				OnLiveApplication::getInstance().bind(false);
			}
		}
	}
	else if (OnLiveApplication::getInstance().isStarted())
	{
		// If we are in a started state, tell OnLive to bind a user.
		if (!OnLiveApplication::getInstance().bind(false))
			return false;
	}
	else if (OnLiveApplication::getInstance().didResolutionChange())
	{
		// If the resolution changed notify the game, this MUST happen before any call to ready_for_client()
		// and should happen before any call to the game functions to bind_user or resume_game.

		onlive_function::change_resolution(OnLiveApplication::getInstance().getResX(), OnLiveApplication::getInstance().getResY(), OnLiveApplication::getInstance().getRefreshRate(), getWindowedMode());
	}
	else if (OnLiveApplication::getInstance().isBound())
	{
		if (mBindComplete)
		{
			// If we've already notified the application that a user is bound, tell OnLive to attach the client.
			mBindComplete = false;
			OnLiveApplication::getInstance().ready_for_client();
		}
		else if (OnLiveUser::getInstance().obtainSystemSettings())
		{
			// Once system settings are retrieved, perform remaining bind functionality and then ready for client.
			#if SUPPORT_SUSPEND_SESSION == 1
			if (OnLiveApplication::getInstance().isRestoredSession())
			{
				onlive_function::resume_game();
			}
			else
			#endif
			{
				onlive_function::bind_user();
			}

			OnLiveApplication::getInstance().ready_for_client();
		}
	}

	// If the application is not bound, exiting, or starting then the main loop does not need to
	// render any graphics.  We can render graphics from Started to Unbinding.
	if (OnLiveApplication::getInstance().isTimeToExit() || OnLiveApplication::getInstance().isExiting() || OnLiveApplication::getInstance().isUnbound() || OnLiveApplication::getInstance().isStarting())
	{
		return false;
	}

	return true;
}

///////////////////////////////////////////////////////////////////////////////

void OnLive::shutdown()
{
	LOG("OnLive::shutdown.");
	// This API function is used to shutdown the SDK.  It checks to see if the
	// game has started the exit process and, if not, it then initiates a complete
	// stop, exit, and shutdown.

	bool wasExiting = OnLiveApplication::getInstance().isExiting();

	// If OnLive is not already stopped, perform a stop.
	// If exit has not been sent, perform an exit.
	stop();

	// Stop all services
	olapi::OLStopServices();

#ifdef USE_LEADERBOARD
	OnLiveLeaderboard::getInstance().deinit();
#endif

#ifdef USE_CONTENT
	OnLiveContent::getInstance().deinit();
#endif

#ifdef USE_VOICE
#ifdef USE_SIMPLE_VOICE
	SimpleVoiceChannel::getSingleChannelInstance().deinit();
#else
	OnLiveVoice::getInstance().deinit();
#endif
#endif

#ifdef USE_ACHIEVEMENT
	OnLiveAchievement::getInstance().deinit();
#endif

#ifdef USE_OVERLAY
	OnLiveOverlay::getInstance().deinit();
#endif

	OnLiveUser::getInstance().deinit();
	OnLiveApplication::getInstance().deinit();
	OnLiveService::getInstance().deinit();

	// If shutdown was called before a stop command, tell the game to exit.
	if (!wasExiting)
	{
		onlive_function::exit_game(mUnsolicitedExit);
	}
}

///////////////////////////////////////////////////////////////////////////////

void OnLive::userExit()
{
	// This API function is used when the user clicks on exit game in your game
	// with the expectation of returning to the OnLive portal.

	// If a multi-session app is running unbind the user otherwise stop the game.
	if (isMultiSessionMode())
	{
		unbind();
	}
	else
	{
		unbind(true);
		stop();
	}
}

///////////////////////////////////////////////////////////////////////////////

// In any while loop, use this function to continue to pump windows messages
// Returns true if a WM_QUIT is received
bool PumpWindowsMessages(DWORD curtime, DWORD timeout)
{
	static bool quit=false;
	
	MSG msg;
	while ( PeekMessage( &msg, NULL, 0, 0, PM_REMOVE ) )
	{
		if (msg.message == WM_QUIT)
		{
			//If a WM_QUIT is received, then bail out, but repost the quit message
			LOG( "OLIntegration::PumpWindowsMessages() WM_QUIT received - going to exit" );
			PostQuitMessage(0);
			return true;
		}
		else
		{
			TranslateMessage( &msg );
			DispatchMessage( &msg );
		}

		//If the timeout expires, then stop processing
		if (GetTickCount() - curtime > timeout)
		{
			return false;
		}
	}

	return false;
}
