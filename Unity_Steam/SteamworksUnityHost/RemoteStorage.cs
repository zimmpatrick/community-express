// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
    using UGCHandle_t = UInt64;
    using SteamAPICall_t = UInt64;
    /// <summary>
    /// Steam Cloud file storage
    /// </summary>
	public class RemoteStorage : ICollection<File>
	{
        int _fsCount = 0;

		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamRemoteStorage();
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamRemoteStorage_GetFileCount(IntPtr remoteStorage);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamRemoteStorage_GetFileSize(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamRemoteStorage_GetFileNameAndSize(IntPtr remoteStorage, int iFile, out int nFileSizeInBytes);
		[DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_WriteFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName, IntPtr fileContents, int fileContentsLength);
		[DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_ForgetFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_DeleteFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_FileExists(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_FilePersisted(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_GetQuota(IntPtr remoteStorage, out Int32 totalSpace, out Int32 availableSpace);
        [DllImport("CommunityExpressSW")]
        private static extern UInt64 SteamUnityAPI_SteamRemoteStorage_FileShare(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
        
	    [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct RemoteStorageFileShareResult_t
        {
            internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 7;

            internal EResult m_eResult;			// The result of the operation
            internal UGCHandle_t m_hFile;		// The handle that can be shared with users and features
        };
        /// <summary>
        /// Arguments for sharing file
        /// </summary>
        public class RemoteStorageFileShareResultArgs : System.EventArgs
        {
            internal RemoteStorageFileShareResultArgs(RemoteStorageFileShareResult_t args, int remainingfiles)
            {
                Result = args.m_eResult;
                RemainingFiles = remainingfiles;
            }
            /// <summary>
            /// Result of sharing
            /// </summary>
            public EResult Result
            {
                get;
                private set;
            }
            /// <summary>
            /// Files left to be shared
            /// </summary>
            public int RemainingFiles
            {
                get;
                private set;
            }
        }
        /// <summary>
        /// Result of shared file
        /// </summary>
        /// <param name="sender">User who sent the file</param>
        /// <param name="args">Result arguments</param>
        public delegate void RemoteStorageFileShareResultHandler(RemoteStorage sender, RemoteStorageFileShareResultArgs args);
        /// <summary>
        /// File is shared
        /// </summary>
        public event RemoteStorageFileShareResultHandler FileShared;
        /// <summary>
        /// Arguments for stream closing
        /// </summary>
        public class FileWriteStreamCloseArgs : System.EventArgs
        {
            internal FileWriteStreamCloseArgs(FileWriteStream stream, bool success)
            {
                FileWriteStream = stream;
                Success = success;
            }
            /// <summary>
            /// Stream is still up
            /// </summary>
            public FileWriteStream FileWriteStream
            {
                get;
                private set;
            }
            /// <summary>
            /// Successfully closed
            /// </summary>
            public bool Success
            {
                get;
                private set;
            }
        }
        /// <summary>
        /// Close the write stream to a file
        /// </summary>
        /// <param name="sender">User who sent the file</param>
        /// <param name="args">Close arguments</param>
        public delegate void FileWriteStreamCloseHandler(RemoteStorage sender, FileWriteStreamCloseArgs args);
        /// <summary>
        /// Write stream is closed
        /// </summary>
        public event FileWriteStreamCloseHandler FileWriteStreamClosed;
        
		private IntPtr _remoteStorage;

		private class FileEnumator : IEnumerator<File>
		{
			private int _index;
			private RemoteStorage _remoteStorage;

			public FileEnumator(RemoteStorage remoteStorage)
			{
				_remoteStorage = remoteStorage;
				_index = -1;
			}

			public File Current
			{
				get
				{
					int fileSize;
					IntPtr fileName = SteamUnityAPI_SteamRemoteStorage_GetFileNameAndSize(_remoteStorage._remoteStorage, _index, out fileSize);
					return new File(Marshal.PtrToStringAnsi(fileName), fileSize);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public bool MoveNext()
			{
				_index++;
				return _index < _remoteStorage.Count;
			}

			public void Reset()
			{
				_index = -1;
			}

			public void Dispose()
			{
			}
		}

        private CommunityExpress _ce;

		internal RemoteStorage(CommunityExpress ce)
		{
            _ce = ce;
			_remoteStorage = SteamUnityAPI_SteamRemoteStorage();


            CommunityExpress.OnEventHandler<RemoteStorageFileShareResult_t> h = new CommunityExpress.OnEventHandler<RemoteStorageFileShareResult_t>(Events_FileShareResultReceived);
            _ce.AddEventHandler(RemoteStorageFileShareResult_t.k_iCallback, h);
		}

        private void Events_FileShareResultReceived(RemoteStorageFileShareResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (FileShared != null)
            {
                _fsCount--;
                FileShared(this, new RemoteStorageFileShareResultArgs(recv, _fsCount));
            }
        }

		internal IntPtr SteamPointer
		{
			get { return _remoteStorage; }
		}
        /// <summary>
        /// Write file to storage in string form
        /// </summary>
        /// <param name="fileName">File to write</param>
        /// <param name="fileContents">Contents of file in string</param>
		public void WriteFile(String fileName, String fileContents)
		{
			SteamUnityAPI_SteamRemoteStorage_WriteFile(_remoteStorage, fileName, Marshal.StringToHGlobalAnsi(fileContents), fileContents.Length);
		}
        /// <summary>
        /// Write file to storage in byte form
        /// </summary>
        /// <param name="fileName">File to write</param>
        /// <param name="fileContents">Contents of file in byte</param>
		public void WriteFile(String fileName, Byte[] fileContents)
		{
			IntPtr fileContentsPtr = Marshal.AllocHGlobal(fileContents.Length);
			Marshal.Copy(fileContents, 0, fileContentsPtr, fileContents.Length);

			SteamUnityAPI_SteamRemoteStorage_WriteFile(_remoteStorage, fileName, fileContentsPtr, fileContents.Length);

			Marshal.FreeHGlobal(fileContentsPtr);
		}

        /// <summary>
        /// Begins writing file to storage
        /// </summary>
        /// <param name="fileName">File to store</param>
        /// <returns>true if begun</returns>
        public FileWriteStream BeginWriteFile(String fileName)
        {
            return new FileWriteStream(fileName);
        }

        /// <summary>
        /// Uploads file to storage asynchronously
        /// </summary>
        /// <param name="localFileName">File on local network</param>
        /// <param name="remoteFileName">File in storage</param>
        public void AsyncWriteUpload(String localFileName, String remoteFileName)
        {
            FileWriteStream stream = BeginWriteFile(remoteFileName);

            stream.Closed += new FileWriteStream.CloseHandler(Stream_Closed);

            _ce.AddFileWriterUpload(localFileName, stream);

            _fsCount++;
        }

        void Stream_Closed(FileWriteStream sender, bool success)
        {
            if (FileWriteStreamClosed != null)
            {
                FileWriteStreamClosed(this, new FileWriteStreamCloseArgs(sender, success));

                sender.Closed -= new FileWriteStream.CloseHandler(Stream_Closed);
            }
        }
        /// <summary>
        /// Gets a file
        /// </summary>
        /// <param name="fileName">File to get</param>
        /// <returns>true if gotten</returns>
		public File GetFile(String fileName)
		{
			return new File(fileName, GetFileSize(fileName));
		}
        /// <summary>
        /// Gets size of file
        /// </summary>
        /// <param name="fileName">File to check</param>
        /// <returns>true if file size found</returns>
		public Int32 GetFileSize(String fileName)
		{
			return SteamUnityAPI_SteamRemoteStorage_GetFileSize(_remoteStorage, fileName);
		}
        /// <summary>
        /// Forgets a file
        /// </summary>
        /// <param name="fileName">File to forget</param>
		public void ForgetFile(String fileName)
		{
			SteamUnityAPI_SteamRemoteStorage_ForgetFile(_remoteStorage, fileName);
		}
        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="fileName">File to delete</param>
		public void DeleteFile(String fileName)
		{
			SteamUnityAPI_SteamRemoteStorage_DeleteFile(_remoteStorage, fileName);
		}
        /// <summary>
        /// If file exists
        /// </summary>
        /// <param name="fileName">File to check</param>
        /// <returns>true if found</returns>
		public Boolean FileExists(String fileName)
		{
			return GetFileSize(fileName) > 0;
		}
        /// <summary>
        /// If file is saved
        /// </summary>
        /// <param name="fileName">File to check</param>
        /// <returns>true if saved</returns>
		public Boolean FilePersisted(String fileName)
		{
			return SteamUnityAPI_SteamRemoteStorage_FilePersisted(_remoteStorage, fileName);
		}
        /// <summary>
        /// Share a file
        /// </summary>
        /// <param name="fileName">File to share</param>
        public void FileShare(String fileName)
        {
            SteamAPICall_t ret = SteamUnityAPI_SteamRemoteStorage_FileShare(_remoteStorage, fileName);
        }
        /// <summary>
        /// Available storage space
        /// </summary>
		public Int32 AvailableSpace
		{
			get
			{
				Int32 totalSpace, availableSpace;

				SteamUnityAPI_SteamRemoteStorage_GetQuota(_remoteStorage, out totalSpace, out availableSpace);

				return availableSpace;
			}
		}
        /// <summary>
        /// Counts files in storage
        /// </summary>
		public int Count
		{
			get { return SteamUnityAPI_SteamRemoteStorage_GetFileCount(_remoteStorage); }
		}
        /// <summary>
        /// If storage is read-only
        /// </summary>
		public bool IsReadOnly
		{
			get { return true; }
		}
        /// <summary>
        /// Adds file to storage
        /// </summary>
        /// <param name="item">File to store</param>
		public void Add(File item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Clears remote storage
        /// </summary>
		public void Clear()
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Checks if a certain file is in storage
        /// </summary>
        /// <param name="item">File to be checked for</param>
        /// <returns>true if file found</returns>
		public bool Contains(File item)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Copies files to index
        /// </summary>
        /// <param name="array">Array of files</param>
        /// <param name="arrayIndex">Index to copy to</param>
		public void CopyTo(File[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
        /// <summary>
        /// Removes file from storage
        /// </summary>
        /// <param name="item">File to be removed</param>
        /// <returns>true if file removed</returns>
		public bool Remove(File item)
		{
			throw new NotSupportedException();
		}
        /// <summary>
        /// Gets enumerator for files
        /// </summary>
        /// <returns>true if enumerator gotten</returns>
		public IEnumerator<File> GetEnumerator()
		{
			return new FileEnumator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
