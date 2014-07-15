using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityExpressNS
{
    /// <summary>
    /// Information about the Web API
    /// </summary>
	public class SteamWebAPI
	{
		private const String BaseURL = "https://api.steampowered.com/";
        /// <summary>
        /// Requests new API
        /// </summary>
        /// <param name="interfaceString">API interface</param>
        /// <param name="functionString">API function</param>
        /// <param name="versionString">API version</param>
        /// <returns>true if successfilly connected</returns>
		public SteamWebAPIRequest NewRequest(String interfaceString, String functionString, String versionString = "v0001")
		{
			return new SteamWebAPIRequest(BaseURL + interfaceString + "/" + functionString + "/" + versionString + "/");
		}
	}
}
