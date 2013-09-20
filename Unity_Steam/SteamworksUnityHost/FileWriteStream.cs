using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
    using UGCFileWriteStreamHandle_t = UInt64;

    /// <summary>
    /// Information about a write stream
    /// </summary>
    public class FileWriteStream
    {
        [DllImport("CommunityExpressSW")]
        private static extern IntPtr SteamUnityAPI_SteamRemoteStorage();
        [DllImport("CommunityExpressSW")]
        private static extern UGCFileWriteStreamHandle_t SteamUnityAPI_SteamRemoteStorage_FileWriteStreamOpen(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamRemoteStorage_FileWriteStreamWriteChunk(IntPtr remoteStorage, UGCFileWriteStreamHandle_t writeHandle, byte[] fileContents, int fileContentsLength);
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamRemoteStorage_FileWriteStreamClose(IntPtr remoteStorage, UGCFileWriteStreamHandle_t writeHandle);
        [DllImport("CommunityExpressSW")]
        private static extern Boolean SteamUnityAPI_SteamRemoteStorage_FileWriteStreamCancel(IntPtr remoteStorage, UGCFileWriteStreamHandle_t writeHandle);

        private IntPtr _remoteStorage;
        private UGCFileWriteStreamHandle_t _fileHandle;

        internal FileWriteStream(string fileName)
        {
            FileName = fileName;

            _remoteStorage = SteamUnityAPI_SteamRemoteStorage();
            _fileHandle = SteamUnityAPI_SteamRemoteStorage_FileWriteStreamOpen(_remoteStorage, fileName);
        }
        /// <summary>
        /// If the data chunk is written
        /// </summary>
        /// <param name="data">The data written</param>
        /// <returns>true if successfully written</returns>
        public bool WriteChunk( byte[] data )
        {
            return WriteChunk(data, data.Length);
        }
        /// <summary>
        /// If the data chunk is written
        /// </summary>
        /// <param name="data">The data written</param>
        /// <param name="length">Length of written data</param>
        /// <returns>true if successfully written</returns>
        public bool WriteChunk(byte[] data, int length)
        {
            SteamUnityAPI_SteamRemoteStorage_FileWriteStreamWriteChunk(_remoteStorage, _fileHandle, data, length);
            return false;
        }
        /// <summary>
        /// If the write is closed
        /// </summary>
        /// <returns>true if successfully closed</returns>
        public bool Close()
        {
            bool ret = SteamUnityAPI_SteamRemoteStorage_FileWriteStreamClose(_remoteStorage, _fileHandle);

            if (Closed != null) Closed(this, ret);

            return ret;
        }
        /// <summary>
        /// If the write is cancelled
        /// </summary>
        /// <returns>true if successfully cancelled</returns>
        public bool Cancel()
        {
            return SteamUnityAPI_SteamRemoteStorage_FileWriteStreamCancel(_remoteStorage, _fileHandle);
        }
        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }
        /// <summary>
        /// Closes stream
        /// </summary>
        /// <param name="sender">Sender of request</param>
        /// <param name="success">Is stream closed</param>
        public delegate void CloseHandler(FileWriteStream sender, bool success);
        /// <summary>
        /// Write stream is closed
        /// </summary>
        public event CloseHandler Closed;
    }
}
