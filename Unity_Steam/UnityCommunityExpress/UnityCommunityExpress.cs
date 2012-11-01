// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    public void Update()
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

    public Texture2D ConvertImageToTexture2D(Image i)
    {
        Texture2D texture = new Texture2D((int)i.Width, (int)i.Height, TextureFormat.RGBA32, false);
        Color32[] colorArray = new Color32[i.Width * i.Height];
        Color32[] colorRow = new Color32[i.Width];
        GCHandle h = GCHandle.Alloc(colorArray, GCHandleType.Pinned);
        try
        {
            IntPtr dest = h.AddrOfPinnedObject();
            i.GetPixels(dest, (int)i.Width * (int)i.Height * 4);
        }
        finally
        {
            h.Free();
        }

        // Flip image in array
        for (int y = 0; y < texture.height / 2; y++)
        {
            Array.Copy(colorArray, y * texture.width, colorRow, 0, texture.width);
            Array.Copy(colorArray, (texture.height - y - 1) * texture.width, colorArray, y * texture.width, texture.width);
            Array.Copy(colorRow, 0, colorArray, (texture.height - y - 1) * texture.width, texture.width);
        }

        texture.SetPixels32(colorArray);
        texture.Apply();
        return texture;
    }
}