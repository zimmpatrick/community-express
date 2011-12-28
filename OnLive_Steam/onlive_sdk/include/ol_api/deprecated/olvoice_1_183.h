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
// OLVoice.h $Revision: 54252 $
//

/// \file olvoice.h


#ifndef OLVOICE_H
#define OLVOICE_H

#include <ol_api/olapi.h>

namespace olapi
{
	#define VOICE_API_VERSION (OLAPI_VERSION)

	///
	/// \brief Enumeration describing voice capabilities of a user's headset
	///
	enum VoiceCapability
	{
		OL_VOICE_NO_VOICE = 0,				///< user is not capable of voice
		OL_VOICE_LOW_QUALITY_VOICE,			///< user is capable of sending voice using low quality codecs
		OL_VOICE_HIGH_QUALITY_VOICE,		///< user is capable of sending voice using high quality codecs
		OL_VOICE_LISTEN_ONLY,				///< user is not capable of transmitting voice but can receive (reserved at this time)
		OL_VOICE_CAPABILITY_MAX_TYPES
	};

	///
	/// \brief Flags that affect the behavior of the JoinChannel operation.
	///
	enum OLVoiceJoinChannelFlags
	{
		OL_VOICE_JOIN_MUTED = (1 << 0),			///< Join the channel in a muted state
		OL_VOICE_PUSH_TO_TALK_MODE = (1 << 1),	///< The channel is "push to talk" mode
		OL_VOICE_JOIN_FLAGS_MAX = (1 << 30)
	};

	/// \addtogroup OLVOICE_API_VOICE_STATUS_CONTAINER
	/// 
	
	///
	/// \brief Key enumeration for accessing the voice container
	///
	enum VoiceContainerKeys
	{
		OLKEY_RO_VoiceName,					///< string, Name of Container
		OLKEY_RO_Voice_Reserved0,			///< string, Reserved/Name of Container
		OLKEY_RO_VoiceChannelURI,			///< string, URI of created channel
		OLKEY_RO_VoiceChannelOwner,         ///< OLID, identity of channel owner
		OLKEY_RO_VoiceChannelDescription,   ///< string, human-readable description of channel
		OLKEY_RO_VoiceCapabilityHeadset4,   ///< enum VoiceCapability, capabilities of voice headset #1
		OLKEY_RO_VoiceCapabilityHeadset3,   ///< enum VoiceCapability, capabilities of voice headset #2
		OLKEY_RO_VoiceCapabilityHeadset2,   ///< enum VoiceCapability, capabilities of voice headset #3
		OLKEY_RO_VoiceCapabilityHeadset1,   ///< enum VoiceCapability, capabilities of voice headset #4
		OLKEY_RO_VoiceKeysMax
	};
	
	/*! \brief The name of the voice container. */
	#define VOICECONTAINERNAME	"VoiceContainer"	
	
	///
	/// \brief Array of keyName strings for the voice container, indexed by the \ref VoiceContainerKeys enum.
	///
	#ifndef NO_OLAPI_DLL_LINKAGE
	extern __declspec(dllimport) char *VoiceContainerKeyNames[olapi::OLKEY_RO_VoiceKeysMax];
	#endif

	/// 

	///
	/// \brief The state of the participant in a channel.
	///
	enum OLVoiceParticipantEventType
	{
		OL_VOICE_PARTICIPANT_ENTERED = 1,
		OL_VOICE_PARTICIPANT_EXITED,
		OL_VOICE_PARTICIPANT_SPEAKING,
		OL_VOICE_PARTICIPANT_IDLE,
		OL_VOICE_PARTICIPANT_MUTE,
		OL_VOICE_PARTICIPANT_RESERVED1,
		OL_VOICE_PARTICIPANT_RESERVED2,
		OL_VOICE_PARTICIPANT_LISTEN_ONLY,
		OL_VOICE_PARTICIPANT_MIC_ACTIVE,
		OL_VOICE_PARTICIPANT_MAX
	};

	class OLVoiceEventHandler;
	class OLVoiceCallbacks;


