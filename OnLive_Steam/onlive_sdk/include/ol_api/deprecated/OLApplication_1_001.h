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
// OnLive SDK
// OLApplication.h $Revision: 86565 $
//

/// \file OLApplication.h


#ifndef OLAPPLICATION_H
#define OLAPPLICATION_H

#include <ol_api/OLAPI.h>

namespace olapi
{
	///
	/// \ingroup OLAPPSESSION_API_LOGIN_STATE
	/// \brief State of the title, used to negotiate with the OnLive Service Platform for starting, user binding, and stopping.
	///
	/// \see \ref OLAPPSESSION_API_LOGIN_STATE.
	///
	enum OLLoginState
	{
		OL_LOGIN_NOT_READY,							///< Reserved. Initial state.  Loading or otherwise not ready to bind a user.
		OL_LOGIN_READY_TO_START,					///< Loaded, ready to enter start mode.
		OL_LOGIN_READY_TO_BIND,						///< Started a session, ready to bind user.
		OL_LOGIN_READY_FOR_CLIENT,					///< Ready for user's audio/video/input to connect to the title. User bound, resolution is set, and at least one frame has been rendered.
		OL_LOGIN_READY_TO_UNBIND,					///< Ready to unbind user.
		OL_LOGIN_READY_TO_STOP,						///< Ready to stop and exit the title.
	};

	///
	/// \ingroup OLAPPSESSION_API_STOP_EVENT
	/// \brief The reason why the title is exiting.
	///
	/// \see \ref OLAPPSESSION_API_STOP_EVENT
	///
	enum OLExitCode
	{
		OL_EXITCODE_ABORTED,						///< Title crashed or was aborted due to error.
		OL_EXITCODE_NORMAL,							///< Title exited cleanly or is exiting cleanly.
		OL_EXITCODE_RESERVED,						///< RESERVED
	};

	///
	/// \ingroup OLAPPSESSION_API_START_MODE
	/// \brief Dictates how your title behaves when a user session ends.
	/// \par Description:
	/// The OnLive Service Platform (OSP) tells your title its start mode in the \ref OLAppSessionEventHandler::Start event 
	/// (depending on runtime system demand).
	///
	///	\note We strongly recommend you initially develop for multi-session mode, but support
	/// both start single & multi-session modes.
	/// It is trivial to support single-session once you have implemented multi-session.
	///
	/// \see 
	/// - \ref SDK_SESSION_FLOW_SECTION_START_MODES 
	/// - \ref OLAppSessionEventHandler::Start.
	///
	enum OLStartMode
	{
		OL_NOT_STARTED = -1,						///< Title has not started yet.  This is the initial state.
		OL_SINGLE_SESSION,							///< Single-session: The title should stop after a single user session.
		OL_MULTI_SESSION,							///< Multi-session: The title should return to the ready_to_bind loginState after each user session.
		OL_MULTI_SESSION_ATTRACT,					///< DEPRECATED - this will never be sent by the OSP.
	};

	///
	/// \ingroup OLAPPSESSION_API_CHANGE_RESOLUTION_EVENT
	/// \brief Resolutions and refresh rates supported by OnLive clients.  All resolutions are 60hz unless otherwise noted.
	/// \par Description:
	///	Your title should always output a progressive signal; resolutions that specify interlaced mode (OLR_SDTV_NTSC_480I)
	/// are interlaced on the OnLive client (MicroConsole).
	///
	/// \see 
	/// - \ref OLAPPSESSION_API_CHANGE_RESOLUTION_EVENT 
	/// - \ref OLAppSessionEventHandler::ChangeResolution
	/// - \ref olapi::OLResolutionToString.
	///
	enum OLResolution
	{
		OLR_VGA_640X480 = 1,						///< 640 X 480
		OLR_SVGA_800X600 = 2,						///< 800 X 600
		OLR_XGA_1024X768 = 3,						///< 1024 X 768				Support Recommended
		OLR_XGAPLUS_1152X864 = 4,					///< 1152 X 864
		OLR_SXGA_1280X1024 = 5,						///< 1280 X 1024
		OLR_SXGAPLUS_1400X1050 = 6,					///< 1400 X 1050
		OLR_UXGA_1600X1200 = 7,						///< 1600 X 1200
		OLR_QXGA_2048X1536 = 8,						///< 2048 X 1536
		OLR_WSXGA_1440X900 = 32,					///< 1440 X 900
		OLR_WSXGAPLUS_1680X1050 = 33,				///< 1680 X 1050
		OLR_WUXGA_1920X1200 = 34,					///< 1920 X 1200
		OLR_WQXGA_2560X1600 = 35,					///< 2560 X 1600
		OLR_SDTV_NTSC_480I = 64,					///< 720 X 480
		OLR_SDTV_NTSC_480P = 65,					///< 720 X 480
		OLR_SDTV_PAL_576I = 66,						///< 720 X 576 50hz
		OLR_SDTV_PAL_576P = 67,						///< 720 X 576 50hz
		OLR_SDTV_PAL_576I60 = 68,					///< 720 X 576 60hz
		OLR_SDTV_PAL_576P60 = 69,					///< 720 X 576 60hz
		OLR_HDTV_720P = 128,						///< 1280 X 720				Support Required
		OLR_HDTV_1080I_1280 = 129,					///< 1280 X 1080
		OLR_HDTV_1080P_1280 = 130,					///< 1280 X 1080
		OLR_HDTV_1080I_1440 = 131,					///< 1440 X 1080
		OLR_HDTV_1080P_1440 = 132,					///< 1440 X 1080
		OLR_HDTV_1080P = 133,						///< 1920 X 1080			Support Recommended		
		// SDK Version 1.006 addition
		OLR_CUST_1024x576 = 164,					///< DSL 3MB Datarate		Support Recommended 
		// SDK Version 1.006 addition
		OLR_CUST_800X450 = 165,						///< DSL 2MB Datarate		Support Recommended 
		// SDK Version 1.006 addition
		OLR_CUST_640X360 = 166,						///< NTSC					Support Recommended 
		// SDK Version 1.006 addition
		OLR_CUST_480X270 = 167,						///< Handheld				Support Recommended 
		OLR_CUST_1600X900 = 196,					///< 1600 X 900				Support Recommended
		OLR_CUST_1440X810 = 197,					///< 1440 X 810				Support Recommended
		OLR_MAX
	};

