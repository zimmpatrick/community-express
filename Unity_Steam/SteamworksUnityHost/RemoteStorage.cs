// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
	public class RemoteStorage : ICollection<File>
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamRemoteStorage();
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamRemoteStorage_GetFileCount(IntPtr remoteStorage);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamRemoteStorage_GetFileSize(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamRemoteStorage_GetFileNameAndSize(IntPtr remoteStorage, int iFile, out int nFileSizeInBytes);
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_WriteFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName,
			IntPtr fileContents, int fileContentsLength);
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

		internal RemoteStorage()
		{
			_remoteStorage = SteamUnityAPI_SteamRemoteStorage();
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

		public File GetFile(String fileName)
		{
			foreach (File file in this)
			{
				if (file.FileName.ToLower()  == fileName.ToLower())
				{
					return file;
				}
			}

			return null;
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