	///
	/// \ingroup OLVOICE_API
	/// \class OLVoice
	/// \brief The OLVoice interface provides communication between a running title and the OLService gateway.
	///
	/// \par API Initialization
	///		The OLVoice singleton is created and retrieved with \ref olapi::OLGetVoice() and destroyed 
	///		with \ref olapi::OLStopVoice().\n\n
	///		NOTE: OLVoice depends on the OLService API and should be initialized after it. \n
	///		See \ref OLVOICE_API_CREATE_AND_DESTROY and \ref SDK_SESSION_FLOW_PAGE for general info about 
	///		initializing the OnLive SDK APIs.
	///
	/// \par API Event and Callback Handling
	///		See \ref SDK_EVENTS_PAGE for an overview or \ref OLVOICE_API_EVENT_HANDLING for specifics.
	///
	/// \par Getting Information about the current user's voice capabilities 
	///		 See \ref GetVoiceCapabilities and \ref OLVOICE_API_VOICE_STATUS_CONTAINER.\n\n
	///
	class OLVoice : public OLEventBase
	{
		public:

		/// \addtogroup OLVOICE_API_EVENT_HANDLING
		/// 
		/// \name OLVoice Callbacks & Event Handling
		/// 

		/// 
		/// \brief Sets the passed in callback handler object with the OLVoice API.
		///
		/// If you intend to handle callbacks for OLVoice, you should set
		/// a callback handler object before you start the OnLive Services with \ref olapi::OLStartServices.
		/// Once services are started, callbacks can be dispatched by the SDK at any time.
		///
		/// OLVoice stores a single callback handler object pointer; calling this method
		/// multiple times will replace the previous pointer.  There is no way to 'unregister'
		/// the pointer (you cannot set it to NULL).
		///
		///	See \ref EVENTS_PAGE_REGISTRATION for details.
		///
		///		\param[in]	callback	Your implementation of the \ref OLVoiceCallbacks interface.
		///
		///	\return Returns a OLStatus code.
		///		\retval OL_SUCCESS			Normal operation; callback handlers can be dispatched on the supplied obj while
		///										the services are running.
		///		\retval OL_INVALID_PARAM	Error - The supplied callback handler obj pointer was NULL (or invalid);
		///										the callbackHandler obj has not been stored.
		///
		virtual OLStatus SetEventWaitingCallback(OLVoiceCallbacks* callback) = 0;




		///
		/// \brief Sets the passed in event handler object with the OLVoice API.
		///
		/// Once you have set the OLVoice API's event handler, calling one of the OLVoiceCallbacks methods
		///		may dispatch events on your object (assuming events are available to process).
		///		See \ref EVENTS_PAGE_HANDLING_OVERVIEW.
		///
		/// OLVoice stores a single event handler pointer; calling this method
		/// multiple times will replace the previous pointer.  There is no way to 'unregister'
		/// the pointer (you cannot set it to NULL).  
		///
		///	See \ref EVENTS_PAGE_REGISTRATION for details.
		///
		///		\param[in]	handler				Your implementation of the OLVoiceEventHandler interface.
		///
		///	\return Returns a OLStatus code.
		///		\retval OL_SUCCESS			Normal operation; events will be dispatched on the supplied obj.
		///		\retval OL_INVALID_PARAM	Error - The supplied event handler pointer was NULL (or invalid);
		///										the event handler has not been set.
		///
		virtual OLStatus SetEventHandler(OLVoiceEventHandler* handler) = 0;




