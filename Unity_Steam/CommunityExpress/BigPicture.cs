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
    using SteamAPICall_t = UInt64;

	/// <summary>
    /// Input modes for the Big Picture gamepad text entry
	/// </summary>
	public enum EGamepadTextInputMode
	{
        /// Normal text input
		k_EGamepadTextInputModeNormal = 0,
        /// Password text input
		k_EGamepadTextInputModePassword = 1
	};

	/// <summary>
    /// Controls number of allowed lines for the Big Picture gamepad text entry
	/// </summary>
	public enum EGamepadTextInputLineMode
	{
        /// Single line text input
		k_EGamepadTextInputLineModeSingleLine = 0,
        /// Multiple line text input
		k_EGamepadTextInputLineModeMultipleLines = 1
	};

    /// <summary>
    /// Big Picture mode
    /// </summary>
	public class BigPicture
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct GamepadTextInputDismissed_t
        {
            internal const int k_iCallback = Events.k_iSteamUtilsCallbacks + 14;

            public Byte m_bSubmitted;			// true if user entered & accepted text (Call ISteamUtils::GetEnteredGamepadTextInput() for text), false if canceled input
            public UInt32 m_unSubmittedText;
        };

        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamUtils_ShowGamepadTextInput(EGamepadTextInputMode inputMode, EGamepadTextInputLineMode lineInputMode,
			[MarshalAs(UnmanagedType.LPStr)] String description, UInt32 maxCharacters);
		[DllImport("CommunityExpressSW")]
		private static extern UInt32 SteamUnityAPI_SteamUtils_GetEnteredGamepadTextLength();
        [DllImport("CommunityExpressSW")]
        [return: MarshalAs(UnmanagedType.I1)]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetEnteredGamepadTextInput(IntPtr text, Int32 maxTextLength);

        private CommunityExpress.OnEventHandler<GamepadTextInputDismissed_t> _onGamepadTextInputDismissedCallback;
        private CommunityExpress _ce;
        /// <summary>
        /// When text input is dismissed
        /// </summary>
        /// <param name="submitted">Is text submitted</param>
        /// <param name="text">Text submitted</param>
        public delegate void OnGamepadTextInputDismissed(Boolean submitted, String text);
        /// <summary>
        /// When the gamepad text input is dismissed by the user
        /// </summary>
        public event OnGamepadTextInputDismissed GamepadTextInputDismissed;

        internal BigPicture(CommunityExpress ce)
        {
            _ce = ce;

            _onGamepadTextInputDismissedCallback = new CommunityExpress.OnEventHandler<GamepadTextInputDismissed_t>(OnGamepadTextInputDismissedCallback);
        }
        /// <summary>
        /// Shows inputted text from gamepad controller
        /// </summary>
        /// <param name="inputMode">What input mode the user has chosen</param>
        /// <param name="lineInputMode">What line the input is going to</param>
        /// <param name="description">Description of the input mode</param>
        /// <param name="maxCharacters">maximum number of characters</param>
        /// <returns>true if input shown</returns>
		public Boolean ShowGamepadTextInput(EGamepadTextInputMode inputMode, EGamepadTextInputLineMode lineInputMode, String description, UInt32 maxCharacters)
        {
            _ce.AddEventHandler(GamepadTextInputDismissed_t.k_iCallback, _onGamepadTextInputDismissedCallback);

			bool ret = SteamUnityAPI_SteamUtils_ShowGamepadTextInput(inputMode, lineInputMode, description, maxCharacters);

            Console.WriteLine(ret);
            return ret;
		}
        /// <summary>
        /// Puts in text inputted from gamepad
        /// </summary>
        /// <param name="text">text input from gamepad controller</param>
		public void GetEnteredGamepadTextInput(out String text)
		{
			Int32 textLength = (Int32)SteamUnityAPI_SteamUtils_GetEnteredGamepadTextLength();
			IntPtr textDataPtr = Marshal.AllocHGlobal(textLength + 1);

			if (SteamUnityAPI_SteamUtils_GetEnteredGamepadTextInput(textDataPtr, textLength))
			{
				text = Marshal.PtrToStringAnsi(textDataPtr);
			}
			else
			{
				text = String.Empty;
			}

			Marshal.FreeHGlobal(textDataPtr);
		}

		private void OnGamepadTextInputDismissedCallback(GamepadTextInputDismissed_t callbackData, Boolean bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _ce.RemoveEventHandler(GamepadTextInputDismissed_t.k_iCallback, _onGamepadTextInputDismissedCallback);

            if (GamepadTextInputDismissed != null)
            {
                Boolean submitted = callbackData.m_bSubmitted != 0;
                String text = String.Empty;
                if (submitted)
                {
                    GetEnteredGamepadTextInput(out text);
                }

                GamepadTextInputDismissed(submitted, text);
            }
		}
	}
}
