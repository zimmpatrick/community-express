using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CommunityExpressNS
{
    using SteamAPICall_t = UInt64;
    using PublishedFileId_t = UInt64;
    using UGCHandle_t = UInt64;
    using PublishedFileUpdateHandle_t = UInt64;
    using AppId_t = UInt32;

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

    public class UserGeneratedContent
    {
        public enum EWorkshopFileType
        {
            // k_EWorkshopFileTypeFirst = 0,

            k_EWorkshopFileTypeCommunity = 0,
            k_EWorkshopFileTypeMicrotransaction = 1,
            k_EWorkshopFileTypeCollection = 2,
            k_EWorkshopFileTypeArt = 3,
            k_EWorkshopFileTypeVideo = 4,
            k_EWorkshopFileTypeScreenshot = 5,
            k_EWorkshopFileTypeGame = 6,
            k_EWorkshopFileTypeSoftware = 7,
            k_EWorkshopFileTypeConcept = 8,
            k_EWorkshopFileTypeWebGuide = 9,
            k_EWorkshopFileTypeIntegratedGuide = 10,

            // Update k_EWorkshopFileTypeMax if you add values
            // k_EWorkshopFileTypeMax = 11

        };

        public enum EWorkshopEnumerationType
        {
            k_EWorkshopEnumerationTypeRankedByVote = 0,
            k_EWorkshopEnumerationTypeRecent = 1,
            k_EWorkshopEnumerationTypeTrending = 2,
            k_EWorkshopEnumerationTypeFavoritesOfFriends = 3,
            k_EWorkshopEnumerationTypeVotedByFriends = 4,
            k_EWorkshopEnumerationTypeContentByFriends = 5,
            k_EWorkshopEnumerationTypeRecentFromFollowedUsers = 6,
        };

        public enum EWorkshopFileAction
        {
            k_EWorkshopFileActionPlayed = 0,
            k_EWorkshopFileActionCompleted = 1,
        };

        public enum ERemoteStoragePublishedFileVisibility
        {
            k_ERemoteStoragePublishedFileVisibilityPublic = 0,
            k_ERemoteStoragePublishedFileVisibilityFriendsOnly = 1,
            k_ERemoteStoragePublishedFileVisibilityPrivate = 2,
        };

        enum EWorkshopVideoProvider
        {
            k_EWorkshopVideoProviderNone = 0,
            k_EWorkshopVideoProviderYoutube = 1
        };

        public class PublishedFileDownloadResultArgs : System.EventArgs
        {
            internal PublishedFileDownloadResultArgs(PublishedFile publishedFile, bool primaryFile, string fileName)
            {
                PublishedFile = publishedFile;
                PrimaryFile = primaryFile;
                FileName = fileName;
            }

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

            public PublishedFile PublishedFile
            {
                get;
                private set;
            }

            public string FileName
            {
                get;
                private set;
            }
        }

        public class PublishedFile
        {
            [DllImport("CommunityExpressSW")]
            private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_UGCDownload(IntPtr remoteStorage, UGCHandle_t hFile, UInt32 unPriority);

            internal PublishedFile(RemoteStorageGetPublishedFileDetailsResult_t args, UInt32 subscribe, IntPtr remoteStorage)
            {
                RemoteStorage = remoteStorage;

                // Result = args.m_eResult;
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

            public string Title
            {
                get;
                private set;
            }

            public string Description
            {
                get;
                private set;
            }

            public string FileName
            {
                get;
                private set;
            }

            public string PreviewFileName
            {
                get;
                internal set;
            }

            public SteamID OwnerSteamID
            {
                get;
                private set;
            }

            public string[] Tags
            {
                get;
                private set;
            }

            public bool TagsTruncated
            {
                get;
                private set;
            }

            public Int32 FileSize
            {
                get;
                private set;
            }

            public string URL
            {
                get;
                private set;
            }

            public EWorkshopFileType FileType
            {
                get;
                private set;
            }

            public DateTime CreatedDate
            {
                get;
                private set;
            }

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

            public string FileDirectory
            {
                get
                {
                    int dir = FileName.LastIndexOf('/');
                    if (dir <= 0) return string.Empty;

                    return FileName.Substring(0, dir - 1);
                }
            }

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
            public void Download(string fileName, UInt32 priority)
            {
                // SteamUnityAPI_SteamRemoteStorage_UGCDownload(RemoteStorage, hFile, priority);
                string fullFileName = System.IO.Path.GetFullPath(fileName);

                System.IO.Directory.CreateDirectory(
                    System.IO.Path.GetDirectoryName(fullFileName));

                UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UGCDownloadToLocation(RemoteStorage, hFile, fullFileName, 0);
                CommunityExpress.Instance.UserGeneratedContent.AddUGCDownload(ret, new PublishedFileDownloadResultArgs(this, true, fileName));
            }

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
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedWorkshopFiles(IntPtr remoteStorage, EWorkshopEnumerationType eEnumerationType, UInt32 unStartIndex, UInt32 unCount, UInt32 unDays);

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


        private IntPtr _remoteStorage;
        private CommunityExpress _ce;
        private CommunityExpress.OnEventHandler<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t> _userSharedFilesHandler;
        private Dictionary<UInt64, PublishedFileDownloadResultArgs> _ugcDownloads = new Dictionary<PublishedFileUpdateHandle_t, PublishedFileDownloadResultArgs>();

        internal UserGeneratedContent(CommunityExpress ce)
        {
            _ce = ce;
            _remoteStorage = SteamUnityAPI_SteamRemoteStorage();

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

            public EResult Result
            {
                get;
                private set;
            }

            public UInt32 StartIndex
            {
                get;
                private set;
            }

            public Int32 TotalCount
            {
                get;
                private set;
            }

            public ICollection<PublishedFile> PublishedFiles
            {
                get;
                private set;
            }
        }

        public class PublishedFileDetailsResultArgs : System.EventArgs
        {
            internal PublishedFileDetailsResultArgs(PublishedFile publishedFile, EResult result)
            {
                Result = result;
                PublishedFile = publishedFile;
            }

            public EResult Result
            {
                get;
                private set;
            }

            public PublishedFile PublishedFile
            {
                get;
                private set;
            }
        }

        public class PublishedFileResultArgs : System.EventArgs
        {
            internal PublishedFileResultArgs(EResult result,
                PublishedFileId_t nPublishedFileId)
            {
                Result = result;
                PublishedFileId = nPublishedFileId;
            }

            public EResult Result
            {
                get;
                private set;
            }

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

        private void Events_GetUserSubscribedFilesReceived(RemoteStorageEnumerateUserSubscribedFilesResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _totalCount = recv.m_nTotalResultCount;
            _subscribeTimes = new Dictionary<PublishedFileUpdateHandle_t, AppId_t>();

            for (int i = 0; i < recv.m_nResultsReturned; i++)
            {
                _subscribeTimes.Add(recv.m_rgPublishedFileId[i], recv.m_rgRTimeSubscribed[i]);
                GetPublishedFileDetails(recv.m_rgPublishedFileId[i]);
            }
        }

        public delegate void PublishedFileDownloadedHandler(UserGeneratedContent sender, PublishedFileDownloadResultArgs file);
        public event PublishedFileDownloadedHandler PublishedFileDownloaded;
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

        public delegate void RemoteStorageUpdatePublishedFileResultHandler(UserGeneratedContent sender, PublishedFileResultArgs result);
        public event RemoteStorageUpdatePublishedFileResultHandler FileUpdated;

        private void Events_UpdatePublishedFileFinished(RemoteStorageUpdatePublishedFileResult_t recv, bool Success, SteamAPICall_t hSteamAPICall)
        {
            if (FileUpdated != null)
            {
                FileUpdated(this, new PublishedFileResultArgs(recv.m_eResult, recv.m_nPublishedFileId));
            }
        }

        public delegate void RemoteStorageSubscribePublishedFileResultHandler(UserGeneratedContent sender, PublishedFileResultArgs result);
        public event RemoteStorageSubscribePublishedFileResultHandler FileSubscribed;

        private void Events_SubscribePublishedFileResult(RemoteStorageSubscribePublishedFileResult_t recv, bool Success, SteamAPICall_t hSteamAPICall)
        {
            if (FileSubscribed != null)
            {
                FileSubscribed(this, new PublishedFileResultArgs(recv.m_eResult, recv.m_nPublishedFileId));
            }
        }

        public delegate void RemoteStorageUnsubscribePublishedFileResultHandler(UserGeneratedContent sender, PublishedFileResultArgs result);
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

        public delegate void RemoteStoragePublishedFileResultHandler(UserGeneratedContent sender, PublishedFileResultArgs result);
        public event RemoteStoragePublishedFileResultHandler FilePublished;
        
        private void Events_FilePublished(RemoteStoragePublishFileResult_t recv, bool Success, SteamAPICall_t hSteamAPICall)
        {
            if (FilePublished != null)
            {
                FilePublished(this, new PublishedFileResultArgs(EResult.EResultOK, recv.m_nPublishedFileId));
            }
        }
        
        public delegate void RemoteStorageDeletedPublishedFileResultHandler(UserGeneratedContent sender, PublishedFileResultArgs result);
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

        public delegate void RemoteStoragePublishFileProgressHandler(UserGeneratedContent sender, float progress);
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
                }

                if (FileDetails != null)
                {
                    FileDetails(this, new PublishedFileDetailsResultArgs(pf, EResult.EResultOK));
                }

                deleteList.Add(pf);
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

                if (pf.PreviewFileName != null) goodFiles++;
            }

            if (goodFiles == _subscribeTimes.Count)
            {
                if (EnumerateFileDetails != null)
                {
                    EnumerateFileDetails(this, new EnumeratePublishedFileResultArgs(_publishedFiles, EResult.EResultOK, _offset, _totalCount));

                    _subscribeTimes.Clear();
                    _publishedFiles = null;
                }
            }
        }
        
        private void Events_UserPublishedFilesResultReceived(RemoteStorageEnumerateUserPublishedFilesResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _totalCount = recv.m_nTotalResultCount;
            _subscribeTimes = new Dictionary<PublishedFileUpdateHandle_t, AppId_t>();

            for (int i = 0; i < recv.m_nResultsReturned; i++)
            {
                _subscribeTimes.Add(recv.m_rgPublishedFileId[i], 0);
                GetPublishedFileDetails(recv.m_rgPublishedFileId[i]);
            }
        }

        private void Events_EnumerateWorkshopFilesResultReceived(RemoteStorageEnumerateWorkshopFilesResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _totalCount = recv.m_nTotalResultCount;
            _subscribeTimes = new Dictionary<PublishedFileUpdateHandle_t, AppId_t>();

            for (int i = 0; i < recv.m_nResultsReturned; i++)
            {
                _subscribeTimes.Add(recv.m_rgPublishedFileId[i], 0);
                GetPublishedFileDetails(recv.m_rgPublishedFileId[i]);
            }
        }

        public delegate void PublishedFileDetailsResultHandler(UserGeneratedContent sender, PublishedFileDetailsResultArgs args);
        public event PublishedFileDetailsResultHandler FileDetails;

        public delegate void EnumeratePublishedFileResultHandler(UserGeneratedContent sender, EnumeratePublishedFileResultArgs args);
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
                    if (recv.m_eResult == EResult.EResultOK)
                    {
                        PublishedFile pf = new PublishedFile(recv, _subscribeTimes[recv.m_nPublishedFileId], _remoteStorage);
                        pf.InernalDownloadPreview(0);

                        _publishedFiles.Add(pf);
                    }
                    else
                    {
                        _publishedFiles = null; // signal done
                        EnumerateFileDetails(this,
                            new EnumeratePublishedFileResultArgs(null, recv.m_eResult, _offset, _totalCount));
                    }
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
                else
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
        /// 
        /// </summary>
        /// <param name="eEnumerationType"></param>
        /// <param name="unStartIndex"></param>
        /// <param name="unCount"></param>
        /// <param name="unDays"></param>
        /// <param name="tags"></param>
        /// <param name="userTags"></param>
        public void EnumeratePublishedWorkshopFiles(EWorkshopEnumerationType eEnumerationType, UInt32 unStartIndex, UInt32 unCount, UInt32 unDays, ICollection<string> tags, ICollection<string> userTags)
        {
            _offset = unStartIndex;
            _totalCount = 0;
            _publishedFiles = new List<PublishedFile>();

            SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedWorkshopFiles(_remoteStorage, eEnumerationType, unStartIndex, unCount, unDays);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unStartIndex"></param>
        public void EnumerateUserPublishedFiles(UInt32 unStartIndex)
        {
            _offset = unStartIndex;
            _totalCount = 0;
            _publishedFiles = new List<PublishedFile>();

            SteamUnityAPI_SteamRemoteStorage_EnumerateUserPublishedFiles(_remoteStorage, unStartIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unStartIndex"></param>
        public void EnumerateUserSubscribedFiles(UInt32 unStartIndex)
        {
            _offset = unStartIndex;
            _totalCount = 0;
            _publishedFiles = new List<PublishedFile>();

            SteamUnityAPI_SteamRemoteStorage_EnumerateUserSubscribedFiles(_remoteStorage, unStartIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unPublishedFile"></param>
        public void SubscribePublishedFile(PublishedFile unPublishedFile)
        {
            SteamUnityAPI_SteamRemoteStorage_SubscribePublishedFile(_remoteStorage, unPublishedFile.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unPublishedFile"></param>
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
        /// 
        /// </summary>
        /// <param name="unPublishedFile"></param>
        /// <param name="bVoteUp"></param>
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
        /// 
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="unStartIndex"></param>
        /// <param name="pRequiredTags"></param>
        /// <param name="pExcludedTags"></param>
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
        /// 
        /// </summary>
        /// <param name="unPublishedFile"></param>
        /// <param name="eAction"></param>
        public void SetUserPublishedFileAction(PublishedFile unPublishedFile, EWorkshopFileAction eAction)
        {
            SteamUnityAPI_SteamRemoteStorage_SetUserPublishedFileAction(_remoteStorage, unPublishedFile.ID, eAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eAction"></param>
        /// <param name="unStartIndex"></param>
        public void EnumeratePublishedFilesByUserAction(EWorkshopFileAction eAction, UInt32 unStartIndex)
        {
            SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedFilesByUserAction(_remoteStorage, eAction, unStartIndex);
        }

        private void CommitPublishedFileUpdate(PublishedFileUpdateHandle_t unPublishedFileId)
        {
            SteamUnityAPI_SteamRemoteStorage_CommitPublishedFileUpdate(_remoteStorage, unPublishedFileId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public void DeletePublishedFile(PublishedFile file)
        {
            SteamUnityAPI_SteamRemoteStorage_DeletePublishedFile(_remoteStorage, file.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unPublishedFileId"></param>
        public void GetPublishedFileDetails(PublishedFileId_t unPublishedFileId)
        {
            SteamUnityAPI_SteamRemoteStorage_GetPublishedFileDetails(_remoteStorage, unPublishedFileId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pchFile"></param>
        /// <param name="pchPreviewFile"></param>
        /// <param name="nConsumerAppId"></param>
        /// <param name="pchTitle"></param>
        /// <param name="pchDescription"></param>
        /// <param name="eVisibility"></param>
        /// <param name="pTags"></param>
        /// <param name="eWorkshopFileType"></param>
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
        /// 
        /// </summary>
        /// <param name="unPublishedFileId"></param>
        /// <param name="pchFile"></param>
        /// <param name="pchPreviewFile"></param>
        /// <param name="pchTitle"></param>
        /// <param name="pchDescription"></param>
        /// <param name="eVisibility"></param>
        /// <param name="pTags"></param>
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
    }
}

