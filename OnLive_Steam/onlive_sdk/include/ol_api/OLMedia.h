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
// OLMedia.h $Revision: 57033 $
//

/// \file OLMedia.h
/// \brief The OLMedia interface allows players to use and manage media features including Brag Clips,
/// spectating, and raising Cheers and Jeers in response to other players.
///



#ifndef OLMEDIA_H
#define OLMEDIA_H

#include <ol_api/OLAPI.h>
//#include <ol_api/OLService.h>

namespace olapi
{

	/// 
	/// \ingroup OLMEDIA_API_CREATE_AND_DESTROY
	/// \brief The default version of the OnLive OLMedia API to use in \ref olapi::OLGetMedia and \ref olapi::OLGetMediaStream.
    ///  
#define MEDIA_API_VERSION (OLAPI_VERSION)
    
    
    /// \ingroup OLMEDIA_API_MEDIA_HANDLING
    /// \brief Sets maximum number of media clips or IDs to retrieve at once.
#define OL_MAX_MEDIA_COUNT		(1500)	

	
	/// \ingroup OLMEDIA_API_MEDIA_HANDLING
    /// \brief Media Sort Types
	///
typedef enum OLMediaSortType
	{
		OL_MEDIA_SORT_NONE = -1,
		OL_MEDIA_SORT_MOST_RECENT,
		OL_MEDIA_SORT_ALPHABETIC,
		OL_MEDIA_SORT_RATING,
		OL_MEDIA_SORT_MOST_VIEWED,
		OL_MEDIA_SORT_MAX_TYPES
	} OLMediaSortType;

	/// \ingroup OLMEDIA_API_MEDIA_HANDLING
	/// \brief An identifier used to control loaded streams.
	///
typedef unsigned __int64 OLStreamId;

	/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
	/// \brief Enumeration specifying keys needed to access the BragClip container
	///
enum BragClipContainerKeys
	{
		OLKEY_RO_BragClipContainerName,						///< string, Name of container.
		OLKEY_RO_BragClipContainerListId,						///< string, ID of parent container list, if any.
		OLKEY_RO_BragClipId,									///< OLID, Brag Clip ID.
		OLKEY_RO_BragClipName, 								///< string, Name of Brag Clip.
		OLKEY_RO_BragClipDescription,							///< string, Description of Brag Clip.
		OLKEY_RO_BragClipUrl,									///< string, URL of Brag Clip.
		OLKEY_RO_BragClipUserId,								///< OLID, User ID creating the Brag Clip.
		OLKEY_RO_BragClipAppId,								///< OLID, Title used to create the Brag Clip. 
		OLKEY_RO_BragClipCreationDate,						 	///< time_t, Creation date in seconds from the UNIX epoch of Brag Clip.
		OLKEY_RO_BragClipViews,								///< int, view count of Brag Clip.
		OLKEY_RO_BragClipRating,								///< 0..1 float.
		OLKEY_RO_BragClipRatingCount,						/// Number of times rated.
		OLKEY_RO_BragClipEventType,							///< int, When monitoring Brag Clips, this defines what type of event occurred OLBragClipEventType.
		OLKEY_RO_BragClipOwnerPrivacySetting,					///< OLMediaOwnerPrivacyType, When monitoring Brag Clips, this identifies the owner's privacy setting for the clip.
		OLKEY_RO_BragClipContainerKeysMax
	};

	/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
	/// \brief Enumeration specifying keys needed to access the SpectatingStream container
	///
	enum SpectatingStreamContainerKeys
	{
		OLKEY_RO_SpectatingStreamContainerName,						///< string, Name of container.
		OLKEY_RO_SpectatingStreamContainerListId,						///< string, ID of parent container list, if any.
		OLKEY_RO_SpectatingStreamId,									///< OLID, spectating stream ID.
		OLKEY_RO_SpectatingStreamUrl,									///< string, URL of Spectating Stream.
		OLKEY_RO_SpectatingStreamUserId,								///< OLID, User ID creating the Spectating Stream.
		OLKEY_RO_SpectatingStreamAppId,								///< OLID, Title used to create the Spectating Stream.
		OLKEY_RO_SpectatingStreamCheers,								///< Number of cheers for this stream.
		OLKEY_RO_SpectatingStreamJeers,								///< Number of jeers for this stream.
		OLKEY_RO_SpectatingStreamPrivacyStatus,					 ///< Current state of the transient privacy status of the stream; see \ref olapi::OLMediaStreamPrivacyStatus.
		OLKEY_RO_SpectatingStreamEventType,							///< int, When monitoring Spectating Streams, this defines the type of event that occurred \ref olapi::OLSpectatingStreamEventType.
		OLKEY_RO_SpectatingStreamOwnerPrivacySetting,					///< OLMediaOwnerPrivacyType, When monitoring Spectating Streams, this identifies the owner's privacy setting for the stream.
		OLKEY_RO_SpectatingStreamNotificationLocation,					///< string, Defines the rectangular location of any visible notification in the form ULX,ULY,LRX,LRY.
		OLKEY_RO_SpectatingStreamViewerCount,                           ///< Number of viewers watching the Spectating Stream.
		OLKEY_RO_SpectatingStreamCreationTime,                          ///< time_t, Creation date in seconds from the UNIX epoch of Spectating Stream.
		OLKEY_RO_SpectatingStreamContainerKeysMax
	};
        

	/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
    /// \brief Brag Clip container name
	#define BRAGCLIPCONTAINERNAME	"BragClipContainer"					

	/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
    /// \brief Spectating stream container name
	#define SPECTATINGSTREAMCONTAINERNAME	"SpectatingStreamContainer"	
        

	/// \ingroup OLMEDIA_API_BRAGLIP_HANDLING
	/// \brief Key strings in the BragClip container
	///
#ifndef NO_OLAPI_DLL_LINKAGE
extern __declspec(dllimport) char *BragClipContainerKeyNames[olapi::OLKEY_RO_BragClipContainerKeysMax];
    #endif
        
	/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
	/// \brief Key strings in the Spectating Stream container
	///
	#ifndef NO_OLAPI_DLL_LINKAGE
	extern __declspec(dllimport) char *SpectatingStreamContainerKeyNames[olapi::OLKEY_RO_SpectatingStreamContainerKeysMax];
	#endif

	/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
	/// \brief Event types, sent in containers when brag clip notifications are enabled via \ref olapi::OLMedia::MonitorBragClips.
	///
	enum OLBragClipEventType
	{
		OL_BRAG_CLIP_CREATED,				///< A new Brag Clip was created.
		OL_BRAG_CLIP_UPDATED,				///< A Brag Clip was updated.
		OL_BRAG_CLIP_DELETED,				///< An existing Brag Clip was deleted.
		OL_BRAG_CLIP_EVENT_MAX
        
	};

	/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
	/// \brief Event types, sent in containers when Brag Clip notifications are enabled via \ref olapi::OLMedia::MonitorSpectatingStreams.
	///
	enum OLSpectatingStreamEventType
	{
		OL_SPECTATING_STREAM_STARTED,		///< A new spectating stream has started.
		OL_SPECTATING_STREAM_UPDATED,		///< A spectating stream was updated.
		OL_SPECTATING_STREAM_HAD_FEEDBACK,	///< A spectating stream had feedback.
		OL_SPECTATING_STREAM_ENDED,			///< An existing spectating stream ended.
		OL_SPECTATING_STREAM_EVENT_MAX
	};

	/// \ingroup OLMEDIA_API_PRIVACY
	/// \brief Privacy types, provided in notifications about Brag Clips and spectating streams when enabled via 
	///		\ref olapi::OLMedia::MonitorBragClips or \ref olapi::OLMedia::MonitorSpectatingStreams.
	///
	enum OLMediaOwnerPrivacyType
	{
		OL_MEDIA_OWNER_PRIVACY_TYPE_ANYBODY,		///< Anybody can see the Brag Clip or spectating stream.
		OL_MEDIA_OWNER_PRIVACY_TYPE_FRIENDS_ONLY,	///< Only friends can see the Brag Clip or spectating stream.
		OL_MEDIA_OWNER_PRIVACY_TYPE_NO_ONE,	        ///< No one can see the Brag Clip or spectating stream.
		OL_MEDIA_OWNER_PRIVACY_TYPE_MAX
	};

