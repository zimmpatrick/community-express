using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using CommunityExpressNS;

public sealed class UnityCommunityExpress : MonoBehaviour
{
	private static UnityCommunityExpress _instance;
	private static CommunityExpress _ceInstance;

	public static UnityCommunityExpress Instance
	{
		get
		{
			return _instance;
		}
	}

	public bool Awake()
	{
		_instance = this;
		_ceInstance = CommunityExpress.Instance;

		return _ceInstance.Initialize();
	}

	public bool RestartAppIfNecessary(uint unOwnAppID)
	{
		return _ceInstance.RestartAppIfNecessary(unOwnAppID);
	}

	public void RunCallbacks()
	{
		_ceInstance.RunCallbacks();
	}

	public void Shutdown()
	{
		_ceInstance.Shutdown();
	}

	public UInt32 AppID
	{
		get { return _ceInstance.AppID; }
	}

	public RemoteStorage RemoteStorage
	{
		get
		{
			return _ceInstance.RemoteStorage;
		}
	}

	public User User
	{
		get
		{
			return _ceInstance.User;
		}
	}

	public GameServer GameServer
	{
		get
		{
			return _ceInstance.GameServer;
		}
	}

	public Friends Friends
	{
		get
		{
			return _ceInstance.Friends;
		}
	}

	public Groups Groups
	{
		get
		{
			return _ceInstance.Groups;
		}
	}

	public Stats UserStats
	{
		get
		{
			return _ceInstance.UserStats;
		}
	}

	public Achievements UserAchievements
	{
		get
		{
			return _ceInstance.UserAchievements;
		}
	}

	public Leaderboards Leaderboards
	{
		get
		{
			return _ceInstance.Leaderboards; 
		}
	}

	public Matchmaking Matchmaking
	{
		get
		{
			return _ceInstance.Matchmaking;
		}
	}

	public GameStats CreateNewGameStats(OnGameStatsSessionInitialized onGameStatsSessionInitialized, Boolean gameserver, SteamID steamID = null)
	{
		return _ceInstance.CreateNewGameStats(onGameStatsSessionInitialized, gameserver, steamID);
	}

	public Boolean IsGameServerInitialized
	{
		get { return _ceInstance.IsGameServerInitialized; }
	}
}
