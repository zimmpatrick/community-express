/*
 * Community Express SDK
 * http://www.communityexpresssdk.com/
 *
 * Copyright (c) 2011-2014, Zimmdot, LLC
 * All rights reserved.
 *
 * Subject to terms and condition provided in LICENSE.txt
 * Dual licensed under a Commercial Development and LGPL licenses.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace CommunityExpressNS
{
    using UGCHandle_t = UInt64;
    using SteamAPICall_t = UInt64;
    using AppId_t = UInt32;
    using PublishedFileId_t = UInt64;
    /// <summary>
    /// Steam Cloud file storage
    /// </summary>
	public class RemoteStorage : ICollection<File>
	{
        int _fsCount = 0;

        public enum EWorkshopVote
        {
            k_EWorkshopVoteUnvoted = 0,
            k_EWorkshopVoteFor = 1,
            k_EWorkshopVoteAgainst = 2,
        };

        public enum EWorkshopFileAction
        {
	        k_EWorkshopFileActionPlayed = 0,
	        k_EWorkshopFileActionCompleted = 1,
        };


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
        internal struct RemoteStorageAppSyncedClient_t
        {
	        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 1;
	        internal AppId_t m_nAppID;
	        internal EResult m_eResult;
	        internal int m_unNumDownloads;
        };
        
	    [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct RemoteStorageAppSyncedServer_t
        {
	        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 2;
            internal AppId_t m_nAppID;
            internal EResult m_eResult;
            internal int m_unNumUploads;
        };
        
	    [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct RemoteStorageAppSyncProgress_t
        {
	        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string m_rgchCurrentFile;				// Current file being transferred
	        internal AppId_t m_nAppID;							// App this info relates to
	        internal UInt32 m_uBytesTransferredThisChunk;		// Bytes transferred this chunk
	        internal double m_dAppPercentComplete;				// Percent complete that this app's transfers are
	        internal bool m_bUploading;							// if false, downloading
        };
        
	    [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct RemoteStorageAppSyncStatusCheck_t
        {
	        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 5;
            internal AppId_t m_nAppID;
            internal EResult m_eResult;
        };
        
	    [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct RemoteStorageConflictResolution_t
        {
	        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 6;
	        internal AppId_t m_nAppID;
	        internal EResult m_eResult;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct RemoteStorageGetPublishedItemVoteDetailsResult_t
        {
	        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 20;
            internal EResult m_eResult;
            internal PublishedFileId_t m_unPublishedFileId;
            internal Int32 m_nVotesFor;
            internal Int32 m_nVotesAgainst;
            internal Int32 m_nReports;
            internal float m_fScore;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct RemoteStoragePublishedFileDeleted_t
        {
	       internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 23;
           internal PublishedFileId_t m_unPublishedFileId;	// The published file id
           internal AppId_t m_nAppID;						// ID of the app that will consume this file.
        };
        
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct RemoteStorageUpdateUserPublishedItemVoteResult_t
        {
	        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 24;
            internal EResult m_eResult;				// The result of the operation.
            internal PublishedFileId_t m_nPublishedFileId;	// The published file id
        };
        
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct RemoteStorageUserVoteDetails_t
        {
	        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 25;
            internal EResult m_eResult;				// The result of the operation.
            internal PublishedFileId_t m_nPublishedFileId;	// The published file id
            internal EWorkshopVote m_eVote;			// what the user voted
        };
        
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct RemoteStorageSetUserPublishedFileActionResult_t
        {
	        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 27;
            internal EResult m_eResult;				// The result of the operation.
            internal PublishedFileId_t m_nPublishedFileId;	// The published file id
            internal EWorkshopFileAction m_eAction;	// the action that was attempted
        };
        
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct RemoteStorageEnumeratePublishedFilesByUserActionResult_t
        {
	        internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 28;
	        internal EResult m_eResult;				// The result of the operation.
	        internal EWorkshopFileAction m_eAction;	// the action that was filtered on
	        internal Int32 m_nResultsReturned;
	        internal Int32 m_nTotalResultCount;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
	        internal PublishedFileId_t[] m_rgPublishedFileId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
	        internal UInt32[] m_rgRTimeUpdated;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Ansi)]
        internal struct RemoteStorageFileShareResult_t
        {
            internal const int k_iCallback = Events.k_iClientRemoteStorageCallbacks + 7;

            internal EResult m_eResult;			// The result of the operation
            internal UGCHandle_t m_hFile;		// The handle that can be shared with users and features

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            string m_rgchFilename;              // The name of the file that was shared
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
            _ce.AddEventHandler(RemoteStorageAppSyncedClient_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageAppSyncedClient_t>(Event_RemoteStorageAppSyncedClient));
            _ce.AddEventHandler(RemoteStorageAppSyncedServer_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageAppSyncedServer_t>(Event_RemoteStorageAppSyncedServer));
            _ce.AddEventHandler(RemoteStorageAppSyncProgress_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageAppSyncProgress_t>(Event_RemoteStorageAppSyncProgress));
            _ce.AddEventHandler(RemoteStorageAppSyncStatusCheck_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageAppSyncStatusCheck_t>(Event_RemoteStorageAppSyncStatusCheck));
            _ce.AddEventHandler(RemoteStorageConflictResolution_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageConflictResolution_t>(Event_RemoteStorageConflictResolution));
            _ce.AddEventHandler(RemoteStorageGetPublishedItemVoteDetailsResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageGetPublishedItemVoteDetailsResult_t>(Event_RemoteStorageGetPublishedItemVoteDetailsResult));
            _ce.AddEventHandler(RemoteStoragePublishedFileDeleted_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStoragePublishedFileDeleted_t>(Event_RemoteStoragePublishedFileDeleted));
            _ce.AddEventHandler(RemoteStorageUpdateUserPublishedItemVoteResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageUpdateUserPublishedItemVoteResult_t>(Event_RemoteStorageUpdateUserPublishedItemVoteResult));
            _ce.AddEventHandler(RemoteStorageUserVoteDetails_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageUserVoteDetails_t>(Event_RemoteStorageUserVoteDetails));
            _ce.AddEventHandler(RemoteStorageSetUserPublishedFileActionResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageSetUserPublishedFileActionResult_t>(Event_RemoteStorageSetUserPublishedFileActionResult));
            _ce.AddEventHandler(RemoteStorageEnumeratePublishedFilesByUserActionResult_t.k_iCallback, new CommunityExpress.OnEventHandler<RemoteStorageEnumeratePublishedFilesByUserActionResult_t>(Event_RemoteStorageEnumeratePublishedFilesByUserActionResult));

            
		}


        /// <summary>
        /// Result of shared file
        /// </summary>
        /// <param name="sender">User who sent the file</param>
        /// <param name="args">Result arguments</param>
        public delegate void RemoteStorageFileShareResultHandler(RemoteStorage sender, RemoteStorageFileShareResultArgs args);
        public event RemoteStorageFileShareResultHandler FileShared;

        private void Events_FileShareResultReceived(RemoteStorageFileShareResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (FileShared != null)
            {
                _fsCount--;
                FileShared(this, new RemoteStorageFileShareResultArgs(recv, _fsCount));
            }
        }

        public delegate void RemoteStorageAppSyncedClientHandler(RemoteStorage sender, AppId_t appID, EResult result, int numDownloads);
        public event RemoteStorageAppSyncedClientHandler RemoteStorageAppSyncedClient;

        private void Event_RemoteStorageAppSyncedClient(RemoteStorageAppSyncedClient_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStorageAppSyncedClient != null)
            {
                RemoteStorageAppSyncedClient(this, recv.m_nAppID, recv.m_eResult, recv.m_unNumDownloads);
            }
        }

        public delegate void RemoteStorageAppSyncedServerHandler(RemoteStorage sender, AppId_t appID, EResult result, int numUploads);
        public event RemoteStorageAppSyncedServerHandler RemoteStorageAppSyncedServer;

        private void Event_RemoteStorageAppSyncedServer(RemoteStorageAppSyncedServer_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStorageAppSyncedServer != null)
            {
                RemoteStorageAppSyncedServer(this, recv.m_nAppID, recv.m_eResult, recv.m_unNumUploads);
            }
        }

        public delegate void RemoteStorageAppSyncProgressHandler(RemoteStorage sender, String currentFile, AppId_t appID, UInt32 chunkSize, double percentage, bool isUploading);
        public event RemoteStorageAppSyncProgressHandler RemoteStorageAppSyncProgress;

        private void Event_RemoteStorageAppSyncProgress(RemoteStorageAppSyncProgress_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStorageAppSyncProgress != null)
            {
                RemoteStorageAppSyncProgress(this, recv.m_rgchCurrentFile.ToString(), recv.m_nAppID, recv.m_uBytesTransferredThisChunk, recv.m_dAppPercentComplete, recv.m_bUploading);
            }
        }

        public delegate void RemoteStorageAppSyncStatusCheckHandler(RemoteStorage sender, AppId_t appID, EResult result);
        public event RemoteStorageAppSyncStatusCheckHandler RemoteStorageAppSyncStatusCheck;

        private void Event_RemoteStorageAppSyncStatusCheck(RemoteStorageAppSyncStatusCheck_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStorageAppSyncStatusCheck != null)
            {
                RemoteStorageAppSyncStatusCheck(this, recv.m_nAppID, recv.m_eResult);
            }
        }

        public delegate void RemoteStorageConflictResolutionHandler(RemoteStorage sender, AppId_t appID, EResult result);
        public event RemoteStorageConflictResolutionHandler RemoteStorageConflictResolution;

        private void Event_RemoteStorageConflictResolution(RemoteStorageConflictResolution_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStorageConflictResolution != null)
            {
                RemoteStorageConflictResolution(this, recv.m_nAppID, recv.m_eResult);
            }
        }

        public delegate void RemoteStorageGetPublishedItemVoteDetailsResultHandler(RemoteStorage sender, PublishedFileId_t fileID, Int32 votesFor, Int32 votesAgaints, Int32 reports, float score, EResult result);
        public event RemoteStorageGetPublishedItemVoteDetailsResultHandler RemoteStorageGetPublishedItemVoteDetailsResult;

        private void Event_RemoteStorageGetPublishedItemVoteDetailsResult(RemoteStorageGetPublishedItemVoteDetailsResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStorageGetPublishedItemVoteDetailsResult != null)
            {
                RemoteStorageGetPublishedItemVoteDetailsResult(this, recv.m_unPublishedFileId, recv.m_nVotesFor, recv.m_nVotesAgainst, recv.m_nReports, recv.m_fScore, recv.m_eResult);
            }
        }

        public delegate void RemoteStoragePublishedFileDeletedHandler(RemoteStorage sender, PublishedFileId_t fileID, AppId_t appID);
        public event RemoteStoragePublishedFileDeletedHandler RemoteStoragePublishedFileDeleted;

        private void Event_RemoteStoragePublishedFileDeleted(RemoteStoragePublishedFileDeleted_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStoragePublishedFileDeleted != null)
            {
                RemoteStoragePublishedFileDeleted(this, recv.m_unPublishedFileId, recv.m_nAppID);
            }
        }

        public delegate void RemoteStorageUpdateUserPublishedItemVoteResultHandler(RemoteStorage sender, PublishedFileId_t fileID, EResult result);
        public event RemoteStorageUpdateUserPublishedItemVoteResultHandler RemoteStorageUpdateUserPublishedItemVoteResult;

        private void Event_RemoteStorageUpdateUserPublishedItemVoteResult(RemoteStorageUpdateUserPublishedItemVoteResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStorageUpdateUserPublishedItemVoteResult != null)
            {
                RemoteStorageUpdateUserPublishedItemVoteResult(this, recv.m_nPublishedFileId, recv.m_eResult);
            }
        }

        public delegate void RemoteStorageUserVoteDetailsHandler(RemoteStorage sender, PublishedFileId_t fileID, EWorkshopVote vote, EResult result);
        public event RemoteStorageUserVoteDetailsHandler RemoteStorageUserVoteDetails;

        private void Event_RemoteStorageUserVoteDetails(RemoteStorageUserVoteDetails_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStorageUserVoteDetails != null)
            {
                RemoteStorageUserVoteDetails(this, recv.m_nPublishedFileId, recv.m_eVote, recv.m_eResult);
            }
        }

        public delegate void RemoteStorageSetUserPublishedFileActionResultHandler(RemoteStorage sender, PublishedFileId_t fileID, EWorkshopFileAction action, EResult result);
        public event RemoteStorageSetUserPublishedFileActionResultHandler RemoteStorageSetUserPublishedFileActionResult;

        private void Event_RemoteStorageSetUserPublishedFileActionResult(RemoteStorageSetUserPublishedFileActionResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStorageSetUserPublishedFileActionResult != null)
            {
                RemoteStorageSetUserPublishedFileActionResult(this, recv.m_nPublishedFileId, recv.m_eAction, recv.m_eResult);
            }
        }

        public delegate void RemoteStorageEnumeratePublishedFilesByUserActionResultHandler(RemoteStorage sender, Int32 resultsReturned, Int32 totalResults, PublishedFileId_t[] fileIDs, UInt32[] timeUpdated, EWorkshopFileAction action, EResult result);
        public event RemoteStorageEnumeratePublishedFilesByUserActionResultHandler RemoteStorageEnumeratePublishedFilesByUserActionResult;

        private void Event_RemoteStorageEnumeratePublishedFilesByUserActionResult(RemoteStorageEnumeratePublishedFilesByUserActionResult_t recv, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            if (RemoteStorageEnumeratePublishedFilesByUserActionResult != null)
            {
                RemoteStorageEnumeratePublishedFilesByUserActionResult(this, recv.m_nResultsReturned, recv.m_nTotalResultCount, recv.m_rgPublishedFileId, recv.m_rgRTimeUpdated, recv.m_eAction, recv.m_eResult);
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
		public bool WriteFile(String fileName, String fileContents)
        {
            bool retB = false;
            retB = SteamUnityAPI_SteamRemoteStorage_WriteFile(_remoteStorage, fileName, Marshal.StringToHGlobalAnsi(fileContents), fileContents.Length);
            return retB;
		}
        /// <summary>
        /// Write file to storage in byte form
        /// </summary>
        /// <param name="fileName">File to write</param>
        /// <param name="fileContents">Contents of file in byte</param>
        public bool WriteFile(String fileName, Byte[] fileContents)
		{
			IntPtr fileContentsPtr = Marshal.AllocHGlobal(fileContents.Length);
			Marshal.Copy(fileContents, 0, fileContentsPtr, fileContents.Length);
            bool retB = false;

            retB = SteamUnityAPI_SteamRemoteStorage_WriteFile(_remoteStorage, fileName, fileContentsPtr, fileContents.Length);

			Marshal.FreeHGlobal(fileContentsPtr);

            return retB;
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