	/// \ingroup OLMEDIA_API_PRIVACY
	/// \brief Transient privacy status for a stream. During gameplay, the service can 
	/// notify your title that a spectating stream being viewed went to
	/// a temporary privacy mode. In a temporary privacy mode, the service blocks video, audio, and/or notification user-private information areas. 
	/// The service notifies your title so the UI can message the situation accordingly.
	///
	enum OLMediaStreamPrivacyStatus
	{
		OL_MEDIA_STREAM_PRIVACY_STATUS_NO_RESTRICTION,  ///< The stream is unrestricted.
		OL_MEDIA_STREAM_PRIVACY_STATUS_BLOCK_VIDEO,	 ///< The stream's video has been temporarily suspended by the service.
		OL_MEDIA_STREAM_PRIVACY_STATUS_BLOCK_AUDIO,	 ///< The stream's audio has been temporarily suspended by the service.
		OL_MEDIA_STREAM_PRIVACY_STATUS_BLOCK_VIDEO_AND_AUDIO, ///< The stream's video and audio have been temporarily suspended by the service.
		OL_MEDIA_STREAM_PRIVACY_STATUS_BLOCK_NOTIFICATION_AREA, ///< The stream is unrestricted, but spectators are required to block the notification area.
		OL_MEDIA_STREAM_PRIVACY_STATUS_BLOCK_VIDEO_AND_AUDIO_AND_NOTIFICATION_AREA, ///< The stream is suspended, and the notification area must be blocked.
		OL_MEDIA_STREAM_PRIVACY_STATUS_BLOCK_AUDIO_AND_NOTIFICATION_AREA, ///< The stream's audio is suspended, and spectators are required to block the notification area.
		OL_MEDIA_STREAM_PRIVACY_STATUS_MAX
	};


	/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
	/// \brief Brag Clip events as strings
	///
	#ifndef NO_OLAPI_DLL_LINKAGE
	extern __declspec(dllimport) char *BragClipEventNames[OL_BRAG_CLIP_EVENT_MAX];
	#endif

	/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
	/// \brief Spectating stream events as strings
	///
	#ifndef NO_OLAPI_DLL_LINKAGE
	extern __declspec(dllimport) char *SpectatingStreamEventNames[OL_SPECTATING_STREAM_EVENT_MAX];
	#endif

	/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
	/// \brief Spectator Feedback Type
	///

	enum OLSpectatorFeedbackType
	{
		OL_SPECTATOR_FEEDBACK_CHEER,	///< The spectator enjoyed what he or she saw
		OL_SPECTATOR_FEEDBACK_JEER		///< The spectator hated what he or she saw
	};

	/// \ingroup OLMEDIA_API_MEDIA_HANDLING
	/// \brief Media Types
	///
	enum OLMediaType
	{
		OL_MEDIA_TYPE_NONE = -1,		///< Unknown type of stream.
		OL_MEDIA_TYPE_BRAG_CLIP,		///< A Brag Clip.
		OL_MEDIA_TYPE_STREAM,			///< A Spectating Stream.
		OL_MEDIA_TYPE_OLMC,				///< An OnLive Media Container.
		OL_MEDIA_TYPE_MAX
	};
	
	class OLMediaEventHandler;
	class OLMediaCallbacks;
	class OLMediaStreamEventHandler;
	class OLMediaStreamCallbacks;


	/// \ingroup OLMEDIA_API OLMEDIA_API_EVENT_HANDLING 
	/// \class OLMedia
	/// \brief Media Interface Class
	///
	/// The OLMedia interface allows players to use and manage media features including Brag Clips,
	/// spectating, and responding to other players with cheers or jeers. 
	/// Players on your title can save and share video clips of their game, called Brag Clips.
	/// Users can watch others play games and either cheer or jeer as play progresses.
	///
	/// Your title has access to information related to these features, such as whether or not a
	/// particular user has viewed a specified brag clip. Users or titles can determine
	/// who may watch game play and which brag clips to save or remove.
	///
	class OLMedia : public OLEventBase
	{
		public:

		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		/// \name OLMedia Callbacks & Event Handling

		/// \brief Allows client to be notified that an event is available.
		/// \par Description:
		/// Sets up a callback class object to be notified when an event is waiting to be handled. Event
		/// notification happens by calling the callback class object's EventWaiting() function.\n\n
		/// \note
		/// The title can use this to optimize Event Handling.
		///
		///		\param[in]	callback		Pointer to OLAppEventWaitingCallback to call when an event is available.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS					Normal operation; callbacks can be dispatched on the supplied object while
		///												the services are running.
		///		\retval OL_INVALID_PARAM			Error - The supplied callback object pointer was NULL (or invalid);
		///												the callback obj has not been set.
		///
		virtual OLStatus SetEventWaitingCallback(OLMediaCallbacks* callback) = 0;


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		/// \brief Sets handler to call when an title event processes.
		/// \par Description:
		/// Handler used by title to process events.
		///
		///		\param[in]	handler			Pointer to olapi::OLMediaEventHandler() interface.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS					Normal operation; events will be dispatched on the supplied obj.
		///		\retval OL_INVALID_PARAM			Error - The supplied event handler pointer was NULL (or invalid);
		///												the event handler has not been set.
		///
		virtual OLStatus SetEventHandler(OLMediaEventHandler* handler) = 0;


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		/// \brief Dispatches the next media API event.
		/// \par Description:
		/// Dispatches the next media API event, if available, to the handler indicated in olapi::SetEventHandler().  Implements an
		/// olapi::OLMediaEventHandler() interface in the title to process events from each of the APIs.
		///
		///		\param[in]	handler			Pointer to \ref olapi::OLMediaEventHandler interface.
		///
		///		\param[in]	time			Time to wait in milliseconds for next event; OL_INFINITE will wait until next event without timing out.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS			An event was dispatched.
		///		\retval OL_INVALID_PARAM	The API does not have an event handler set (see \ref OLMedia::SetEventHandler).
		///		\retval OL_EVENT_TIMEOUT	No events were available for the duration of the wait.
		///
		virtual OLStatus HandleEvent(OLMediaEventHandler* handler, unsigned int time) = 0;


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		/// \brief Retrieves next event, if available.
		/// \par Description:
		/// Call upon event callback or per frame to handle (dispatch) the next event.
		/// The event handler must be previously set up with a SetEventHandler call in each API.
		///
		///		\param[in]	time		Time to wait for next event in milliseconds or\n
		///								OL_INFINITE to wait for next event without timing out.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS			An event was dispatched.
		///		\retval OL_INVALID_PARAM	The API does not have an event handler set (see \ref OLMedia::SetEventHandler).
		///		\retval OL_EVENT_TIMEOUT	No events were available for the duration of the wait.
		///
		virtual OLStatus HandleEvent(unsigned int time) = 0;


		/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
		/// \brief Gets metadata for available brag clips.
		/// \par Description:
		///  The GetBragClips() function retrieves the metadata for Brag Clips that are visible to the requesting User ID and that match
		///  the ID types given in the mixed ID list. 
		/// \par mixedIDs:		
		/// mixedIDs is a list of IDs used to filter and return certain Brag Clips. Each ID must be one  
		/// the following types:
		///			- OL_IDENTITY_ID: The player object.
		///			- OL_APPLICATION_ID: The application instance ID.
		///			- OL_BRAG_CLIP_ID: The Brag Clip ID.
		///\par	
		/// Clips matching the supplied ID(s) are returned. For example, to receive all clips from "SomeGame" made by 
		/// "Joe" and "Bob," two OL_IDENTITY_IDs (one for Joe and one for Bob) are supplied along with one OL_APPLICATION_ID (for SomeGame).
		/// If no ID is supplied for a type, that type is not filtered.
		///
		/// \responseEventList
		///		The OnLive Service Platform (OSP) responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent OLMediaEventHandler::IDListResult
		///			\eventDesc	Returns the list of IDs to BragClipContainers that match the query.
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR	    Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the containers.
		///				\statusResult OL_MEDIA_ID_NOT_FOUND	    One of the supplied ids doesn't exist.
		///
		/// \responseEventListEnd
		///
		///	  \param[in]	context					Used to pair up async operations.
		///	  \param[in]	requestingUserID  		The user ID requesting the brag clips.
		///	  \param[in]	mixedIDs				A list of IDs to match, see above.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus GetBragClips(OLContext context, OLID requestingUserID, OLIDList *mixedIDs) = 0;


		/// \ingroup OLMEDIA_API_MEDIA_HANDLING
		/// \brief Specifies which Brag Clip and spectating stream metadata to retrieve and generates an IDListResult() event to get 
		/// the requested media's full metadata.
		/// \par Description:
		/// The GetMediaIDs() function retrieves the metadata for media clips that are visible to the requesting User ID, and that match the 
		/// ID types given in the mixedIDs list.
		/// \par mixedIDs:		
		/// mixedIDs is a list of IDs used to filter and return certain Brag Clips and Spectating Streams. Each ID must be one  
		/// the following types:
		///			- OL_IDENTITY_ID: The player object.
		///			- OL_APPLICATION_ID: The application instance ID.
		///			- OL_STREAM_ID: The media stream ID.
		///			- OL_BRAG_CLIP_ID: The Brag Clip ID.
		///\par	
		/// Clips matching the supplied ID(s) are returned. For example, to receive all clips from "SomeGame" made by 
		/// "Joe" and "Bob," two OL_IDENTITY_IDs (one for Joe and one for Bob) are supplied along with one OL_APPLICATION_ID (for SomeGame).
		/// If no ID is supplied for a type, that type is not filtered.
		///
		/// \responseEventList
		///		The OnLive Service Platform (OSP) responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent OLMediaEventHandler::IDListResult
		///			\eventDesc   Returns a list of the IDs that match the query.  These IDs 
		///						 can then be passed to GetBragClips or GetSpectatingStreams 
		///						 to get the media's full metadata.
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR			Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY			Failed to allocate space for the containers.
		///				\statusResult OL_MEDIA_ID_NOT_FOUND	    One of the supplied IDs does not exist.
		///
		/// \responseEventListEnd
		///
		/// Params
		///		\param[in]	context				Used to pair up async operations.
		///	  	\param[in]	requestingUserID  	The user ID requesting the Brag Clips or Spectator Stream.
		///	  	\param[in]	mixedIDs			A list of IDs to match, see above.
		///	  	\param[in]	media_type			The media type requested, typically OL_MEDIA_TYPE_BRAG_CLIP or OL_MEDIA_TYPE_STREAM.
		///		\param[in]	sort_type			The order to sort the results in, see OLTypes.h.
		///		\param[in]	max_count			The maximum number of results allowed.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus GetMediaIDs(OLContext context, 
									 OLID requestingUserID, 
									 OLIDList *mixedIDs, 
									 OLMediaType media_type, 
									 OLMediaSortType sort_type, 
									 int max_count) = 0;


