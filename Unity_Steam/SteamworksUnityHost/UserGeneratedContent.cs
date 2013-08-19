using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
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


        [DllImport("CommunityExpressSW")]
        private static extern IntPtr SteamUnityAPI_SteamRemoteStorage();

        [DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedWorkshopFiles(IntPtr remoteStorage, EWorkshopEnumerationType eEnumerationType, UInt32 unStartIndex, UInt32 unCount, UInt32 unDays);
        
        [DllImport("CommunityExpressSW")]
		private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_EnumerateUserSubscribedFiles(IntPtr remoteStorage, UInt32 unStartIndex);

        private IntPtr _remoteStorage;

        internal UserGeneratedContent()
        {
            _remoteStorage = SteamUnityAPI_SteamRemoteStorage();
        }
        
        public void EnumeratePublishedWorkshopFiles( EWorkshopEnumerationType eEnumerationType, UInt32 unStartIndex, UInt32 unCount, UInt32 unDays, ICollection<string> tags, ICollection<string> userTags )
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_EnumeratePublishedWorkshopFiles(_remoteStorage, eEnumerationType, unStartIndex, unCount, unDays);

            Console.WriteLine(ret);

        }

        public void EnumerateUserSubscribedFiles( UInt32 unStartIndex )
        {
            UInt64 ret = SteamUnityAPI_SteamRemoteStorage_EnumerateUserSubscribedFiles(_remoteStorage, unStartIndex);

            Console.WriteLine(ret);

        }
    }
}