		///
		/// \brief Sets the passed in event handler and attempts to handle a single event. NOTE: Prefer to use the simpler
		///  overload \ref HandleEvent(unsigned int).
		///
		/// This method acts as a combination of \ref SetEventHandler() and \ref HandleEvent(unsigned int).
		/// If the supplied event handler is not NULL, we set the event handler to the supplied value then try to handle a single event.
		/// If the supplied event handler is NULL, we try to handle a single event using the previously set handler.
		///
		/// See \ref SDK_EVENTS_PAGE for details.
		///
		///		\param[in]	handler		If NULL, we use the last set event handler.
		///									Otherwise, the API's event handler is set to this handler.
		///
		///		\param[in]	time		The max amount of time to wait for an event in milliseconds.
		///										Use 0 to return immediately if there is no event waiting (non-blocking).
		///										Otherwise, this call may block for up to waitTimeMs if no event is available.
		///										You can use \ref OL_INFINITE to wait forever for the next event.
		///
		///	\return Returns a OLStatus code.
		///		\retval OL_SUCCESS			An event was dispatched.
		///		\retval OL_INVALID_PARAM	The supplied handler was NULL, but the API does not have an event handler set.
		///		\retval OL_EVENT_TIMEOUT	No events were available for the duration of the wait.
		///
		virtual OLStatus HandleEvent(OLVoiceEventHandler* handler, unsigned int time) = 0;

		///
		/// \brief Attempts to dispatch a single event on this API's registered event handler object.
		///
		/// You must register an OLVoiceEventHandler instance with \ref OLVoice::SetEventHandler() before
		///		calling HandleEvent().
		///
		/// See \ref SDK_EVENTS_PAGE for details.
		///
		///		\param[in]	time		The max amount of time to wait for an event in milliseconds.
		///										Use 0 to return immediately if there is no event waiting (non-blocking).
		///										Otherwise, this call may block for up to waitTimeMs if no event is available.
		///										You can use \ref OL_INFINITE to wait forever for the next event.
		///
		///	\return Returns a OLStatus code.
		///		\retval OL_SUCCESS			An event was dispatched.
		///		\retval OL_INVALID_PARAM	The API does not have an event handler stored (see \ref OLVoice::SetEventHandler).
		///		\retval OL_EVENT_TIMEOUT	No events were available for the duration of the wait.
		///
		virtual OLStatus HandleEvent(unsigned int time) = 0;

		/// 

		/// 

		/// \addtogroup OLVOICE_API_VOICE_STATUS_CONTAINER
		/// 

		///
		/// \brief Get voice capabilities of the client
		///
		/// Retrieves a container ID for the current voice capabilities of the client. 
		/// This will trigger a ContainerID event. This call must be preceded by a call to InitChat.
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent OLVoiceEventHandler::ContainerIDResult
		///			\eventDesc	Returns the ID of the Voice Container.
		///						Note: The SDK Test Harness does not populate this container
		///						(you will get a container with empty or default values).
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR		Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the container.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context		Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus GetVoiceCapabilities(OLContext context) = 0;

		/// 


		/// \addtogroup OLVOICE_API_INIT_VOICE_CHAT_SYSTEM
		/// 

		///
		/// \brief Initialize the chat subsystem
		///
		/// This initializes the chat subsystem for the user session, creating any necessary connections to
		/// the voice servers and retrieving information from the connected client about its chat capabilities. This will
		/// trigger a StatusResult() event.
		///
		/// The title must initialize the voice chat subsystem at the start of a user session. 
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR		Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the container.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context		Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus InitChat(OLContext context) = 0;


		///
		/// \brief Stop the chat subsystem
		///
		/// This shuts down the chat subsystem for the user session, removing any necessary connections to
		/// the voice servers. This will trigger a StatusResult event.
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR		Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the container.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context		Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus StopChat(OLContext context) = 0;

		/// 


		/// \addtogroup OLVOICE_API_RESERVED_CONTEXT
		/// 