		/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
		/// \brief Gets metadata for available spectating streams.
		/// \par Description:
		/// The GetSpectatingStreams() function retrieves the metadata for spectating streams visible to the requesting user ID and that match the mixed ID list.
		/// 
		/// \par mixedIDs:		
		/// mixedIDs is a list of IDs used to filter and return certain Brag Clips and Spectating Streams. Each ID must be one  
		/// the following types:
		///			- OL_IDENTITY_ID: The player's ID.
		///			- OL_APPLICATION_ID: The application instance ID.
		///			- OL_STREAM_ID: The media stream ID.
		///\par	
		/// Clips matching the supplied ID(s) are returned. For example, to receive all clips from "SomeGame" made by 
		/// "Joe" and "Bob," two OL_IDENTITY_IDs (one for Joe and one for Bob) are supplied along with one OL_APPLICATION_ID (for SomeGame).
		/// If no ID is supplied for a type, that type is not filtered.
		/// 
		///
		/// \responseEventList
		///		The OnLive Service Platform (OSP) responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent OLMediaEventHandler::IDListResult
		///			\eventDesc	Returns the list of IDs to SpectatingStream Containers that match the query.
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR	    Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY		Failed to allocate space for the containers.
		///				\statusResult OL_MEDIA_ID_NOT_FOUND	    One of the supplied ids doesn't exist.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context				Used to pair up async operations.
		///		\param[in]	requestingUserID	The user ID requesting the spectating streams.
		///		\param[in]	mixedIDs			A list of IDs to match, see above.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus GetSpectatingStreams(OLContext context, OLID requestingUserID, OLIDList *mixedIDs) = 0;


		/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
		/// \brief Gets media ratings provided by a user for clip IDs.
		/// \par Description:
		///   Gets the ratings given by a user for the set of brag clips provided.
		///   Causes an IDListResult event with container IDs, one for each brag clip requested. The containers
		///   define the OLKEY_RO_BragClipId key, and the OLKEY_RO_BragClipRatings key, but no others.
		///
		/// \responseEventList
		///		The OnLive Service Platform (OSP) responds with one of the following events if this request is successfully sent: 
		/// 
		///		\ospEvent OLMediaEventHandler::IDListResult
		///			\eventDesc	Returns the list of IDs to BragClip Containers that match the query.  
		///						These BragClip containers will only have OLKEY_RO_BragClipId and 
		///						OLKEY_RO_BragClipRatings set.
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR			Generic OSP failure.
		///				\statusResult OL_OUT_OF_MEMORY			Failed to allocate space for the containers.
		///				\statusResult OL_MEDIA_ID_NOT_FOUND	    Specified Media doesn't exist.
		///
		/// \responseEventListEnd
		///
		///	  	\param[in]	context		Used to pair up async operations.
		///	  	\param[in]	userID		The user ID providing the ratings.
		///	  	\param[in]	bragClipIDs	The brag clip IDs rated.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus GetMediaRatingsByUserForIDs(OLContext context, OLID userID, OLIDList *bragClipIDs) = 0;


		/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
		/// \brief Starts monitoring creation, update, and deletion of Brag Clips.
		/// \par Description:
		///   Tells the media service to monitor, create, update, and delete events for Brag Clips that match 
		///   the appID and userID supplied.  Title will be notified of matching events via the
		///   event handler as a ContainerIDResult() event.
		///
		/// \responseEventList
		///		The OnLive Service Platform (OSP) responds with the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///
		///				\statusResult OL_SUCCESS				Call completed successfully.
		///
		/// \responseEventListEnd
		///
		///	  	\param[in]	context		A reserved context used to pair up async operations; Use the same context used for \ref olapi::OLMedia::StopMonitoringBragClips().  
		//								see \ref CONTEXT_PAGE_RESERVED_CONTEXT.
		///								Note: You must use a reserved context for this call.
		///
		///   	\param[in]	appID		The title ID used in the requested Brag Clips, or OLID_INVALID for all.
		///   	\param[in]	userID		The user ID who created the Brag Clips, or OLID_INVALID for all.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus MonitorBragClips(OLContext context, OLID appID, OLID userID) = 0;


		/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
		/// \brief Stops monitoring Brag Clip updates.
		/// \par Description:
		///   Tells the media service to stop monitoring Brag Clip create/delete events.
		///
		/// \responseEventList
		///		The OnLive Service Platform (OSP) responds with the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///
		///				\statusResult OL_SUCCESS					Call completed successfully.
		///				\statusResult OL_MEDIA_CONTEXT_NOT_FOUND	Specified Context cant be found.
		///
		/// \responseEventListEnd
		///
		///	  	\param[in]	context		Used to pair up async operations. Use the same context used for \ref olapi::OLMedia::MonitorBragClips(). 
		///								Note: You must use a reserved context for this call (see \ref CONTEXT_PAGE_RESERVED_CONTEXT).
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus StopMonitoringBragClips(OLContext context) = 0;


		/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
		///
		/// \brief Monitor the start, updates, and end of spectating streams.
		/// \par Description:
		///   Tells the media service to monitor create, update and delete events for spectating streams that match 
		///   the appID and userID supplied.  Title will be notified of matching events via the
		///   event handler as a ContainerIDResult() event.
		///
		/// \responseEventList
		///		The OnLive Service Platform responds with the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///
		///				\statusResult OL_SUCCESS				Call completed successfully.
		///
		/// \responseEventListEnd
		///
		///	  	\param[in]	context		A reserved context used to pair up async operations; 
		///								Note: You must use a reserved context for this call (see \ref CONTEXT_PAGE_RESERVED_CONTEXT).
		///
		///   	\param[in]	appID		The title ID used in the requested spectating streams, or OLID_INVALID for all.
		///   	\param[in]	userID		The user ID who created the spectating streams, or OLID_INVALID for all.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus MonitorSpectatingStreams(OLContext context, OLID appID, OLID userID) = 0;


		/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
		/// \brief Stops monitoring spectating stream updates.
		/// \par Description:
		///   Tells the media service to stop monitoring spectating stream start/end events.
		///
		/// \responseEventList
		///		The OnLive Service Platform responds with the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///				\statusResult OL_SUCCESS					call completed successfully.
		///				\statusResult OL_MEDIA_CONTEXT_NOT_FOUND	Specified Context cant be found.
		///
		/// \responseEventListEnd
		///
		///	  	\param[in]	context		Used to pair up async operations, use the same context you used for
		///			  					\ref olapi::OLMedia::MonitorSpectatingStreams; see \ref CONTEXT_PAGE_RESERVED_CONTEXT
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus StopMonitoringSpectatingStreams(OLContext context) = 0;


		/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
		/// \brief Sets the user's rating for a Brag Clip.
		/// \par Description:
		///	Passes the user's rating for a given Brag Clip to the media service.
		/// \note
		/// The title must not call the RateBragClip() function more than 1 time every 
		/// 5 seconds for any one Stream or Brag Clip™ since these result in a user notification.
		///
		/// \responseEventList
		///		The OnLive Service Platform (OSP) responds with the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///				\statusResult OL_SUCCESS				Call completed successfully.
		///				\statusResult OL_INTERNAL_ERROR			Generic OSP failure.
		///				\statusResult OL_MEDIA_ID_NOT_FOUND	    Specified Brag Clip doesn't exist.
		///
		/// \responseEventListEnd
		///
		///	  	\param[in]	context			Used to pair up async operations
		///	  	\param[in]	bragClipID		The Brag Clip ID to rate
		///   	\param[in]	userID			The user ID rating the Brag Clip
		///	  	\param[in]	rating			A number from 0 to 4, with 4 being better
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus RateBragClip(OLContext context, OLID bragClipID, OLID userID, double rating) = 0;