	///
	/// \ingroup OLAPPSESSION_API_CHANGE_RESOLUTION_EVENT
	/// \brief Retrieve the OLResolution enum as a string.
	///
	/// Convert OLResolution code to a string.
	///
	///		\return const char*			The string of the OLResolution enum.
	///
	extern "C" __declspec(dllexport) const char* __cdecl OLResolutionToString( OLResolution res );

	/// \addtogroup OLAPPSESSION_API_SESSION_STATUS_CONTAINER
	/// 
	///
	/// \brief Used only for the purpose of debugging, the application session status container indexes into 
	/// the \ref olapi::AppSessionStatusContainerKeyNames() array and contains information about the current application session, such as the sessionID, 
	/// session's uptime, and the number of User Sessions serviced so far. 
	///
	/// Also, see 
	///		- \ref SDK_CONTAINERS_PAGE.
	///
	/// 	- \ref CONTAINERS_PAGE_USING_KEYS.
	/// 
	///
	enum AppSessionStatusContainerKeys
	{
		OLKEY_RO_AppSessionStatusName,				///< string, Name of container (see \#define \ref APPSESSIONSTATUSCONTAINERNAME).
		OLKEY_RO_AppSessionID,						///< OLID, ID of current user's application session.
		OLKEY_RO_UpTime,							///< unsigned int, Seconds since the title's launch. Will roll over at 32bits.
		OLKEY_RO_NumberOfGamesPlayed,				///< int, number of games played (technically, the number of user sessions); only useful in multi-session mode.
		// SDK Version 1.173 addition
		OLKEY_RO_TitleID,							///< OLID, ID of the current title.
		// SDK Version 1.178 addition
		OLKEY_RO_Version,							///< string, submission type and version of the current title.
		OLKEY_RO_AppSessionStatusKeysMax			///< Indicates the array size; not an index.
	};

	///
	/// \brief Array of keyName strings for the ApplicationSessionStatus container, indexed by the \ref AppSessionStatusContainerKeys enum.
	///
	/// \see 
	/// - \ref OLAPPSESSION_API_SESSION_STATUS_CONTAINER 
	/// - \ref CONTAINERS_PAGE_USING_KEYS.
	///
	#ifndef NO_OLAPI_DLL_LINKAGE
	extern __declspec(dllimport) char *AppSessionStatusContainerKeyNames[olapi::OLKEY_RO_AppSessionStatusKeysMax];
	#endif


	/*! \brief The name of the ApplicationSessionStatus container. */
	#define APPSESSIONSTATUSCONTAINERNAME	"AppSessionStatus"

	/// 

	class OLAppSessionEventHandler;
	class OLAppSessionCallbacks;


	///
	/// \ingroup OLAPPLICATION_API
	/// \class OLAppSession
	/// \brief The OLAppSession API is used to manage your title's session flow and to provide access and controls for starting your title, 
	///	 receiving users, suspending your title, changing resolutions, and stopping your title. 
	///		
	///
	/// \par API Initialization
	///		The OLAppSession singleton is created and retrieved with \ref olapi::OLGetApplication() and destroyed 
	///		with \ref olapi::OLStopApplication().\n\n
	///		NOTE: OLAppSession depends on the OLService API and should be initialized after it. \n
	///		See \ref OLAPPSESSION_API_CREATE_AND_DESTROY and \ref SDK_SESSION_FLOW_PAGE for general info about 
	///		initializing the OnLive SDK APIs.
	///
	/// \par API Event and Callback Handling
	///		Unlike most other OnLive APIs, you MUST implement the OLAppSession's callback object
	///		since it defines the ExitReceived callback (telling your title when it is safe to terminate). \n
	///		See \ref SDK_EVENTS_PAGE for an overview or \ref OLAPPSESSION_API_EVENT_HANDLING for specifics.
	///
	/// \par Managing your Application's Session Flow
	///		The main use of OLAppSession is to manage your title's session flow by manipulating
	///		the OSP's application session.  The AppSession encompasses your title's entire lifetime,
	///		from the process being launched by the OnLive Service Platform (OSP) to the process terminating.
	///
	///		\par
	///		Use the \ref SetLoginState and \ref GetLoginState methods to advance your application session through 
	///		its session flow (think of it as a state machine).  Your title progresses by setting the login 
	///		state then handling the event(s) returned by the OSP.  See \ref SDK_SESSION_FLOW_PAGE for information 
	///		about the application session flow
	///		
	/// \par Getting information about the AppSession
	///		You can retrieve information about the application session in the application session status container, see 
	///		\ref GetStatus and \ref OLAPPSESSION_API_SESSION_STATUS_CONTAINER.  You can also retrieve the AppSession's
	///		ID with \ref GetSessionID, but it is not usually needed by SDK users.
	///
	class OLAppSession : public OLEventBase
	{
		public:

		/// \addtogroup OLAPPSESSION_API_EVENT_HANDLING
		/// 
		/// \name OLAppSession Callbacks & Event Handling
		/// 
		
		/// \brief Sets your passed in callback handler object with the OLAppSession API.
		///
		/// \par Description:
		///		If you intend to handle callbacks for OLAppSession, you should set
		///		a callback handler object before you start the OnLive Services with \ref olapi::OLStartServices().
		///		Once services are started, callbacks can be dispatched by the SDK at any time.
		/// \note 
		/// OLAppSession stores a single callback handler object pointer; calling this method
		/// multiple times will replace the previous pointer.  There is no way to 'unregister'
		/// the pointer (you cannot set it to NULL).
		///
		///	\see \ref EVENTS_PAGE_REGISTRATION
		///
		///		\param[in]	callbackHandler	Your implementation of the OLAppSessionCallbacks interface.
		///
		///	\return Returns a OLStatus code.
		///		\retval OL_SUCCESS			Normal operation; callbacks can be dispatched on the supplied obj while
		///										the services are running.
		///		\retval OL_INVALID_PARAM	Error - the supplied callback handler obj pointer was NULL (or invalid);
		///										the callback obj has not been set.
		///
		virtual OLStatus SetEventWaitingCallback(OLAppSessionCallbacks* callbackHandler) = 0;



