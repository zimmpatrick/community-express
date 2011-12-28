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
// OLLeaderboard.h $Revision: 57033 $
//

/// \file OLLeaderboard.h

#ifndef OLLEADERBOARD_H
#define OLLEADERBOARD_H

#include <ol_api/OLAPI.h>

namespace olapi
{
#define OL_LB_MAX_ROW_QUERY_COUNT 100

	/*! \ingroup OLLEADERBOARD_API_INIT_DESTROY 
		\brief The default version of the OnLive OLLeaderboard API to use in \ref olapi::OLGetLeaderboard.
	*/
	#define LEADERBOARD_API_VERSION (OLAPI_VERSION)

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	/// \brief Key enumeration for accessing the LeaderboardDefinition container
	///
	enum LeaderboardDefinitionContainerKeys
	{
		OLKEY_RO_LB_DefContainerName,        ///< string, Name of Container
		OLKEY_RO_LB_DefContainerListID,      ///< Internal ID of Container List
		OLKEY_RO_LB_DefID,                   ///< OLID, The unique ID of this Leaderboard
		OLKEY_RO_LB_DefFlags,                ///< int, Flags associated with this leaderboard
		OLKEY_RO_LB_DefType,                 ///< LeaderboardType, Type of leaderboad
		OLKEY_RO_LB_DefName,                 ///< string, Short name of this board
		OLKEY_RO_LB_DefDescription,          ///< string, Text description of this board
		OLKEY_RO_LB_DefSortStatID,           ///< OLID, The stat that this leaderboard is sorted using
		OLKEY_RO_LB_DefDirection,            ///< LeaderboardSortDirType, which direction this leaderboard should be sorted
		OLKEY_RO_LB_DefIconURIList,          ///< A list of URL strings, Icons associated with this board.
		OLKEY_RO_LB_DefStatlist_IDList,      ///< OLIDList, List of the unique IDs for each stat, Stats will be listed in the order defined in the yaml
		OLKEY_RO_LB_DefKeysMax
	};

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	/// \brief Enumeration for flags given in OLKEY_RO_LB_DefFlags
	///
	enum LeaderboardFlags
	{
		OL_LB_Flags_None     = 0x0000,					///< No flags set
		OL_LB_Flags_Mask     = 0x0000,
	};

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	/// \brief Key enumeration for accessing the LeaderboardStatDefinition container
	///
	enum LeaderboardStatDefinitionContainerKeys

