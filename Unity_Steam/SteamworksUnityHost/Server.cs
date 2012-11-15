// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace CommunityExpressNS
{
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

        public Int32 Version
        {
            get;
            private set;
        }

		public IPAddress IP
		{
            get;
            private set;
		}

        public UInt16 Port
        {
            get;
            private set;
        }

        public UInt16 QueryPort
        {
            get;
            private set;
        }

        public Int32 Ping
        {
            get;
            private set;
        }

        public UInt32 AppId
        {
            get;
            private set;
        }

        public String ServerName
        {
            get;
            private set;
        }

        public String MapName
        {
            get;
            private set;
        }

        public String GameDescription
        {
            get;
            private set;
        }

        public String GameDir
        {
            get;
            private set;
        }

        public Boolean IsSecure
        {
            get;
            private set;
        }

        public Boolean IsPassworded
        {
            get;
            private set;
        }

        public Int32 Players
        {
            get;
            private set;
        }

        public Int32 MaxPlayers
        {
            get;
            private set;
        }

        public Int32 BotPlayers
        {
            get;
            private set;
        }

        public String GameTags
        {
            get;
            private set;
        }
	}
}