		///
		/// \brief Sets your passed in event handler object with the OLAppSession API.
		///
		/// \par Description:
		///		Once you have set the OLAppSession API's event handler, calling one of the OLAppSessionCallbacks() methods
		///		may dispatch events on your event handler object (assuming events are available to process).
		///
		/// \note 
		///		OLAppSession sets a single event handler pointer; calling this method
		///		multiple times will replace the previous pointer.  There is no way to 'unregister'
		///		the pointer (you cannot set it to NULL).  
		///
		/// \see
		///		- \ref EVENTS_PAGE_HANDLING_OVERVIEW
		///		- \ref EVENTS_PAGE_REGISTRATION
		///
		///		\param[in]	eventHandler	Your implementation of the OLAppSessionEventHandler interface.
		///
		///	\return Returns a OLStatus code.
		///		\retval OL_SUCCESS			Normal operation; events will be dispatched on the supplied obj.
		///		\retval OL_INVALID_PARAM	Error - The supplied eventHandler pointer was NULL (or invalid);
		///										the eventHandler has not been set.
		///
		virtual OLStatus SetEventHandler(OLAppSessionEventHandler* eventHandler) = 0;



		///
		/// \brief Sets this API's passed in event handler and attempts to handle a single event. NOTE: Prefer to use the simpler
		///  method \ref HandleEvent(unsigned int).
		/// \par Description:
		/// This method acts as a combination of \ref SetEventHandler() and \ref olapi::OLEventBase::HandleEvent().
		/// \note 
		/// - If the supplied event handler is not NULL, we set the eventHandler to the supplied value, then try to handle a single event.
		/// - If the supplied event handler is NULL, we try to handle a single event using the previously set handler.
		///
		/// \see \ref SDK_EVENTS_PAGE 
		///
		///		\param[in]	eventHandler	If NULL, we use the last set event handler.
		///									Otherwise, the API's event handler is set to this handler.
		///
		///		\param[in]	waitTimeMs	The max amount of time to wait for an event in milliseconds.
		///										Use 0 to return immediately if there is no event waiting (non-blocking).
		///										Otherwise, this call may block for up to waitTimeMs if no event is available.
		///										You can use \ref OL_INFINITE to wait forever for the next event.
		///
		///	\return Returns a OLStatus code.
		///		\retval OL_SUCCESS			An event was dispatched.
		///		\retval OL_INVALID_PARAM	The supplied event handler was NULL, but the API does not have an event handler stored.
		///		\retval OL_EVENT_TIMEOUT	No events were available for the duration of the wait
		///
		virtual OLStatus HandleEvent(OLAppSessionEventHandler* eventHandler, unsigned int waitTimeMs) = 0;



		///
		/// \brief Attempts to dispatch a single event by dispatching a method on the registered OLAppSessionEventHandler object.
		///
		/// \par Description:
		///		You must register an OLAppSessionEventHandler instance with \ref SetEventHandler() before
		///		calling HandleEvent().
		///
		/// \see \ref SDK_EVENTS_PAGE 
		///
		///		\param[in]	waitTimeMs	The max amount of time to wait for an event in milliseconds.
		///										Use 0 to return immediately if there is no event waiting (non-blocking).
		///										Otherwise, this call may block for up to waitTimeMs if no event is available.
		///										You can use \ref OL_INFINITE to wait forever for the next event.
		///
		///	\return Returns a OLStatus code.
		///		\retval OL_SUCCESS			An event was dispatched.
		///		\retval OL_INVALID_PARAM	The supplied event handler was NULL, but the API does not have an event handler stored.
		///		\retval OL_EVENT_TIMEOUT	No events were available for the duration of the wait.
		///
		virtual OLStatus HandleEvent(unsigned int waitTimeMs) = 0;

		/// 

		/// 
		
		///
		/// \ingroup OLAPPSESSION_EXIT_METHOD
		/// \brief Tells the OnLive Service Platform (OSP) that we are finishing the shutdown process (that we need to terminate our process).
		///		Results in an \ref OLAppSessionCallbacks::ExitReceived callback dispatch.
		/// \par Description:
		///	This is usually the second step in the controlled shutdown process; you typically call Exit() after handling the 
		/// \ref olapi::OLAppSessionEventHandler::Stop event.
		///
		/// \note 
		/// - You cannot call Exit() from within the \ref OLAppSessionEventHandler::Stop EventHandler method;
		///			you must call Exit() after you have finished handling the Stop() event.
		/// \par
		/// - Your title should not actually terminate until you have received the ExitReceived callback from the SDK.
		///
		/// \see 
		/// - \ref OLAPPSESSION_API_STOP_EVENT 
		/// - \ref SDK_SESSION_FLOW_SECTION_SHUTDOWN.
		///
		///		\param[in]	olExitCode		Tells the OSP if this is a clean exit or due to an error of some sort.
		///
		///	\return Returns a OLStatus code.
		///		\retval OL_SUCCESS			Success; the OSP will dispatch the ExitReceived callback.
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus Exit(OLExitCode olExitCode) = 0;
		

		/// \name Managing the Application Session flow
		/// 
		
		/// \ingroup OLAPPSESSION_API_LOGIN_STATE
		/// \brief Advances the title's LoginState (the primary indication of the Application's session flow state machine).
		///			The OnLive Service Platform (OSP) generally responds with one or more events (depending on the login state transition).
		///
		///	
		///
		/// \section STATE_TRANSACTIONS State Transitions
		/// A number of state transitions have additional time requirements:
		///
		/// \par 90 Second Startup Timer
		///		Your title must transition to OL_LOGIN_READY_TO_BIND within 90 seconds of being launched by the OSP (or the OSP will terminate
		///		your process as unresponsive).  This timeout is configurable, default is 30 seconds, max is 90 seconds.
		///		- See \ref SDK_SESSION_FLOW_SECTION_INIT.
		///
		/// \par 25 Second User Binding Timer 
		///		Your title must transition to OL_LOGIN_READY_FOR_CLIENT within 25 seconds of receiving the ChangeResolution event
		///		(which precedes the BindUser event).
		///		- See \ref SDK_SESSION_FLOW_SECTION_USER_BINDING.
		///
		/// \par 235 Second User Unbinding or Suspend Timer
		///		You have up to 235 seconds from the Unbind or SuspendUser events to transition to OL_LOGIN_READY_TO_BIND 
		///		or OL_LOGIN_READY_TO_STOP (depending on your title's start mode).  Ideally, your title will 
		///		transition as quickly as possible; but please aim for under 15 seconds.
		///		- See \ref SDK_SESSION_FLOW_SECTION_USER_UNBINDING.
		///
		/// \par 15 Second Application Stop Timer
		///		You must call Exit within 15 seconds of receiving the Stop() event. 
		///		- See \ref SDK_SESSION_FLOW_SECTION_SHUTDOWN_CONTROLLED 
		///
		/// \note The title should make sure it is done reading/writing to any O: files before calling 
		/// SetLoginState(OL_LOGIN_READY_TO_BIND | OL_LOGIN_READY_TO_STOP), since that transition unmounts the O: drive.
		/// 
		/// \see \ref SDK_SESSION_FLOW_PAGE 
		///
		///		\param	loginState	The new login state to transition to.
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect the OSP event(s) corresponding to the new state.
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus SetLoginState(enum OLLoginState loginState) = 0;
		