		/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
		/// \brief Provides feedback for a spectating stream.
		/// \par Description:
		///	Passes a CHEER or JEER value about a given spectating stream to the media service. 
		/// \note
		/// The title must not call the AudienceFeedbackForSpectatingStream() function more than 1 time every 5 
		/// seconds for any one Stream or Brag Clip&tm; since these result in a user notification.
		///
		/// \responseEventList
		///		The OnLive Service Platform (OSP) responds with the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///
		///				\statusResult OL_SUCCESS				Call completed successfully.
		///				\statusResult OL_INTERNAL_ERROR			Generic OSP failure.
		///				\statusResult OL_MEDIA_ID_NOT_FOUND	    Specified Stream doesn't exist.
		///
		/// \responseEventListEnd
		///
		///	  	\param[in]	context					Used to pair up async operations.
		///	  	\param[in]	spectatingStreamID		The spectating stream ID to receiving feedback.
		///	  	\param[in]	userID					The user ID providing the feedback.
		///	  	\param[in]	type					The type of feedback provided, OL_SPECTATOR_FEEDBACK_CHEER or OL_SPECTATOR_FEEDBACK_JEER.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus AudienceFeedbackForSpectatingStream(OLContext context, OLID spectatingStreamID, OLID userID, OLSpectatorFeedbackType type) = 0;


		/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
		/// \brief Notifies the media service that a user viewed a Brag Clip.
		/// \par Description:
		///	Notifies the media service that a user viewed a Brag Clip.
		///
		/// \responseEventList
		///		The OnLive Service Platform responds with the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///
		///				\statusResult OL_SUCCESS				call completed successfully.
		///				\statusResult OL_MEDIA_ID_NOT_FOUND	    Brag Clip doesn't exist.
		///
		/// \responseEventListEnd
		///
		///	  	\param[in]	context			Used to pair up async operations.
		///	  	\param[in]	bragClipID		The brag clip that a user viewed.
		///	  	\param[in]	appsessionID	The application session of the viewer.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus BragClipViewed(OLContext context, OLID bragClipID, OLID appsessionID) = 0;


		/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
		/// \brief The Title notifies the media service that a viewer started spectating a stream. 
		///
		/// \responseEventList
		///		The OnLive Service Platform responds with the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///
		///				\statusResult OL_SUCCESS				call completed successfully.
		///				\statusResult OL_MEDIA_ID_NOT_FOUND	    Spectating Stream doesn't exist.
		///
		/// \responseEventListEnd
		///
		///	  	\param[in]	context				Used to pair up async operations.
		///	  	\param[in]	spectatingStreamID	The Spectating Stream the user began to view.
		///	  	\param[in]	appsessionID		The application session of the spectator.
		///
		///	\return Returns an OLStatus.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus ViewerBeganSpectating(OLContext context, OLID spectatingStreamID, OLID appsessionID) = 0;


		/// \ingroup OLMEDIA_API_SPECTATING_HANDLING
		/// \brief The Title notifies the media service that a viewer stopped spectating a stream.
		///
		/// \responseEventList
		///		The OnLive Service Platform responds with the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///
		///				\statusResult OL_SUCCESS				call completed successfully.
		///				\statusResult OL_MEDIA_ID_NOT_FOUND	    Spectating Stream doesn't exist.
		///
		/// \responseEventListEnd
		///
		///	  	\param[in]	context				Used to pair up async operations.
		///	  	\param[in]	spectatingStreamID	The Spectating Stream the user began to view.
		///	  	\param[in]	appsessionID		The title session of the spectator.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus ViewerEndedSpectating(OLContext context, OLID spectatingStreamID, OLID appsessionID) = 0;


