using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SteamworksUnityHost
{
    public class RemoteStorage
    {
        [DllImport("SteamworksUnity.dll")]
        private static extern IntPtr SteamUnityAPI_SteamRemoteStorage();
        [DllImport("SteamworksUnity.dll")]
        private static extern int SteamUnityAPI_SteamRemoteStorage_GetFileCount(IntPtr remoteStorage);
        [DllImport("SteamworksUnity.dll")]
        private static extern IntPtr SteamUnityAPI_SteamRemoteStorage_GetFileNameAndSize(IntPtr remoteStorage, int iFile, out int nFileSizeInBytes);
        [DllImport("SteamworksUnity.dll")]
        private static extern int SteamUnityAPI_SteamRemoteStorage_FileRead(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] string fileName, 
           [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder sb, int nSize);

        private IntPtr _remoteStorage;

        internal RemoteStorage()
        {
            _remoteStorage = SteamUnityAPI_SteamRemoteStorage();
        }

        public int FileCount
        {
            get { return SteamUnityAPI_SteamRemoteStorage_GetFileCount(_remoteStorage); }
        }

        // Could use an interator class or something here!
        public string GetFileNameAndSize(int iFile, out int nFileSizeInBytes)
        {
            IntPtr fileName = SteamUnityAPI_SteamRemoteStorage_GetFileNameAndSize(_remoteStorage, iFile, out nFileSizeInBytes);
            return Marshal.PtrToStringAnsi(fileName);
        }

        // Could use an interator class or something here!
        public string FileRead(string fileName, int nFileSizeInBytes)
        {
            StringBuilder sb = new StringBuilder(nFileSizeInBytes);
            SteamUnityAPI_SteamRemoteStorage_FileRead(_remoteStorage, fileName, sb, nFileSizeInBytes);
            return sb.ToString();
        }

    }
}