		/// 
		
		/// \name Application Session Information
		/// 
		
		/// \ingroup OLAPPSESSION_API_LOGIN_STATE
		/// \brief Requests the application session's current OLLoginState from the OnLive Service Platform (OSP).  The OSP will respond
		///		with either a LoginStateResult event on success or a StatusResult event on failure.
		///
		/// \responseEventList
		///		The SDK responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent olapi::OLAppSessionEventHandler::LoginStateResult
		///			\eventDesc	Provides the current \ref olapi::OLLoginState.
		///
		///		\ospEvent olapi::OLAppSessionEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates that the OSP could not fulfill this request
		///						 due to one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR		Generic OSP failure.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context		Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a LoginStateResult or StatusResult event matching the supplied context.
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus GetLoginState(OLContext context) = 0;
		
		/// 
		

		/// \ingroup OLAPPSESSION_API_SESSION_ID
		/// \brief Requests the current application session's ID from the OnLive Service Platform (OSP).  The OSP will respond
		///		with either an IDListResult event on success or a StatusResult event on failure.
		/// \par Description:
		/// The application session ID is not normally needed by SDK users, but it is used internally for tracking session metrics.
		///
		/// \responseEventList
		///		The SDK responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent OLAppSessionEventHandler::IDListResult
		///			\eventDesc	Returns the ID of the application session status container.
		///						If the SessionID is already cached on the client, the GetSessionID call can 
		///						trigger the event dispatch immediately as a side effect.
		///
		///		\ospEvent OLAppSessionEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates that the OSP could not fulfill this request
		///						 due to one of the following errors:
		///
		///				\statusResult OL_SESSION_NOT_STARTED		The application session was invalid when the OSP got the request.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context		Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect an IDListResult or StatusResult event matching the supplied context.
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus GetSessionID(OLContext context) = 0;



		/// \ingroup OLAPPSESSION_API_SESSION_STATUS_CONTAINER
		/// \brief Requests the current application session status container from the OnLive Service Platform (OSP).  The OSP will respond
		///		with either a ContainerIdResult event on success or a StatusResult event on failure.
		/// \par Description:
		/// Used only for the purpose of debugging, the application session status container provides information about this application
		///	instance.  For example, the process's uptime and the number of user sessions handled.
		/// \see \ref olapi::AppSessionStatusContainerKeys.
		///
		/// \responseEventList
		///		The SDK responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent OLAppSessionEventHandler::ContainerIDResult
		///			\eventDesc	Provides the ContainerID of the application session status container.
		///
		///		\ospEvent OLAppSessionEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates that the OSP could not fulfill this request
		///						 due to one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR		Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the container.
		///
		/// \responseEventListEnd
		/// Application Session Information
		///		\param[in]	context		Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a StatusResult or ContainerIDResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of an application session.
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus GetStatus(OLContext context) = 0;
	};

	///
	/// \ingroup OLAPPLICATION_API OLAPPSESSION_API_EVENT_HANDLING
	/// \class OLAppSessionEventHandler
	///
	/// \brief This is the OLAppSession API's Event interface; your OLAppSessionEventHandler() must implement this interface.
	///		Each method represents an asynchronous event returned by the SDK.
	///
	/// See \ref EVENTS_PAGE_CONCEPTS for information about callbacks and events.
	///

	class OLAppSessionEventHandler
	{
		public:


		///
		/// \brief Event returning an OLStatus code result; this typically means an error was
		///		encountered while the OnLive Service Platform was processing an async request.
		///
		/// \par Description:
		/// For example, if the OSP ends an application session at the same time as a request 
		/// for the application session's status is made, the SDK may return OL_SUCCESS (because the SDK is 
		/// unaware of the OSP ending the user session at the time the request was made). However, by the 
		/// time the request for the application session's status gets 
		/// to the OSP, the application session has ended, and the request is invalid. The OSP dispatches a status 
		/// result event with OL_SESSION_NOT_STARTED.
		///
		/// \see \ref EVENTS_PAGE_ASYNC_COMM
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
		/// \par Description:
		/// The actual IDs in the list will depend on the type of request you made.
		/// For example, if this event is in response to a \ref olapi::OLAppSession::GetSessionID request,
		/// the list will contain a single application session ID.
		/// 
		/// \see \ref OLIDList
		///
		///		\param[in] context		The matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in] idList		The list of IDs; the list is invalidated when this method returns, so save off any needed data.
		///
		///	\return Returns a OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///
		virtual OLStatus IDListResult(OLContext context, OLIDList *idList) = 0;



		///
		/// \brief Event returning a containerID result; indicates that a request successfuly resulted
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
		///	\return Returns a OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled; the container is valid until destroyed.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored (and the container will be destroyed immediately).
		///
		virtual OLStatus ContainerIDResult(OLContext context, OLID containerID) = 0;



		///
		/// \ingroup OLAPPSESSION_API_LOGIN_STATE
		/// \brief Event returning the OLLoginState result from a \ref OLAppSession::GetLoginState request.
		///
		/// \note This event is NOT triggered by \ref OLAppSession::SetLoginState.
		///
		///		\param[in] context		The matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in]	loginState	Current login state.
		///
		///	\return Returns a OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///
		virtual OLStatus LoginStateResult(OLContext context, OLLoginState loginState) = 0;