	{
		OLKEY_RO_LB_StatDefContainerName,     ///< Name of Container
		OLKEY_RO_LB_StatDefContainerListID,   ///< Internal ID of Container List
		OLKEY_RO_LB_StatDefID,                ///< OLID, The unique ID of this stat
		OLKEY_RO_LB_StatDefFlags,             ///< int, Flags associated with this stat
		OLKEY_RO_LB_StatDefName,              ///< string, Short name of this stat
		OLKEY_RO_LB_StatDefDescription,       ///< string, Text description of this stat
		OLKEY_RO_LB_StatDefIconURIList,       ///< A list of URI strings, Icons associated with this stat.
		OLKEY_RO_LB_StatDefStatType,        ///< LeaderboardStatType, The value type stored in this stat (specifies enum StatType).  
		OLKEY_RO_LB_StatDefKeysMax
	};

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	/// \brief Enumeration for flags given in OLKEY_RO_LB_StatDefFlags
	///
	enum LeaderboardStatFlags
	{
		OL_LB_StatFlags_None         = 0x0000,					///< No flags set
		OL_LB_StatFlags_Mask         = 0x0000,
	};

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_ROWS
	/// \brief Key enumeration for accessing the LeaderboardStatContainer 
	///
	enum LeaderboardStatContainerKeys
	{
		OLKEY_RO_LB_StatContainerName,     ///< Name of Container
		OLKEY_RO_LB_StatContainerListID,   ///< Internal ID of Container List
		OLKEY_RO_LB_StatID,				   ///< The OLID of the stat
		OLKEY_RO_LB_StatUserID,			   ///< The OLID of the user
		OLKEY_RO_LB_StatTitleID,		   ///< The OLID of the title
		OLKEY_RO_LB_StatType,			   ///< Type of the stat
		OLKEY_RO_LB_StatValue,			   ///< The value to set the stat to. Must be the same type as the stat.
		OLKEY_RO_LB_StatSetFlags,		   ///< LeaderboardSetStatFlags for this set operation
		OLKEY_RO_LB_StatKeysMax
	};

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_ROWS
	/// \brief Key enumeration for accessing the LeaderboardHeaderContainer 
	///
	enum LeaderboardHeaderContainer
	{
		OLKEY_RO_LB_HeaderContainerName,	 ///< Name of Container
		OLKEY_RO_LB_HeaderContainerListID,   ///< Internal ID of Container List
		OLKEY_RO_LB_HeaderLeaderboardID,	 ///< The OLID of the leaderboard
		OLKEY_RO_LB_HeaderTitleID,		     ///< The OLID of the title
		OLKEY_RO_LB_HeaderNumRows,		     ///< The number of rows in the leaderboard
		OLKEY_RO_LB_HeaderKeysMax
	};

	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	///\brief The name of the LeaderboardDefContainer container.
	#define LEADERBOARDDEFCONTAINERNAME					"LeaderboardDefContainer"

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	///\brief The name of the LeaderboardStatDefContainer container.
	#define LEADERBOARDSTATDEFCONTAINERNAME				"LeaderboardStatDefContainer"

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_ROWS
	/// \brief The name of the LeaderboardStatContainer container.
	///
	#define LEADERBOARDSTATCONTAINERNAME				"LeaderboardStatContainer"

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_ROWS
	/// \brief The name of the LeaderboardHeaderContainer container.
	///
	#define LEADERBOARDHEADERCONTAINERNAME				"LeaderboardHeaderContainer"

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	/// \brief Enumeration for specifying the type of leaderboard
	///
	enum LeaderboardType
	{
		OL_LB_Leaderboard = 0,       ///< A collection of stats sorted off the defined sort stat.
		OL_LB_Statboard,             ///< A collection of non sorted stats used for comparing users. **currently not supported**
		OL_LB_TypeMax
	};

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	/// \brief Enumeration for specifying the type of stat
	///
	enum LeaderboardStatType
	{
		OL_LB_StatType_Invalid = -1,          ///< An invalid stat type
		OL_LB_StatType_Integer = 0,           ///< A 64 bit integer.
		OL_LB_StatType_FloatingPoint,         ///< A double precision floating point value.
		OL_LB_StatType_Max
	};

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	/// \brief Enumeration for specifying how a leaderboard is sorted. 
	/// \par Description:
	/// This determines how the rank and index of rows are calculated.
	///
	enum LeaderboardSortType
	{
		OL_LB_SortType_Ascend = 0,      ///< Rows are returned sorted with low values first
		OL_LB_SortType_Descend,         ///< Rows are returned sorted with high values first
		OL_LB_SortType_Max
	};

	///
	/// \ingroup OLLEADERBOARD_RETRIEVE_ROWS
	/// \brief Enumeration for specifying how to modify a query a leaderboard before querying for rows
	///
	enum LeaderboardQueryFlags
	{
		OL_LB_QueryFlags_None					= 0x0000,			///< No modifier on the query
		OL_LB_QueryFlags_Filter_Friends			= 0x0001,			///< Only include friends in the query
		OL_LB_QueryFlags_Mask					= 0x0001,
	};

	///
	/// \ingroup OLLEADERBOARD_SET_STATISTIC
	/// \brief Enumeration for specifying how to modify a set stat call
	///
	enum LeaderboardSetStatFlags
	{
		OL_LB_SetStatFlags_None					= 0x0000,			///< No modifier on the set call
		OL_LB_SetStatFlags_Keep_Highest			= 0x0001,			///< Keep the higher of this new value and the previous value
		OL_LB_SetStatFlags_Keep_Lowest			= 0x0002,			///< Keep the lower of this new value and the previous value
		OL_LB_SetStatFlags_Mask					= 0x0003,
	};


	
	#ifndef NO_OLAPI_DLL_LINKAGE
	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	/// \brief Key strings in the LeaderboardDefContainer container.
	extern __declspec(dllimport) char *LeaderboardDefinitionContainerKeyNames[olapi::OLKEY_RO_LB_DefKeysMax];
	/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
	/// \brief Key strings in the LeaderboardStatDefContainer container.
	extern __declspec(dllimport) char *LeaderboardStatDefinitionContainerKeyNames[olapi::OLKEY_RO_LB_StatDefKeysMax];
	/// \ingroup OLLEADERBOARD_RETRIEVE_ROWS
	/// \brief Key strings in the LeaderboardStatContainer container.
	extern __declspec(dllimport) char *LeaderboardStatContainerKeyNames[olapi::OLKEY_RO_LB_StatKeysMax];	
	/// \ingroup OLLEADERBOARD_RETRIEVE_ROWS
	/// \brief Key strings in the LeaderboardHeaderContainer container.
	extern __declspec(dllimport) char *LeaderboardHeaderContainerKeyNames[olapi::OLKEY_RO_LB_HeaderKeysMax];	
	#endif

