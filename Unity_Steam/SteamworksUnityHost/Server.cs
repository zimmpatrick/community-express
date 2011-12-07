using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace SteamworksUnityHost
{
	public class Server
	{
		private Int32 _version;
		private IPAddress _ip;
		private UInt16 _port;
		private UInt16 _queryPort;
		private Int32 _ping;
		private String _serverName;
		private String _mapName;
		private String _gameDescription;
		private Boolean _isSecure;
		private Boolean _isPassworded;
		private Int32 _players;
		private Int32 _maxPlayers;
		private Int32 _botPlayers;

		public Server(Int32 version, IPAddress ip, UInt16 port, UInt16 queryPort, Int32 ping, String serverName, String mapName, String gameDesc,
			Boolean isSecure, Boolean isPassworded, Int32 players, Int32 maxPlayers, Int32 botPlayers)
		{
			_version = version;
			_ip = ip;
			_port = port;
			_queryPort = queryPort;
			_ping = ping;
			_serverName = serverName;
			_mapName = mapName;
			_gameDescription = gameDesc;
			_isSecure = isSecure;
			_isPassworded = isPassworded;
			_players = players;
			_maxPlayers = maxPlayers;
			_botPlayers = botPlayers;
		}

		public Int32 Version
		{
			get { return _version; }
		}

		public IPAddress IP
		{
			get { return _ip; }
		}

		public UInt16 Port
		{
			get { return _port; }
		}

		public UInt16 QueryPort
		{
			get { return _queryPort; }
		}

		public Int32 Ping
		{
			get { return _ping; }
		}

		public String ServerName
		{
			get { return _serverName; }
		}

		public String MapName
		{
			get { return _mapName; }
		}

		public String GameDescription
		{
			get { return _gameDescription; }
		}

		public Boolean IsSecure
		{
			get { return _isSecure; }
		}

		public Boolean IsPassworded
		{
			get { return _isPassworded; }
		}

		public Int32 Players
		{
			get { return _players; }
		}

		public Int32 MaxPlayers
		{
			get { return _maxPlayers; }
		}

		public Int32 BotPlayers
		{
			get { return _botPlayers; }
		}
	}
}