		///
		/// \ingroup OLAPPSESSION_API_START_MODE
		/// \brief Event suggesting the OLStartMode to use for this application session.  Sent in response
		///			to SetLoginState( OL_LOGIN_READY_TO_START ).
		/// \par Description:
		/// The startMode arg in this event is a suggestion from the OnLive Service Platform (OSP) based on its current runtime needs.
		/// If you can support the suggested start mode, please use it by returning OL_SUCCESS for the event.
		///
		/// \note If your title does not support the suggested start mode, you should return OL_FAIL and the OSP will stop
		/// your title.
		///  
		///
		/// \warning You cannot call \ref OLAppSession::SetLoginState from inside this event handler.
		///
		/// \see 
		/// - \ref SDK_SESSION_FLOW_SECTION_INIT.
		/// - \ref SDK_SESSION_FLOW_SECTION_SHUTDOWN_CONTROLLED
		///		\param[in]	olStartMode		The startMode suggested by the OSP; SINGLE_SESSION or MULTI_SESSION.
		///
		///	\return Returns a OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_ALREADY_STARTED		The title has already started; the OSP will tell your title to stop.
		///		\retval OL_FAIL					The title does not support the suggested StartMode; the OSP will
		///											tell your title to stop.
		///
		virtual OLStatus Start(OLStartMode olStartMode) = 0;



		///
		/// \ingroup OLAPPSESSION_API_USER_BINDING
		/// \brief Event telling you which user is binding to your application session (part of the user binding process).
		///			Also indicates that the user's O: drive has been mounted and is accessible.
		///
		/// 
		/// \par Description:
		///	Typically, you kick off a request for the User's System Settings from this event; this lets you query the user's hardware so you can 
		/// initialize your title's audio mode and set mouse/keyboard/controller availability.
		///
		/// \par After Handling the BindUser() Event
		/// Your title must call the SetLoginState(READY FOR CLIENT) function to attach the user's audio/video/input 
		/// to the title.  The title cannot require or expect user input before calling the SetLoginState(READY FOR CLIENT) function. The title should 
		/// not render graphics or play audio until just before calling the SetLoginState(READY FOR CLIENT) function.
		///
		/// \note If your title supports the SuspendSession() event and receives a BindUser() event with the RestoreSession flag set to true, it should 
		/// load restoration data from the O:\\SESSION folder (see item 14.1.6 in the <i>OnLive Technical Compliance Checklist</i> for more info). 
		/// Once loaded, delete the restoration data from disk and restore the game's state to where the user suspended from, skipping past any 
		/// introduction videos or menu screens.
		///
		/// \par User Binding Timer
		///		Keep in mind that the OnLive Service Platform (OSP) started a 25 second "user binding timer" when you negotiated a resolution
		///		with the OSP.  This timer expires once you advance the login state to OL_LOGIN_READY_FOR_CLIENT.
		///
		/// \par Restoring Suspended Sessions
		///		If restoreSession is true, the binding user is trying to restore a previously suspended session (see \ref SuspendSession).
		///		You should return OL_SUCCESS, finish binding the user, then restore the user's previous game play session
		///		by loading the user's suspended session data from O:\\SESSION (and possible the last saved game).
		/// \par
		///		The overall intent is to seamlessly restore the user's suspended gameplay session; ideally, you would skip any game loading
		///		videos and bypass the main menu.  From the user's perspective, it should appear as if their game was paused when
		///		they were disconnected and they are resuming from the point where they left off.
		///
		/// \warning You cannot call \ref OLAppSession::SetLoginState from inside this event handler.
		///
		/// \see 
		/// - \ref SDK_SESSION_FLOW_SECTION_USER_BINDING
		/// - \ref OLUSER_API_USER_SYSTEM_SETTINGS_CONTAINER
		///
		///		\param[in]	restoreSession	If true, restore the binding user's session data (and/or their last save game).
		///
		///		\param[in]	userID			The binding user's OnLive ID.
		///
		///		\param[in] userSessionStatusContainerId		The UserSessionStatus containerId; the container is destroyed when
		///													the user unbinds (or if you return an error for this event).
		///													The containerId is the same as you would get from 
		///													\ref olapi::OLUserSession::GetStatus().
		///
		///	\return Returns a OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_ALREADY_BOUND		The title already has a user bound (or thinks it does); the OSP will stop the game
		///										and destroy the UserSessionStatus container.
		///		\retval OL_FAIL					The event was not handled due to an error; the OSP will tell your title to stop.
		///
		virtual OLStatus BindUser(bool restoreSession, OLID userID, OLID userSessionStatusContainerId) = 0;



		///
		/// \ingroup OLAPPSESSION_API_STOP_EVENT
		/// \brief Event telling your title to begin the shutdown process.
		/// \par Description:
		/// The title must handle a Stop() event regardless of what state it is in.  The proper response to a Stop() event is to send the appropriate 
		/// OLExitCode using the Exit(OLExitCode) function call and then wait for an ExitReceived() event followed by exiting the process.
		///
		/// \note The title can trigger a Stop() event by setting its loginState to \ref olapi::OL_LOGIN_READY_TO_STOP
		/// with \ref OLAppSession::SetLoginState.  The OnLive Service Platform (OSP) can also decide to stop a title on its own.
		///
		/// \par Shutdown Process Overview
		///		After handling the Stop() event, the title has a 15 second window to call \ref OLAppSession::Exit.
		///		NOTE: You cannot call Exit from inside the Stop() event handler method.
		///		Once you have called Exit, the OSP dispatches the \ref OLAppSessionCallbacks::ExitReceived
		///		callback.  After handling ExitReceived, you can stop the OnLive services with \ref olapi::OLStopServices
		///		and exit the title (shutdown the process).
		///
		///	\par
		///		The Stop() event is not usually sent while a user is bound to a title; however, if you
		///		receive a Stop() event while a user is bound, you should unload the bound user's data, but do not
		///		set the login state to OL_LOGIN_READY_TO_UNBIND.  The fact that you have received a Stop event
		///		means that the user was already implicitly unbound from the title.
		///
		/// \par Stop Timer
		/// The elapsed time from when a title receives a Stop() event to the time it calls
		/// Exit(...) must not exceed 15 seconds. If you exceed this timer, the OSP will try to 
		/// shut your title down with a unsolicited ExitReceived() event, which has its own 15 second timer. 
		/// If the unsolicited ExitReceived() timer expires, the OSP will terminate your title.
		///
		/// \warning You cannot call \ref OLAppSession::SetLoginState or \ref OLAppSession::Exit from inside this event handler.
		///
		/// \see \ref SDK_SESSION_FLOW_SECTION_SHUTDOWN 
		///
		///	\return Returns a OLStatus code to the SDK.
		///		\retval OL_SUCCESS		The event was handled.
		///		\retval OL_FAIL			The event was not handled due to a fatal error; 
		///									The OSP will send an unsolicited ExitReceived event and possibly terminate your process.
		///
		virtual OLStatus Stop() = 0;