	class OLLeaderboardEventHandler;
	class OLLeaderboardCallbacks;

	///
	/// \ingroup OLLEADERBOARD_API
	/// \class OLLeaderboard
	/// \brief OLLeaderboard API
	///
	/// The leaderboard interface class
	///
	class OLLeaderboard : public OLEventBase
	{
		public:

		/// \addtogroup OLLEADERBOARD_EVENT_HANDLING
		/// 
		/// \name OLLeaderboard Callbacks & Event Handling
		/// 

		/// \brief Allows client to be notified that an event is available.
		/// \par Description:
		/// Sets up a callback class object to be notified when an event is waiting to be handled. Event
		/// notification is achieved by calling the callback class object's EventWaiting() function.\n\n
		/// \note The application can use this to optimize Event Handling.
		///
		///		\param[in]	callback		Pointer to OLAppEventWaitingCallback to call when an event is available.
		///
		///	\return Returns an OLStatus code.	
		///		\retval			OL_SUCCESS Normal operation.\n
		///		\retval			OL_INVALID_PARAM Invalid or NULL parameter callback.
		///
		virtual OLStatus SetEventWaitingCallback(OLLeaderboardCallbacks* callback) = 0;

		///
		/// \brief Sets handler to call when an application event is processed.
		/// \par Description:
		/// Handler used by application to process events.
		///
		///		\param[in]	handler			Pointer to OLLeaderboardEventHandler interface.
		///
		///	\return Returns an OLStatus code.		
		///		\retval			OL_SUCCESS Normal operation.\n
		///		\retval			OL_INVALID_PARAM Invalid or NULL parameter handler.
		///
		virtual OLStatus SetEventHandler(OLLeaderboardEventHandler* handler) = 0;

		///
		/// \brief Retrieves next event, if available.
		/// \par Description:
		/// Call upon event callback or per frame to handle (dispatch) the next event.
		/// The eventHandler must be previous setup with a SetEventHandler() call in each API.
		///
		///		\param[in]	time    Time to wait for next event in milliseconds or\n
		///						    OL_INFINITE to wait for next event.
		///
		///	\return Returns an OLStatus code.		
		///		\retval			OL_SUCCESS Normal operation.\n
		///		\retval			OL_INVALID_PARAM Invalid or NULL parameter handler and handler not already set. \n
		///		\retval			OL_EVENT_TIMEOUT If no events available.
		///
		virtual OLStatus HandleEvent(unsigned int time) = 0;
		/// end name OLLeaderboard Callbacks & Event Handling
		/// 
		/// end addtogroup OLLEADERBOARD_EVENT_HANDLING
		/// 

		///
		/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
		/// \brief Retrieves leaderboard definitions for a title ID
		/// \par Description:
		/// The leaderboard definitions are received as container lists; this will
		/// be an OLIDList of LeaderboardDefContainer IDs.
		///
		/// \responseEventList
		///		The result will be returned through one of the following events:
		///	
		///		\ospEvent OLLeaderboardEventHandler::IDListResult
		///			\eventDesc	Returns an ID List of the LeaderboardDefinition Containers.
		///
		///		\ospEvent OLLeaderboardEventHandler::ContainerIDResult
		///			\eventDesc	If only a single definition was requested, it is returned 
		///						through the ContainerIDResult callback.
		///
		///		\ospEvent OLLeaderboardEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR					Generic OSP failure.
		///				\statusResult OL_INVALID_PARAM					Invalid parameters in request.
		///				\statusResult OL_BUSY							OSP cannot process the request.
		///
		/// \responseEventListEnd
		///
		/// \param[in]  context    Used to pair up asynchronous requests and messages.
		///
		/// \param[in]  titleID    (optional) The title ID to retrieve leaderboard
		///                      definitions for.  When called from a title,
		///                      this must be this title's Title ID.
		///                      If OLID(NULL) is provided, the current title will be assumed.
		///
		/// \param[in]  boardID    (optional) The board ID to retrieve definitions for. 
		///                      If all leaderboard definitions are desired for a title, 
		///                      then use OLID(NULL)
		///
		/// \return An OLStatus code
		///		\retval OL_SUCCESS Normal operation.
		///		\retval OL_INVALID_ID Invalid title ID.
		///		\retval OL_INTERNAL_IO_ERROR Communication with OSP could not complete; this is a fatal error.
		///
		virtual OLStatus GetLeaderboardDefs( OLContext context, OLID titleID, OLID boardID ) = 0;

