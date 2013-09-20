// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
    /// <summary>
    /// Information about a file
    /// </summary>
	public class File
	{
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamRemoteStorage_FileRead(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] string fileName,
		   [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder sb, int nSize);
		[DllImport("CommunityExpressSW")]
		private static extern int SteamUnityAPI_SteamRemoteStorage_FileRead(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] string fileName,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] data, int nSize);
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

		private String _fileName;
		private int _fileSize;

		internal File(String fileName, int fileSize)
		{
			_fileName = fileName;
			_fileSize = fileSize;
		}
        /// <summary>
        /// Name of the file
        /// </summary>
		public String FileName
		{
			get { return _fileName; }
		}
        /// <summary>
        /// Size of the file
        /// </summary>
		public int FileSize
		{
			get { return _fileSize; }
		}
        /// <summary>
        /// If the file exists
        /// </summary>
		public Boolean Exists
		{
			get { return SteamUnityAPI_SteamRemoteStorage_FileExists(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName); }
		}
        /// <summary>
        /// If the file is saved
        /// </summary>
		public Boolean Persisted
		{
			get { return SteamUnityAPI_SteamRemoteStorage_FilePersisted(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName); }
		}
        /// <summary>
        /// Tries to read the file
        /// </summary>
        /// <returns>true if file can be read</returns>
		public string ReadFile()
		{
			StringBuilder sb = new StringBuilder(_fileSize);
			SteamUnityAPI_SteamRemoteStorage_FileRead(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName, sb, _fileSize);
			return sb.ToString();
		}
        /// <summary>
        /// Reads the file data
        /// </summary>
        /// <returns>true if file is read successfully</returns>
		public Byte[] ReadBytes()
		{
			Byte[] ret = new Byte[_fileSize];
			SteamUnityAPI_SteamRemoteStorage_FileRead(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName, ret, _fileSize);
			return ret;
		}
        /// <summary>
        /// If the file is forgotten
        /// </summary>
        /// <returns>true if successfully forgotten</returns>
		public Boolean Forget()
		{
			return SteamUnityAPI_SteamRemoteStorage_ForgetFile(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName);
		}
        /// <summary>
        /// If the file is deleted
        /// </summary>
        /// <returns>true if successfully deleted</returns>
		public Boolean Delete()
		{
			return SteamUnityAPI_SteamRemoteStorage_DeleteFile(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName);
		}
	}
}
