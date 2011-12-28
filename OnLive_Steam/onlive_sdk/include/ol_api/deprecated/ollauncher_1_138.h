/*************************************************************************************
			Copyright(c)   2010 - 2011 OnLive, Inc.  All Rights Reserved.

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
// OLLauncher.h $Revision: 54252 $
//

/// \file OLLauncher.h


#ifndef OLLAUNCHER_H
#define OLLAUNCHER_H

#include <ol_api/OLAPI.h>

namespace olapi
{

	/// \ingroup OLLAUNCHER_API_CREATE_AND_DESTROY 
	/// \brief The default version of the OnLive OLLauncher API to use in \ref olapi::OLGetLauncher.
	#define LAUNCHER_API_VERSION (OLAPI_VERSION)

	class OLLauncherEventHandler;
	class OLLauncherCallbacks;

	///
	/// \ingroup OLLAUNCHER_API_EVENT_HANDLING LAUNCHER_API
	/// \class OLLauncher
	/// \brief The OLLauncher API is a rarely used API that allows your title to send its bound user to another product, 
	///			then eventually return the user back to the original product. You must coordinate with your 
	///			OnLive Product Manager before using the OLLauncher API. 
	///
	/// \par API Initialization
	///		The OLLauncher singleton is created and retrieved with \ref olapi::OLGetLauncher() and destroyed 
	///		with \ref olapi::OLStopLauncher().\n\n
	///		NOTE: OLLauncher depends on the OLService API and should be initialized after it. \n
	///		See \ref OLLAUNCHER_API_CREATE_AND_DESTROY and \ref SDK_SESSION_FLOW_PAGE for general info about 
	///		initializing the OnLive SDK APIs.
	///
	/// \par API Event and Callback Handling
	///		See \ref SDK_EVENTS_PAGE for an overview or \ref OLLAUNCHER_API_EVENT_HANDLING for specifics.
	///
	/// \par Launching another Product
	///		Use \ref OLLauncher::StartProduct to launch another product.  Once the other title has started,
	///		the user will unbind from your title and bind into the new title.  You can send an 
	///		argument string to the launched title.
	///
	/// \par Retrieving Launch Information
	///		Launched products call OLLauncher::GetProductInfoForParent to retrieve info about their parent.
	///
	/// \par Returning to the Original Product
	///		Once the end user is finished using the launched product, they can return to the original product
	///		by calling OLLauncher::ReturnFromSubProduct.  NOTE: The user is returning to a product, not a specific
	///		machine or process.  There is no guarantee that the end user will return to the specific process
	///		that called StartProduct.
	///
	class OLLauncher : public OLEventBase
	{
		public:

		/// \addtogroup OLLAUNCHER_API_EVENT_HANDLING
		/// 
		/// \name OLLauncher Callbacks & Event Handling
		/// 

		/// 
		/// \brief Registers your callback handler object with the OLLauncher API.
		///
		/// If you intend to handle callbacks for OLLauncher, you should set
		/// a callback object before you start the OnLive Services with \ref olapi::OLStartServices.
		/// Once services are started, callbacks can be dispatched by the SDK at any time.
		///
		/// OLLauncher stores a single callback object pointer; calling this method
		/// multiple times will replace the previous pointer.  There is no way to 'unregister'
		/// the pointer (you cannot set it to NULL).
		///
		///	See \ref EVENTS_PAGE_REGISTRATION for details.
		///
		///		\param[in]	callbackHandler		Your implementation of the \ref OLLauncherCallbacks interface.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS			Normal operation; callbacks can be dispatched on the supplied obj while
		///										the services are running.
		///		\retval OL_INVALID_PARAM	Error - The supplied callback obj pointer was NULL (or invalid);
		///										the callback obj has not been set.
		///
		virtual OLStatus SetEventWaitingCallback(OLLauncherCallbacks* callbackHandler) = 0;

		///
		/// \brief Registers your event handler object with the OLLauncher API.
		///
		/// Once you have set the OLLauncher API's eventHandler, calling one of the handle event methods
		///		may dispatch events on your object (assuming events are available to process).
		///		See \ref EVENTS_PAGE_HANDLING_OVERVIEW.
		///
		/// OLLauncher stores a single eventHandler pointer; calling this method
		/// multiple times will replace the previous pointer.  There is no way to 'unregister'
		/// the pointer (you cannot set it to NULL).  
		///
		///	See \ref EVENTS_PAGE_REGISTRATION for details.
		///
		///		\param	eventHandler	Your implementation of the \ref OLLauncherEventHandler interface.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS			Normal operation; events will be dispatched on the supplied obj.
		///		\retval OL_INVALID_PARAM	Error - The supplied eventHandler pointer was NULL (or invalid);
		///										the eventHandler has not been set.
		///
		virtual OLStatus SetEventHandler(OLLauncherEventHandler* eventHandler) = 0;

		///
		/// \brief Sets this API's EventHandler and attempts to handle a single event. NOTE: Prefer to use the simpler
		///  overload \ref HandleEvent(unsigned int).
		///
		/// This method acts as a combination of \ref SetEventHandler and \ref HandleEvent(unsigned int).
		/// If the supplied eventHandler is not NULL, we set the event handler to the supplied value then try to handle a single event.
		/// If the supplied eventHandler is NULL, we try to handle a single event using the previously set handler.
		///
		/// See \ref SDK_EVENTS_PAGE for details.
		///
		///		\param[in]	eventHandler	If NULL, we use the last set eventHandler.
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
		virtual OLStatus HandleEvent(OLLauncherEventHandler* eventHandler, unsigned int waitTimeMs) = 0;

		///
		/// \brief Attempts to dispatch a single event on this API's registered event handler object.
		///
		/// You must register an event handler object with /ref OLLauncher::SetEventHandler before
		///		calling HandleEvent.
		///
		/// See \ref SDK_EVENTS_PAGE for details.
		///
		///		\param[in]	waitTimeMs	The max amount of time to wait for an event in milliseconds.
		///										Use 0 to return immediately if there is no event waiting (non-blocking).
		///										Otherwise, this call may block for up to waitTimeMs if no event is available.
		///										You can use \ref OL_INFINITE to wait forever for the next event.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS			An event was dispatched.
		///		\retval OL_INVALID_PARAM	The API does not have an event handler set (see \ref OLLauncher::SetEventHandler).
		///		\retval OL_EVENT_TIMEOUT	No events were available for the duration of the wait.
		///
		virtual OLStatus HandleEvent(unsigned int waitTimeMs) = 0;

		// end addtogroup OLLAUNCHER_API_EVENT_HANDLING
		// 
		// end name OLLauncher Callbacks & Event Handling
		// 

		/// \name Launching and Returning to Products
		/// 

		///
		/// \ingroup OLLAUNCHER_API_LAUNCHING_PRODUCTS
		/// \brief Directs the OSP to launch a product/sub-product (referenced by product key) and transition the end user to it.
		///
		/// Limitations:
		///		\li OnLive does not provide a way at runtime to determine if a title was launched normally or by StartProduct;
		///				so each product can only be launched in one way (either normally by the OSP or launched by OLLauncher::StartProduct).
		///		\li You cannot store user state across calls to StartProduct.  The OSP does not guarantee that a user will
		///				return to the same title instance they called StartProduct from
		///		\li StartProduct cannot 'nest' or recurse titles.  A title launched by StartProduct cannot call StartProduct 
		///				to launch a third title.
		///		\li The title calling the StartProduct will return OL_FAIL if its license type is not OL_LICENSE_TYPE_GAME or OL_LICENSE_TYPE_RENTAL.
		///		\li OLLauncher does not provide any user-facing UI while another product is launching.  It is your responsibility
		///				to show the user an appropriate "waiting..." screen.		
		///	
		/// \responseEventList
		///		The SDK responds with the StatusResult event if this request is successfully sent: 
		///
		///		\ospEvent OLLauncherEventHandler::StatusResult
		///			\eventDesc	 Indicates if the product/subProduct was successfully launched.
		///
		///				\statusResult OL_SUCCESS				The product was successfully launched and the user was (or will be) unbound
		///															from this title to join the launched title.
		///															Note: If polling for events, you may receive the user's unbind event
		///															before or after the StatusResult event (it depends on the order you
		///															poll the APIs in).
		///				\statusResult OL_HOST_NOT_AVAILABLE		The OSP could not find an available OnLiveGameServer machine to start the product/subproduct on.
		///				\statusResult OL_INVALID_PARAM			The productKey was invalid.
		///				\statusResult OL_INTERNAL_ERROR			A generic error in the OSP.
		///				\statusResult OL_INVALID_IDENTITY_ID	The userId was invalid (internal OSP error).
		///				\statusResult OL_INVALID_SESSION_ID		The application or user session was invalid (internal OSP error).
		///				\statusResult OL_FAIL					The license type of the title calling StartProduct() was not 
		///														OL_LICENSE_TYPE_GAME or OL_LICENSE_TYPE_RENTAL.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context			Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///		\param[in]	productKey		The product key of the title to launch; speak to your OnLive product manager.
		///									See \ref OL_MAX_VALUE_PRODUCT_KEY_SIZE.
		///
		///		\param[in]	arguments		A string of arguments to pass along to the launched product/sub-product.
		///									Must be <= \ref OL_MAX_LAUNCHED_ARG_SIZE chars.
		///									See \ref GetProductArguments (these are not cmd line args).
		///
		///	\return	Returns an OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect an \ref OLLauncherEventHandler::StatusResult response.
		///		\retval OL_FAIL						The data requested is not available (or other general failure).
		///		\retval OL_INVALID_PARAM			One of the supplied productKey or arg strings was null.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus StartProduct(olapi::OLContext context, const char *productKey, const char *arguments) = 0;

		///
		/// \ingroup OLLAUNCHER_API_RETURNING_FROM_LAUNCH_PRODUCTS
		/// \brief Return the end user to the product that launched this one (the title that called \ref StartProduct).
		///			NOTE: The end user is not guaranteed to land on the same machine/process that called StartProduct.
		///
		/// \responseEventList
		///		The SDK responds with the StatusResult event if this request is successfully sent: 
		///
		///		\ospEvent OLLauncherEventHandler::StatusResult
		///			\eventDesc	 Indicates if the parent product was successfully launched & the end user will return to it.
		///
		///				\statusResult OL_SUCCESS				The parent product was successfully launched and the user was (or will be) unbound
		///															from this title to return to the parent.
		///															Note: If polling for events, you may receive the user's unbind event
		///															before or after the StatusResult event (it depends on the order you
		///															poll the APIs in).
		///				\statusResult OL_HOST_NOT_AVAILABLE		The OSP could not find an available OnLiveGameServer machine to start the product/subproduct on.
		///				\statusResult OL_INVALID_PARAM			The productKey was invalid.
		///				\statusResult OL_INTERNAL_ERROR			A generic error in the OSP.
		///				\statusResult OL_INVALID_IDENTITY_ID	The userId was invalid (internal OSP error).
		///				\statusResult OL_INVALID_SESSION_ID		The application or user session was invalid (internal OSP error).
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context		Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///	\return	Returns an OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect an \ref OLLauncherEventHandler::StatusResult response.
		///		\retval OL_INTERNAL_EROR			The data requested is not available (or other general failure).
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus ReturnFromSubProduct(olapi::OLContext context) = 0;

		// end name Launching products and returning from them.
		// 

		/// \name Getting Information about Launched Products
		/// 

		///
		/// \ingroup OLLAUNCHER_API_RETRIEVE_LAUNCH_INFO
		/// \brief Retrieves the application ID and product key of your parent (the title called \ref StartProduct).
		///			Only call this from a product that is launched by StartProduct.
		///
		///		\param[out] applicationId		The supplied OLID is set to your parent's application ID.
		///										Use NULL if you do not need the application ID.
		///
		///		\param[out] productKey			The product_key string is copied into this buffer; ignored if NULL.
		///										The buffer should be at most \ref OL_MAX_VALUE_PRODUCT_KEY_SIZE + 1 chars.
		///
		///		\param[in] productKeySize		The size of the supplied productKey buffer in bytes.
		///
		///	\return	Returns an OLStatus code.
		///		\retval OL_SUCCESS					Success; the applicationId and productKey have been populated.
		///		\retval OL_FAIL						The data requested is not available (was this title launched by StartProduct?).
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///
		virtual OLStatus GetProductInfoForParent(olapi::OLID *applicationId, char *productKey, unsigned int productKeySize) = 0;

		///
		/// \brief Retrieve the argument string set by the parent's \ref StartProduct call.
		///			Only call this from a product that's launched by StartProduct.
		///
		///		\param[out] argumentBuf				The argument string is copied into this buffer on success.
		///											The buffer must be <= \ref OL_MAX_LAUNCHED_ARG_SIZE + 1 chars.
		///
		///		\param[in] bufSize					The size of the supplied buffer in bytes.
		///
		///	\return	Returns an OLStatus code.
		///		\retval OL_SUCCESS					Success; the argument string has been populated.
		///		\retval OL_FAIL						The data requested is not available (was this title launched by StartProduct?).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_INVALID_PARAM			One or more of the input parameters is not valid.
		///
		virtual OLStatus GetProductArguments(char *argumentBuf, unsigned int bufSize) = 0;

		// name Getting Information about Launched Products
		// 

	};

	///
	/// \ingroup LAUNCHER_API OLLAUNCHER_API_EVENT_HANDLING
	/// \class OLLauncherEventHandler
	/// 
	/// \brief This is the OLLauncher API's Event interface; your OLLauncher event handler must implement this interface.
	///		Each method represents an asynchronous event returned by the SDK.
	///
	/// See \ref EVENTS_PAGE_CONCEPTS for an overview of event handling.
	///
	class OLLauncherEventHandler
	{
		public:

		///
		/// \brief Event returning an OLStatus code result; this typically means an error was
		///		encountered while the OSP was processing an async request.
		///
		/// See \ref EVENTS_PAGE_ASYNC_COMM.
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
	};

	///
	/// \ingroup OLLAUNCHER_API_EVENT_HANDLING LAUNCHER_API
	/// \class OLLauncherCallbacks
	///
	/// \brief This is the OLLauncher API's callback interface; your title's OLLauncher callback handler must implement this interface.
	///
	/// Each method in this class represents an OLLauncher callback.  If you choose to handle OLLauncher callbacks,
	/// register your callback handler using \ref OLLauncher::SetEventWaitingCallback.  Callback methods
	/// may be invoked at any time once the OnLive Services are running (see \ref OLSERVICE_API_CONNECTION_DISCONNECTION).
	///
	/// \note Callback methods are always invoked from an internal OnLive SDK thread, so all callback implementations must be thread-safe.
	///
	/// See \ref EVENTS_PAGE_CONCEPTS for details.
	///
	class OLLauncherCallbacks
	{
		public:

		///
		/// \brief Callback dispatched when an OLLauncher event is waiting to be handled.
		///
		/// If your title handles events from callbacks, call \ref OLLauncher::HandleEvent(unsigned int) to dispatch the waiting event.
		///	If you handle events by polling, you can ignore this event.
		///
		/// \note The EventWaiting callback must be thread-safe and must complete in less than two milliseconds.
		///
		/// See \ref EVENTS_PAGE_HANDLING_OVERVIEW for more info.
		///
		virtual void EventWaiting() = 0;	
	};

	/// \addtogroup OLLAUNCHER_API_CREATE_AND_DESTROY
	/// 

	///
	/// \brief Get the OLLauncher API singleton (the instance is created if necessary).
	///
	///	The OLLauncher pointer can be stored until the API is destroyed with \ref olapi::OLStopLauncher().
	///
	/// See \ref OLLAUNCHER_API_CREATE_AND_DESTROY.
	///
	///		\param[in] version		The API version string (for class compatibility), use \ref LAUNCHER_API_VERSION.
	///
	///		\param[out] olLauncher	The supplied pointer is set to the OLLauncher instance on success.
	///									The pointer is not modified on failure.
	///
	///	\return An OLStatus code.
	///		\retval	OL_SUCCESS			Success; the olLauncher pointer is set to the singleton.
	///		\retval OL_INVALID_PARAM	The supplied version string or olLauncher pointer is NULL.
	///		\retval OL_INVALID_VERSION	The API version requested is not supported. This is a fatal 
	///										error and your title should exit.
	///		\retval OL_INTERNAL_ERROR	A general internal or system error.
	///		\retval OL_OUT_OF_MEMORY	The SDK could not alloc memory for the singleton.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLGetLauncher(const char* version, OLLauncher** olLauncher);


	///
	/// \brief Destroy the OLLauncher API singleton.
	///
	/// The OnLive Services should be stopped before you destroy an API instance; see \ref olapi::OLStopServices().
	///
	/// See \ref OLLAUNCHER_API_CREATE_AND_DESTROY.
	///
	///		\param[in] olLauncher		Pointer to the OLLauncher instance to destroy.
	///
	///	\return Returns an OLStatus code.
	///		\retval	OL_SUCCESS			Success; the olLauncher was destroyed.
	///		\retval OL_INVALID_PARAM	The supplied olLauncher pointer was NULL.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLStopLauncher(OLLauncher* olLauncher);

	// end addtogroup OLLAUNCHER_API_CREATE_AND_DESTROY
	// 

// doxygen group comments
/*! 

\addtogroup LAUNCHER_API
	\brief The OLLauncher API is a rarely used API that allows a title to send its bound user to another product,
			then eventually return the user back to the original product.  The OLLauncher.dll file is only distributed 
			upon request. Speak with your OnLive Product Manager if you need to use the #OLLauncher API.

	Note: #OLLauncher is only to be used in certain cases.  OnLive highly recommends that you build all functionality
	into your title and do not rely on external applications.

\addtogroup OLLAUNCHER_API_CREATE_AND_DESTROY 
	\brief The OLLauncher singleton is created and retrieved with \ref olapi::OLGetLauncher() and destroyed 
		with \ref olapi::OLStopLauncher(). Do not destroy any of the OnLive APIs (or their handlers) until 
		after you have stopped the OnLive Services. 
		
		See \ref SDK_SESSION_FLOW_PAGE for information about the session flow.

		See \ref OLSERVICE_API_CONNECTION_DISCONNECTION for information about OSP connection and disconnection.

		The classes and functions described below are used for registering and handling OLService callbacks and events. 

\addtogroup OLLAUNCHER_API_EVENT_HANDLING
	\brief Callbacks and events are used for asynchronous communication to and from the OSP.

	The classes and functions described below are used for registering and handling OLLauncher callbacks and events. 


	See \ref SDK_EVENTS_PAGE for an overview of callback and events.

\addtogroup OLLAUNCHER_API_LAUNCHING_PRODUCTS
	\brief Your title (main product) calls \ref olapi::OLLauncher::StartProduct to launch another product; once the other product has launched,
	the currently bound user unbinds from your title and binds to the new product. 
	
	The define directives and functions described below are used for launching products.

\addtogroup OLLAUNCHER_API_RETURNING_FROM_LAUNCH_PRODUCTS
	\brief Once the end user is finished using the launched product, they can return to the original product
	by calling \ref olapi::OLLauncher::ReturnFromSubProduct.
	
	NOTE: The user is returning to a product, not a specific machine or process.  There is no guarantee that 
	the end user will return to the specific process that called \ref olapi::OLLauncher::StartProduct. 
	
	The function described below is used for returning an end user from a launched product.

\addtogroup OLLAUNCHER_API_RETRIEVE_LAUNCH_INFO
	\brief Launched products call \ref olapi::OLLauncher::GetProductInfoForParent to retrieve info about their parent
	(the product that called \ref olapi::OLLauncher::StartProduct to launch them). 
	
	The functions described below are used for retrieving launch information.

*/
}; // namespace olapi

#endif // OLLAUNCHER_H
