using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
	public class SteamWebAPI
	{
		private const String BaseURL = "https://api.steampowered.com/";

		public SteamWebAPIRequest NewRequest(String interfaceString, String functionString, String versionString = "v0001")
		{
			return new SteamWebAPIRequest(BaseURL + interfaceString + "/" + functionString + "/" + versionString + "/");
		}
	}
}
