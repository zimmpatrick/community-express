// Copyright (c) 2011-2012, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
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

		public Achievement(Achievements achievements, IntPtr stats)
		{
			_stats = stats;
			_achievements = achievements;
		}

		public Achievement(Achievements achievements, IntPtr stats, String achievementName, bool isAchieved)
		{
			_stats = stats;
			_achievements = achievements;

			_achievementName = achievementName;
			_isAchieved = isAchieved;
		}

		public String AchievementName
		{
			get { return _achievementName; }
			private set { _achievementName = value; }
		}

		public bool IsAchieved
		{
			get { return _isAchieved; }
			internal set { _isAchieved = true; }
		}

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

		public Image Icon
		{
			get {
				if (_image == null && _stats != IntPtr.Zero)
				{
					Int32 iconImage = SteamUnityAPI_SteamUserStats_GetAchievementIcon(_stats, _achievementName);
					_image = new Image(iconImage);
				}

				return _image;
			}
		}

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