		///
		/// \ingroup OLAPPSESSION_API_USER_SUSPENDSESSION
		/// \brief Sent when a user unbinds from a title because their connection to OnLive was lost
		///			or the inactivity timer disconnected the user (\ref SDK_SESSION_FLOW_SECTION_UNGRACEFUL_EXIT).
		///
		/// \par Description:
		/// The SuspendSession event is sent as the start of the SuspendUser event sequence (one of the ways a user
		///	can unbind from a title).  The user has already been unbound from your title by the time you receive
		/// SuspendSession (their audio/video/input is no longer connected to the title).  The SuspendSession event
		/// is sent in lieu of an UnbindUser event.  
		/// \see \ref SDK_SESSION_FLOW_SECTION_USER_UNBINDING
		///
		/// \par Supporting Suspend (saving suspend data):
		///		If you support suspending, return OL_SUCCESS to this event then save the data necessary to 
		///		restore the user's game to the O:\\SESSION dir (and/or save the game to the O:\\PROFILE dir)
		///		before you advance the login state (see below).
		///		\par
		///		To properly suspend/resume, you must write at least a dummy file to the O:\\SESSION folder; the
		///		file write is what notifies the OnLive Service Platform (OSP) to flag the user's next login for (possible) restoration.
		///		See the "Restoring Suspended Sessions" section under \ref BindUser.
		///
		/// \par Not Supporting Suspend:
		///		If you do not support suspending, treat this event as if it were an unbindUser() event and return OL_SUCCESS.
		///		Your title may choose to write savegame data to O:\\SESSION.  This data will be available at the next BindUser() only 
		///		and will be deleted thereafter. The contents of O:\\SESSION are removed at the end of any user session that was not suspended.
		///
		/// \par Finishing the Sequence
		///		Whether or not Suspend is suported, you must finish the event sequence by advancing the title's loginState to
		///		either OL_LOGIN_READY_TO_BIND (for multi-session titles) or OL_LOGIN_READY_TO_STOP (for single-session titles).
		///
		/// \note 
		/// - Do not advance the state until you have completed saving all data and close all open files.
		/// - Do not advance the state until you have handled the final event in the sequence (ie: the SaveGameRequest event
		/// if you use the deprecated behavior).
		/// - There is a 235 second timer in the OSP (started with the SuspendSession() event); if you have not advanced the loginState
		/// by the time the timer expires, the OSP will stop your title.
		///
		/// \warning You cannot call \ref OLAppSession::SetLoginState from inside this event handler.
		///
		///	\return Returns a OLStatus code to the SDK.
		///		\retval OL_SUCCESS			The event was handled; the title will handle any data saving before
		///										advancing the loginState.
		///		\retval OL_FAIL				The event was not handled due to a fatal error; 
		///										The OSP will stop your title.
		///
		virtual OLStatus SuspendSession() = 0;



		///
		/// \ingroup OLAPPSESSION_API_USER_UNBINDING
		/// \brief Event signaling that a user is no longer bound (the user's audio/video/input is no longer attached to the game).
		/// \par Description:
		/// Your title must properly handle UnbindUser() events by first saving any user data to O:\\PROFILE, and then calling 
		/// SetLoginState(READY TO BIND) if in Multi-Session or SetLoginState(READY TO STOP) if in single-session.  In multi-session, the title 
		///	must unload the current users profile, restore any configurations or settings to their default value, and then wait for a new user 
		/// then advance the loginState to \ref olapi::OL_LOGIN_READY_TO_BIND. 
		/// \par
		/// Once an UnbindUser() (or SuspendSession()) event is received, your title may access O:\\PROFILE and/or O:\\SESSION up until the point 
		/// where they call SetLoginState(READY TO STOP) or for multi-session call SetLoginState(READY TO BIND).
		/// The user's user session is destroyed after this method returns (invalidating any containers associated with the
		/// user session).  The user's O: drive remains mounted until you advance the login state to the next state
		/// (depending on your title's \ref OLStartMode).
		///
		/// \par UnbindUser Timer: 
		/// The OSP will terminate your title if it takes longer than 235 seconds to advance
		/// the login state after a user unbinds.
		///
		///	\note 
		/// - Your title must not advance the login state if it is busy with saving.
		/// - Your title must continue rendering until receiving an UnbindUser() event; this also applies to titles supporting an in-game user exit.
		/// - Your title should destroy its rendering window and stop all rendering once an UnbindUser() event is received and before calling the 
		/// SetLoginState(READY TO BIND) or SetLoginState(READY TO STOP) functions. This allows the OnLive Service Platform (OSP) hardware to 
		/// remain in low-power state until a user is bound.
		///
		/// \warning You cannot call \ref OLAppSession::SetLoginState from inside this event handler.
		///
		/// \see 
		/// - \ref SDK_SESSION_FLOW_SECTION_USER_UNBINDING 
		/// - \ref OLAppSession::SetLoginState.
		///
		///
		///		\param[in]	userID		ID of user who is no longer bound.
		///
		///	\return Returns a OLStatus code to the SDK.
		///		\retval OL_SUCCESS			The event was handled successfully.  You have 235 seconds to
		///										advance the login state.
		///		\retval OL_FAIL				The event was not handled due to an error.  The OSP will Stop the title.
		///
		virtual OLStatus UnbindUser( OLID userID ) = 0;