		///
		/// \brief Join a chat channel
		///
		/// This joins a chat channel. Channels are identified by SIP URIs, which are assigned upon channel creation.
		/// It is the application's responsibility to exchange channel URIs among multiple instances of the application,
		/// as would happen in a multiplayer chat scenario. On success, this will generate a ChannelJoined event followed by a
		/// stream of ParticipantEvent calls listing each user presently in the channel. Further ParticipantEvent events will
		/// generated as users interact. On failure, this will generate a StatusResult event.
		///
		/// The user may have voice chat disabled via the Overlay API. In that situation, the Join Channel and Leave Channel
		/// API calls are available and will succeed, but the user's participant events will say that the user is disabled until
		/// the user enables themselves. The workflow is that the application joins and leaves channels even when the user is
		/// disabled in order to communicate to the user when these channels are available should they wish to join.
		///
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLVoiceEventHandler::ChannelJoinedEvent
		///			\eventDesc	 The current user has successfuly joined the channel and the joined channel information.
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR			Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY			Failed to allocate space for the container.
		///				\statusResult OL_IDENTIFIER_NOT_FOUND	The specified channel or the user does not exist.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context				A reserved context used to pair up async operations; see \ref CONTEXT_PAGE_RESERVED_CONTEXT.
		///
		///		\param[in]	channel_uri			The SIP URI of the channel to join.
		///
		///		\param[in]	channel_password	An optional password to enter the channel.
		///
		///		\param[in]	join_flags			A set of OLVoiceJoinChannelFlags that describe the behavior of the channel.
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus JoinChannel(OLContext context, const char *channel_uri, const char *channel_password, 
			OLVoiceJoinChannelFlags join_flags) = 0;

		/// 

		///
		/// \brief Leave a chat channel
		///
		/// This leaves a previously joined channel. This will generate a StatusResult event.
		///
		/// The user may have voice chat disabled via the Overlay API. In that situation, the Join Channel and Leave Channel
		/// API calls are available and will succeed, but the user's participant events will say that the user is disabled until
		/// the user enables themselves.
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLVoiceEventHandler::ChannelExitedEvent
		///			\eventDesc	 The current user has successfuly left the channel and the left channel information.
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR			Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY			Failed to allocate space for the container.
		///				\statusResult OL_IDENTIFIER_NOT_FOUND	The specified channel or the user does not exist.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context				Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///		\param[in]	channel_uri			The SIP URI of the channel to join.
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus LeaveChannel(OLContext context, const char *channel_uri) = 0;

		/// \addtogroup OLVOICE_API_VOICE_STATUS_CONTAINER
		/// 

		///
		/// \brief Create a voice channel
		///
		/// This creates a new voice channel for others to join. The creator is not automatically joined to 
		/// the channel, and must use JoinChannel to enter it as any other user must. On failure, this will
		/// generate a StatusResult event, and on success it will generate a ContainerID event from which the
		/// OLKEY_RO_VoiceChannelURI member can be retrieved.
		///
		/// If the channel is not explicitly destroyed by the creator, it will be destroyed upon the termination
		/// of the application session of the creator. This will happen even if users are actively using the channel.
		///
		/// The name of the channel must only be unique to the creating user. That is, Alice can create only one 
		/// channel named "Stuff", but Bob can create a channel named "Stuff" even if Alice already has done so.
		/// Each channel will be assigned a unique SIP URI.
		///
		/// Users for whom voice chat is disabled are unable to create channels.
		///
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLVoiceEventHandler::ContainerIDResult
		///			\eventDesc	Returns the ID of the Voice Container.
		///						Note: The SDK Test Harness does not populate this container
		///						(you will get a container with empty or default values).
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR		Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the container.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context				Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///		\param[in]	channel_name		A human readable name for the channel.
		///
		///		\param[in]	channel_password	An optional password for the channel if the channel is to be password protected.
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus CreateChannel(OLContext context, const char *channel_name, const char *channel_password) = 0;

		/// 

		///
		/// \brief Destroy a voice channel
		///
		/// This destroys a voice channel previously created by the caller. This generates a StatusResult event.
		///
		/// If the channel is not explicitly destroyed by the creator, it will be destroyed upon the termination
		/// of the application session of the creator. This will happen even if users are actively using the channel.
		/// Channels may be destroyed even if they are in active use.
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	Indicates a success (the channel was successfully destroyed), or that the
		///						operation failed.
		///				\statusResult OL_SUCCESS				The channel has been destroyed.
		///				\statusResult OL_INTERNAL_ERROR			Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY			Failed to allocate space for the container.
		///				\statusResult OL_IDENTIFIER_NOT_FOUND	The specified channel or the user does not exist.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context				Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///		\param[in]	channel_uri			The URI of the channel, returned in a prior ChannelID event.
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus DestroyChannel(OLContext context, const char *channel_uri) = 0;


		/// \addtogroup OLVOICE_API_RESERVED_CONTEXT
		/// 