		///
		/// \ingroup OLLEADERBOARD_RETRIEVE_DEFINITIONS
		/// \brief Retrieves stat definitions for a title ID
		/// \par Description:
		/// The stat definitions are received as container lists; this will
		/// be an OLIDList of LeaderboardStatDefContainer IDs.
		///
		/// \responseEventList
		///		The result will be returned through one of the following events:
		///	
		///		\ospEvent OLLeaderboardEventHandler::IDListResult
		///			\eventDesc	Returns an ID List of the LeaderboadrStatDefinition Containers.
		///
		///		\ospEvent OLLeaderboardEventHandler::ContainerIDResult
		///			\eventDesc	If only a single definition was requested, it is returned 
		///						through the ContainerIDResult callback.
		///
		///		\ospEvent OLLeaderboardEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR					Generic OSP failure.
		///				\statusResult OL_INVALID_PARAM					Invalid parameters in request.
		///				\statusResult OL_BUSY							OSP cannot process the request.
		///
		/// \responseEventListEnd
		///
		///  \param[in]  context    Used to pair up asynchronous requests and messages.
		///
		///  \param[in]  titleID    (optional) The title ID to retrieve stat
		///                      definitions for.  When called from a title,
		///                      this must be this title's Title ID.
		///                      If OLID(NULL) is provided, the current title will be assumed.
		///
		///  \param[in]  statID     (optional) The stat ID to retrieve definitions for. 
		///                      If all stat definitions are desired for a title, 
		///                      then use OLID(NULL)
		///
		/// \return An OLStatus code
		///		\retval OL_SUCCESS Normal operation.
		///		\retval OL_INVALID_ID Invalid title ID.
		///		\retval OL_INTERNAL_IO_ERROR Communication with OSP could not complete; this is a fatal error.
		///
		virtual OLStatus GetLeaderboardStatDefs( OLContext context, OLID titleID, OLID statID) = 0;

		///        
		/// \ingroup OLLEADERBOARD_RETRIEVE_ROWS
		/// \brief  Retrieves a specified set of rows for a given leaderboard
		/// \par Description:
		/// Each leaderboard row is received as container lists; this will
		/// be an OLIDList of LeaderboardStatContainer IDs.
		///
		/// \responseEventList
		///		The result will be returned through one of the following events:
		///	
		///		\ospEvent OLLeaderboardEventHandler::RowRetrieved
		///			\eventDesc	The RowRetrieved call back will be called for each row that results from the query.
		///
		///		\ospEvent OLLeaderboardEventHandler::ContainerIDResult
		///			\eventDesc	On success, this will be called with a LeaderboardHeaderContainer 
		///						before the rows are returned.
		///
		///		\ospEvent OLLeaderboardEventHandler::StatusResult
		///			\eventDesc	The StatusResult() event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR					Generic OSP failure.
		///				\statusResult OL_INVALID_PARAM					Invalid parameters in request.
		///				\statusResult OL_BUSY							OSP cannot process the request.
		///
		/// \responseEventListEnd
		///
		///  \param[in]  context         Used to pair up asynchronous requests
		///                          and messages.
		///
		///  \param[in]  titleID         (optional) The title ID to get leaderboard rows for.
		///                          When called from a title, this must be this title's Title ID.
		///					    	 If OLID(NULL) is provided, the current title will be assumed.
		///
		///  \param[in]  boardID		 The leaderboard ID to get rows for.
		///
		///  \param[in]  startRow        The index of the first row to retrieve (indices start at 0).
		///
		///  \param[in]  endRow          The index of the last row to retrieve (indices start at 0).
		///
		///  \param[in]  flags		     (optional) Flags modifying the query.  
		///	
		///  \param[in]  userList		 Not currently in use.
		///
		/// \return An OLStatus code
		///		\retval OL_SUCCESS Normal operation.
		///		\retval OL_INVALID_ID Invalid title ID.
		///		\retval OL_INTERNAL_IO_ERROR Communication with OSP could not complete; this is a fatal error.
		///
		virtual OLStatus GetLeaderboardRowsByIndex(OLContext context, 
												   OLID titleID, 
												   OLID boardID,
												   unsigned int startRow, 
												   unsigned int endRow, 
												   LeaderboardQueryFlags flags, 
												   OLID * userList) = 0;
	 