		///
		/// \ingroup OLAPPSESSION_API_CHANGE_RESOLUTION_EVENT
		/// \brief Event sent by the OnLive Service Platform (OSP) to negotiate the game's resolution as part of the user binding process.
		/// 
		/// \par Description:
		///	If the OSP suggests a resolution that your title does not support, you should return OL_RESOLUTION_INVALID;
		/// the OSP will send another ChangeResolution() event with the next best resolution.  At a minimum, you must support
		/// 720p.  The OSP will stop your title if it rejects all resolutions.
		///
		/// <b>Note:</b>
		///	- It is important for the resolution to be determined quickly; changing resolutions is part of the overall \ref SDK_SESSION_FLOW_SECTION_USER_BINDING "bind user process" and is governed by the 25 second "user binding" timer.
		/// This timer is started just before the OSP sends the ChangeResolution() event, and the timer is destroyed
		/// once you have finished binding a user by advancing the loginState to OL_LOGIN_READY_FOR_CLIENT. If the 25 second timer expires 
		/// before you finish binding the user, the OSP will stop your title.
		///
		/// - The title must force full screen mode in response to the ChangeResolution() event.  If the OSP detects a title is no longer in full screen 
		/// while a user is bound, it will assume the title has terminated.  The title must not respond to losing focus or the windows minimize message.
		/// Full screen for this purpose is defined as a window using the full resolution of the display and a borderless window. 
		/// After setting up a full screen display, the title must have rendered two frames before setting the login state to READY_FOR_CLIENT.  Upon 
		///	receiving READY_FOR_CLIENT, the OSP will switch the end user's audio/video/input immediately to the title.
		///
		/// - The title should not render anything or set up their rendering window until the ChangeResolution() event has been received. This allows 
		/// the OSP's hardware to remain in low-power state until a user is bound.
		///
		///- The user's video is not connected to your title until you set the login state to OL_LOGIN_READY_FOR_CLIENT.
		/// You must have rendered at least one frame before saying you are ready for the client, but do not kick off bumper 
		/// movies or other items you want the user to see until immediately before setting the login state.
		/// 
		///
		///	\param[in] resolution	The resolution (and refresh rate) suggested by the OSP.
		///
		///	\return Returns a OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled; the title will switch into this resolution.
		///		\retval OL_RESOLUTION_INVALID	The title does not support the suggested resolution; 
		///										the OSP will send another ChangeResolution() event with another mode.
		///
		virtual OLStatus ChangeResolution(OLResolution resolution) = 0;
	};


	///
	/// \ingroup OLAPPSESSION_API_EVENT_HANDLING OLAPPLICATION_API
	/// \class OLAppSessionCallbacks
	///
	/// \brief The OLAppSession API's callback interface; your title's OLAppSession callback handler must implement this interface.
	///
	/// Each method in this class represents an OLAppSession callback.  You MUST implement and register
	/// a callback object for OLAppSession using \ref olapi::OLAppSession::SetEventWaitingCallback.  Callback methods
	/// may be invoked at any time once the OnLive Services are running (see \ref OLSERVICE_API_CONNECTION_DISCONNECTION).
	///
	/// \note Callback methods are always invoked from an internal OnLive SDK thread, so all callback implementations must be thread-safe.
	///
	/// See \ref EVENTS_PAGE_CONCEPTS for information about callbacks and events.
	///
	class OLAppSessionCallbacks
	{
		public:


		///
		/// \brief Callback dispatched when an OLAppSession event is waiting to be handled.
		/// \par Description:
		/// If your title handles events from callbacks, call \ref OLAppSession::HandleEvent(unsigned int) to dispatch the waiting event;
		///	If you handle events by polling, you can ignore this event.
		///
		/// \note The EventWaiting() callback must be thread-safe and must complete in less than 2 milliseconds.
		///
		/// \see \ref EVENTS_PAGE_HANDLING_OVERVIEW for more info.
		///
		virtual void EventWaiting() { };


		///
		/// \ingroup OLAPPSESSION_API_STOP_EVENT
		/// \brief Callback dispatched when the OnLive Service Platform (OSP) has cleared your title for shutdown 
		/// (in response to an \ref OLAppSession::Exit request); you should shut down/terminate your process.
		/// \par Description:
		/// Your title must handle an ExitReceived() event regardless of what state it is in and whether or not it is solicited.  The proper 
		/// response to a solicited ExitReceived() event is to close all open files, shut down the OnLive SDK and call the Windows exit(0) 
		/// function.  The proper response to an unsolicited ExitReceived() event is to close all open files, shut down the OnLive SDK and call 
		/// the Windows exit(-1) function.  If the title exits without receiving an ExitReceived() event, then it must use a non-zero negative 
		/// number other than -1 or a System Error Code.
		/// \par
		/// In general, you should disconnect from the OSP by calling \ref olapi::OLStopServices, then destroy each of the OnLive APIs
		///	you initialized (in the reverse order you initialized them).  After that, terminate your title, preferably with the
		/// syscall exit(0).
		///
		/// \see \ref SDK_SESSION_FLOW_SECTION_SHUTDOWN.
		///
		virtual void ExitReceived() = 0;

	};

	/// \addtogroup OLAPPSESSION_API_CREATE_AND_DESTROY
	/// 
	
	///
	/// \brief Gets the OLAppSession API singleton (the instance is created if necessary).
	///
	/// \par Description:
	///	The OLAppSession pointer can be stored until the API is destroyed with \ref olapi::OLStopApplication().
	///
	/// \see \ref OLAPPSESSION_API_CREATE_AND_DESTROY.
	///
	///		\param[in] version		The API version string (for class compatibility), use \ref OLAPI_VERSION.
	///
	///		\param[out] olAppSession	The supplied pointer is set to the OLAppSession instance on success.
	///									The pointer is not modified on failure.
	///
	///	\return Returns a OLStatus code.
	///		\retval	OL_SUCCESS			Success; the olAppSession pointer is set to the singleton.
	///		\retval OL_INVALID_PARAM	The supplied version string or olAppSession pointer is NULL.
	///		\retval OL_INVALID_VERSION	The API version requested is not supported. This is a fatal 
	///										error and your title should exit.
	///		\retval OL_INTERNAL_ERROR	A general internal or system error.
	///		\retval OL_OUT_OF_MEMORY	The SDK could not alloc memory for the singleton.
	///		\retval	OL_SERVICES_ALREADY_STARTED
	///									This API was initialized after olapi::OLStartServices() was called.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLGetApplication(const char* version, OLAppSession** olAppSession);