		///
		/// \brief Start Voice Audio Monitoring
		///
		/// This begins feeding the application information about the voice detected from the microphone. The voice 
		/// subsystem will indicate if the user is speaking, the sonic energy of the audio, and the smoothed energy 
		/// of the audio. The events of type VoiceMonitorUpdate will continue until StopVoiceMonitor is called. This
		/// request generates a StatusResult event.
		///
		/// If the joinEchoChannel parameter is true, the user will be joined to a special channel that takes their
		/// microphone output and plays it back through their speakers after a brief delay.
		///
		/// Note: The SDK Test Harness does not support this feature.
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///				\statusResult OL_SUCCESS			Voice audio monitoring is started.
		///				\statusResult OL_INTERNAL_ERROR		Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the container.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context				A reserved context used to pair up async operations; see \ref CONTEXT_PAGE_RESERVED_CONTEXT.
		///
		///		\param[in]	joinEchoChannel		Join echo channel. 
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus StartVoiceMonitor(OLContext context, bool joinEchoChannel) = 0;

		/// 

		///
		/// \brief Stop Voice Audio Monitoring
		///
		/// Previously enabled Voice Monitor events are cancelled, and the user is removed from the echo
		/// channel. If the user was previously in a channel, they will rejoin that channel.
		/// This request generates a StatusResult event.
		///
		/// Note: The SDK Test Harness does not support this feature.
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///				\statusResult OL_SUCCESS			Voice audio monitoring is stopped.
		///				\statusResult OL_INTERNAL_ERROR		Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the container.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context				Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus StopVoiceMonitor(OLContext context) = 0;

		///
		/// \brief Set Participant Playback Level (0-100)
		///
		/// The playback level of the participant is adjusted to the given level, with 0 being silence and 100
		/// being maximum volume. This call affects only the playback in the speakers of the caller, not the playback
		/// level of all listeners in the channel. This request generates a StatusResult event.
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///				\statusResult OL_SUCCESS				The specified user's playback level is set.
		///				\statusResult OL_INTERNAL_ERROR			Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY			Failed to allocate space for the container.
		///				\statusResult OL_IDENTIFIER_NOT_FOUND	The specified channel or the user does not exist.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context				Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///		\param[in]	channel_uri			The URI of the channel, returned in a prior ChannelID event.
		///
		///		\param[in]	participantId		The User ID of the participant to manipulate.
		///
		///		\param[in]	level				The volume level from 0-100 of the playback.
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus SetParticipantPlaybackLevel(OLContext context, const char *channel_uri, olapi::OLID participantId, int level) = 0;

		///
		/// \brief Set Participant Playback Mute Status
		///
		/// The given participant is either muted (set to playback volume level 0), or returned to their prior playback
		/// level. This call affects only the playback in the speakers of the caller, not the playback
		/// level of all listeners in the channel. This request generates a StatusResult event.
		///
		/// If this call is made with participantId being the current user, the mute will be applied to the user's
		/// microphone and no participant will hear from the current user again until they unmute themselves.
		///
		/// \responseEventList
		///		The OSP responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLVoiceEventHandler::StatusResult
		///			\eventDesc	 The StatusResult event indicates one of the following errors:
		///				\statusResult OL_SUCCESS				The specified user's mute status is changed.
		///				\statusResult OL_INTERNAL_ERROR			Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY			Failed to allocate space for the container.
		///				\statusResult OL_IDENTIFIER_NOT_FOUND	The specified channel or the user does not exist.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context				Used to pair this request with one of the two event responses (see \ref olapi::OLGetNextContext).
		///
		///		\param[in]	channel_uri			The URI of the channel, returned in a prior ChannelID event.
		///
		///		\param[in]	participantId		The User ID of the participant to manipulate.
		///
		///		\param[in]	mute				Whether or not to mute the participant.
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus SetParticipantPlaybackMuteStatus(OLContext context, const char *channel_uri, olapi::OLID participantId, bool mute) = 0;