		/// \ingroup OLMEDIA_API_BRAGCLIP_HANDLING
		/// \brief Removes a Brag Clip permanently.
		/// \par Description:
		///		Tells the media service to permanently remove a Brag Clip.
		/// \note
		/// The title must only call the RemoveBragClip() function for Brag Clips owned by the active user (see OLUser::GetStatus() function).
		///
		/// \responseEventList
		///		The OnLive Service Platform (OSP) responds with the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///
		///				\statusResult OL_SUCCESS				Call completed successfully.
		///				\statusResult OL_INTERNAL_ERROR			Generic OSP failure.
		///				\statusResult OL_MEDIA_ID_NOT_FOUND	    Specified Brag Clip doesn't exist.
		///
		/// \responseEventListEnd
		///
		///		\param[in]	context		Used to pair up async operations
		///		\param[in]	bragClipID	The Brag Clip ID to permanently remove.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The request was successfully sent.
		///		\retval OL_INVALID_PARAM		An invalid parameter was provided.
		///		\retval OL_INTERNAL_ERROR		The request could not be sent.
		///
		virtual OLStatus RemoveBragClip(OLContext context, OLID bragClipID) = 0;
	};


	/// \ingroup OLMEDIA_API OLMEDIA_API_EVENT_HANDLING
	/// \class OLMediaStream
	/// \brief Media Stream Interface Class
	/// \par Description:
	/// The OLMediaStream interface provides your title with access to media streams for fixed assets
	/// such as images and trailers as well as user-generated assets such as brag clips and spectating streams.
	/// The API manages video frames in YUV420 format and audio frames as interleaved, little-endian PCM
	/// samples. 
	/// \note
	/// The API also provides jitter buffering, such that the title is notified when video or audio needs
	/// to be updated.
	///
	class OLMediaStream
	{
	public:


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		/// \brief Allows client to receive notification that a stream event became available.
		/// \par Description:
		/// Sets up a callback class object to be notified when a stream event is waiting to be handled. Event
		/// notification is achieved by calling the callback class object's StreamEventWaiting() function.\n\n
		/// The title can use this to optimize Event Handling.
		/// 
		/// \note Your title must call either the \ref olapi::OLMediaStream::UnloadStream() or \ref olapi::OLMediaStream::UnloadStreamImmediate() function for all loaded 
		/// streams before exiting from outside an OLMediaStreamCallback.  
		///
		///		\param[in]	callback	Pointer to OLAppEventWaitingCallback to call when an event is available.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS					Normal operation; callbacks can be dispatched on the supplied object while
		///												the services are running.
		///		\retval OL_INVALID_PARAM			Error - The supplied callback object pointer was NULL (or invalid);
		///												the callback obj has not been set.
		///
		virtual OLStatus SetStreamEventWaitingCallback(OLMediaStreamCallbacks* callback) = 0;


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		/// \brief Sets handler to call when an title stream event processes.
		/// \par Description:
		/// Handler used by the title to process events.
		///
		///		\param[in]	handler		Pointer to OLMediaStreamEventHandler interface.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS					Normal operation; events will be dispatched on the supplied obj.
		///		\retval OL_INVALID_PARAM			Error - The supplied event handler pointer was NULL (or invalid);
		///												the event handler has not been set.
		///
		virtual OLStatus SetStreamEventHandler(OLMediaStreamEventHandler* handler) = 0;


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		/// \brief Dispatches the next media stream event.
		/// \par Description:
		/// Dispatches the next media stream api event, if available, to the handler indicated in SetEventHandler.  Implements an
		/// OLMediaEventHandler() interface in the title to process events from each of the APIs. Unlike other event handler
		/// APIs in the OnLive Service Platform, this event handler does not support waiting for an event. The title is required to use its own
		/// mechanisms if the Title desires waiting behavior.
		///
		///	NOTE:  Unlike other APIs, you cannot call HandleStreamEvent from within a callback.  You must call it on your own thread.
		///
		///		\param[in]	handler			Pointer to OLMediaStreamEventHandler interface, or NULL to use the one 
		///							 		previously set with SetStreamEventHandler.
		///
		///	\return Returns an OLStatus.
		///		\retval OL_SUCCESS			An event was dispatched.
		///		\retval OL_INVALID_PARAM	The API does not have an event handler set (see \ref OLMediaStream::SetStreamEventHandler).
		///		\retval OL_EVENT_TIMEOUT	No events were available for the duration of the wait.
		///
		virtual OLStatus HandleStreamEvent(OLMediaStreamEventHandler* handler) = 0;


		/// \ingroup OLMEDIA_API_MEDIA_HANDLING
		/// \brief Determines if a URL is for a local file or remote stream.
		/// \par Description:
		///   Examines the URL provided and returns true if the URL refers to a file on the local filesystem.
		///   Remote stream URLs tend to have the form onlive://mediaserver/somestream.mov, while local files
		///   tend to start with file:/// or c:
		/// 
		///		\param[in]	url The url to examine.
		///
		///	\return Returns a Boolean.
		///		\retval true	Url refers to a local file.
		///		\retval false	Url refers to a remote stream.
		///
		virtual bool LocationIsFile(const char *url) = 0;

		/// \addtogroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// \name Loading & Unloading Streams
		/// 

		/// \brief Loads a video/audio stream from a given URL.
		/// \par Description:
		///   Initiates a background load of a stream specified by a given URL. The file loads either
		///   on a mediaserver or on the local computer, which parses its metadata.
		///
		///		\note
		/// It is an error to call any functions that relate to the stream between calling LoadStream 
		///			  and your title recieving the StreamLoaded callback.
		/// 
		/// \responseEventList
		///		The OnLive Service Platform responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaStreamEventHandler::StreamLoaded
		///			\eventDesc	 The StreamLoaded event returns the data for the successfully loaded stream.
		///
		///		\ospEvent OLMediaStreamEventHandler::StreamLoadFailed
		///			\eventDesc	 The StreamLoadFailed event indicates a failure to load the stream. 
		///						 Examine the error string for more information.
		///
		/// \responseEventListEnd
		///
		///   	\param[in]	url			Specifies the stream to load, typically of the form onlive://mediaserver/some-movie.mov
		///
		///	\return Returns a stream ID
		///		\retval OLMediaStream::INVALID_STREAM_ID    An error was encountered.
		///		\retval <Any-Valid-ID>				 On success, a valid stream ID is returned.
		///
		virtual OLStreamId LoadStream(const char *url) = 0;


		///
		/// \brief Unloads a previously loaded stream
		/// \par Description
		///   Initiates a background unload of a stream specified by a given ID. Further callback events
		///   for the given stream ID may generate for a brief time, though stream updates should
		///   terminate immediately. The stream stops playback which frees its associated resources. 
		/// 
		/// \responseEventList
		///		The OnLive Service Platform responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaStreamEventHandler::StreamUnloaded
		///			\eventDesc	 The StreamUnloaded event is called when the stream has been successfully unloaded.
		///
		/// \responseEventListEnd
		///
		///   	\param[in]	streamId 	A previously returned stream ID from the \ref LoadStream or \ref olapi::OLMediaStream::LoadStreamImmediate.
		///
		///	\return Returns an OLStatus code.
		///   	\retval OL_SUCCESS			The stream was found and the operation was enqueued.
		///		\retval OL_INVALID_PARAM	The stream was not found.
		///
		virtual OLStatus UnloadStream(OLStreamId streamId) = 0;


		///
		/// \brief Loads a stream such that it is immediately ready for use.
		/// \par Description:
		///   Loads the stream specified by the given URL immediately. Blocks the calling thread until the stream
		///   has been loaded. A description of the stream is immediately available via the streamDescription()
		///   method. No event is generated as a result of this method.
		/// 
		///		\param[in]	url				Specifies the stream to load, typically of the form onlive://mediaserver/some-movie.mov.
		///
		///		\param[in]	streamIsLive	Informs the title if the stream is from a live session or prerecorded.
		///
		///		\param[in]	duration_us		Gives the duration of the stream, if it is not live.
		///
		///		\param[in]	width			Gives the width of the video stream in pixels.
		///
		///		\param[in]	height			Gives the height of the video stream in pixels.
		///
		///		\param[in]	sampleRate		Gives the audio sample rate, if the stream has audio, or 0 if not.
		///
		///		\param[in]	channelCount	Gives the number of channels in the audio if the stream has audio, 0 if not.
		///
		///		\param[in]	bitsPerSample	Gives the number of bits in each audio sample if the stream has audio, 0 if not.
		/// 
		///	
		///	\return Returns a stream ID
		///		\retval OLMediaStream::INVALID_STREAM_ID    An error was encountered.
		///		\retval <Any-Valid-ID>					On success, a valid stream ID is returned.
		///
		virtual OLStreamId LoadStreamImmediate(const char *url, 
											   bool *streamIsLive, 
											   unsigned __int64 *duration_us, 
											   unsigned int *width, 
											   unsigned int *height,
											   unsigned int *sampleRate, 
											   unsigned int *channelCount, 
											   unsigned int *bitsPerSample) = 0;


		///
		/// \brief Unload a stream immediately.
		/// \par Description:
		///   Immediately stops the stream, unloads it, and frees its resources. No event is generated as a
		///   result of this method.
		///
		///   	\param[in]	streamId		A previously returned stream ID from the \ref LoadStream or \ref LoadStreamImmediate. 
		///
		///	\return Returns an OLStatus code.
		///   	\retval OL_SUCCESS			The stream was found and the operation was enqueued.
		///		\retval OL_INVALID_PARAM	The stream was not found.
		///
		virtual OLStatus UnloadStreamImmediate(OLStreamId streamId) = 0;

		/// end addtogroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// end name Loading & Unloading Streams
		/// 

		/// \addtogroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// \name Setting Streams
		/// 

		///
		/// \brief Sets the texture size used by the title to copy video data.
		/// \par Description:
		///   Specifies the texture size for the YUV420 data provided to the title for the
		///   given stream ID. It is safe to set this to the video width and height specified when the stream
		///   loaded. 
		///   \par 
        ///   This call does not modify the size of the video returned, just how much padding 
		///   should be placed arround the video. It is safe to set this to any size equal to or larger than the 
		///   video width and height specified when the stream was loaded. 
		///
		///	  \note It is an error to play a stream whose texture size has not been specified.
		///
		///   	\param[in]	streamId		A previously returned stream ID from the \ref LoadStream or \ref LoadStreamImmediate. 
		///
		///   	\param[in]	textureWidth	Specifies the width in pixels of the texture displaying the video.
		///
		///   	\param[in]	textureHeight	Specifies the height in pixels of the texture displaying the video.
		///
		///	\return Returns an OLStatus code.
		///   	\retval OL_SUCCESS			The stream was found and the operation was enqueued.
		///		\retval OL_INVALID_PARAM	The stream was not found or if the height or width are too small.
		///
		virtual OLStatus SetTextureSize(OLStreamId streamId, 
										unsigned int textureWidth,
										unsigned int textureHeight) = 0;

		
		/// \ingroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// \brief Starts playback of a stream.
		/// \par Description:
		///   Begins playback of a previously loaded stream for which a texture size was also set via
		///   the setTextureSize() method. 
		///
		///   	\param[in]	streamId		A previously returned stream ID from the \ref LoadStream or \ref olapi::OLMediaStream::LoadStreamImmediate.
		///
		///	\return Returns an OLStatus.
		///   	\retval OL_SUCCESS			The stream was found and the operation was enqueued.
		///		\retval OL_INVALID_PARAM	The stream was not found.
		///
		virtual OLStatus PlayStream(OLStreamId streamId) = 0;


		/// \ingroup OLMEDIA_API_SETTING_STREAMS
		///
		/// \brief Suspend playback of a stream.
		/// \par Description:
		///   Suspends playback of a previously loaded stream for which a texture size was also set via
		///   the setTextureSize() method. Calling this method before a stream has finished loading is 
		///   allowed.
		///
		///   	\param[in]	streamId	A previously returned stream ID from the \ref LoadStream or \ref olapi::OLMediaStream::LoadStreamImmediate. 
		///
		///	\return Returns an OLStatus code.
		///   	\retval OL_SUCCESS			The stream was found and the operation was enqueued.
		///		\retval OL_INVALID_PARAM	The stream was not found.
		///
		virtual OLStatus PauseStream(OLStreamId streamId) = 0;


		/// \ingroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// \brief Seek within a stream
		/// \par Description:
		///   Moves the playback position to the given offset within a stream previously loaded via the loadStream
		///   or loadStreamImmediate methods. Offset is a floating point value in the range of 0.0-1.0, with 0.0 
		///   corresponding to the beginning of the stream and 1.0 corresponding to the end. 
		///
		///   	\param[in]	streamId	A previously returned stream ID from the \ref LoadStream or \ref olapi::OLMediaStream::LoadStreamImmediate.
		///
		///   	\param[in]	offset		A floating point value in the range 0.0-1.0, with 0.0 
		///	 							corresponding to the beginning of the stream and 1.0 corresponding to the end. 
		///
		///	\return Returns an OLStatus code.
		///   	\retval OL_SUCCESS			The stream was found and the operation was enqueued.
		///		\retval OL_INVALID_PARAM	The stream was not found.
		///
		virtual OLStatus SeekStream(OLStreamId streamId, double offset) = 0;


		/// \ingroup OLMEDIA_API_SETTING_STREAMS
		///
		/// \brief Sets a stream to looping playback
		/// \par Description:
		///   Specifies whether or not a stream previously loaded via the LoadStream or LoadStreamImmediate methods
		///   should loop. A non-looping stream pauses at the end of playback, whereas a looping
		///   stream continues from its beginning continuously and linearly. If a stream is
		///   looping, the StreamLastFrame callback will be initiated only *once*.
		///
		///   	\param[in]	streamId	A previously returned stream ID from \ref LoadStream or \ref olapi::OLMediaStream::LoadStreamImmediate.
		///
		///   	\param[in]	looping		Specifies whether or not playback should loop.
		///
		///	\return Returns an OLStatus code.
		///   	\retval OL_SUCCESS			The stream was found and the operation was enqueued.
		///		\retval OL_INVALID_PARAM	The stream was not found.
		///
		virtual OLStatus SetLooping(OLStreamId streamId, bool looping) = 0;

		/// end addtogroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// end name Setting Streams
		/// 

		/// \ingroup OLMEDIA_API_STILL_IMAGES
		/// 
		/// \brief Starts an image load operation in the background.
		/// \par Description:
		///   Initiates loading an image file in the background from the local disk or a remote mediaserver.
		///   Local URLs have the form file:///some-file.jpg and mediaserver URLs have the form 
		///   onlive://mediaserver/foo.jpg.
		///   The image file returns as a blob of bytes via the ImageLoaded event, or the ImageLoadFailed even specifies an error.
		///
		/// \responseEventList
		///		The OnLive Service Platform responds with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaStreamEventHandler::ImageLoaded
		///			\eventDesc	 The ImageLoaded event returns the data for the successfully loaded image.
		///
		///		\ospEvent OLMediaStreamEventHandler::ImageLoadFailed
		///			\eventDesc	 The ImageLoadFailed event indicates a failure to load the image. 
		///						 Examine the error string for more information.
		///
		/// \responseEventListEnd
		///
		///   	\param[in]	url		The URL of the image to load, of the form onlive://mediaserver/some-file.jpg or as a local file.
		/// 
		///	\return Returns an OLStatus.
		///   	\retval OL_SUCCESS			The stream was found and the operation was enqueued.
		///		\retval OL_INVALID_PARAM	The stream was not found.
		///
		virtual OLStatus LoadImage(const char *url) = 0;


		/// \ingroup OLMEDIA_API_STILL_IMAGES
		///
		/// \brief Loads an image immediately.
		/// \par Description:
		///   Immediately loads an image file from the given URL, either from the local disk or a remote mediaserver.
		///   Local URLs have the form file:///some-file.jpg and mediaserver URLs have the form 
		///   onlive://mediaserver/foo.jpg
		/// 
		///   	\param[in]	url					The URL of the image to load, of the form onlive://mediaserver/some-file.jpg or as a local file.
		///
		///   	\param[in]	data				Bytes from the file. 
		///
		///   	\param[in]	maxByteCount		Specifies the maximum number of bytes that can be copied into the data buffer.
		///
		///   	\param[in]	actualByteCount		Actual number of bytes copied into the data buffer.
		/// 
		///	\return Returns an OLStatus.
		///   	\retval OL_SUCCESS			The stream was found and the operation was enqueued.
		///		\retval OL_INVALID_PARAM	The stream was not found.
		///
		virtual OLStatus LoadImageImmediate(const char *url, 
											unsigned char *data, 
											unsigned int maxByteCount, 
											unsigned int *actualByteCount) = 0;

		/// \addtogroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// \name Setting Streams
		/// 

		///
		/// \brief Updates the video and/or audio for a stream.
		/// \par Description:
		///   Checks the given stream ID for updates. If the service finds updates, invokes the VideoUpdate or
		///   AudioUpdate callbacks. The title can call this method whenever it gets a 
		///   StreamNeedsUpdate callback, or it can poll this method once per frame to check for updates.
		///
		///	\note Unlike other APIs you cannot call UpdateStream from within a callback.  You must call it on your own thread.
		///
		/// \responseEventList
		///		The OnLive Service Platform MAY respond with one of the following events if this request is successfully sent: 
		///
		///		\ospEvent OLMediaStreamEventHandler::VideoUpdate
		///			\eventDesc	 The VideoUpdate event returns a new frame of video.
		///
		///		\ospEvent OLMediaStreamEventHandler::AudioUpdate
		///			\eventDesc	 The AudioUpdate event returns a new packet of audio. 
		///
		///		\ospEvent OLMediaStreamEventHandler::FlushVideo
		///			\eventDesc	 The old video buffer should be flushed.
		///
		///		\ospEvent OLMediaStreamEventHandler::FlushAudio
		///			\eventDesc	 Old audio data should be flushed. 
		///
		///		\ospEvent OLMediaStreamEventHandler::FlushAudio
		///			\eventDesc	 Old audio data should be flushed. 
		///
		///		\ospEvent OLMediaStreamEventHandler::StreamLastVideoFrame
		///			\eventDesc	 The last video frame for the video has been sent.
		///						 If the video is set to loop, then the title will continue 
		///						 to recieve VideoUpdate and AudioUpdate events.
		///
		/// \responseEventListEnd
		///
		///   	\param[in]	streamId		A previously returned stream ID from \ref LoadStream or \ref olapi::OLMediaStream::LoadStreamImmediate.
		///
		///		\param[in]	handler			Pointer to OLMediaStreamEventHandler interface, or NULL to use the one 
		///									previously set with SetStreamEventHandler.
		/// 
		///	\return Returns an OLStatus.
		///   	\retval OL_SUCCESS			The stream was found and the operation was enqueued.
		///		\retval OL_INVALID_PARAM	The stream was not found.
		///
		virtual OLStatus UpdateStream(OLStreamId streamId, OLMediaStreamEventHandler* handler) = 0;


		/// \ingroup OLMEDIA_API_SETTING_STREAMS
		///
		/// \brief Establishes the requested presentation offsets for video and audio frames for all streams.
		///  \par Description:
		///   Each frame in a stream has a presentation offset, which is the time the frame should be
		///   visible or audible to the user. By default, a stream gets a video update at exactly the presentation
		///   time for the frame, and an audio update 20ms before the audio begins playing. The Media API allows the
		///   title to adjust these offsets. The offset can be positive, in which case the Title gives a frame to
		///   the title by the specified number of milliseconds before it is presented. It can be negative, wherin 
        ///   the frame is held for the specified period (in milliseconds). OnLive recommends that the title set these numbers
		///   before loading any streams for the most predictable results.
		///
		///   \note This setting has no effect on live streams.
		///
		///   \param[in]	videoOffset_ms is the number of milliseconds before presentation time that video frames  
		///					are given to the title (default 0).
		///
		///   \param[in]	audioOffset_ms is the number of milliseconds before presentation time that audio frames 
		///					are given to the title (default 20ms).
		///
		virtual void SetStreamDisplayOffsets(int videoOffset_ms, int audioOffset_ms) = 0;

		/// end addtogroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// end name Setting Streams
		/// 

		///ingroup OLMEDIA_API_SETTING_STREAMS
		/// \brief   A constant defining an invalid stream. This is used to signal errors.
		///
		static const OLStreamId INVALID_STREAM_ID = 0;
	};


	/// \ingroup OLMEDIA_API OLMEDIA_API_EVENT_HANDLING
	/// \class OLMediaEventHandler
	///
	/// \brief Handles events from the OnLive Service Platform (OSP).
	/// \par Description:
	/// Event handler for the OLMedia API.  Used to receive asynchronous events.	
	///
	class OLMediaEventHandler
	{
		public:


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		///
		/// \brief Provides the ID for a container.
		/// \par Description:
		/// Provides the ID for the data container requested by an olapi::OLMedia function.
		/// The container receives updates when using \ref olapi::OLMedia::MonitorBragClips and \ref olapi::OLMedia::MonitorSpectatingStreams.
		///
		///		\param[in]	context			Pairs asynchronous operations.
		///
		///		\param[in]	containerID		Identifies the container that has the requested data.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The event was handled; the container is valid until destroyed.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored (and the container will be destroyed immediately).
		///
		virtual OLStatus ContainerIDResult(OLContext context, OLID containerID) = 0;


		/// \ingroup OLMEDIDA_API_EVENT_HANDLING
        /// \brief Receive the ID or IDs requested.
		/// \par Description:
		/// 	- GetBragClips: Returns a set of container IDs with metadata for each matching Brag Clip.
		/// 	- GetSpectatingStreams: Returns a set of container IDs with metadata for each matching Spectating Stream.
		/// 	- GetMediaRatingsByUserForIDs: Returns a set of Container IDs with info for each requested Brag Clip rating by the specified user.
		/// 	- GetMediaIDs: Returns a set of media IDs, which can be passed to GetBragClips or GetSpectatingStreams respectively.
		/// 
		/// 	\param[in]	context				Used to pair up async operations.
		///
		/// 	\param[in]	idlist				Pointer to null terminated list of IDs.
		///
		/// 	\param[in]	containerlistid		Pointer to ID of the list holding the container IDs.
		///
		///	\return Returns an OLStatus code.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///
		virtual OLStatus IDListResult(OLContext context, OLIDList *idlist, OLID *containerlistid) = 0;


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
        ///  \brief Contains an OLStatus result code and context of a command that returned only a status value.
		///
		/// \par Description:
		/// Contains an OLStatus result code and context of a command that returned only a status value.
		///
		///	  	\param[in]	context		Used to pair up async operations.
		///
		///	  	\param[in]	status		An OLStatus result code for operation that matches context.
		///
		///	  	\param[in]	id			If available, the first ID passed in the request that caused the result.
		///
		///	\return An OLStatus code indicating if you have handled the event.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///
		virtual OLStatus StatusResult(OLContext context, OLStatus status, OLID id) = 0;
	};


	/// \ingroup OLMEDIA_API OLMEDIA_API_SETTING_STREAMS
	/// \class OLMediaStreamEventHandler
	///
	/// \brief Handles events from OnLive Service Platform (OSP).
	///
	/// \par Description:
	/// Event handler for the OLMediaStream API.  Used to receive asynchronous stream events.
	///
	class OLMediaStreamEventHandler
	{
		public:


		/// \addtogroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// \name Audio and Video Event Handler Functions
		/// 

		///
		/// \brief Notifies the title that a stream loaded.
		/// \par Description:
		///	The service calls this callback when a stream previously requested via LoadStream successfully 
		///	completed loading, and the title has not explicitly called UnloadStream() on it. If a 
		///	stream loaded in immediate mode, this method is not called.
		///
		///   \param[in]	streamId		A previously returned stream ID from OLMedia's LoadStream method.
		///
		///   \param[in]	url				The URL provided to the loadStream method that returned this stream ID.
		///
		///   \param[in]	streamIsLive 	Informs the title if the stream is from a live session or prerecorded.
		///
		///   \param[in]	duration_us 	Gives the duration of the stream in microseconds, if it is not live.
		///
		///   \param[in]	width 			Gives the width of the video stream in pixels.
		///
		///   \param[in]	height 			Gives the height of the video stream in pixels.
		///
		///   \param[in]	sampleRate		Gives the audio sample rate, if the stream has audio, or 0 if not.
		///
		///   \param[in]	channelCount	Gives the number of channels in the audio if the stream has audio, 0 if not.
		///
		///   \param[in]	bitsPerSample	Gives the number of bits in each audio sample if the stream has audio, 0 if not.
		///
		virtual void StreamLoaded(OLStreamId streamId, 
								  const char *url, 
								  bool streamIsLive, 
								  unsigned __int64 duration_us, 
								  unsigned int width, 
								  unsigned int height, 
								  unsigned int sampleRate, 
								  unsigned int channelCount, 
								  unsigned int bitsPerSample) = 0;


		/// 
		/// \brief A prior LoadStream call failed to complete.
		/// \par Description:
		///   The service calls this callback when a stream previously requested via LoadStream() failed to load,
		///   and the title has not explicitly called UnloadStream() on it. If a stream was loaded in immedate
		///   mode via LoadStreamImmediate(), this method is not called.
		///
		///   \param[in]	streamId		A previously returned stream ID from the OLMedia's LoadStream method.
		///
		///   \param[in]	url				The URL provided to the loadStream method that returned this stream ID.
		///
		///   \param[in]	error			A string describing the error as accurately as possible.
		///
		virtual void StreamLoadFailed(OLStreamId streamId, const char *url, const char *error) = 0;


		///
		/// \brief A stream encountered a playback error.
		/// \par Description:
		///		The service calls this callback when a previously loaded stream has encountered an error that prevents
		///		it from playing further. Such errors are considered fatal and OnLive recommends that the stream 
		///		unload at that point.
		/// 
		///		\param[in]	streamId		A previously returned stream ID from OLMediaStream::LoadStream or 
		///									OLMediaStream::LoadStreamImmediate methods.
		///
		///		\param[in]	url				The URL provided to the loadStream or loadStreamImmedate methods that returned 
		///									this stream ID.
		///
		///		\param[in]	error			A string describing the error as accurately as possible.
		///
		virtual void StreamEncounteredError(OLStreamId streamId, const char *url, const char *error) = 0;


		///
		/// \brief Notifies the title that a stream completed unloading.
		/// \par Description:
		///   The service calls this callback when a previously loaded stream finished unloading after a
		///   Media API unloadStream() request. When the title invokes this callback, the resources associated
		///   with the given stream ID have been fully released.
		///
		///  \param	[in] streamId A previously returned stream ID from OLMediaStream::LoadStream or OLMediaStream::LoadStreamImmediate.
		///
		virtual void StreamUnloaded(OLStreamId streamId) = 0;


		///
		/// \brief Notifies the title that the stream has reached its last frame.
		///	\par Description:
		///   This callback is called when a stream that has been played has delivered the last displayable 
		///   video frame in the stream. If the stream is set to looping via OLMedia's SetStreamLooping
		///   method, the stream will continue playback from the beginning. Otherwise, playback will pause.
		///   This callback is only invoked on the first loop of a looping stream.
		///
		///		\param[in]	 streamId A previously returned stream ID from OLMediaStream::LoadStream or OLMediaStream::LoadStreamImmediate.
		///
		virtual void StreamLastVideoFrame(OLStreamId streamId) = 0;

		/// end addtogroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// end name Audio and Video Event Handler Functions
		/// 

		/// \ingroup OLMEDIA_API_STILL_IMAGES
		///
		/// \brief A previously requested image finished loading.
		/// \par Description:
		///   The service invokes this callback when an image file, previously requested via the OLMediaStream::LoadImage
		///   method, is ready for use. 
		///
		///		Note: The bytes associated with imgfile *must* be copied into the title's local storage,
		///				as the memory will be freed or reused when the callback returns.
		/// 
		///		\param[in]	url			The URL provided to OLMedia's LoadImage method.
		///
		///		\param[in]	data		The set of compressed bytes found in the image file. This data is freed when the callback
		///								returns. The title must copy the data.
		///
		///		\param[in]	byteCount	The number of bytes in the data.
		/// 
		virtual void ImageLoaded(const char *url, const unsigned char *data, unsigned int byteCount) = 0;


		/// \ingroup OLMEDIA_API_STILL_IMAGES
		///
		/// \brief A previously requested image failed to load.
		/// \par Description:
		///   The service invokes this callback when an image previously requested via the OLMediaStream::LoadImage method
		///   failed to load.
		/// 
		///		\param[in]	url The URL provided to OLMedia's LoadImage method.
		///
		///		\param[in]	error A string describing the error as accurately as possible.
		/// 
		virtual void ImageLoadFailed(const char *url, const char *error) = 0;

		/// \addtogroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// \name Audio and Video Event Handler Functions
		/// 

		///
		/// \brief Notifies the title that it must update the video of a stream.
		/// \par Description:
		///   The service invokes this callback when a stream previously loaded by OLMediaStream::LoadStream or 
		///   OLMediaStream::LoadStreamImmediate, sized via the OLMediaStream::SetTextureSize() method, and played via the 
		///   OLMediaStream::PlayStream() method has a new video frame to display. The data is provided as YUV420
		///   data, with a line stride equal to the width of the texture specified by the title to 
		///   the OLMediaStream::SetTextureSize method. The YUV420 data size is equal to the texture width
		///   times the texture height times 3/2.
		///
		///   The service invokes this callback by the thread that invoked the OLMediaStream::HandleStreamEvent() method.
		/// 
		///		\param[in]	streamId		A previously returned stream ID from OLMediaStream::LoadStream or OLMediaStream::LoadStreamImmediate.
		///
		///		\param[in]	data 			The YUV data for this frame. It is reused or destroyed after this callback returns.
		///
		///		\param[in]	dataByteCount	Specifies the number of bytes in this update.
		///
		///		\param[in]	offset_us		Specifies the offset in microseconds from the beginning of the stream of this update.
		/// 
		virtual void VideoUpdate(OLStreamId streamId, 
								 unsigned char *data, 
								 unsigned int dataByteCount,
								 unsigned __int64 offset_us) = 0;


		///
		///
		/// \brief Notifies the title that audio is ready to be queued for playback.
		/// \par Description:
		///   The service invokes this callback when a stream previously loaded by OLMedia's LoadStream or
		///   LoadStreamImmediate methods, sized via the SetTextureSize() method, and played via the PlayStream()
		///   method has new audio samples ready to be played back immediately. The data is provided as PCM
		///   samples, 16 bits wide little endian if the audio bits per sample specifies 16 bit audio, and
		///   interleaved if the audio channel count specifies stereo audio.
		///   The service invokes this callback by the thread that invoked OLMedia's HandleStreamEvent() method.
		/// 
		///   \param[in]	streamId 		A previously returned stream ID from OLMediaStream::LoadStream or OLMediaStream::LoadStreamImmediate.
		///
		///   \param[in]	data			The PCM data for this frame. It is reused or destroyed after this callback returns.
		///
		///   \param[in]	dataByteCount	Specifies the number of bytes in this update.
		///
		///   \param[in]	offset_us		Specifies the offset in microseconds from the beginning of the stream of this update.
		/// 
		virtual void AudioUpdate(OLStreamId streamId, 
								 unsigned char *data, 
								 unsigned int dataByteCount,
								 unsigned __int64 offset_us) = 0;


		///
		/// \brief Tells the title to flush any video display it might have.
		/// \par Description:
		///   The title invokes this callback when the stream is about to start, or about to seek, and the 
		///   video's associated display should be cleared.
		/// 
		///		\param[in]	streamId	A previously returned stream ID from OLMediaStream::LoadStream or OLMediaStream::LoadStreamImmediate.
		/// 
		virtual void FlushVideo(OLStreamId streamId) = 0;


		///
		/// \brief Tells the title to flush any queued audio samples it might have.
		///\par Description:
		///   The title invokes this callback when the stream is about to start, or about to seek, and the 
		///   audio's associated playback buffers should be cleared.
		/// 
		///		\param[in]	streamId	A previously returned stream ID from OLMediaStream::LoadStream or OLMediaStream::LoadStreamImmediate.
		/// 
		virtual void FlushAudio(OLStreamId streamId) = 0;


	};
		/// end addtogroup OLMEDIA_API_SETTING_STREAMS
		/// 
		/// end name Audio and Video Event Handler Functions
		/// 


	/// \ingroup OLMEDIA_API OLMEDIA_API_EVENT_HANDLING
	/// \class OLMediaCallbacks
	///
	/// \brief Callback interface to receive asynchronous notification that events are waiting.
	/// \par Description:
	/// Notifies the title that an event is waiting to be handled. Handled with OLMedia::HandleEvent().
	///
	/// \note Your title must call either the \ref olapi::OLMediaStream::UnloadStream() or \ref olapi::OLMediaStream::UnloadStreamImmediate() function for all loaded streams 
	/// before exiting from outside an OLMediaStreamCallback.  
	///
	class OLMediaCallbacks
	{
		public:


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		///
		/// An event is waiting. Call OLMedia::HandleEvent() to receive the event.
		///
		///
		/// \par notes 
		/// - Callbacks must take less than 2 milliseconds.
		/// - Your title must call either the \ref olapi::OLMediaStream::UnloadStream() or \ref olapi::OLMediaStream::UnloadStreamImmediate() function for all loaded 
		/// streams before exiting from outside an OLMediaStreamCallback.  
		///		
		virtual void EventWaiting() = 0;	
	};

	/// \ingroup OLMEDIA_API OLMEDIA_API_EVENT_HANDLING 
	/// \class OLMediaStreamCallbacks
	/// \brief Callback interface to receive asynchronous notification that stream events are waiting.
	///\par Description:
	/// Notifies the title that an event is waiting to be handled. Handled with OLMediaStream::HandleStreamEvent().
	///
	class OLMediaStreamCallbacks
	{
		public:


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		///
		/// A stream event is waiting. Call OLMediaStream::HandleStreamEvent() to receive the event.
		///
		/// \note Your title must call either the \ref olapi::OLMediaStream::UnloadStream() or \ref olapi::OLMediaStream::UnloadStreamImmediate() function for all loaded 
		/// streams before exiting from outside an OLMediaStreamCallback.  
		/// 
		virtual void StreamEventWaiting() = 0;


		/// \ingroup OLMEDIA_API_EVENT_HANDLING
		/// 
		/// \brief A stream's video or audio data requires update. Call OLMediaStream::UpdateStream(id) to update the stream.
		/// \param id	The stream ID that needs update, previously returned from OLMediaStream::LoadStream or OLMediaStream::LoadStreamImmediate.
		///
		/// \note Your title must call either the \ref olapi::OLMediaStream::UnloadStream() or \ref olapi::OLMediaStream::UnloadStreamImmediate() function for all loaded 
		/// streams before exiting from outside an OLMediaStreamCallback.  
		///
		virtual void StreamNeedsUpdate(OLStreamId id) = 0;
	};

	/// \ingroup OLMEDIA_API_CREATE_AND_DESTROY
	/// \brief DLL Export: Starts the media API and retrieve the media API interface class pointer.
	/// \note The media service is a singleton, multiple calls to OLGetMedia will return the same pointer.
	///
	///		\param[in]	version			API version, use MEDIA_API_VERSION.
	///
	///		\param[out]	service			Pointer to an OLMedia class pointer.
	///
	///	\return Returns an OLStatus code.
	///		\retval	OL_SUCCESS						Success; the olContent pointer is set to the singleton.
	///		\retval OL_INVALID_PARAM				The supplied version string or olContent pointer is NULL.
	///		\retval OL_INVALID_VERSION				The API version requested is not supported. This is a fatal 
	///													error and your title should exit.
	///		\retval OL_INTERNAL_ERROR				A general internal or system error.
	///		\retval OL_OUT_OF_MEMORY				The SDK could not alloc memory for the singleton.
	///		\retval OL_SERVICES_ALREADY_STARTED		This API was initialized after olapi::OLStartServices() was called.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLGetMedia(const char* version, OLMedia** service);


	/// \ingroup OLMEDIA_API_CREATE_AND_DESTROY
	/// \brief DLL Export: Stops the media API.
	///
	///  
	///
	///		\param[in]	service			Pointer to an OLMedia class.
	///
	///	\return Returns an OLStatus code.
	///		\retval	OL_SUCCESS			Success; the olContent was destroyed.
	///		\retval OL_INVALID_PARAM	The supplied olContent pointer was NULL.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLStopMedia(OLMedia* service);


	/// \ingroup OLMEDIA_API_CREATE_AND_DESTROY
	/// \brief DLL Export: Starts the media stream API and retrieve the media stream API interface class pointer.
	/// \note The media stream API is *NOT* a singleton. You can instantiate multiple media stream services.
	///
	///		\param[in]	version			API version, use MEDIA_API_VERSION.
	///
	///		\param[out]	service			Pointer to an OLMediaStream class pointer.
	///
	///	\return Returns an OLStatus code.
	///		\retval	OL_SUCCESS			Success; the olContent pointer is set to the singleton.
	///		\retval OL_INVALID_PARAM	The supplied version string or olContent pointer is NULL.
	///		\retval OL_INVALID_VERSION	The API version requested is not supported. This is a fatal 
	///										error and your title should exit.
	///		\retval OL_INTERNAL_ERROR	A general internal or system error.
	///		\retval OL_OUT_OF_MEMORY	The SDK could not alloc memory for the singleton.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLGetMediaStream(const char* version, OLMediaStream** service);


	/// \ingroup OLMEDIA_API_CREATE_AND_DESTROY
	/// \brief DLL Export: Stops the media stream API.
	///
	///		\param[in]	service		Pointer to an OLMediaStream class.
	///
	///	\return Returns an OLStatus code.
	///		\retval	OL_SUCCESS			Success; the olContent was destroyed.
	///		\retval OL_INVALID_PARAM	The supplied olContent pointer was NULL.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLStopMediaStream(OLMediaStream* service);