	///
	/// \brief Destroys the OLAppSession API singleton.
	///
	/// \par Description: 
	///The OnLive Services should be stopped before you destroy an API instance. 
	///
	/// \see 
	/// - \ref OLAPPSESSION_API_CREATE_AND_DESTROY
	/// - \ref olapi::OLStopServices()
	///
	///		\param[in] olAppSession		Pointer to the OLAppSession instance to destroy.
	///
	///	\return Returns a OLStatus code.
	///		\retval	OL_SUCCESS			Success; the OLAppSession instance was destroyed.
	///		\retval OL_INVALID_PARAM	The supplied OLAppSession pointer was NULL.
	///
	extern "C" __declspec(dllexport) olapi::OLStatus __cdecl OLStopApplication(OLAppSession* olAppSession);

	/// 


	/// \ingroup OLAPPSESSION_API_TITLE_ID
	///
	/// \brief Gets the title ID
	///
	/// \par Description:
	/// Gets the titleID of the currently running title.
	///
	///		\param[out]	titleID		Pointer to an OLID in which to return the titleID.
	///
	///		\retval	OLStatus		OL_SUCCESS normal operation, \n
	///								OL_INVALID_PARAM invalid parameter, titleID is NULL, \n
	///								OL_INTERNAL_ERROR there was an error performing the operation
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLGetTitleID(OLID *titleID);

}; // namespace olapi


/*! 

\addtogroup OLAPPSESSION_API_SESSION_FLOW
\brief The application session flow is the sequence of interactions that your title has with the OnLive Service Platform (OSP), 
beginning when the OSP launches your title. The sequence of interactions consists of three main steps: the 
initialization and startup, the handling of the user session, and the shutdown and exit of your title. 

Also, see \ref SDK_SESSION_FLOW_PAGE.

See the Modules section below for links to the application session flow subsections.

\addtogroup OLAPPSESSION_API_START_MODE
\brief The OnLive Service Platform sends the Start() event as part of the title startup and initialization process. 
	
	Also, see \ref SDK_SESSION_FLOW_SECTION_INIT.

\addtogroup OLAPPSESSION_API_CHANGE_RESOLUTION_EVENT
\brief Your title negotiates with the OnLive Service Platform to determine the resolution a game runs in (as part of the user binding sequence). 
	
	Also, see \ref SDK_SESSION_FLOW_SECTION_USER_BINDING.

\addtogroup OLAPPSESSION_API_LOGIN_STATE
\brief Once you have initialized the SDK and started the OnLive services, the title's session flow is controlled by its
	\ref olapi::OLLoginState(). The LoginState is the main indication of a title's state.  Think of it as the state machine that controls
	a title's overall session flow.  

Also, see \ref SDK_SESSION_FLOW_PAGE.


\addtogroup OLAPPSESSION_API_USER_BINDING
\brief The user binding phase of the application session flow begins when the OnLive Service Platform sends the ChangeResolution() event to your title
	and it ends when your title advances its loginState to OL_LOGIN_READY_FOR_CLIENT (indicating that the title is ready for the client's 
	audio/video/input to connect to the title). 
	
	Also see:
	<ul><li> \ref SDK_SESSION_FLOW_SECTION_USER_BINDING
		<li> \ref SDK_SESSION_FLOW_SECTION_USERSESSIONS
	</ul>

\addtogroup OLAPPSESSION_API_USER_SUSPENDSESSION
\brief The SuspendSession() event sequence is started any time we lose our connection to the user. This can be 
	due to unexpected connection loss (networking problems or powering off a microconsole), or the user being 
	disconnected from OnLive due to inactivity. 

	See \ref SDK_SESSION_FLOW_SECTION_UNGRACEFUL_EXIT for information about the SuspendSession() event sequence.

\addtogroup OLAPPSESSION_API_USER_UNBINDING
\brief The UnbindUser sequence begins when a user exits your title. 

The function described below is used by the OLAppSession API when a user exits your title. 

See \ref SDK_SESSION_FLOW_SECTION_EXIT_IN_GAME for information about the UnbindUser event.

\addtogroup OLAPPSESSION_EXIT_METHOD
\brief The final phase of an application session is the title's shutdown and exit. 

Also, see \ref SDK_SESSION_FLOW_SECTION_SHUTDOWN. 

\addtogroup OLAPPSESSION_API_STOP_EVENT
\brief The final phase of an application session is the title's shutdown and exit. 

Also, see \ref SDK_SESSION_FLOW_SECTION_SHUTDOWN. 

\addtogroup OLAPPLICATION_API
	\brief \copybrief olapi::OLAppSession

	See \ref SDK_SESSION_FLOW_PAGE.


\addtogroup OLAPPSESSION_API_SESSION_ID
	\brief The application session ID is not normally needed by SDK users; it is used internally for tracking session metrics.

\addtogroup OLAPPSESSION_API_TITLE_ID
	\brief The title ID is needed for some SDK API calls.

\addtogroup OLAPPSESSION_API_CREATE_AND_DESTROY
	\brief The OLAppSession singleton can be created and retrieved with \ref olapi::OLStartServices()  and destroyed with \ref olapi::OLStopServices(). Do 
	not destroy any of the OnLive APIs (or their handlers) until after you have stopped the OnLive Services. 
	
	Also see:
	<ul>
		<li> \ref SDK_SESSION_FLOW_PAGE.
		<li> \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
	</ul>

\addtogroup OLAPPSESSION_API_EVENT_HANDLING
	\brief Callbacks and events are used for asynchronous communication to and from the OnLive Service Platform. 

	\note
	Unlike most other OnLive APIs, you MUST implement the OLAppSession's callback object,
	since it defines the ExitReceived callback (telling your title when it is safe to terminate).

	Also, see \ref SDK_EVENTS_PAGE.

\addtogroup OLSERVICE_API_POLLING
	\brief This section describes the functions used for polling each of the OnLive APIs.
*/


#endif // OLAPPSESSION_H

