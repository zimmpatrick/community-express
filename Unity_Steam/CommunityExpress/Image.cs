using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CommunityExpressNS
{
	/// <summary>
	/// Image
	/// </summary>
    public class Image
	{
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamUtils_GetImageSize(Int32 iconIndex, out UInt32 iconWidth, out UInt32 iconHeight);
		[DllImport("CommunityExpressSW")]
		private static extern void SteamUnityAPI_SteamUtils_GetImageRGBA(Int32 iconIndex,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] Byte[] data, Int32 dataSize);
		[DllImport("CommunityExpressSW", EntryPoint="SteamUnityAPI_SteamUtils_GetImageRGBA")]
		private static extern void SteamUnityAPI_SteamUtils_GetImageRGBA_Ptr(Int32 iconIndex, IntPtr data, Int32 dataSize);

		private Int32 _steamImage;
		private UInt32 _iconWidth = 0;
		private UInt32 _iconHeight = 0;

        public delegate void AvatarImageLoadedHandler(SteamID ID, Image img, bool result);
        public static event AvatarImageLoadedHandler AvatarImageLoaded;

		internal Image(Int32 steamImage)
		{
			_steamImage = steamImage;
			SteamUnityAPI_SteamUtils_GetImageSize(_steamImage, out _iconWidth, out _iconHeight);
		}
        /// <summary>
        /// Gets the data for the image
        /// </summary>
        /// <param name="data">The data of the image</param>
        /// <param name="dataSize">The size of the image data</param>
		public void GetPixels(IntPtr data, Int32 dataSize)
		{
			if (_iconWidth > 0 && _iconHeight > 0)
			{
				SteamUnityAPI_SteamUtils_GetImageRGBA_Ptr(_steamImage, data, dataSize);
			}
		}
        /// <summary>
        /// Turns image into game texture
        /// </summary>
        /// <returns>true if image is converted</returns>
		public Byte[] AsBytes()
		{
			if (_iconWidth > 0 && _iconHeight > 0)
			{
				// Get the actual raw RGBA data from Steam and turn it into a texture in our game engine
				Byte[] iconData = new Byte[_iconWidth * _iconHeight * 4];
				SteamUnityAPI_SteamUtils_GetImageRGBA(_steamImage, iconData, (Int32)(_iconWidth * _iconHeight * 4));
				return iconData;
			}

			return null;
		}
        /// <summary>
        /// Width of the image
        /// </summary>
		public UInt32 Width
		{
			get { return _iconWidth; }
		}
        /// <summary>
        /// Height of the image
        /// </summary>
		public UInt32 Height
		{
			get { return _iconHeight; }
		}

        internal static void NotifyAvatarImageLoaded(SteamID ID, Image image, bool result)
        {
            if (AvatarImageLoaded != null)
            {
                AvatarImageLoaded(ID, image, result);
            }
        }
	}
}