		///    
		/// \ingroup OLLEADERBOARD_RETRIEVE_ROWS
		/// \brief  Retrieves a specified set of rows for a given leaderboard.
		/// \par Description:
		/// Each leaderboard row is received as container lists; this will
		/// be an OLIDList of LeaderboardStatContainer IDs.
		///
		/// \responseEventList
		///		The result will be returned through one of the following events:
		///	
		///		\ospEvent OLLeaderboardEventHandler::RowRetrieved
		///			\eventDesc	The RowRetrieved call back will be called for each row that results from the query.
		///
		///		\ospEvent OLLeaderboardEventHandler::ContainerIDResult
		///			\eventDesc On success, this will be called with a LeaderboardHeaderContainer 
		///					   before the rows are returned.
		///
 		///		\ospEvent OLLeaderboardEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR					Generic OSP failure.
		///				\statusResult OL_INVALID_PARAM					Invalid parameters in request.
		///				\statusResult OL_BUSY							OSP cannot process the request.
		///
		/// \responseEventListEnd
		///
		///  \param[in]  context         Used to pair up asynchronous requests
		///                          and messages.
		///
		///  \param[in]  titleID         (optional) The title ID to get leaderboard rows for.
		///                          When called from a title,
		///                          this must be this title's Title ID.
		///						     If OLID(NULL) is provided, the current title will be assumed.
		///
		///  \param[in]  boardID		 The leaderboard ID to get rows for.
		///
		///  \param[in]  referenceUser   (optional) Reference user to get data relative to.
		///						     If OLID(NULL) is provided the current user will be assumed.
		///
		///  \param[in]  rowsAbove       The number of rows to retrieve above the referenceIndex.
		///
		///  \param[in]  rowsBelow       The number of rows to retrieve below the referenceIndex.
		///
		///  \param[in]  flags		     (optional) Flags modifying the query.  
		///	
		///  \param[in]  userList		 Not currently in use.
		///
		/// \return An OLStatus code
		///		\retval OL_SUCCESS Normal operation.
		///		\retval OL_INVALID_ID Invalid title ID.
		///		\retval OL_INTERNAL_IO_ERROR Communication with OSP could not complete; this is a fatal error.
		///
		virtual OLStatus GetLeaderboardRowsByUser( OLContext context, 
												   OLID titleID, 
												   OLID boardID, 
												   OLID referenceUser,
												   unsigned int rowsAbove, 
												   unsigned int rowsBelow, 
												   LeaderboardQueryFlags flags,
												   OLID * userList) = 0;

		///    
		/// \ingroup OLLEADERBOARD_GET_POSITION
		/// \brief   Gets the numerical position of the user on a given filtered leaderboard
		///
		/// \responseEventList
		///		The result will be returned through one of the following events:
		///	
		///		\ospEvent OLLeaderboardEventHandler::RowRetrieved
		///			\eventDesc	The RowRetrieved call back will be called for the row for the 
		///						requested user in the requested leaderboard.
		///
		///		\ospEvent OLLeaderboardEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR					Generic OSP failure.
		///				\statusResult OL_INVALID_PARAM					Invalid parameters in request.
		///				\statusResult OL_BUSY							OSP cannot process the request.
		///
		/// \responseEventListEnd
		///
		///  \param[in]  context    Used to pair up asynchronous requests
		///                      and messages.
		///
		///  \param[in]  titleID    (optional) The title ID to get rank for.
		///                      When called from a title,
		///                      this must be this title's Title ID.
		///						 If OLID(NULL) is provided, the current title will be assumed.
		///
		///  \param[in]  boardID     The leaderboard ID to get rank for.
		///
		///  \param[in]  userID     (optional)  The user ID to get the rank for.
		///						 If OLID(NULL) is provided the current user will be assumed.
		///
		/// \return An OLStatus code
		///		\retval OL_SUCCESS Normal operation.
		///		\retval OL_INVALID_ID Invalid title ID.
		///		\retval OL_INTERNAL_IO_ERROR Communication with OSP could not complete; this is a fatal error.
		///
		virtual OLStatus GetLeaderboardRowForUser( OLContext context, OLID titleID, OLID boardID, OLID userID ) = 0;