/*!

\addtogroup OLMEDIA_API

	\brief The OLMedia interface allows your title's users to save and share video clips of their gameplay (Brag Clips) and 
	 watch the gameplay of other users (Spectating). While Spectating, users can cheer the player on or jeer to voice 
	 disapproval using the Cheer and Jeer icons; each time a Cheer or Jeer icon is selected, the points in the selected category
	 increase for the current gameplay of the user being watched.

	 Your title has access to information related to these features, such as a user's privacy settings, the number of times the Cheer or
	 Jeer icon has been selected for a user, and whether or not a particular user has viewed a specified Brag Clip. Users or titles can 
	 determine who is allowed to watch gameplay and which Brag Clips to save or remove.

	 The OLMediaStream interface provides your title with access to media streams for fixed assets,
	 such as images and trailers as well as user-generated assets, such as Brag Clips and Spectating streams.
	 The API manages video frames in YUV420 format and audio frames as interleaved, little-endian PCM
	 samples.

 \addtogroup OLMEDIA_API_CREATE_AND_DESTROY
	
	\brief The OLMedia singleton is created and retrieved with \ref olapi::OLGetMedia() and destroyed 
	with \ref olapi::OLStopMedia(). Do not destroy any of the OnLive APIs (or their handlers) until 
	after you have stopped the OnLive Services. 
		
	See \ref SDK_SESSION_FLOW_PAGE and \ref OLSERVICE_API_CONNECTION_DISCONNECTION.


\addtogroup OLMEDIA_API_EVENT_HANDLING
	
	\brief Callbacks and events are used for asynchronous communication to and from the OnLive Service Platform. 

	The classes and functions described below are used for registering and handling OLMedia callbacks and events. 

	Also, see \ref SDK_EVENTS_PAGE.


\addtogroup OLMEDIA_API_BRAGCLIP_HANDLING

	\brief Brag Clips are recorded gameplay videos that users can create for themselves. The BragClipContainer stores the Brag Clip metadata.


\addtogroup OLMEDIA_API_SPECTATING_HANDLING

	\brief The Spectating feature allows users to watch other users playing your title; as one user (spectator) watches another (player), the spectator can rate the 
	player's gameplay by selecting the Cheers or Jeers icons. Each time an icon is selected, a point is added to the appropriate category (Cheers or Jeers), and the 
	spectator may select each icon multiple times. 
	
	The SpectatingStreamContainer holds the metadata for the OLMedia's Spectating feature.

\addtogroup OLMEDIA_API_SETTING_STREAMS

	\brief The OLMedia sets your Brag Clip and Spectating audio and video streams.


\addtogroup OLMEDIA_API_STILL_IMAGES
	
	\brief The OLMedia API loads your icon image assets for your title. 


\addtogroup OLMEDIA_API_PRIVACY

	\brief The \ref olapi::OLKEY_RO_BragClipOwnerPrivacySetting and \ref olapi::OLKEY_RO_SpectatingStreamOwnerPrivacySetting container keys use the OLMediaOwnerPrivacyType
	and OLMediaStreamPrivacyStatus enums to relay the user's privacy settings to your title.

\addtogroup OLMEDIA_API_MEDIA_HANDLING

	\brief The OLMedia API identifies media types, sorts the Brag Clip and Spectating streams, and relays the information to your title.

*/


}; // namespace olapi

#endif // OLMEDIA_H
