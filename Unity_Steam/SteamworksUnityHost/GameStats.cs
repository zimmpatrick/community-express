using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SteamworksUnityHost
{
	using SteamAPICall_t = UInt64;

	//-----------------------------------------------------------------------------
	// Purpose: nAccountType for GetNewSession
	//-----------------------------------------------------------------------------
	public enum EGameStatsAccountType
	{
		k_EGameStatsAccountType_Steam = 1,				// ullAccountID is a 64-bit SteamID for a player
		k_EGameStatsAccountType_Xbox = 2,				// ullAccountID is a 64-bit XUID
		k_EGameStatsAccountType_SteamGameServer = 3,	// ullAccountID is a 64-bit SteamID for a game server
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct GameStatsSessionIssued_t
	{
		public UInt64 m_ulSessionID;
		public EResult m_eResult;
		public Boolean m_bCollectingAny;
		public Boolean m_bCollectingDetails;
	}

	delegate void OnGameStatsSessionIssuedBySteam(ref GameStatsSessionIssued_t CallbackData);
	public delegate void OnGameStatsSessionInitialized(GameStats gamestats);

	public class GameStats
	{
		[DllImport("SteamworksUnity.dll")]
		private static extern IntPtr SteamUnityAPI_SteamGameStats();
		[DllImport("SteamworksUnity.dll")]
		private static extern UInt32 SteamUnityAPI_SteamUtils_GetAppID();
		[DllImport("SteamworksUnity.dll")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamGameStats_GetNewSession(IntPtr gamestats, EGameStatsAccountType accountType,
			UInt64 accountID, Int32 appID, UInt32 timeStarted);
		[DllImport("SteamworksUnity.dll")]
		private static extern SteamAPICall_t SteamUnityAPI_SteamGameStats_EndSession(IntPtr gamestats, UInt64 sessionID, UInt32 timeEnded, Int32 reason);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_AddSessionAttributeInt(IntPtr gamestats, UInt64 sessionID,
			 [MarshalAs(UnmanagedType.LPStr)] String name, Int32 value);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_AddSessionAttributeInt64(IntPtr gamestats, UInt64 sessionID,
			 [MarshalAs(UnmanagedType.LPStr)] String name, Int64 value);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_AddSessionAttributeFloat(IntPtr gamestats, UInt64 sessionID,
			[MarshalAs(UnmanagedType.LPStr)] String name, float value);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_AddSessionAttributeString(IntPtr gamestats, UInt64 sessionID,
			[MarshalAs(UnmanagedType.LPStr)] String name, [MarshalAs(UnmanagedType.LPStr)] String value);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_AddNewRow(IntPtr gamestats, out UInt64 rowID, UInt64 sessionID,
			[MarshalAs(UnmanagedType.LPStr)] String tableName);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_AddRowAttributeInt(IntPtr gamestats, UInt64 rowID,
			 [MarshalAs(UnmanagedType.LPStr)] String name, Int32 value);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_AddRowAttributeInt64(IntPtr gamestats, UInt64 rowID,
			 [MarshalAs(UnmanagedType.LPStr)] String name, Int64 value);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_AddRowAttributeFloat(IntPtr gamestats, UInt64 rowID,
			[MarshalAs(UnmanagedType.LPStr)] String name, float value);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_AddRowAttributeString(IntPtr gamestats, UInt64 rowID,
			[MarshalAs(UnmanagedType.LPStr)] String name, [MarshalAs(UnmanagedType.LPStr)] String value);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_CommitRow(IntPtr gamestats, UInt64 rowID);
		[DllImport("SteamworksUnity.dll")]
		private static extern EResult SteamUnityAPI_SteamGameStats_CommitOutstandingRows(IntPtr gamestats, UInt64 sessionID);

		private IntPtr _gamestats;
		private EGameStatsAccountType _accountType;
		private SteamID _id;
		private UInt64 _sessionId;
		private Boolean _initialized = false;
		private List<UInt64> _rowIds = new List<UInt64>();

		private OnGameStatsSessionInitialized _onGameStatsSessionInitialized;

		internal GameStats()
		{
			_gamestats = SteamUnityAPI_SteamGameStats();
		}

		~GameStats()
		{
			if (_rowIds.Count > 0)
			{
				SteamUnityAPI_SteamGameStats_CommitOutstandingRows(_gamestats, _sessionId);
			}
		}

		public void StartNewSession(OnGameStatsSessionInitialized onGameStatsSessionInitialized, EGameStatsAccountType accountType = EGameStatsAccountType.k_EGameStatsAccountType_Xbox, SteamID accountID = null)
		{
			if (_initialized)
			{
				EndCurrentSession();
			}

			_onGameStatsSessionInitialized = onGameStatsSessionInitialized;

			if (accountID == null)
			{
				if (accountType != EGameStatsAccountType.k_EGameStatsAccountType_Steam && SteamUnity.Instance.IsGameServerInitialized)
				{
					_accountType = EGameStatsAccountType.k_EGameStatsAccountType_SteamGameServer;
					_id = SteamUnity.Instance.GameServer.SteamID;
				}
				else
				{
					_accountType = EGameStatsAccountType.k_EGameStatsAccountType_Steam;
					_id = SteamUnity.Instance.User.SteamID;
				}
			}
			else
			{
				_accountType = accountType;
				_id = accountID;
			}

			DateTime time = DateTime.Now;

			SteamAPICall_t result = SteamUnityAPI_SteamGameStats_GetNewSession(_gamestats, _accountType, _id.ToUInt64(), (Int32)SteamUnityAPI_SteamUtils_GetAppID(), (UInt32)time.ToFileTime());
			SteamUnity.Instance.AddGameStatsSessionIssuedCallback(result, OnGameStatsSessionIssuedCallback);
		}

		private void OnGameStatsSessionIssuedCallback(ref GameStatsSessionIssued_t CallbackData)
		{
			if (CallbackData.m_eResult == EResult.EResultOK)
			{
				_initialized = true;

				_sessionId = CallbackData.m_ulSessionID;

				_onGameStatsSessionInitialized(this);
			}
			else
			{
				_onGameStatsSessionInitialized(null);
			}
		}

		public void AddSessionValue(String name, Int32 value)
		{
			SteamUnityAPI_SteamGameStats_AddSessionAttributeInt(_gamestats, _sessionId, name, value);
		}

		public void AddSessionValue(String name, Int64 value)
		{
			SteamUnityAPI_SteamGameStats_AddSessionAttributeInt64(_gamestats, _sessionId, name, value);
		}

		public void AddSessionValue(String name, float value)
		{
			SteamUnityAPI_SteamGameStats_AddSessionAttributeFloat(_gamestats, _sessionId, name, value);
		}

		public void AddSessionValue(String name, String value)
		{
			SteamUnityAPI_SteamGameStats_AddSessionAttributeString(_gamestats, _sessionId, name, value);
		}

		public void EndCurrentSession()
		{
			_initialized = false;

			DateTime time = DateTime.Now;

			SteamUnityAPI_SteamGameStats_EndSession(_gamestats, _sessionId, (UInt32)time.ToFileTime(), 0);
		}

		public UInt64 CreateNewRow(String tableName)
		{
			UInt64 rowId;

			SteamUnityAPI_SteamGameStats_AddNewRow(_gamestats, out rowId, _sessionId, tableName);

			_rowIds.Add(rowId);

			return rowId;
		}

		public void AddRowValue(UInt64 rowId, String name, Int32 value)
		{
			SteamUnityAPI_SteamGameStats_AddRowAttributeInt(_gamestats, rowId, name, value);
		}

		public void AddRowValue(UInt64 rowId, String name, Int64 value)
		{
			SteamUnityAPI_SteamGameStats_AddRowAttributeInt64(_gamestats, rowId, name, value);
		}

		public void AddRowValue(UInt64 rowId, String name, float value)
		{
			SteamUnityAPI_SteamGameStats_AddRowAttributeFloat(_gamestats, rowId, name, value);
		}

		public void AddRowValue(UInt64 rowId, String name, String value)
		{
			SteamUnityAPI_SteamGameStats_AddRowAttributeString(_gamestats, rowId, name, value);
		}

		public void CommitRow(UInt64 rowId)
		{
			SteamUnityAPI_SteamGameStats_CommitRow(_gamestats, rowId);

			_rowIds.Remove(rowId);
		}

		public void CommitAllRows()
		{
			SteamUnityAPI_SteamGameStats_CommitOutstandingRows(_gamestats, _sessionId);

			_rowIds.Clear();
		}

		public Boolean IsInitialized
		{
			get { return _initialized; }
		}

		public SteamID SteamID
		{
			get { return _id; }
		}

		public EGameStatsAccountType AccountType
		{
			get { return _accountType; }
		}
	}
}