		///    
		/// \ingroup OLLEADERBOARD_GET_STATISTIC
		/// \brief   Gets the current value of a specified stat for a specified user.
		/// \par Description:
		/// The given user's score is retrieved for a stat ID.
		///
		/// \responseEventList
		///		The result will be returned through one of the following events:
		///	
		///		\ospEvent OLLeaderboardEventHandler::ContainerIDResult
		///			\eventDesc	On success, the stat will be returned as a LeaderboardStatContainer.
		///
		///		\ospEvent OLLeaderboardEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following errors:
		///
		///				\statusResult OL_INTERNAL_ERROR					Generic OSP failure.
		///				\statusResult OL_INVALID_PARAM					Invalid parameters in request.
		///				\statusResult OL_BUSY							OSP cannot process the request.
		///
		/// \responseEventListEnd
		///
		///  \param[in]  context    Used to pair up asynchronous requests
		///                      and messages.
		///
		///  \param[in]  userID     (optional) The user ID to get the value for.
		///						 If OLID(NULL) is provided the current user will be assumed.
		///
		///  \param[in]  titleID    (optional) The title ID to get value for.
		///                      When called from a title,
		///                      this must be this title's Title ID.
		///						 If OLID(NULL) is provided, the current title will be assumed.
		///
		///  \param[in]  statID      The stat ID to get value for.
		///
		/// \return An OLStatus code
		///		\retval OL_SUCCESS Normal operation.
		///		\retval OL_INVALID_ID Invalid title ID.
		///		\retval OL_INTERNAL_IO_ERROR Communication with OSP could not complete; this is a fatal error.
		///
		virtual OLStatus GetLeaderboardStatistic( OLContext context, OLID titleID, OLID userID, OLID statID ) = 0;

		///    
		/// \ingroup OLLEADERBOARD_SET_STATISTIC
		/// \brief   Sets a new stat value for the current user
		/// \par Description
		/// The given user's score is set for a leaderboard ID.
		///
		/// \responseEventList
		///		The result will be returned through one of the following events:
		///	
		///		\ospEvent OLLeaderboardEventHandler::StatusResult
		///			\eventDesc	 The StatusResult() event indicates one of the following results:
		///
		///				\statusResult OL_SUCCESS						Request succeeded.
		///				\statusResult OL_INTERNAL_ERROR					Generic OSP failure.
		///				\statusResult OL_INVALID_PARAM					Invalid parameters in request.
		///				\statusResult OL_BUSY							OSP cannot process the request.
		///
		/// \responseEventListEnd
		///
		/// \param[in]  context			  Used to pair up asynchronous requests
		///                      and messages.
		///
		/// \param[in]  statContainerID   The Id for the container containing the update.
		///                     When called from a title the update must be for this title.
		///						The container must have set OLKEY_RO_LB_StatID, 
		///						OLKEY_RO_LB_StatType, and OLKEY_RO_LB_StatValue.
		///						If OLKEY_RO_LB_StatUserID and/or OLKEY_RO_LB_StatTitleID are 
		///						not set then the current user/title will be used.
		///
		/// \return An OLStatus code
		///		\retval OL_SUCCESS Normal operation.
		///		\retval OL_INVALID_ID Invalid title ID.
		///		\retval OL_INTERNAL_IO_ERROR Communication with OSP could not complete; this is a fatal error.
		///
		virtual OLStatus SetLeaderboardStatistic( OLContext context, OLID statContainerID ) = 0;

	};


	///
	/// \ingroup OLLEADERBOARD_API OLLEADERBOARD_EVENT_HANDLING
	/// \class OLLeaderboardEventHandler
	/// \brief This is the OLLeaderboard API's Event interface; your OLLeaderboard event handler must implement this interface.
	///		Each method represents an asynchronous event returned by the SDK.
	///
	/// \see \ref EVENTS_PAGE_CONCEPTS 
	///
	class OLLeaderboardEventHandler
	{
		public:

		///
		/// \ingroup OLLEADERBOARD_API_EVENT_HANDLING
		/// \brief Event returning an OLStatus code result.
		///
		/// \see \ref EVENTS_PAGE_ASYNC_COMM.
		///
		///		\param[in] context		The matching context you gave to the request for this result (see \ref EVENTS_PAGE_ASYNC_COMM).
		///
		///		\param[in] status		An OLStatus code indicating the result of your request.
		///
		///	\return An OLStatus code indicating if you have handled the event.
		///		\retval OL_SUCCESS				The event was handled.
		///		\retval OL_NOT_IMPLEMENTED		The event was ignored.
		///
		virtual OLStatus StatusResult(OLContext context, OLStatus status) = 0;

