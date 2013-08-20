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

        EResult m_eResult;				// The result of the operation.
        Int32 m_nResultsReturned;
        Int32 m_nTotalResultCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        PublishedFileId_t[] m_rgPublishedFileId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        UInt32[] m_rgRTimeSubscribed;
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
    internal struct RemoteStoragePublishFileProgress_t
    {
        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 29;

	    internal double m_dPercentFile;
        internal bool m_bPreview;
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
            k_EWorkshopFileTypeFirst = 0,

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
            k_EWorkshopFileTypeMax = 11

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
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_GetPublishedFileDetails(IntPtr remoteStorage, PublishedFileUpdateHandle_t updateHandle);
      
        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_DeletePublishedFile(IntPtr remoteStorage, PublishedFileUpdateHandle_t updateHandle);
        
        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_PublishWorkshopFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] string pchFile, [MarshalAs(UnmanagedType.LPStr)] string pchPreviewFile, AppId_t nConsumerAppId, [MarshalAs(UnmanagedType.LPStr)] string pchTitle, [MarshalAs(UnmanagedType.LPStr)] string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, SteamParamStringArray_t pTags, EWorkshopFileType eWorkshopFileType);
        
        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_CreatePublishedFileUpdateRequest(IntPtr remoteStorage, PublishedFileUpdateHandle_t updateHandle);
        
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

        internal UserGeneratedContent(CommunityExpress ce)
        {
            _ce = ce;
            _remoteStorage = SteamUnityAPI_SteamRemoteStorage();

            _userSharedFilesHandler = new CommunityExpress.OnEventHandler<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t>(Events_UserSharedWorkshopFilesReceived);



            CommunityExpress.OnEventHandler<RemoteStoragePublishFileProgress_t> h = new CommunityExpress.OnEventHandler<RemoteStoragePublishFileProgress_t>(Events_PublishFileProgressReceived);
            _ce.AddEventHandler(RemoteStoragePublishFileProgress_t.k_iCallback, h);

            CommunityExpress.OnEventHandler<RemoteStorageEnumerateUserPublishedFilesResult_t> h2 = new CommunityExpress.OnEventHandler<RemoteStorageEnumerateUserPublishedFilesResult_t>(Events_UserPublishedFilesResultReceived);
            _ce.AddEventHandler(RemoteStorageEnumerateUserPublishedFilesResult_t.k_iCallback, h2);
        }

        private void Events_PublishFileProgressReceived(RemoteStoragePublishFileProgress_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            Console.WriteLine(recv.m_dPercentFile);
        }

        private void Events_UserPublishedFilesResultReceived(RemoteStorageEnumerateUserPublishedFilesResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            Console.WriteLine(recv.m_eResult);
        }

        private void Events_UserSharedWorkshopFilesReceived(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _ce.RemoveEventHandler(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.k_iCallback, _userSharedFilesHandler);

            // TODO: Send event
            // if (UserStatsReceived != null) UserStatsReceived(this, new UserStatsReceivedArgs(recv));
        }
        
        public void EnumeratePublishedWorkshopFiles( EWorkshopEnumerationType eEnumerationType, UInt32 unStartIndex, UInt32 unCount, UInt32 unDays, ICollection<string> tags, ICollection<string> userTags )
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedWorkshopFiles(_remoteStorage, eEnumerationType, unStartIndex, unCount, unDays);

            Console.WriteLine(ret);

        }

        public void EnumerateUserPublishedFiles(UInt32 unStartIndex)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_EnumerateUserPublishedFiles(_remoteStorage, unStartIndex);

            Console.WriteLine(ret);
        }

        public void EnumerateUserSubscribedFiles( UInt32 unStartIndex )
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_EnumerateUserSubscribedFiles(_remoteStorage, unStartIndex);

            Console.WriteLine(ret);

        }

        public void SubscribePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_SubscribePublishedFile(_remoteStorage, unPublishedFileId);

            Console.WriteLine(ret);
        }

        public void UnubscribePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UnsubscribePublishedFile(_remoteStorage, unPublishedFileId);

            Console.WriteLine(ret);
        }

        public void GetPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_GetPublishedItemVoteDetails(_remoteStorage, unPublishedFileId);

            Console.WriteLine(ret);
        }

        public void UpdateUserPublishedItemVote(PublishedFileId_t unPublishedFileId, bool bVoteUp)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UpdateUserPublishedItemVote(_remoteStorage, unPublishedFileId, bVoteUp);

            Console.WriteLine(ret);
        }

        public void GetUserPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_GetUserPublishedItemVoteDetails(_remoteStorage, unPublishedFileId);

            Console.WriteLine(ret);
        }

        public void EnumerateUserSharedWorkshopFiles(SteamID steamId, UInt32 unStartIndex, ICollection<string> pRequiredTags, ICollection<string> pExcludedTags)
        {
            _ce.AddEventHandler(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.k_iCallback, _userSharedFilesHandler);

            SteamParamStringArray_t pRequiredTagsArray = new CommunityExpressNS.SteamParamStringArray_t(pRequiredTags);
            SteamParamStringArray_t pExcludedTagsArray = new CommunityExpressNS.SteamParamStringArray_t(pExcludedTags);

            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_EnumerateUserSharedWorkshopFiles(_remoteStorage, steamId.ToUInt64(), unStartIndex, pRequiredTagsArray, pExcludedTagsArray);

            pRequiredTagsArray.Free();
            pExcludedTagsArray.Free();

            Console.WriteLine(ret);
        }
        
        /*
        public void PublishVideo(EWorkshopVideoProvider eVideoProvider, const char *pchVideoAccount, const char *pchVideoIdentifier, const char *pchPreviewFile, AppId_t nConsumerAppId, const char *pchTitle, const char *pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, SteamParamStringArray_t *pTags)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_PublishVideo(_remoteStorage,  eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags);

            Console.WriteLine(ret);
        }
         */
        public void SetUserPublishedFileAction(PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_SetUserPublishedFileAction(_remoteStorage, unPublishedFileId, eAction);

            Console.WriteLine(ret);
        }

        public void EnumeratePublishedFilesByUserAction(EWorkshopFileAction eAction, UInt32 unStartIndex)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedFilesByUserAction(_remoteStorage, eAction, unStartIndex);

            Console.WriteLine(ret);
        }

        public void UGCDownloadToLocation(UGCHandle_t hContent, string pchLocation, UInt32 unPriority)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UGCDownloadToLocation(_remoteStorage, hContent, pchLocation, unPriority);

            Console.WriteLine(ret);
        }

        public void CommitPublishedFileUpdate(PublishedFileUpdateHandle_t updateHandle)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_CommitPublishedFileUpdate(_remoteStorage, updateHandle);

            Console.WriteLine(ret);
        }

        public void GetPublishedFileDetails(PublishedFileUpdateHandle_t updateHandle)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_GetPublishedFileDetails(_remoteStorage, updateHandle);

            Console.WriteLine(ret);
        }

        public void DeletePublishedFile(PublishedFileUpdateHandle_t updateHandle)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_DeletePublishedFile(_remoteStorage, updateHandle);

            Console.WriteLine(ret);
        }
        
        public void PublishWorkshopFile(string pchFile, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, ICollection<string> pTags, EWorkshopFileType eWorkshopFileType)
        {
            SteamParamStringArray_t pTagsArray = new CommunityExpressNS.SteamParamStringArray_t(pTags);

            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_PublishWorkshopFile(_remoteStorage, pchFile, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTagsArray, eWorkshopFileType);

            pTagsArray.Free();

            Console.WriteLine(ret);
        }

        public void CreatePublishedFileUpdateRequest(PublishedFileUpdateHandle_t updateHandle)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_CreatePublishedFileUpdateRequest(_remoteStorage, updateHandle);

            Console.WriteLine(ret);
        }

        public void UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileFile(_remoteStorage, updateHandle, pchFile);

            Console.WriteLine(ret);
        }

        public void UpdatePublishedFilePreviewFile(PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFilePreviewFile(_remoteStorage, updateHandle, pchPreviewFile);

            Console.WriteLine(ret);
        }

        public void UpdatePublishedFileTitle(PublishedFileUpdateHandle_t updateHandle, string pchTitle)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileTitle(_remoteStorage, updateHandle, pchTitle);

            Console.WriteLine(ret);
        }

        public void UpdatePublishedFileDescription(PublishedFileUpdateHandle_t updateHandle, string pchDescription)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileDescription(_remoteStorage, updateHandle, pchDescription);

            Console.WriteLine(ret);
        }

        public void UpdatePublishedFileVisibility(PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility)
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileVisibility(_remoteStorage, updateHandle, eVisibility);

            Console.WriteLine(ret);
        }

        public void UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, ICollection<string> pTags)
        {
            SteamParamStringArray_t pTagsArray = new CommunityExpressNS.SteamParamStringArray_t(pTags);

            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_UpdatePublishedFileTags(_remoteStorage, updateHandle, pTagsArray);

            pTagsArray.Free();

            Console.WriteLine(ret);
        }
    }
}

