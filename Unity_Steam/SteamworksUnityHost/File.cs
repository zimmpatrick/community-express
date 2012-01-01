using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
	public class File
	{
		[DllImport("CommunityExpressSW.dll")]
		private static extern int SteamUnityAPI_SteamRemoteStorage_FileRead(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] string fileName,
		   [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder sb, int nSize);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_ForgetFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_DeleteFile(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_FileExists(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Boolean SteamUnityAPI_SteamRemoteStorage_FilePersisted(IntPtr remoteStorage, [MarshalAs(UnmanagedType.LPStr)] String fileName);

		private String _fileName;
		private int _fileSize;

		internal File(String fileName, int fileSize)
		{
			_fileName = fileName;
			_fileSize = fileSize;
		}

		public String FileName
		{
			get { return _fileName; }
		}

		public int FileSize
		{
			get { return _fileSize; }
		}

		public Boolean Exists
		{
			get { return SteamUnityAPI_SteamRemoteStorage_FileExists(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName); }
		}

		public Boolean Persisted
		{
			get { return SteamUnityAPI_SteamRemoteStorage_FilePersisted(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName); }
		}

		public string ReadFile()
		{
			StringBuilder sb = new StringBuilder(_fileSize);
			SteamUnityAPI_SteamRemoteStorage_FileRead(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName, sb, _fileSize);
			return sb.ToString();
		}

		public Boolean Forget()
		{
			return SteamUnityAPI_SteamRemoteStorage_ForgetFile(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName);
		}

		public Boolean Delete()
		{
			return SteamUnityAPI_SteamRemoteStorage_DeleteFile(CommunityExpress.Instance.RemoteStorage.SteamPointer, _fileName);
		}
	}
}
