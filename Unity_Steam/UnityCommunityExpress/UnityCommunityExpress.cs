// Copyright (c) 2011-2013, Zimmdot, LLC
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
    private Exception _exception = null;

    private void ValidateSDK()
    {
        if (_exception != null)
        {
            throw _exception;
        }
    }

	public static UnityCommunityExpress Instance
	{
		get { return _instance; }
	}

    public void Awake()
    {
        _instance = this;
        _ceInstance = CommunityExpress.Instance;
        _ceInstance.Logger = new CommunityExpress.OnLog(onLog);

        bool result = false;
        try
        {
            result = _ceInstance.Initialize();
            if (!result)
            {
                if (!IsCommunityRunning)
                {
                    _exception = new Exception("Steam must be running to play this game.");
                }
            }
        }
        catch (LicenseException e)
        {
            _exception = e;
            _ceInstance = null;
        }
        finally
        {
            print("SteamAPI_Init: " + result);
        }
    }

	public void OnDestroy()
	{
        ValidateSDK();

		_ceInstance.Shutdown();
		_instance = null;
	}

	public bool RestartAppIfNecessary(uint unOwnAppID)
    {
        ValidateSDK();

		return _ceInstance.RestartAppIfNecessary(unOwnAppID);
	}

	public void Update()
    {
        ValidateSDK();

		_ceInstance.RunCallbacks();
	}

	public void Shutdown()
    {
        if (_ceInstance != null)
        {
            _ceInstance.Shutdown();
        }
	}

    public bool IsCommunityRunning
    {
        get
        {
            ValidateSDK();

            if (_ceInstance != null)
            {
                return _ceInstance.IsCommunityRunning;
            }

            return false;
        }
    }
        
	public UInt32 AppID
	{
        get
        {
            ValidateSDK();
            return _ceInstance.AppID;
        }
	}

	public App App
	{
        get
        {
            ValidateSDK();
            return _ceInstance.App;
        }
	}

	public User User
	{
        get
        {
            ValidateSDK();
            return _ceInstance.User;
        }
	}

	public GameServer GameServer
	{
        get
        {
            ValidateSDK();
            return _ceInstance.GameServer;
        }
	}

	public Friends Friends
	{
        get
        {
            ValidateSDK();
            return _ceInstance.Friends;
        }
	}

	public Groups Groups
	{
        get
        {
            ValidateSDK();
            return _ceInstance.Groups;
        }
	}

	public Stats UserStats
	{
        get
        {
            ValidateSDK();
            return _ceInstance.UserStats;
        }
	}

	public Achievements UserAchievements
	{
        get
        {
            ValidateSDK();
            return _ceInstance.UserAchievements;
        }
	}

	public Leaderboards Leaderboards
	{
        get
        {
            ValidateSDK();
            return _ceInstance.Leaderboards;
        }
	}

	public Matchmaking Matchmaking
	{
        get
        {
            ValidateSDK();
            return _ceInstance.Matchmaking;
        }
	}

	public RemoteStorage RemoteStorage
	{
        get
        {
            ValidateSDK();
            return _ceInstance.RemoteStorage;
        }
	}

	public Networking Networking
	{
        get
        {
            ValidateSDK();
            return _ceInstance.Networking;
        }
	}

	public InGamePurchasing InGamePurchasing
	{
        get
        {
            ValidateSDK();
            return _ceInstance.InGamePurchasing;
        }
	}

	public SteamWebAPI SteamWebAPI
	{
        get
        {
            ValidateSDK();
            return _ceInstance.SteamWebAPI;
        }
	}

	public BigPicture BigPicture
	{
        get
        {
            ValidateSDK();
            return _ceInstance.BigPicture;
        }
	}

	public Boolean IsGameServerInitialized
	{
        get
        {
            ValidateSDK();
            return _ceInstance.IsGameServerInitialized;
        }
	}

	public Texture2D ConvertImageToTexture2D(Image i)
    {
        ValidateSDK();

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

	private void onLog(string msg)
	{
		print(msg);
	}
}