		///
		/// \ingroup OLLEADERBOARD_API_EVENT_HANDLING
		/// \brief Event returning a containerID result; indicates that a request Successfully resulted
		///		in a container.
		///
		/// \note If you return OL_SUCCESS, the container will have its normal
		///		life cycle (which varies by container type).  Any other return value causes
		///		the SDK to destroy the container immediately after this method returns.
		///
		/// \see \ref SDK_CONTAINERS_PAGE 
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
		/// \ingroup OLLEADERBOARD_API_EVENT_HANDLING
		/// \brief Contains a null terminated list of IDs the application requested using an OLLeaderboard function
		/// call.
		/// \par Description:
		/// Provides a null terminated list of IDs the application requested using an OLLeaderboard function call
		/// such as \ref olapi::OLLeaderboard::GetLeaderboardDefs().
		///
		///		\param[in] context			  Matches the API call with the corresponding
		///								OLAppSession::HandleEvent. Pass a unique value with each call.
		///								This operation is asynchronous.
		///
		///		\param[in]	idlist			  Pointer to null terminated list of IDs.
		///
		///		\param[in]	containerlistid	  ID of the list holding the container IDs.
		///
		///		\return	OLStatus
		///			\retval OL_SUCCESS		    Event handled. \n
		///			\retval OL_INVALID_PARAM    Event not handled due to invalid parameter. \n
		///			\retval OL_NOT_IMPLEMENTED  Event not handled. \n
		///			\retval OL_FAIL				Event not handled, event unexpected, invalid context, or other.
		///
		///		
		///			  OL_SUCCESS is returned then it is the responsibility of the title to destroy the 
		///			  container or let the container be cleaned up automatically be cleaned up when exiting.
		///
		virtual OLStatus IDListResult(OLContext context, OLIDList *idlist, OLID *containerlistid) = 0;

		///
		/// \ingroup OLLEADERBOARD_RETRIEVE_ROWS
		/// \brief Contains a null terminated list of IDs the application requested using an OLLeaderboard function
		/// call.
		/// \par Description:
		/// Provides a null terminated list of statContainerIDs that make up one set of rows the application requested
		/// using an OLLeaderboard function call such as \ref olapi::OLLeaderboard::GetLeaderboardRowsByUser.
		///
		///		\param[in] context					Context passed to original request.
		///
		///		\param[in] boardID					The OLID of this row's leaderboard.
		///
		///		\param[in] userID					The user's OLID for this row.
		///
		///		\param[in] tag						Pointer to a buffer holding this row's user's tag. This buffer will be freed after this call returns.
		///
		///		\param[in] rowRank					The rank of this row in the full leaderboard (ranks start at 1 and may be shared).
		///
		///		\param[in] rowIndex					The index of this row in the full leaderboard (indices start at 0 and will be unique).
		///
		///		\param[in] statContainerIDList		Pointer to null terminated list of stat container IDs.
		///											Note: If a stat in the leaderboard row has not set by the title
		///											      and no default value had been provided then an empty container
		///											      will be provided in its place.
		///
		///		\return	OLStatus		
		///			\retval OL_SUCCESS			Event handled successfully.
		///			\retval OL_INVALID_PARAM	Event not handled due to invalid parameter.
		///			\retval OL_NOT_IMPLEMENTED	Event not handled.
		///			\retval OL_FAIL				Event not handled, event unexpected, invalid context, or other.
		///
		///	
		///			Copy any needed data out of the containers and tag before returning.
		///
		virtual OLStatus RowRetrieved(OLContext context, 
									  OLID boardID, 
									  OLID userID, 
									  const char * tag, 
									  unsigned int rowRank, 
									  unsigned int rowIndex, 
									  OLIDList *statContainerIDList) = 0;
	};

	///
	/// \ingroup OLLEADERBOARD_API OLLEADERBOARD_EVENT_HANDLING
	/// \class OLLeaderboardCallbacks
	/// \brief This is the OLLeaderboard API's callback interface; your title's OLLeaderboard callback handler must implement this interface.
	///
	/// Each method in this class represents an OLLeaderboard callback.  If you choose to handle OLLeaderboard callbacks,
	/// register your callback handler using \ref OLLeaderboard::SetEventWaitingCallback.  Callback methods
	/// may be invoked at any time once the OnLive Services are running (see \ref OLSERVICE_API_CONNECTION_DISCONNECTION).
	///
	/// \note Callback methods are always invoked from an internal OnLive SDK thread, so all callback implementations must be thread-safe.
	///
	/// \see \ref EVENTS_PAGE_CONCEPTS
	///
	class OLLeaderboardCallbacks
	{
		public:

