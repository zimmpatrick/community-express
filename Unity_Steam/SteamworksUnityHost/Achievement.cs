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

namespace CommunityExpressNS
{
    /// <summary>
    /// Achievement
    /// </summary>
	public class Achievement
	{
		[DllImport("CommunityExpressSW")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats_GetAchievementDisplayAttribute(IntPtr stats,
			[MarshalAs(UnmanagedType.LPStr)] string pchName, [MarshalAs(UnmanagedType.LPStr)] string pchAttribute);
		[DllImport("CommunityExpressSW")]
		private static extern Int32 SteamUnityAPI_SteamUserStats_GetAchievementIcon(IntPtr stats,
			[MarshalAs(UnmanagedType.LPStr)] string pchName);
	
		private IntPtr _stats;
		private Achievements _achievements;
		private String _achievementName;
		private bool _isAchieved;
		private String _displayName = null;
		private String _displayDescription = null;
		private Image _image = null;

		internal Achievement(Achievements achievements, IntPtr stats)
		{
			_stats = stats;
			_achievements = achievements;
		}

        internal Achievement(Achievements achievements, IntPtr stats, String achievementName, bool isAchieved)
		{
			_stats = stats;
			_achievements = achievements;

			_achievementName = achievementName;
			_isAchieved = isAchieved;
		}
        /// <summary>
        /// Name of the achievement
        /// </summary>
		public String AchievementName
		{
			get { return _achievementName; }
			private set { _achievementName = value; }
		}
        /// <summary>
        /// Whether or not the achievement is unlocked
        /// </summary>
		public bool IsAchieved
		{
			get { return _isAchieved; }
			internal set { _isAchieved = true; }
		}

        /// <summary>
        /// Localized achievement name
        /// </summary>
		public String DisplayName
		{
			get
			{
				if (_displayName == null && _stats != IntPtr.Zero)
				{
					IntPtr name = SteamUnityAPI_SteamUserStats_GetAchievementDisplayAttribute(_stats, _achievementName, "name");
					_displayName = Marshal.PtrToStringAnsi(name);
				}
				
				return _displayName;
			}
		}

        /// <summary>
        /// Localized achievement description
        /// </summary>
		public String DisplayDescription
		{
			get
			{
				if (_displayDescription == null && _stats != IntPtr.Zero)
				{
					IntPtr description = SteamUnityAPI_SteamUserStats_GetAchievementDisplayAttribute(_stats, _achievementName, "desc");
					_displayDescription = Marshal.PtrToStringAnsi(description);
				}
				
				return _displayDescription;
			}
		}

        /// <summary>
        /// Gets the icon of the achievement, or null if none set. 
        /// A return value of null may indicate we are still fetching data, and you can wait for the UserAchievementIconFetched_t callback
        /// which will notify you when the bits are ready. If the callback still returns null, then there is no image set for the
        /// specified achievement.
        /// </summary>
		public Image Icon
		{
			get {
				if (_image == null && _stats != IntPtr.Zero)
				{
					Int32 iconImage = SteamUnityAPI_SteamUserStats_GetAchievementIcon(_stats, _achievementName);
                    if (iconImage != 0)
                    {
                        _image = new Image(iconImage);
                    }
				}

				return _image;
			}
		}

        /// <summary>
        /// Gets the icon of the achievement, or null if none set.
        /// (See Icon description for details)
        /// </summary>
		public Byte[] IconData
		{
			get
			{
				Image icon = Icon;
				if (icon != null)
				{
					return icon.AsBytes();
				}

				return null;
			}
		}

        /// <summary>
        /// Width of the Icon
        /// </summary>
		public UInt32 IconWidth
		{
			get
			{
				Image icon = Icon;
				if (icon != null)
				{
					return icon.Width;
				}

				return 0;
			}
		}
        /// <summary>
        /// Height of the Icon
        /// </summary>
		public UInt32 IconHeight
		{
			get
			{
				Image icon = Icon;
				if (icon != null)
				{
					return icon.Height;
				}

				return 0;
			}
		}
	}
}