		///
		/// \brief Set Overlay's Speaking Indicator Status
		///
		/// Turns on and off the overlay's default speaking indicator.  The speaking indicator will only
		/// turn on if voice is enabled in the Title.
		///
		///		\param[in]	visible			Whether or not to show the speaking indicator.
		///
		///	\return	Returns a OLStatus code.
		///		\retval OL_SUCCESS					Request sent; expect a ContainerIDResult or StatusResult event matching the supplied context.
		///		\retval OL_SESSION_NOT_STARTED		Calling the method outside of a user session (user not bound).
		///		\retval OL_SERVICES_NOT_STARTED		The OnLive Services are not running; see \ref OLSERVICE_API_CONNECTION_DISCONNECTION.
		///		\retval OL_INTERNAL_IO_ERROR		The SDK failed to send the request to the OSP.
		///
		virtual OLStatus SetSpeakingIndicatorStatus(bool visible) = 0;
	};


	///
	/// \ingroup OLVOICE_API_EVENT_HANDLING
	/// \class OLVoiceEventHandler
	/// \brief This is the OLVoice API's event interface; your OLVoiceEventHandler() must implement this interface.
	///		Each method represents an asynchronous event returned by the SDK.
	///
	/// See \ref EVENTS_PAGE_CONCEPTS for an overview of event handling.
	///
	///
	class OLVoiceEventHandler
	{
		public:

		///
		/// \brief Event returning a containerID result; indicates that a request successfuly resulted
		///		in a container.
		///
		/// \note If you return OL_SUCCESS, the container will have its normal
		///		session flow (which varies by container type).  Any other return value causes
		///		the SDK to destroy the container immediately after this method returns.
		///
		/// See \ref SDK_CONTAINERS_PAGE for an overview of containers.
		///
		///		\param[in] context		The matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in] id			The requested container's ID.  The container's type will depend on what you requested.
		///
		///	\return Returns a OLStatus code to the SDK.
		///		\retval OL_SUCCESS				The event was handled; the container is valid until destroyed.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored (and the container will be destroyed immediately).
		///
		virtual OLStatus ContainerIDResult(OLContext context, OLID id) = 0;

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
		///		\retval OL_FAIL					Event not handled.
		///
		virtual OLStatus StatusResult(OLContext context, OLStatus status) = 0;

		/// \addtogroup OLVOICE_API_RESERVED_CONTEXT
		/// 

		///
		/// \brief A voice chat channel was joined.
		///
		/// The user has been joined to a voice chat channel. The owner of the channel
		/// as well as the URI and name of the channel are provided.
		///
		/// The user may have voice chat disabled via the Overlay API. In that situation, the Join Channel and Leave Channel
		/// API calls are available and will succeed, but the user's participant events will say that the user is disabled until
		/// the user enables themselves.
		///
		///		\param[in]	context			The matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in]	channelOwner	The creator of the channel, also its owner.
		///
		///		\param[in]	channelURI		The URI of the channel
		///
		///		\param[in]	channelName		The human readable name of the channel.
		///
		///	\return An OLStatus code indicating if you have handled the event.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///		\retval OL_FAIL					Event not handled.
		///
		virtual OLStatus ChannelJoinedEvent(OLContext context, OLID channelOwner, const char *channelURI, 
			const char *channelName) = 0;

		
		/// 

		///
		/// \brief A voice chat channel was exited.
		///
		/// The user has exited a voice chat channel. The owner of the channel
		/// as well as the URI and name of the channel are provided.
		///
		/// The user may have voice chat disabled via the Overlay API. In that situation, the Join Channel and Leave Channel
		/// API calls are available and will succeed, but the user's participant events will say that the user is disabled until
		/// the user enables themselves.
		///
		///		\param[in]	context			The matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in]	channelOwner	The creator of the channel, also its owner.
		///
		///		\param[in]	channelURI		The URI of the channel
		///
		///		\param[in]	channelName		The human readable name of the channel.
		///
		///	\return An OLStatus code indicating if you have handled the event.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///		\retval OL_FAIL					Event not handled.
		///
		virtual OLStatus ChannelExitedEvent(OLContext context, OLID channelOwner, const char *channelURI, 
			const char *channelName) = 0;

