using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CommunityExpressNS
{
    using UGCQueryHandle_t = UInt64;
    using UGCUpdateHandle_t = UInt64;

    using SteamAPICall_t = UInt64;
    using PublishedFileId_t = UInt64;
    using UGCHandle_t = UInt64;
    using PublishedFileUpdateHandle_t = UInt64;
    using AppId_t = UInt32;
    using AccountID_t = UInt32;
    
   

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStorageEnumerateUserSubscribedFilesResult_t
    {
        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 14;

        internal EResult m_eResult;				// The result of the operation.
        internal Int32 m_nResultsReturned;
        internal Int32 m_nTotalResultCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        internal PublishedFileId_t[] m_rgPublishedFileId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        internal UInt32[] m_rgRTimeSubscribed;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStorageEnumerateUserSharedWorkshopFilesResult_t
    {
        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 26;

        EResult m_eResult;				// The result of the operation.
        Int32 m_nResultsReturned;
        Int32 m_nTotalResultCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        PublishedFileId_t[] m_rgPublishedFileId;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStorageEnumerateUserPublishedFilesResult_t
    {
        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 12;

        internal EResult m_eResult;				// The result of the operation.
        internal Int32 m_nResultsReturned;
        internal Int32 m_nTotalResultCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        internal PublishedFileId_t[] m_rgPublishedFileId;
    };
    
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStorageEnumerateWorkshopFilesResult_t
    {
	    internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 19;
	    internal EResult m_eResult;
	    internal Int32 m_nResultsReturned;
	    internal Int32 m_nTotalResultCount;

	    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        internal PublishedFileId_t[] m_rgPublishedFileId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        internal float[] m_rgScore;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStoragePublishFileProgress_t
    {
        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 29;

        internal double m_dPercentFile;
        internal bool m_bPreview;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStorageSubscribePublishedFileResult_t
    {
        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 13;
        internal EResult m_eResult;				// The result of the operation.
        internal PublishedFileId_t m_nPublishedFileId;
    };
    
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStorageUnsubscribePublishedFileResult_t
    {
	    internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 15;
        internal EResult m_eResult;				// The result of the operation.
        internal PublishedFileId_t m_nPublishedFileId;
    };


    //-----------------------------------------------------------------------------
    // Purpose: User subscribed to a file for the app (from within the app or on the web)
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStoragePublishedFileSubscribed_t
    {
	    internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 21;
	    internal PublishedFileId_t m_nPublishedFileId;	// The published file id
	    internal AppId_t m_nAppID;						// ID of the app that will consume this file.
    };

    //-----------------------------------------------------------------------------
    // Purpose: User unsubscribed from a file for the app (from within the app or on the web)
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStoragePublishedFileUnsubscribed_t
    {
	    internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 22;
	    internal PublishedFileId_t m_nPublishedFileId;	// The published file id
	    internal AppId_t m_nAppID;						// ID of the app that will consume this file.
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Ansi)]
    internal struct RemoteStorageGetPublishedFileDetailsResult_t
    {
        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 18;
        internal EResult m_eResult;				// The result of the operation.
        internal PublishedFileId_t m_nPublishedFileId;
        internal AppId_t m_nCreatorAppID;		// ID of the app that created this file.
        internal AppId_t m_nConsumerAppID;		// ID of the app that will consume this file.

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
        internal string m_rgchTitle;		// title of document

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        internal string m_rgchDescription;	// description of document

        internal UGCHandle_t m_hFile;			// The handle of the primary file
        internal UGCHandle_t m_hPreviewFile;		// The handle of the preview file
        internal UInt64 m_ulSteamIDOwner;		// Steam ID of the user who created this content.
        internal UInt32 m_rtimeCreated;			// time when the published file was created
        internal UInt32 m_rtimeUpdated;			// time when the published file was last updated
        internal UserGeneratedContent.ERemoteStoragePublishedFileVisibility m_eVisibility;
        [MarshalAs(UnmanagedType.I1)]
        internal bool m_bBanned;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1025)]
        internal string m_rgchTags;	// comma separated list of all tags associated with this file
        [MarshalAs(UnmanagedType.I1)]
        internal bool m_bTagsTruncated;			// whether the list of tags was too long to be returned in the provided buffer

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string m_pchFileName;		// The name of the primary file
        internal Int32 m_nFileSize;				// Size of the primary file
        internal Int32 m_nPreviewFileSize;		// Size of the preview file

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        internal string m_rgchURL;	// URL (for a video or a website)
        internal UserGeneratedContent.EWorkshopFileType m_eFileType;	// Type of the file
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Ansi)]
    struct RemoteStorageUpdatePublishedFileResult_t
    {
        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 16;
        internal EResult m_eResult;				// The result of the operation.
        internal PublishedFileId_t m_nPublishedFileId;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Ansi)]
    internal struct RemoteStorageDownloadUGCResult_t
    {
        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 17;

        internal EResult m_eResult;				// The result of the operation.
        internal UGCHandle_t m_hFile;			// The handle to the file that was attempted to be downloaded.
        internal AppId_t m_nAppID;				// ID of the app that created this file.
        internal Int32 m_nSizeInBytes;			// The size of the file that was downloaded, in bytes.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string m_pchFileName;		// The name of the file that was downloaded. 
        internal UInt64 m_ulSteamIDOwner;		// Steam ID of the user who created this content.
    };
    
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStoragePublishFileResult_t
    {
        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 9;

	    internal EResult m_eResult;				// The result of the operation.
	    internal PublishedFileId_t m_nPublishedFileId;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct RemoteStorageDeletePublishedFileResult_t
    {
            internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 11;

            internal EResult m_eResult;				// The result of the operation.
            internal PublishedFileId_t m_nPublishedFileId;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct SteamParamStringArray_t
    {
        public SteamParamStringArray_t(ICollection<string> args)
        {
            List<byte[]> bindata = new List<byte[]>();
            int length = 0;
            if (args != null)
            {
                foreach (string arg in args)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(arg);
                    bindata.Add(bytes);

                    length += (bytes.Length + 1); // +1 for each null
                }

                m_nNumStrings = (UInt32)args.Count;
            }
            else
            {
                m_nNumStrings = 0;
            }

            m_ppStrings = Marshal.AllocHGlobal(length + IntPtr.Size * (int)m_nNumStrings);

            int pointerOffset = 0;
            int stringOffset = IntPtr.Size * (int)m_nNumStrings;
            foreach (byte[] bytes in bindata)
            {
                IntPtr stringPtr = new IntPtr(m_ppStrings.ToInt64() + stringOffset);
                IntPtr stringPtrNull = new IntPtr(stringPtr.ToInt64() + bytes.Length);

                Marshal.Copy(bytes, 0, stringPtr, bytes.Length);
                Marshal.WriteByte(stringPtrNull, 0);

                Marshal.WriteIntPtr(m_ppStrings, pointerOffset, stringPtr);
                pointerOffset += IntPtr.Size;
                stringOffset += (bytes.Length + 1);
            }
        }

        internal IntPtr m_ppStrings;
        internal UInt32 m_nNumStrings;

        public void Free()
        {
            if (m_ppStrings != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_ppStrings);
                m_ppStrings = IntPtr.Zero;
            }

            m_nNumStrings = 0;
        }
    };

    /// <summary>
    /// User generated content information and uploading
    /// </summary>
    public class UserGeneratedContent
    {
        /// <summary>
        /// Type of workshop file
        /// </summary>
        public enum EWorkshopFileType
        {
            // k_EWorkshopFileTypeFirst = 0,

            /// Community file
            k_EWorkshopFileTypeCommunity = 0,
            /// Microtransaction file
            k_EWorkshopFileTypeMicrotransaction = 1,
            /// Collection of files
            k_EWorkshopFileTypeCollection = 2,
            /// Art file
            k_EWorkshopFileTypeArt = 3,
            /// Video file
            k_EWorkshopFileTypeVideo = 4,
            /// Screenshot file
            k_EWorkshopFileTypeScreenshot = 5,
            /// Game file
            k_EWorkshopFileTypeGame = 6,
            /// Software file
            k_EWorkshopFileTypeSoftware = 7,
            /// Concept file
            k_EWorkshopFileTypeConcept = 8,
            /// Web guide file
            k_EWorkshopFileTypeWebGuide = 9,
            /// Steam integrated guide file
            k_EWorkshopFileTypeIntegratedGuide = 10,
            // Update k_EWorkshopFileTypeMax if you add values
            // k_EWorkshopFileTypeMax = 11

        };
        /// <summary>
        /// Enumeration type of file ranking
        /// </summary>
        public enum EWorkshopEnumerationType
        {
            /// Ranked by vote
            k_EWorkshopEnumerationTypeRankedByVote = 0,
            /// Most recent addition
            k_EWorkshopEnumerationTypeRecent = 1,
            /// Recently popular
            k_EWorkshopEnumerationTypeTrending = 2,
            /// Favorites of user's friends
            k_EWorkshopEnumerationTypeFavoritesOfFriends = 3,
            /// Voted on by friends
            k_EWorkshopEnumerationTypeVotedByFriends = 4,
            /// Content made by friends
            k_EWorkshopEnumerationTypeContentByFriends = 5,
            /// Content made by users followed by the current user
            k_EWorkshopEnumerationTypeRecentFromFollowedUsers = 6,
        };
        /// <summary>
        /// File action
        /// </summary>
        public enum EWorkshopFileAction
        {
            /// File has been played
            k_EWorkshopFileActionPlayed = 0,
            /// File has been completed
            k_EWorkshopFileActionCompleted = 1,
        };
        /// <summary>
        /// Visibility of published file
        /// </summary>
        public enum ERemoteStoragePublishedFileVisibility
        {
            /// Anyone can view the file
            k_ERemoteStoragePublishedFileVisibilityPublic = 0,
            /// Friends of the user can view the file
            k_ERemoteStoragePublishedFileVisibilityFriendsOnly = 1,
            /// Only the user can view the file
            k_ERemoteStoragePublishedFileVisibilityPrivate = 2,
        };

        enum EWorkshopVideoProvider
        {
            k_EWorkshopVideoProviderNone = 0,
            k_EWorkshopVideoProviderYoutube = 1
        };

        #region isteamugc.h
        // Matching UGC types for queries
        public enum EUGCMatchingUGCType
        {
            k_EUGCMatchingUGCType_Items = 0,		// both mtx items and ready-to-use items
            k_EUGCMatchingUGCType_Items_Mtx = 1,
            k_EUGCMatchingUGCType_Items_ReadyToUse = 2,
            k_EUGCMatchingUGCType_Collections = 3,
            k_EUGCMatchingUGCType_Artwork = 4,
            k_EUGCMatchingUGCType_Videos = 5,
            k_EUGCMatchingUGCType_Screenshots = 6,
            k_EUGCMatchingUGCType_AllGuides = 7,		// both web guides and integrated guides
            k_EUGCMatchingUGCType_WebGuides = 8,
            k_EUGCMatchingUGCType_IntegratedGuides = 9,
            k_EUGCMatchingUGCType_UsableInGame = 10,		// ready-to-use items and integrated guides
            k_EUGCMatchingUGCType_ControllerBindings = 11,
        };

        // Different lists of published UGC for a user.
        // If the current logged in user is different than the specified user, then some options may not be allowed.
        public enum EUserUGCList
        {
            k_EUserUGCList_Published,
            k_EUserUGCList_VotedOn,
            k_EUserUGCList_VotedUp,
            k_EUserUGCList_VotedDown,
            k_EUserUGCList_WillVoteLater,
            k_EUserUGCList_Favorited,
            k_EUserUGCList_Subscribed,
            k_EUserUGCList_UsedOrPlayed,
            k_EUserUGCList_Followed,
        };

        // Sort order for user published UGC lists (defaults to creation order descending)
        public enum EUserUGCListSortOrder
        {
            k_EUserUGCListSortOrder_CreationOrderDesc,
            k_EUserUGCListSortOrder_CreationOrderAsc,
            k_EUserUGCListSortOrder_TitleAsc,
            k_EUserUGCListSortOrder_LastUpdatedDesc,
            k_EUserUGCListSortOrder_SubscriptionDateDesc,
            k_EUserUGCListSortOrder_VoteScoreDesc,
            k_EUserUGCListSortOrder_ForModeration,
        };

        // Combination of sorting and filtering for queries across all UGC
        public enum EUGCQuery
        {
            k_EUGCQuery_RankedByVote = 0,
            k_EUGCQuery_RankedByPublicationDate = 1,
            k_EUGCQuery_AcceptedForGameRankedByAcceptanceDate = 2,
            k_EUGCQuery_RankedByTrend = 3,
            k_EUGCQuery_FavoritedByFriendsRankedByPublicationDate = 4,
            k_EUGCQuery_CreatedByFriendsRankedByPublicationDate = 5,
            k_EUGCQuery_RankedByNumTimesReported = 6,
            k_EUGCQuery_CreatedByFollowedUsersRankedByPublicationDate = 7,
            k_EUGCQuery_NotYetRated = 8,
            k_EUGCQuery_RankedByTotalVotesAsc = 9,
            k_EUGCQuery_RankedByVotesUp = 10,
            k_EUGCQuery_RankedByTextSearch = 11,
        };

        public const UInt32 kNumUGCResultsPerPage = 50;

        // Details for a single published file/UGC
        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Ansi)]
        public struct SteamUGCDetails_t
        {
	        PublishedFileId_t m_nPublishedFileId;
	        EResult m_eResult;												// The result of the operation.	
	        EWorkshopFileType m_eFileType;									// Type of the file
	        AppId_t m_nCreatorAppID;										// ID of the app that created this file.
	        AppId_t m_nConsumerAppID;										// ID of the app that will consume this file.
            
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 119)]
	        string m_rgchTitle;		                                		// title of document

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
	        string m_rgchDescription;                                   	// description of document

	        UInt64 m_ulSteamIDOwner;										// Steam ID of the user who created this content.
	        UInt32 m_rtimeCreated;											// time when the published file was created
	        UInt32 m_rtimeUpdated;											// time when the published file was last updated
	        UInt32 m_rtimeAddedToUserList;									// time when the user added the published file to their list (not always applicable)
	        ERemoteStoragePublishedFileVisibility m_eVisibility;			// visibility
	        bool m_bBanned;													// whether the file was banned
	        bool m_bAcceptedForUse;											// developer has specifically flagged this item as accepted in the Workshop
	        bool m_bTagsTruncated;											// whether the list of tags was too long to be returned in the provided buffer
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1025)]
	        string m_rgchTags;								                // comma separated list of all tags associated with this file	
	        // file/url information
	        UGCHandle_t m_hFile;											// The handle of the primary file
	        UGCHandle_t m_hPreviewFile;										// The handle of the preview file
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
	        string m_pchFileName;							                // The cloud filename of the primary file
	        UInt32 m_nFileSize;												// Size of the primary file
	        UInt32 m_nPreviewFileSize;										// Size of the preview file
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
	        string m_rgchURL;						                        // URL (for a video or a website)
	        // voting information
	        UInt32 m_unVotesUp;												// number of votes up
	        UInt32 m_unVotesDown;											// number of votes down
	        float m_flScore;												// calculated score
        }

        #endregion


        /// <summary>
        /// Arguments for result of file download
        /// </summary>
        public class PublishedFileDownloadResultArgs : System.EventArgs
        {
            internal PublishedFileDownloadResultArgs(PublishedFile publishedFile, bool primaryFile, string fileName)
            {
                PublishedFile = publishedFile;
                PrimaryFile = primaryFile;
                FileName = fileName;
            }
            /// <summary>
            /// If the file was downloaded successfully
            /// </summary>
            public bool Success
            {
                get;
                internal set;
            }

            internal bool PrimaryFile
            {
                get;
                private set;
            }
            /// <summary>
            /// The published file
            /// </summary>
            public PublishedFile PublishedFile
            {
                get;
                private set;
            }
            /// <summary>
            /// Name of published file
            /// </summary>
            public string FileName
            {
                get;
                private set;
            }
        }
        /// <summary>
        /// Information about the published file
        /// </summary>
        public class PublishedFile
        {
            [DllImport("CommunityExpressSW")]
            private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UGCDownload(IntPtr remoteStorage, UGCHandle_t hFile, UInt32 unPriority);

            internal PublishedFile(RemoteStorageGetPublishedFileDetailsResult_t args, UInt32 subscribe, IntPtr remoteStorage)
            {
                RemoteStorage = remoteStorage;

                Result = args.m_eResult;
                Title = args.m_rgchTitle;
                Description = args.m_rgchDescription;
                OwnerSteamID = new SteamID(args.m_ulSteamIDOwner);
                Tags = args.m_rgchTags.Split(',');
                TagsTruncated = args.m_bTagsTruncated;
                FileSize = args.m_nFileSize;
                URL = args.m_rgchURL;
                FileType = args.m_eFileType;
                FileName = args.m_pchFileName;
                hFile = args.m_hFile;
                hPreviewFile = args.m_hPreviewFile;
                ID = args.m_nPublishedFileId;

                DateTime epoc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                CreatedDate = epoc.AddSeconds(args.m_rtimeCreated);
                if (subscribe != 0)
                {
                    SubscribeDate = epoc.AddSeconds(subscribe);
                }
                else
                {
                    SubscribeDate = null;
                }
            }
            /// <summary>
            /// Title of published file
            /// </summary>
            public string Title
            {
                get;
                private set;
            }
            /// <summary>
            /// Description of published file
            /// </summary>
            public string Description
            {
                get;
                private set;
            }
            /// <summary>
            /// Name of published file
            /// </summary>
            public string FileName
            {
                get;
                private set;
            }
            /// <summary>
            /// File name preview
            /// </summary>
            public string PreviewFileName
            {
                get;
                internal set;
            }
            /// <summary>
            /// File creator's Steam ID
            /// </summary>
            public SteamID OwnerSteamID
            {
                get;
                private set;
            }
            /// <summary>
            /// File tags
            /// </summary>
            public string[] Tags
            {
                get;
                private set;
            }
            /// <summary>
            /// Shortened file tags
            /// </summary>
            public bool TagsTruncated
            {
                get;
                private set;
            }
            /// <summary>
            /// Size of file
            /// </summary>
            public Int32 FileSize
            {
                get;
                private set;
            }
            /// <summary>
            /// URL of file
            /// </summary>
            public string URL
            {
                get;
                private set;
            }
            /// <summary>
            /// Type of file
            /// </summary>
            public EWorkshopFileType FileType
            {
                get;
                private set;
            }
            /// <summary>
            /// Date file was created
            /// </summary>
            public DateTime CreatedDate
            {
                get;
                private set;
            }
            /// <summary>
            /// Date file was subscribed to
            /// </summary>
            public DateTime? SubscribeDate
            {
                get;
                private set;
            }
            
            internal UGCHandle_t hFile
            {
                get;
                set;
            }

            internal UGCHandle_t hPreviewFile
            {
                get;
                set;
            }

            public EResult Result
            {
                get;
                private set;
            }

            /// <summary>
            /// Directory of file
            /// </summary>
            public string FileDirectory
            {
                get
                {
                    int dir = FileName.LastIndexOf('/');
                    if (dir <= 0) return string.Empty;

                    return FileName.Substring(0, dir - 1);
                }
            }
            /// <summary>
            /// Preview of file directory
            /// </summary>
            public string PreviewFileDirectory
            {
                get
                {
                    if (PreviewFileName == null) return null;

                    int dir = PreviewFileName.LastIndexOf('/');
                    if (dir <= 0) return string.Empty;

                    return PreviewFileName.Substring(0, dir - 1);
                }
            }

            /// <summary>
            /// True if the user has deleted the file
            /// </summary>
            public bool Deleted
            {
                get { return (Result == EResult.EResultFileNotFound); }
            }

            /// <summary>
            /// Published File ID
            /// </summary>
            public UInt64 ID
            {
                get;
                private set;
            }

            private IntPtr RemoteStorage
            {
                get;
                set;
            }

            /// <summary>
            /// Downloads a UGC file.  A priority value of 0 will download the file immediately,
            /// otherwise it will wait to download the file until all downloads with a lower priority
            /// value are completed.  Downloads with equal priority will occur simultaneously.
            /// </summary>
            /// <param name="fileName">Name of file</param>
            /// <param name="priority">Priority of download</param>
            public void Download(string fileName, UInt32 priority)
            {
                // SteamUnityAPI_SteamRemoteStorage_UGCDownload(RemoteStorage, hFile, priority);
                string fullFileName = System.IO.Path.GetFullPath(fileName);

                System.IO.Directory.CreateDirectory(
                    System.IO.Path.GetDirectoryName(fullFileName));

                UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UGCDownloadToLocation(RemoteStorage, hFile, fullFileName, 0);
                CommunityExpress.Instance.UserGeneratedContent.AddUGCDownload(ret, new PublishedFileDownloadResultArgs(this, true, fileName));
            }
            /// <summary>
            /// Preview of file to be uploaded
            /// </summary>
            /// <param name="fileName">Name of file</param>
            public void WritePreviewFile(string fileName)
            {
                string fullFileName = System.IO.Path.GetFullPath(fileName);

                System.IO.Directory.CreateDirectory(
                    System.IO.Path.GetDirectoryName(fullFileName));

                // copy off preview file
                UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UGCDownloadToLocation(RemoteStorage, hPreviewFile, fullFileName, 0);
                CommunityExpress.Instance.UserGeneratedContent.AddUGCDownload(ret, new PublishedFileDownloadResultArgs(this, false, fileName));
            }

            /// <summary>
            /// Downloads a UGC file.  A priority value of 0 will download the file immediately,
            /// otherwise it will wait to download the file until all downloads with a lower priority
            /// value are completed.  Downloads with equal priority will occur simultaneously.
            /// </summary>
            internal void InernalDownloadPreview(UInt32 priority)
            {
                SteamUnityAPI_SteamRemoteStorage_UGCDownload(RemoteStorage, hPreviewFile, priority);
            }
        }

        [DllImport("CommunityExpressSW")]
        private static extern IntPtr SteamUnityAPI_SteamRemoteStorage();

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedWorkshopFiles(IntPtr remoteStorage, EWorkshopEnumerationType eEnumerationType, UInt32 unStartIndex, UInt32 unCount, UInt32 unDays, SteamParamStringArray_t pTags, SteamParamStringArray_t pUserTags);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_EnumerateUserSubscribedFiles(IntPtr remoteStorage, UInt32 unStartIndex);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_EnumerateUserPublishedFiles(IntPtr remoteStorage, UInt32 unStartIndex);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_SubscribePublishedFile(IntPtr remoteStorage, PublishedFileId_t unPublishedFileId);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UnsubscribePublishedFile(IntPtr remoteStorage, PublishedFileId_t unPublishedFileId);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_GetPublishedItemVoteDetails(IntPtr remoteStorage, PublishedFileId_t unPublishedFileId);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UpdateUserPublishedItemVote(IntPtr remoteStorage, PublishedFileId_t unPublishedFileId, bool bVoteUp);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_GetUserPublishedItemVoteDetails(IntPtr remoteStorage, PublishedFileId_t unPublishedFileId);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_EnumerateUserSharedWorkshopFiles(IntPtr remoteStorage, UInt64 steamId, UInt32 unStartIndex, SteamParamStringArray_t pRequiredTags, SteamParamStringArray_t pExcludedTags);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_PublishVideo(IntPtr remoteStorage, EWorkshopVideoProvider eVideoProvider, [MarshalAs(UnmanagedType.LPStr)] string pchVideoAccount, [MarshalAs(UnmanagedType.LPStr)] string pchVideoIdentifier, [MarshalAs(UnmanagedType.LPStr)] string pchPreviewFile, AppId_t nConsumerAppId, [MarshalAs(UnmanagedType.LPStr)] string pchTitle, [MarshalAs(UnmanagedType.LPStr)] string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, SteamParamStringArray_t pTags);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_SetUserPublishedFileAction(IntPtr remoteStorage, PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedFilesByUserAction(IntPtr remoteStorage, EWorkshopFileAction eAction, UInt32 unStartIndex);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UGCDownloadToLocation(IntPtr remoteStorage, UGCHandle_t hContent, [MarshalAs(UnmanagedType.LPStr)] string pchLocation, UInt32 unPriority);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_CommitPublishedFileUpdate(IntPtr remoteStorage, PublishedFileUpdateHandle_t updateHandle);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_GetPublishedFileDetails(IntPtr remoteStorage, PublishedFileId_t unPublishedFileId);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_DeletePublishedFile(IntPtr remoteStorage, PublishedFileId_t unPublishedFileId);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_PublishWorkshopFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] string pchFile, [MarshalAs(UnmanagedType.LPStr)] string pchPreviewFile, AppId_t nConsumerAppId, [MarshalAs(UnmanagedType.LPStr)] string pchTitle, [MarshalAs(UnmanagedType.LPStr)] string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, SteamParamStringArray_t pTags, EWorkshopFileType eWorkshopFileType);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_CreatePublishedFileUpdateRequest(IntPtr remoteStorage, PublishedFileId_t unPublishedFileId);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileFile(IntPtr remoteStorage, PublishedFileUpdateHandle_t updateHandle, [MarshalAs(UnmanagedType.LPStr)] string pchFile);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFilePreviewFile(IntPtr remoteStorage, PublishedFileUpdateHandle_t updateHandle, [MarshalAs(UnmanagedType.LPStr)] string pchPreviewFile);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileTitle(IntPtr remoteStorage, PublishedFileUpdateHandle_t updateHandle, [MarshalAs(UnmanagedType.LPStr)] string pchTitle);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileDescription(IntPtr remoteStorage, PublishedFileUpdateHandle_t updateHandle, [MarshalAs(UnmanagedType.LPStr)] string pchDescription);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileVisibility(IntPtr remoteStorage, PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileTags(IntPtr remoteStorage, PublishedFileUpdateHandle_t updateHandle, SteamParamStringArray_t pTags);


        #region steamugc

        [DllImport("CommunityExpressSW")]
        private static extern IntPtr SteamUnityAPI_SteamUserGeneratedContent();
      
        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_UserGeneratedContent_CreateQueryUserUGCRequest(IntPtr pSteamUGC, AccountID_t unAccountID, EUserUGCList eListType, EUGCMatchingUGCType eMatchingUGCType, EUserUGCListSortOrder eSortOrder, AppId_t nCreatorAppID, AppId_t nConsumerAppID, UInt32 unPage);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_UserGeneratedContent_CreateQueryAllUGCRequest(IntPtr pSteamUGC, EUGCQuery eQueryType, EUGCMatchingUGCType eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, UInt32 unPage);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_UserGeneratedContent_SendQueryUGCRequest(IntPtr pSteamUGC, UGCQueryHandle_t handle);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_GetQueryUGCResult(IntPtr pSteamUGC, UGCQueryHandle_t handle, UInt32 index, SteamUGCDetails_t pDetails);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_ReleaseQueryUGCRequest(IntPtr pSteamUGC, UGCQueryHandle_t handle);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_AddRequiredTag(IntPtr pSteamUGC, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.LPStr)] string pTagName);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_AddExcludedTag(IntPtr pSteamUGC, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.LPStr)] string pTagName);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_SetReturnLongDescription(IntPtr pSteamUGC, UGCQueryHandle_t handle, bool bReturnLongDescription);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_SetReturnTotalOnly(IntPtr pSteamUGC, UGCQueryHandle_t handle, bool bReturnTotalOnly);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_SetAllowCachedResponse(IntPtr pSteamUGC, UGCQueryHandle_t handle, UInt32 unMaxAgeSeconds);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_SetCloudFileNameFilter(IntPtr pSteamUGC, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.LPStr)] string pMatchCloudFileName);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_SetMatchAnyTag(IntPtr pSteamUGC, UGCQueryHandle_t handle, bool bMatchAnyTag);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_SetSearchText(IntPtr pSteamUGC, UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.LPStr)] string pSearchText);

        [DllImport("CommunityExpressSW")]
        private static extern bool SteamUnityAPI_UserGeneratedContent_SetRankedByTrendDays(IntPtr pSteamUGC, UGCQueryHandle_t handle, UInt32 unDays);

        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_UserGeneratedContent_RequestUGCDetails(IntPtr pSteamUGC, PublishedFileId_t nPublishedFileID, UInt32 unMaxAgeSeconds);


        #endregion

        private IntPtr _remoteStorage;
        private IntPtr _steamUgc;
        private CommunityExpress _ce;
        private CommunityExpress.OnEventHandler<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t> _userSharedFilesHandler;
        private Dictionary<UInt64, PublishedFileDownloadResultArgs> _ugcDownloads = new Dictionary<PublishedFileUpdateHandle_t, PublishedFileDownloadResultArgs>();

        internal UserGeneratedContent(CommunityExpress ce)
        {
            _ce = ce;
            _remoteStorage = SteamUnityAPI_SteamRemoteStorage();
            _steamUgc = SteamUnityAPI_SteamUserGeneratedContent();

            _userSharedFilesHandler = new CommunityExpress.OnEventHandler<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t>(Events_UserSharedWorkshopFilesReceived);



            CommunityExpress.OnEventHandler<RemoteStoragePublishFileProgress_t> h = new CommunityExpress.OnEventHandler<RemoteStoragePublishFileProgress_t>(Events_PublishFileProgressReceived);
            _ce.AddEventHandler(RemoteStoragePublishFileProgress_t.k_iCallback, h);

            CommunityExpress.OnEventHandler<RemoteStorageGetPublishedFileDetailsResult_t> h3 = new CommunityExpress.OnEventHandler<RemoteStorageGetPublishedFileDetailsResult_t>(Events_GetPublishedFileDetailsReceived);
            _ce.AddEventHandler(RemoteStorageGetPublishedFileDetailsResult_t.k_iCallback, h3);

            _ce.AddEventHandler(RemoteStorageEnumerateUserSubscribedFilesResult_t.k_iCallback,
                new CommunityExpress.OnEventHandler<RemoteStorageEnumerateUserSubscribedFilesResult_t>(Events_GetUserSubscribedFilesReceived));

            _ce.AddEventHandler(RemoteStorageDownloadUGCResult_t.k_iCallback,
                new CommunityExpress.OnEventHandler<RemoteStorageDownloadUGCResult_t>(Events_DownloadUGCReceived));

            _ce.AddEventHandler(SteamAPICallCompleted_t.k_iCallback,
                new CommunityExpress.OnEventHandler<SteamAPICallCompleted_t>(Events_SteamAPICallCompleted));


            _ce.AddEventHandler(RemoteStorageEnumerateUserPublishedFilesResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageEnumerateUserPublishedFilesResult_t>(Events_UserPublishedFilesResultReceived));

            _ce.AddEventHandler(RemoteStorageEnumerateWorkshopFilesResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageEnumerateWorkshopFilesResult_t>(Events_EnumerateWorkshopFilesResultReceived));

            _ce.AddEventHandler(RemoteStorageUpdatePublishedFileResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageUpdatePublishedFileResult_t>(Events_UpdatePublishedFileFinished));

            _ce.AddEventHandler(RemoteStorageSubscribePublishedFileResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageSubscribePublishedFileResult_t>(Events_SubscribePublishedFileResult));

            _ce.AddEventHandler(RemoteStorageUnsubscribePublishedFileResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageUnsubscribePublishedFileResult_t>(Events_UnsubscribePublishedFileResult));

            _ce.AddEventHandler(RemoteStoragePublishedFileSubscribed_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStoragePublishedFileSubscribed_t>(Events_SubscribePublishedFileResult2));

            _ce.AddEventHandler(RemoteStoragePublishedFileUnsubscribed_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStoragePublishedFileUnsubscribed_t>(Events_UnsubscribePublishedFileResult2));

            _ce.AddEventHandler(RemoteStoragePublishFileResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStoragePublishFileResult_t>(Events_FilePublished));

            _ce.AddEventHandler(RemoteStorageDeletePublishedFileResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageDeletePublishedFileResult_t>(Events_FileDeleted));
            
        }
        /// <summary>
        /// Enumerates published files
        /// </summary>
        public class EnumeratePublishedFileResultArgs : System.EventArgs
        {
            internal EnumeratePublishedFileResultArgs(ICollection<PublishedFile> publishedFiles, EResult result,
                UInt32 offset, Int32 totalCount)
            {
                StartIndex = offset;
                TotalCount = totalCount;
                Result = result;
                PublishedFiles = publishedFiles;
            }
            /// <summary>
            /// Result of the enumeration
            /// </summary>
            public EResult Result
            {
                get;
                private set;
            }
            /// <summary>
            /// Where the index starts
            /// </summary>
            public UInt32 StartIndex
            {
                get;
                private set;
            }
            /// <summary>
            /// Total count of published files
            /// </summary>
            public Int32 TotalCount
            {
                get;
                private set;
            }
            /// <summary>
            /// Lists published files
            /// </summary>
            public ICollection<PublishedFile> PublishedFiles
            {
                get;
                private set;
            }
        }
        /// <summary>
        /// Arguments for writing published file details
        /// </summary>
        public class PublishedFileDetailsResultArgs : System.EventArgs
        {
            internal PublishedFileDetailsResultArgs(PublishedFile publishedFile, EResult result)
            {
                Result = result;
                PublishedFile = publishedFile;
            }
            /// <summary>
            /// Results of the publish
            /// </summary>
            public EResult Result
            {
                get;
                private set;
            }
            /// <summary>
            /// The published file
            /// </summary>
            public PublishedFile PublishedFile
            {
                get;
                private set;
            }
        }
        /// <summary>
        /// Arguments for publishing a file
        /// </summary>
        public class PublishedFileResultArgs : System.EventArgs
        {
            internal PublishedFileResultArgs(EResult result,
                PublishedFileId_t nPublishedFileId)
            {
                Result = result;
                PublishedFileId = nPublishedFileId;
            }
            /// <summary>
            /// Result of publish
            /// </summary>
            public EResult Result
            {
                get;
                private set;
            }
            /// <summary>
            /// ID of the published file
            /// </summary>
            public PublishedFileId_t PublishedFileId
            {
                get;
                private set;
            }
        }

        private UInt32 _offset = 0;
        private Int32 _totalCount = 0;
        private List<PublishedFile> _publishedFiles = null;
        private List<PublishedFile> _oneOffpublishedFiles = new List<PublishedFile>();
        private Dictionary<PublishedFileId_t, UInt32> _subscribeTimes = null;
        /// <summary>
        /// Result of published file details
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="args">Arguments for result</param>
        public delegate void PublishedFileDetailsResultHandler(UserGeneratedContent sender, PublishedFileDetailsResultArgs args);
        /// <summary>
        /// Result of the search for published file details
        /// </summary>
        public event PublishedFileDetailsResultHandler FileDetails;

        private void Events_GetUserSubscribedFilesReceived(RemoteStorageEnumerateUserSubscribedFilesResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _totalCount = recv.m_nTotalResultCount;
            _subscribeTimes = new Dictionary<PublishedFileUpdateHandle_t, AppId_t>();

            for (int i = 0; i < recv.m_nResultsReturned; i++)
            {
                _subscribeTimes.Add(recv.m_rgPublishedFileId[i], recv.m_rgRTimeSubscribed[i]);
                GetPublishedFileDetails(recv.m_rgPublishedFileId[i]);
            }

            if (recv.m_nResultsReturned == 0 && EnumerateFileDetails != null)
            {
                EnumerateFileDetails(this, new EnumeratePublishedFileResultArgs(_publishedFiles, recv.m_eResult, _offset, _totalCount));
            }
        }
        /// <summary>
        /// Result of download of file
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="file">Arguments for result</param>
        public delegate void PublishedFileDownloadedHandler(UserGeneratedContent sender, PublishedFileDownloadResultArgs file);
        /// <summary>
        /// Published file was downloaded
        /// </summary>
        public event PublishedFileDownloadedHandler PublishedFileDownloaded;
        /// <summary>
        /// File is downloaded
        /// </summary>
        public event PublishedFileDownloadedHandler PublishedPreviewFileDownloaded;

        private void Events_SteamAPICallCompleted(SteamAPICallCompleted_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (_ugcDownloads.ContainsKey(recv.m_hAsyncCall))
            {
                PublishedFileDownloadResultArgs args = _ugcDownloads[recv.m_hAsyncCall];
                args.Success = !bIOFailure;

                if (args.PrimaryFile)
                {
                    if (PublishedFileDownloaded != null)
                    {
                        PublishedFileDownloaded(this, args);
                    }
                }
                else
                {
                    if (PublishedPreviewFileDownloaded != null)
                    {
                        PublishedPreviewFileDownloaded(this, args);
                    }
                }

                _ugcDownloads.Remove(recv.m_hAsyncCall);
            }
        }
        /// <summary>
        /// Result of updating published file
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="result">Arguments for result</param>
        public delegate void RemoteStorageUpdatePublishedFileResultHandler(UserGeneratedContent sender, PublishedFileResultArgs result);
        /// <summary>
        /// The published file was updated on Steam Cloud
        /// </summary>
        public event RemoteStorageUpdatePublishedFileResultHandler FileUpdated;

        private void Events_UpdatePublishedFileFinished(RemoteStorageUpdatePublishedFileResult_t recv, bool Success, SteamAPICall_t hSteamAPICall)
        {
            if (FileUpdated != null)
            {
                FileUpdated(this, new PublishedFileResultArgs(recv.m_eResult, recv.m_nPublishedFileId));
            }
        }
        /// <summary>
        /// Subscribing to a published file
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="result">Argument for result</param>
        public delegate void RemoteStorageSubscribePublishedFileResultHandler(UserGeneratedContent sender, PublishedFileResultArgs result);
        /// <summary>
        /// The published file was subscribed to
        /// </summary>
        public event RemoteStorageSubscribePublishedFileResultHandler FileSubscribed;

        private void Events_SubscribePublishedFileResult(RemoteStorageSubscribePublishedFileResult_t recv, bool Success, SteamAPICall_t hSteamAPICall)
        {
            if (FileSubscribed != null)
            {
                FileSubscribed(this, new PublishedFileResultArgs(recv.m_eResult, recv.m_nPublishedFileId));
            }
        }
        /// <summary>
        /// Unsubscribing from a published file
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="result">Argument for result</param>
        public delegate void RemoteStorageUnsubscribePublishedFileResultHandler(UserGeneratedContent sender, PublishedFileResultArgs result);
        /// <summary>
        /// The published file was unsubscribed from
        /// </summary>
        public event RemoteStorageUnsubscribePublishedFileResultHandler FileUnsubscribed;

        private void Events_UnsubscribePublishedFileResult(RemoteStorageUnsubscribePublishedFileResult_t recv, bool Success, SteamAPICall_t hSteamAPICall)
        {
            if (FileUnsubscribed != null)
            {
                FileUnsubscribed(this, new PublishedFileResultArgs(recv.m_eResult, recv.m_nPublishedFileId));
            }
        }

        private void Events_SubscribePublishedFileResult2(RemoteStoragePublishedFileSubscribed_t recv, bool Success, SteamAPICall_t hSteamAPICall)
        {
            if (FileSubscribed != null)
            {
                FileSubscribed(this, new PublishedFileResultArgs(EResult.EResultOK, recv.m_nPublishedFileId));
            }
        }

        private void Events_UnsubscribePublishedFileResult2(RemoteStoragePublishedFileUnsubscribed_t recv, bool Success, SteamAPICall_t hSteamAPICall)
        {
            if (FileUnsubscribed != null)
            {
                FileUnsubscribed(this, new PublishedFileResultArgs(EResult.EResultOK, recv.m_nPublishedFileId));
            }
        }
        /// <summary>
        /// Result of saving file to Steam Cloud
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="result">Argument for result</param>
        public delegate void RemoteStoragePublishedFileResultHandler(UserGeneratedContent sender, PublishedFileResultArgs result);
        /// <summary>
        /// The published file was saved to the Steam Cloud
        /// </summary>
        public event RemoteStoragePublishedFileResultHandler FilePublished;
        
        private void Events_FilePublished(RemoteStoragePublishFileResult_t recv, bool Success, SteamAPICall_t hSteamAPICall)
        {
            if (FilePublished != null)
            {
                FilePublished(this, new PublishedFileResultArgs(EResult.EResultOK, recv.m_nPublishedFileId));
            }
        }
        /// <summary>
        /// Result of deleting file from storage
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="result">Argument for result</param>
        public delegate void RemoteStorageDeletedPublishedFileResultHandler(UserGeneratedContent sender, PublishedFileResultArgs result);
        /// <summary>
        /// The published file was deleted from the Steam Cloud
        /// </summary>
        public event RemoteStorageDeletedPublishedFileResultHandler FileDeleted;
        
        private void Events_FileDeleted(RemoteStorageDeletePublishedFileResult_t recv, bool Success, SteamAPICall_t hSteamAPICall)
        {
            if (FileDeleted != null)
            {
                FileDeleted(this, new PublishedFileResultArgs(EResult.EResultOK, recv.m_nPublishedFileId));
            }
        }

        internal void AddUGCDownload(UInt64 hSteamAPICall, PublishedFileDownloadResultArgs file)
        {
            _ugcDownloads.Add(hSteamAPICall, file);
        }
        /// <summary>
        /// Progress for file's storage
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="progress">Storage progress</param>
        public delegate void RemoteStoragePublishFileProgressHandler(UserGeneratedContent sender, float progress);
        /// <summary>
        /// The progress of the published file while being saved to the Steam Cloud
        /// </summary>
        public event RemoteStoragePublishFileProgressHandler FileProgress;

        private void Events_PublishFileProgressReceived(RemoteStoragePublishFileProgress_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            float progress = (float)recv.m_dPercentFile;
            if (FileProgress != null)
            {
                FileProgress(this, progress);
            }
        }

        private void Events_DownloadUGCReceived(RemoteStorageDownloadUGCResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            List<PublishedFile> deleteList = new List<PublishedFile>();
            foreach (PublishedFile pf in _oneOffpublishedFiles)
            {
                if (pf.hPreviewFile == recv.m_hFile)
                {
                    pf.PreviewFileName = recv.m_pchFileName;
                    
                    if (FileDetails != null)
                    {
                        FileDetails(this, new PublishedFileDetailsResultArgs(pf, pf.Result));
                    }

                    deleteList.Add(pf);
                }
            }

            foreach (PublishedFile df in deleteList)
            {
                _oneOffpublishedFiles.Remove(df);
            }

            if (_publishedFiles == null) return;

            int goodFiles = 0;
            foreach (PublishedFile pf in _publishedFiles)
            {
                if (pf.hPreviewFile == recv.m_hFile)
                {
                    pf.PreviewFileName = recv.m_pchFileName;
                }

                if (pf.PreviewFileName != null || pf.Deleted) goodFiles++;
            }

            if (goodFiles == _subscribeTimes.Count)
            {
                if (EnumerateFileDetails != null)
                {
                    EnumerateFileDetails(this, new EnumeratePublishedFileResultArgs(_publishedFiles, EResult.EResultOK, _offset, _totalCount));

                }
                _subscribeTimes.Clear();
                _publishedFiles = null;
            }
        }
        
        private void Events_UserPublishedFilesResultReceived(RemoteStorageEnumerateUserPublishedFilesResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _totalCount = recv.m_nTotalResultCount;
            _subscribeTimes = new Dictionary<PublishedFileUpdateHandle_t, AppId_t>();

            for (int i = 0; i < recv.m_nResultsReturned; i++)
            {
                if (!_subscribeTimes.ContainsKey(recv.m_rgPublishedFileId[i]))
                {
                    _subscribeTimes.Add(recv.m_rgPublishedFileId[i], 0);
                    GetPublishedFileDetails(recv.m_rgPublishedFileId[i]);
                }
                else
                {
                    _totalCount--;
                }
            }

            if (recv.m_nResultsReturned == 0 && EnumerateFileDetails != null)
            {
                EnumerateFileDetails(this, new EnumeratePublishedFileResultArgs(_publishedFiles, recv.m_eResult, _offset, _totalCount));
            }
        }

        private void Events_EnumerateWorkshopFilesResultReceived(RemoteStorageEnumerateWorkshopFilesResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _totalCount = recv.m_nTotalResultCount;
            _subscribeTimes = new Dictionary<PublishedFileUpdateHandle_t, AppId_t>();

            for (int i = 0; i < recv.m_nResultsReturned; i++)
            {
                if (!_subscribeTimes.ContainsKey(recv.m_rgPublishedFileId[i]))
                {
                    _subscribeTimes.Add(recv.m_rgPublishedFileId[i], 0);
                    GetPublishedFileDetails(recv.m_rgPublishedFileId[i]);
                }
                else
                {
                    _totalCount--;
                }
            }

            if (recv.m_nResultsReturned == 0 && EnumerateFileDetails != null)
            {
                EnumerateFileDetails(this, new EnumeratePublishedFileResultArgs(_publishedFiles, recv.m_eResult, _offset, _totalCount));
            }
        }
        /// <summary>
        /// Result of file enumeration
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="args">Arguments for result</param>
        public delegate void EnumeratePublishedFileResultHandler(UserGeneratedContent sender, EnumeratePublishedFileResultArgs args);
        /// <summary>
        /// Enumerates published files
        /// </summary>
        public event EnumeratePublishedFileResultHandler EnumerateFileDetails;

        private void Events_GetPublishedFileDetailsReceived(RemoteStorageGetPublishedFileDetailsResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            bool found = false;
            if (EnumerateFileDetails != null && _publishedFiles != null)
            {
                if (_subscribeTimes.ContainsKey(recv.m_nPublishedFileId) &&
                    _publishedFiles.Find(pf => pf.ID == recv.m_nPublishedFileId) == null)
                {
                    found = true;

                    // enumerate request
                    PublishedFile pf = new PublishedFile(recv, _subscribeTimes[recv.m_nPublishedFileId], _remoteStorage);
                    pf.InernalDownloadPreview(0);

                    _publishedFiles.Add(pf);
                }
            }

            if (!found)
            {
                if (recv.m_eResult == EResult.EResultOK)
                {
                    // one off request
                    PublishedFile pf = new PublishedFile(recv, 0, _remoteStorage);
                    pf.InernalDownloadPreview(0);

                    _oneOffpublishedFiles.Add(pf);
                }
                else if (FileDetails != null)
                {
                    // one off request
                    FileDetails(this,
                        new PublishedFileDetailsResultArgs(null, recv.m_eResult));
                }
            }
        }
        
        private void Events_UserSharedWorkshopFilesReceived(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _ce.RemoveEventHandler(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.k_iCallback, _userSharedFilesHandler);

            // TODO: Send event
            // if (UserStatsReceived != null) UserStatsReceived(this, new UserStatsReceivedArgs(recv));
        }

        /// <summary>
        /// Enumerates published files on the workshop
        /// </summary>
        /// <param name="eEnumerationType">Type of enumeration</param>
        /// <param name="unStartIndex">Where the index starts</param>
        /// <param name="unCount">How many files are shown</param>
        /// <param name="unDays">Days when the items were published</param>
        /// <param name="tags">File tags</param>
        /// <param name="userTags">File tags given by the user</param>
        public void EnumeratePublishedWorkshopFiles(EWorkshopEnumerationType eEnumerationType, UInt32 unStartIndex, UInt32 unCount, UInt32 unDays, ICollection<string> tags, ICollection<string> userTags)
        {
            _offset = unStartIndex;
            _totalCount = 0;
            _publishedFiles = new List<PublishedFile>();

            SteamParamStringArray_t pTagsArray = new CommunityExpressNS.SteamParamStringArray_t(tags);
            SteamParamStringArray_t pUserTagsArray = new CommunityExpressNS.SteamParamStringArray_t(userTags);

            SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedWorkshopFiles(_remoteStorage, eEnumerationType, unStartIndex, unCount, unDays, pTagsArray, pUserTagsArray);

        }

        /// <summary>
        /// Enumerates files published by the user
        /// </summary>
        /// <param name="unStartIndex">Where the index starts</param>
        public void EnumerateUserPublishedFiles(UInt32 unStartIndex)
        {
            _offset = unStartIndex;
            _totalCount = 0;
            _publishedFiles = new List<PublishedFile>();

            SteamUnityAPI_SteamRemoteStorage_EnumerateUserPublishedFiles(_remoteStorage, unStartIndex);
        }

        /// <summary>
        /// Enumerates files subscribed to by the user
        /// </summary>
        /// <param name="unStartIndex">Where the index starts</param>
        public void EnumerateUserSubscribedFiles(UInt32 unStartIndex)
        {
            _offset = unStartIndex;
            _totalCount = 0;
            _publishedFiles = new List<PublishedFile>();

            SteamUnityAPI_SteamRemoteStorage_EnumerateUserSubscribedFiles(_remoteStorage, unStartIndex);
        }

        /// <summary>
        /// Subscribing to a published file
        /// </summary>
        /// <param name="unPublishedFile">Published file</param>
        public void SubscribePublishedFile(PublishedFile unPublishedFile)
        {
            SteamUnityAPI_SteamRemoteStorage_SubscribePublishedFile(_remoteStorage, unPublishedFile.ID);
        }

        /// <summary>
        /// Unsubscribes from a file
        /// </summary>
        /// <param name="unPublishedFile">Published file</param>
        public void UnubscribePublishedFile(PublishedFile unPublishedFile)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UnsubscribePublishedFile(_remoteStorage, unPublishedFile.ID);

            Console.WriteLine(ret);
        }

        /*
        public void GetPublishedItemVoteDetails(PublishedFile unPublishedFile)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_GetPublishedItemVoteDetails(_remoteStorage, unPublishedFile.ID);

            Console.WriteLine(ret);
        }*/

        /// <summary>
        /// Updates the vote number for items published by the user
        /// </summary>
        /// <param name="unPublishedFile">Published file</param>
        /// <param name="bVoteUp">If voted up</param>
        public void UpdateUserPublishedItemVote(PublishedFile unPublishedFile, bool bVoteUp)
        {
            SteamUnityAPI_SteamRemoteStorage_UpdateUserPublishedItemVote(_remoteStorage, unPublishedFile.ID, bVoteUp);
        }

        /*
        public void GetUserPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_GetUserPublishedItemVoteDetails(_remoteStorage, unPublishedFileId);

            Console.WriteLine(ret);
        }*/

        /// <summary>
        /// Enumerates files shared by the user
        /// </summary>
        /// <param name="steamId">User's Steam ID</param>
        /// <param name="unStartIndex">Where the index starts</param>
        /// <param name="pRequiredTags">Required tags</param>
        /// <param name="pExcludedTags">Excluded tags</param>
        public void EnumerateUserSharedWorkshopFiles(SteamID steamId, UInt32 unStartIndex, ICollection<string> pRequiredTags, ICollection<string> pExcludedTags)
        {
            _ce.AddEventHandler(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.k_iCallback, _userSharedFilesHandler);

            SteamParamStringArray_t pRequiredTagsArray = new CommunityExpressNS.SteamParamStringArray_t(pRequiredTags);
            SteamParamStringArray_t pExcludedTagsArray = new CommunityExpressNS.SteamParamStringArray_t(pExcludedTags);

            SteamUnityAPI_SteamRemoteStorage_EnumerateUserSharedWorkshopFiles(_remoteStorage, steamId.ToUInt64(), unStartIndex, pRequiredTagsArray, pExcludedTagsArray);

            pRequiredTagsArray.Free();
            pExcludedTagsArray.Free();
        }

        /*
        public void PublishVideo(EWorkshopVideoProvider eVideoProvider, const char *pchVideoAccount, const char *pchVideoIdentifier, const char *pchPreviewFile, AppId_t nConsumerAppId, const char *pchTitle, const char *pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, SteamParamStringArray_t *pTags)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_PublishVideo(_remoteStorage,  eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags);

            Console.WriteLine(ret);
        }
         */

        /// <summary>
        /// Sets user action for file
        /// </summary>
        /// <param name="unPublishedFile">Published file</param>
        /// <param name="eAction">Action</param>
        public void SetUserPublishedFileAction(PublishedFile unPublishedFile, EWorkshopFileAction eAction)
        {
            SteamUnityAPI_SteamRemoteStorage_SetUserPublishedFileAction(_remoteStorage, unPublishedFile.ID, eAction);
        }

        /// <summary>
        /// Enumerates files by user's action
        /// </summary>
        /// <param name="eAction">Action</param>
        /// <param name="unStartIndex">Where the index starts</param>
        public void EnumeratePublishedFilesByUserAction(EWorkshopFileAction eAction, UInt32 unStartIndex)
        {
            SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedFilesByUserAction(_remoteStorage, eAction, unStartIndex);
        }

        private void CommitPublishedFileUpdate(PublishedFileUpdateHandle_t unPublishedFileId)
        {
            SteamUnityAPI_SteamRemoteStorage_CommitPublishedFileUpdate(_remoteStorage, unPublishedFileId);
        }

        /// <summary>
        /// Deletes a file published by the user
        /// </summary>
        /// <param name="file">Published file</param>
        public void DeletePublishedFile(PublishedFile file)
        {
            SteamUnityAPI_SteamRemoteStorage_DeletePublishedFile(_remoteStorage, file.ID);
        }

        /// <summary>
        /// Gets details on a published file
        /// </summary>
        /// <param name="unPublishedFileId">Published file</param>
        public void GetPublishedFileDetails(PublishedFileId_t unPublishedFileId)
        {
            SteamUnityAPI_SteamRemoteStorage_GetPublishedFileDetails(_remoteStorage, unPublishedFileId);
        }

        /// <summary>
        /// Publishes a workship file
        /// </summary>
        /// <param name="pchFile">File to publish</param>
        /// <param name="pchPreviewFile">Preview of published file</param>
        /// <param name="nConsumerAppId">ID of app to upload to</param>
        /// <param name="pchTitle">Title of file</param>
        /// <param name="pchDescription">Description of file</param>
        /// <param name="eVisibility">Who can see the file</param>
        /// <param name="pTags">File tags</param>
        /// <param name="eWorkshopFileType">File type</param>
        public void PublishWorkshopFile(string pchFile, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, ICollection<string> pTags, EWorkshopFileType eWorkshopFileType)
        {
            SteamParamStringArray_t pTagsArray = new CommunityExpressNS.SteamParamStringArray_t(pTags);

            SteamUnityAPI_SteamRemoteStorage_PublishWorkshopFile(_remoteStorage, pchFile, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTagsArray, eWorkshopFileType);

            pTagsArray.Free();
        }

        private PublishedFileUpdateHandle_t CreatePublishedFileUpdateRequest(PublishedFileId_t unPublishedFileId)
        {
            PublishedFileUpdateHandle_t ret = SteamUnityAPI_SteamRemoteStorage_CreatePublishedFileUpdateRequest(_remoteStorage, unPublishedFileId);
            return ret;
        }

        /// <summary>
        /// Updates a published file
        /// </summary>
        /// <param name="unPublishedFile">Published file</param>
        /// <param name="pchFile">File to publish</param>
        /// <param name="pchPreviewFile">Preview of published file</param>
        /// <param name="pchTitle">Title of file</param>
        /// <param name="pchDescription">Description of file</param>
        /// <param name="eVisibility">Who can see the file</param>
        /// <param name="pTags">File tags</param>
        public void UpdatePublishedFile(PublishedFile unPublishedFile, string pchFile, string pchPreviewFile, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility? eVisibility, ICollection<string> pTags)
        {
            PublishedFileUpdateHandle_t updateHandle = CreatePublishedFileUpdateRequest(unPublishedFile.ID);

            if (pchFile != null)
            {
                UpdatePublishedFileFile(updateHandle, pchFile);
            }
            if (pchPreviewFile != null)
            {
                UpdatePublishedFilePreviewFile(updateHandle, pchPreviewFile);
            }
            if (pchTitle != null)
            {
                UpdatePublishedFileTitle(updateHandle, pchTitle);
            }
            if (pchDescription != null)
            {
                UpdatePublishedFileDescription(updateHandle, pchDescription);
            }
            if (eVisibility.HasValue)
            {
                UpdatePublishedFileVisibility(updateHandle, eVisibility.Value);
            }
            if (pTags != null)
            {
                UpdatePublishedFileTags(updateHandle, pTags);
            }

            CommitPublishedFileUpdate(updateHandle);
        }

        private void UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
        {
           SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileFile(_remoteStorage, updateHandle, pchFile);
        }

        private void UpdatePublishedFilePreviewFile(PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
        {
            SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFilePreviewFile(_remoteStorage, updateHandle, pchPreviewFile);
        }

        private void UpdatePublishedFileTitle(PublishedFileUpdateHandle_t updateHandle, string pchTitle)
        {
            SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileTitle(_remoteStorage, updateHandle, pchTitle);
        }

        private void UpdatePublishedFileDescription(PublishedFileUpdateHandle_t updateHandle, string pchDescription)
        {
            SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileDescription(_remoteStorage, updateHandle, pchDescription);
        }

        private void UpdatePublishedFileVisibility(PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility)
        {
            SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileVisibility(_remoteStorage, updateHandle, eVisibility);
        }

        private void UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, ICollection<string> pTags)
        {
            SteamParamStringArray_t pTagsArray = new CommunityExpressNS.SteamParamStringArray_t(pTags);

            SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileTags(_remoteStorage, updateHandle, pTagsArray);

            pTagsArray.Free();
        }




        #region isteamugc methods

        // Query UGC associated with a user. Creator app id or consumer app id must be valid and be set to the current running app. unPage should start at 1.
	    public UGCQueryHandle_t CreateQueryUserUGCRequest( AccountID_t unAccountID, EUserUGCList eListType, EUGCMatchingUGCType eMatchingUGCType, EUserUGCListSortOrder eSortOrder, AppId_t nCreatorAppID, AppId_t nConsumerAppID, UInt32 unPage )
        {
            return SteamUnityAPI_UserGeneratedContent_CreateQueryUserUGCRequest(_steamUgc, unAccountID, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID, nCreatorAppID, unPage);
        }

	    // Query for all matching UGC. Creator app id or consumer app id must be valid and be set to the current running app. unPage should start at 1.
	    public UGCQueryHandle_t CreateQueryAllUGCRequest( EUGCQuery eQueryType, EUGCMatchingUGCType eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, UInt32 unPage )
        {
            return SteamUnityAPI_UserGeneratedContent_CreateQueryAllUGCRequest(_steamUgc, eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
        }

	    // Send the query to Steam
	    public SteamAPICall_t SendQueryUGCRequest( UGCQueryHandle_t handle )
        {
            return SteamUnityAPI_UserGeneratedContent_SendQueryUGCRequest(_steamUgc, handle);
        }

	    // Retrieve an individual result after receiving the callback for querying UGC
	    public bool GetQueryUGCResult( UGCQueryHandle_t handle, UInt32 index, SteamUGCDetails_t pDetails )
        {
            return SteamUnityAPI_UserGeneratedContent_GetQueryUGCResult(_steamUgc, handle, index, pDetails);
        }

	    // Release the request to free up memory, after retrieving results
	    public bool ReleaseQueryUGCRequest( UGCQueryHandle_t handle )
        {
            return SteamUnityAPI_UserGeneratedContent_ReleaseQueryUGCRequest(_steamUgc, handle);
        }

	    // Options to set for querying UGC
        public bool AddRequiredTag(UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.LPStr)] string pTagName)
        {
            return SteamUnityAPI_UserGeneratedContent_AddRequiredTag(_steamUgc, handle, pTagName);
        }

	    public bool AddExcludedTag( UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.LPStr)] string pTagName )
        {
            return SteamUnityAPI_UserGeneratedContent_AddExcludedTag(_steamUgc, handle, pTagName);
        }

	    public bool SetReturnLongDescription( UGCQueryHandle_t handle, bool bReturnLongDescription )
        {
            return SteamUnityAPI_UserGeneratedContent_SetReturnLongDescription(_steamUgc, handle, bReturnLongDescription);
        }

	    public bool SetReturnTotalOnly( UGCQueryHandle_t handle, bool bReturnTotalOnly )
        {
            return SteamUnityAPI_UserGeneratedContent_SetReturnTotalOnly(_steamUgc, handle, bReturnTotalOnly);
        }

	    public bool SetAllowCachedResponse( UGCQueryHandle_t handle, UInt32 unMaxAgeSeconds )
        {
            return SteamUnityAPI_UserGeneratedContent_SetAllowCachedResponse(_steamUgc, handle, unMaxAgeSeconds);
        }

	    // Options only for querying user UGC
	    public bool SetCloudFileNameFilter( UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.LPStr)] string pMatchCloudFileName )
        {
            return SteamUnityAPI_UserGeneratedContent_SetCloudFileNameFilter(_steamUgc, handle, pMatchCloudFileName);
        }

	    // Options only for querying all UGC
	    public bool SetMatchAnyTag( UGCQueryHandle_t handle, bool bMatchAnyTag )
        {
            return SteamUnityAPI_UserGeneratedContent_SetMatchAnyTag(_steamUgc, handle, bMatchAnyTag);
        }

	    public bool SetSearchText( UGCQueryHandle_t handle, [MarshalAs(UnmanagedType.LPStr)] string pSearchText )
        {
            return SteamUnityAPI_UserGeneratedContent_SetSearchText(_steamUgc, handle, pSearchText);
        }

	    public bool SetRankedByTrendDays( UGCQueryHandle_t handle, UInt32 unDays )
        {
            return SteamUnityAPI_UserGeneratedContent_SetRankedByTrendDays(_steamUgc, handle, unDays);
        }

	    // Request full details for one piece of UGC
	    SteamAPICall_t RequestUGCDetails( PublishedFileId_t nPublishedFileID, UInt32 unMaxAgeSeconds )
        {
            return SteamUnityAPI_UserGeneratedContent_RequestUGCDetails(_steamUgc, nPublishedFileID, unMaxAgeSeconds);
        }

        #endregion isteamugc methods

        //-----------------------------------------------------------------------------
        // Purpose: Callback for querying UGC
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct SteamUGCQueryCompleted_t
        {
            internal const int k_iCallback = Events.k_iClientUGCCallbacks + 1;
	        UGCQueryHandle_t m_handle;
	        EResult m_eResult;
	        UInt32 m_unNumResultsReturned;
	        UInt32 m_unTotalMatchingResults;
	        bool m_bCachedData;	// indicates whether this data was retrieved from the local on-disk cache
        }

        //-----------------------------------------------------------------------------
        // Purpose: Callback for requesting details on one piece of UGC
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct SteamUGCRequestUGCDetailsResult_t
        {
	        internal const int k_iCallback = Events.k_iClientUGCCallbacks + 2;
	        SteamUGCDetails_t m_details;
	        bool m_bCachedData; // indicates whether this data was retrieved from the local on-disk cache
        }

    }
   
}