		///
		/// \brief Callback dispatched when an OLLeaderboard event is waiting to be handled.
		/// \par Description:
		/// If your title handles events from callbacks, call \ref OLLeaderboard::HandleEvent(unsigned int) to dispatch the waiting event.
		///	If you handle events by polling, you can ignore this event.
		///
		/// \note The EventWaiting() callback must be thread-safe and must complete in less than 2 milliseconds.
		///
		/// \see \ref EVENTS_PAGE_HANDLING_OVERVIEW for more info.
		///
		virtual void EventWaiting() = 0;	
	};

	///
	/// \ingroup OLLEADERBOARD_API_INIT_DESTROY
	/// \brief Gets the OLLeaderboard API singleton (the instance is created if necessary).
	///
	/// \par Description:
	///	The OLLeaderboard pointer can be stored until the API is destroyed with \ref olapi::OLStopLeaderboard().
	///
	/// \see \ref OLLEADERBOARD_API_INIT_DESTROY
	///
	///		\param[in] version		The API version string (for class compatibility); use \ref LEADERBOARD_API_VERSION.
	///
	///		\param[out] olLeaderboard	The supplied pointer is set to the OLLeaderboard instance on success.
	///									The pointer is not modified on failure.
	///
	///	\return An OLStatus code.
	///		\retval	OL_SUCCESS			Success; the olLeaderboard pointer is set to the singleton.
	///		\retval OL_INVALID_PARAM	The supplied version string or olLeaderboard pointer is NULL.
	///		\retval OL_INVALID_VERSION	The API version requested is not supported. This is a fatal 
	///										error and your title should exit.
	///		\retval OL_INTERNAL_ERROR	An general internal or system error.
	///		\retval OL_OUT_OF_MEMORY	The SDK could not alloc memory for the singleton.
	///		\retval	OL_SERVICES_ALREADY_STARTED
	///									This API was initialized after \ref olapi::OLStartServices() was called.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLGetLeaderboard(const char* version, OLLeaderboard** olLeaderboard);

	///
	/// \ingroup OLLEADERBOARD_API_INIT_DESTROY
	/// \brief Destroys the OLLeaderboard API singleton.
	/// \par Description:
	/// The OnLive Services should be stopped before you destroy an API instance.
	///
	/// \see 
	/// - \ref OLLEADERBOARD_API_INIT_DESTROY
	/// - \ref olapi::OLStopServices()
	///
	///		\param[in] olLeaderboard		Pointer to the OLLeaderboard instance to destroy.
	///
	///	\return Returns an OLStatus code.
	///		\retval	OL_SUCCESS			Success; the olLeaderboard was destroyed.
	///		\retval OL_INVALID_PARAM	The supplied olLeaderboard pointer was NULL.
	///
	extern "C" __declspec(dllexport) OLStatus __cdecl OLStopLeaderboard(OLLeaderboard* olLeaderboard);

/// doxygen group comments
/*!

	\addtogroup OLLEADERBOARD_API
		\brief The OLLeaderboard API gives you access to the OnLive leaderboard and stat tracking system.

	\addtogroup OLLEADERBOARD_API_INIT_DESTROY
		\brief The OLLeaderboard singleton is created and retrieved with \ref olapi::OLGetLeaderboard() and destroyed with 
		\ref olapi::OLStopLeaderboard().Do not destroy any of the OnLive APIs (or their handlers) until after you have stopped 
		the OnLive Services. 

		\see 	
			- \ref SDK_SESSION_FLOW_PAGE
			- \ref OLSERVICE_API_CONNECTION_DISCONNECTION 


	\addtogroup OLLEADERBOARD_EVENT_HANDLING
		\brief Callbacks and events are used for asynchronous communication to and from the OnLive Service Platform; this page lists and 
		describes the items related to processing OLLeaderboard callbacks and events.

		\see \ref SDK_EVENTS_PAGE 

	\addtogroup OLLEADERBOARD_RETRIEVE_DEFINITIONS 
		\brief Items related to retrieving OnLive Leaderboard and Stat definitions.
	\addtogroup OLLEADERBOARD_RETRIEVE_ROWS
		\brief Items related to retrieving rows from the OnLive Leaderboard system.
	\addtogroup OLLEADERBOARD_GET_POSITION 
		\brief Items related to retrieving the current position of a user.
	\addtogroup OLLEADERBOARD_GET_STATISTIC 
		\brief Items related to retrieving the current value of a specified stat in the OnLive Leaderboard system.
	\addtogroup OLLEADERBOARD_SET_STATISTIC 
		\brief Items related to updating the value of a stat in the Leaderboard system.


*/

}; // namespace olapi

#endif // OLLEADERBOARD_H