		/// \addtogroup OLVOICE_API_RESERVED_CONTEXT
		/// 

		///
		/// \brief Updates the status of a channel participant
		///
		/// After joining a channel, events are generated describing the state of its participants, whether
		/// speaking, not speaking, muted, or unmuted. Upon joining a channel, the applicatin will receive
		/// ParticipantEvent callbacks for each user in the channel. Their state should be set to IDLE unless
		/// a followup event describes them as SPEAKING or MUTE.
		///
		///		\param[in]	context			The reserved matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in]	participantID	The User ID of the participant being updated
		///
		///		\param[in]	participantTag	The Tag of the participant being updated
		///
		///		\param[in]	type			The new state of the participant
		///
		///	\return An OLStatus code indicating if you have handled the event.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///		\retval OL_FAIL					Event not handled.
		///
		virtual OLStatus ParticipantEvent(OLContext context, OLID participantID, const char *participantTag,
			OLVoiceParticipantEventType type) = 0;

		///
		/// \brief Voice Monitor Data Update
		///
		/// On requesting voice data monitor information, provides information about the analyzed voice signal.
		///
		///		\param[in]	context			The reserved matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in]	voiceDetected	Describes whether or not voice was detected in the audio frame.
		///
		///		\param[in]	energy			Describes the audio energy in the audio frame.
		///
		///		\param[in]	smoothedEnergy	Describes the smoothed energy contained in the audio frame.
		///
		///	\return An OLStatus code indicating if you have handled the event.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///		\retval OL_FAIL					Event not handled.
		///
		virtual OLStatus VoiceMonitorUpdate(OLContext context, bool voiceDetected, double energy, int smoothedEnergy) = 0;

		
		/// 
	};

	///
	/// \ingroup OLVOICE_API_EVENT_HANDLING 
	/// \class OLVoiceCallbacks
	/// \brief This is the OLVoice API's callback interface; your title's OLVoice callbackHandler must implement this interface.
	///
	/// Each method in this class represents an OLVoice callback.  If you choose to handle OLVoice callbacks,
	/// store the passed in callbackHandler using \ref OLVoice::SetEventWaitingCallback.  Callback methods
	/// may be invoked at any time once the OnLive Services are running (see \ref OLSERVICE_API_CONNECTION_DISCONNECTION).
	///
	/// \note Callback methods are always invoked from an internal OnLive SDK thread, so all callback implementations must be thread-safe.
	///
	/// See \ref EVENTS_PAGE_CONCEPTS for details.
	///
	class OLVoiceCallbacks
	{
		public:

		///
		/// \brief Callback dispatched when an OLVoice event is waiting to be handled.
		///
		/// If your title handles events from callbacks, call \ref OLVoice::HandleEvent(unsigned int) to dispatch the waiting event.
		///	If you handle events by polling, you can ignore this event.
		///
		/// \note The EventWaiting callback must be thread-safe and must complete in less than two milliseconds.
		///
		/// See \ref EVENTS_PAGE_HANDLING_OVERVIEW for more info.
		///
		virtual void EventWaiting() = 0;	
	};

	/// \addtogroup OLVOICE_API_CREATE_AND_DESTROY
	/// 

	///
	/// \brief Get the OLVoice API singleton (the instance is created if necessary).
	///
	///	The OLVoice pointer can be stored until the API is destroyed with \ref olapi::OLStopVoice().
	///
	/// See \ref OLVOICE_API_CREATE_AND_DESTROY.
	///
	///		\param[in] version			The API version string (for class compatibility), use \ref OLAPI_VERSION.
	///
	///		\param[out] service			The supplied pointer is set to the OLVoice instance on success.
	///									The pointer is not modified on failure.
	///
	///	\return Returns a OLStatus code.
	///		\retval	OL_SUCCESS			Success; the service pointer is set to the singleton.
	///		\retval OL_INVALID_PARAM	The supplied version string or service pointer is NULL.
	///		\retval OL_INVALID_VERSION	The API version requested is not supported. This is a fatal 
	///										error and your title should exit.
	///		\retval OL_INTERNAL_ERROR	A general internal or system error.
	///		\retval OL_OUT_OF_MEMORY	The SDK could not alloc memory for the singleton.
	///		\retval	OL_SERVICES_ALREADY_STARTED
	///									This API was initialized after olapi::OLStartServices() was called.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLGetVoice(const char* version, OLVoice** service);

