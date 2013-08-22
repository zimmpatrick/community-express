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
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_WriteFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName, IntPtr fileContents, int fileContentsLength);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_ForgetFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_DeleteFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_FileExists(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_FilePersisted(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
		[DllImport("CommunityExpressSW")]
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

        public class RemoteStorageFileShareResultArgs : System.EventArgs
        {
            internal RemoteStorageFileShareResultArgs(RemoteStorageFileShareResult_t args, int remainingfiles)
            {
                Result = args.m_eResult;
                RemainingFiles = remainingfiles;
            }

            public EResult Result
            {
                get;
                private set;
            }

            public int RemainingFiles
            {
                get;
                private set;
            }
        }

        public delegate void RemoteStorageFileShareResultHandler(RemoteStorage sender, RemoteStorageFileShareResultArgs args);
        public event RemoteStorageFileShareResultHandler FileShared;

        public class FileWriteStreamCloseArgs : System.EventArgs
        {
            internal FileWriteStreamCloseArgs(FileWriteStream stream, bool success)
            {
                FileWriteStream = stream;
                Success = success;
            }

            public FileWriteStream FileWriteStream
            {
                get;
                private set;
            }

            public bool Success
            {
                get;
                private set;
            }
        }

        public delegate void FileWriteStreamCloseHandler(RemoteStorage sender, FileWriteStreamCloseArgs args);
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

		public void WriteFile(String fileName, String fileContents)
		{
			SteamUnityAPI_SteamRemoteStorage_WriteFile(_remoteStorage, fileName, Marshal.StringToHGlobalAnsi(fileContents), fileContents.Length);
		}
      
		public void WriteFile(String fileName, Byte[] fileContents)
		{
			IntPtr fileContentsPtr = Marshal.AllocHGlobal(fileContents.Length);
			Marshal.Copy(fileContents, 0, fileContentsPtr, fileContents.Length);

			SteamUnityAPI_SteamRemoteStorage_WriteFile(_remoteStorage, fileName, fileContentsPtr, fileContents.Length);

			Marshal.FreeHGlobal(fileContentsPtr);
		}


        public FileWriteStream BeginWriteFile(String fileName)
        {
            return new FileWriteStream(fileName);
        }


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

		public File GetFile(String fileName)
		{
			return new File(fileName, GetFileSize(fileName));
		}

		public Int32 GetFileSize(String fileName)
		{
			return SteamUnityAPI_SteamRemoteStorage_GetFileSize(_remoteStorage, fileName);
		}

		public void ForgetFile(String fileName)
		{
			SteamUnityAPI_SteamRemoteStorage_ForgetFile(_remoteStorage, fileName);
		}

		public void DeleteFile(String fileName)
		{
			SteamUnityAPI_SteamRemoteStorage_DeleteFile(_remoteStorage, fileName);
		}

		public Boolean FileExists(String fileName)
		{
			return GetFileSize(fileName) > 0;
		}

		public Boolean FilePersisted(String fileName)
		{
			return SteamUnityAPI_SteamRemoteStorage_FilePersisted(_remoteStorage, fileName);
		}

        public void FileShare(String fileName)
        {
            SteamAPICall_t ret = SteamUnityAPI_SteamRemoteStorage_FileShare(_remoteStorage, fileName);
        }

		public Int32 AvailableSpace
		{
			get
			{
				Int32 totalSpace, availableSpace;

				SteamUnityAPI_SteamRemoteStorage_GetQuota(_remoteStorage, out totalSpace, out availableSpace);

				return availableSpace;
			}
		}

		public int Count
		{
			get { return SteamUnityAPI_SteamRemoteStorage_GetFileCount(_remoteStorage); }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public void Add(File item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(File item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(File[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(File item)
		{
			throw new NotSupportedException();
		}

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
