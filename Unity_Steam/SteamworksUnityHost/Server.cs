// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace CommunityExpressNS
{
    /// <summary>
    /// Information about a server
    /// </summary>
	public class Server
	{
		internal Server(Int32 version, IPAddress ip, UInt16 port, UInt16 queryPort, Int32 ping, String serverName, String mapName, String gameDesc,
			Boolean isSecure, Boolean isPassworded, Int32 players, Int32 maxPlayers, Int32 botPlayers, String gameTags, String gameDir, UInt32 appId)
		{
			Version = version;
			IP = ip;
			Port = port;
			QueryPort = queryPort;
			Ping = ping;
			ServerName = serverName;
			MapName = mapName;
			GameDescription = gameDesc;
			IsSecure = isSecure;
			IsPassworded = isPassworded;
			Players = players;
			MaxPlayers = maxPlayers;
			BotPlayers = botPlayers;
			GameTags = gameTags;
			GameDir = gameDir;
			AppId = appId;
		}
        /// <summary>
        /// Server version
        /// </summary>
		public Int32 Version
		{
			get;
			private set;
		}
        /// <summary>
        /// Server IP
        /// </summary>
		public IPAddress IP
		{
			get;
			private set;
		}
        /// <summary>
        /// Server port
        /// </summary>
		public UInt16 Port
		{
			get;
			private set;
		}
        /// <summary>
        /// Server query port
        /// </summary>
		public UInt16 QueryPort
		{
			get;
			private set;
		}
        /// <summary>
        /// Server ping
        /// </summary>
		public Int32 Ping
		{
			get;
			private set;
		}
        /// <summary>
        /// Server app ID
        /// </summary>
		public UInt32 AppId
		{
			get;
			private set;
		}
        /// <summary>
        /// Server name
        /// </summary>
		public String ServerName
		{
			get;
			private set;
		}
        /// <summary>
        /// Server map name
        /// </summary>
		public String MapName
		{
			get;
			private set;
		}
        /// <summary>
        /// Server game description
        /// </summary>
		public String GameDescription
		{
			get;
			private set;
		}
        /// <summary>
        /// Server game directory
        /// </summary>
		public String GameDir
		{
			get;
			private set;
		}
        /// <summary>
        /// If the server is secure
        /// </summary>
		public Boolean IsSecure
		{
			get;
			private set;
		}
        /// <summary>
        /// If the server has a password
        /// </summary>
		public Boolean IsPassworded
		{
			get;
			private set;
		}
        /// <summary>
        /// Number of players on server
        /// </summary>
		public Int32 Players
		{
			get;
			private set;
		}
        /// <summary>
        /// Maximum number of players on server
        /// </summary>
		public Int32 MaxPlayers
		{
			get;
			private set;
		}
        /// <summary>
        /// Number of bots on the server
        /// </summary>
		public Int32 BotPlayers
		{
			get;
			private set;
		}
        /// <summary>
        /// Server game tags
        /// </summary>
		public String GameTags
		{
			get;
			private set;
		}
	}
}