	///
	/// \brief Destroy the OLVoice API singleton.
	///
	/// The OnLive Services should be stopped before you destroy an API instance; see \ref olapi::OLStopServices().
	///
	/// See \ref OLVOICE_API_CREATE_AND_DESTROY.
	///
	///		\param[in] service			Pointer to the OLVoice instance to destroy.
	///
	///	\return Returns a OLStatus code.
	///		\retval	OL_SUCCESS			Success; the service was destroyed.
	///		\retval OL_INVALID_PARAM	The supplied service pointer was NULL.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLStopVoice(OLVoice* service);

	/// 



/*!

\addtogroup OLVOICE_API
	\brief The OLVoice interface allows your title to interact with OnLive Voice Chat System.
	It is responsibe for communicating voice chat events while the user is bound to your title. This includes
	managing voice channels for the currently bound user and retriving information about other participants in a voice channel,

\addtogroup OLVOICE_API_CREATE_AND_DESTROY
	\brief The OLVoice singleton is created and retrieved with \ref olapi::OLGetVoice() and destroyed with \ref olapi::OLStopVoice(). Do 
	not destroy any of the OnLive APIs (or their handlers) until after you have stopped the OnLive Services. 
	
		See \ref SDK_SESSION_FLOW_PAGE for information about the session flow.

		See \ref OLSERVICE_API_CONNECTION_DISCONNECTION for information about OSP connection and disconnection.

\addtogroup OLVOICE_API_EVENT_HANDLING
	\brief Callbacks and events are used for asynchronous communication to and from the OSP. 
	
	See \ref SDK_EVENTS_PAGE for an overview.

\addtogroup OLVOICE_API_VOICE_STATUS_CONTAINER
	\brief The voice status container holds information about the voice chat system in the current user session; 
	the information inside the container is depending on the caller function. 
	For example, the currently bound user's headset capabilities are populated in the container when \ref olapi::OLVoice::GetVoiceCapabilities() is called;
	The voice channel's URI, owner's user ID, and channel description are populated in the container when \ref olapi::OLVoice::CreateChannel() is called.
		
		See \ref SDK_CONTAINERS_PAGE for information about OnLive SDK Containers.

\addtogroup OLVOICE_API_INIT_VOICE_CHAT_SYSTEM
	\brief In order to use the OnLive Voice Chat System in your title, you must call \ref olapi::OLVoice::InitChat() to initialize 
	the subsystem for each user session (after the user is bound). The voice chat system will not functional if the subsystem is not initialized. For example,
	attempting to create a channel using \ref olapi::OLVoice::CreateChannel() function will result an error OLStatus code. 
	
	It is not necessary to call \ref olapi::OLVoice::StopChat() at the end of a user session (after the user is unbound). The OSP will clean up itself; however,
	no event will be sent to the title to inform that the voice chat subsystem has been reset.

\addtogroup OLVOICE_API_RESERVED_CONTEXT
	\brief The \ref olapi::OLVoice::JoinChannel() and \ref olapi::OLVoice::StartVoiceMonitor() functions 
	require the input context to be a reserved context (see \ref CONTEXT_PAGE_RESERVED_CONTEXT).
	Your title should store the reserved context generated by \ref olapi::OLReserveNextContext() with duration set to \ref olapi::OL_DURATION_USER_SESSION,
	and use it to pair subsequence asynchronous events from OSP.
	For example, your title will receive participant updates through \ref olapi::OLVoiceEventHandler::ParticipantEvent() after the current bound user successfuly joins the channel.
	To process only the participant updates for the channel that the user is in, you will need to compare the incoming context in the ParticipantEvent()
	with the reserved context your title passed in in the \ref olapi::OLVoice::JoinChannel() call, and throw out any event that does not have the same context.

*/

}; // namespace olapi

#endif // OLVOICE_H
