using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
	public class Achievement
	{
		[DllImport("CommunityExpressSW.dll")]
		private static extern IntPtr SteamUnityAPI_SteamUserStats_GetAchievementDisplayAttribute(IntPtr stats,
			[MarshalAs(UnmanagedType.LPStr)] string pchName, [MarshalAs(UnmanagedType.LPStr)] string pchAttribute);
		[DllImport("CommunityExpressSW.dll")]
		private static extern Int32 SteamUnityAPI_SteamUserStats_GetAchievementIcon(IntPtr stats,
			[MarshalAs(UnmanagedType.LPStr)] string pchName);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamUtils_GetImageSize(Int32 iconIndex, out UInt32 iconWidth, out UInt32 iconHeight);
		[DllImport("CommunityExpressSW.dll")]
		private static extern void SteamUnityAPI_SteamUtils_GetImageRGBA(Int32 iconIndex,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] data, Int32 dataSize);

		private IntPtr _stats;
		private Achievements _achievements;
		private String _achievementName;
		private bool _isAchieved;
		private String _displayName = null;
		private String _displayDescription = null;
		private Byte[] _iconData = null;
		private UInt32 _iconWidth = 0;
		private UInt32 _iconHeight = 0;

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

		public Byte[] IconData
		{
			get
			{
				if (_iconData == null && _stats != IntPtr.Zero)
				{
					Int32 iconImage = SteamUnityAPI_SteamUserStats_GetAchievementIcon(_stats, _achievementName);
					if (iconImage != 0)
					{
						SteamUnityAPI_SteamUtils_GetImageSize(iconImage, out _iconWidth, out _iconHeight);
						if (_iconWidth > 0 && _iconHeight > 0)
						{
							// Get the actual raw RGBA data from Steam and turn it into a texture in our game engine
							_iconData = new Byte[_iconWidth * _iconHeight * 4];
							SteamUnityAPI_SteamUtils_GetImageRGBA(iconImage, _iconData, (Int32)(_iconWidth * _iconHeight * 4));
						}
					}
				}

				return _iconData;
			}
		}

		public UInt32 IconWidth
		{
			get
			{
				if (_iconWidth == 0 && _stats != IntPtr.Zero)
				{
					Int32 iconImage = SteamUnityAPI_SteamUserStats_GetAchievementIcon(_stats, _achievementName);
					if (iconImage != 0)
					{
						SteamUnityAPI_SteamUtils_GetImageSize(iconImage, out _iconWidth, out _iconHeight);
					}
				}

				return _iconWidth;
			}
		}

		public UInt32 IconHeight
		{
			get
			{
				if (_iconHeight == 0 && _stats != IntPtr.Zero)
				{
					Int32 iconImage = SteamUnityAPI_SteamUserStats_GetAchievementIcon(_stats, _achievementName);
					if (iconImage != 0)
					{
						SteamUnityAPI_SteamUtils_GetImageSize(iconImage, out _iconWidth, out _iconHeight);
					}
				}

				return _iconHeight;
			}
		}
	}
}