// Copyright (c) 2011-2013, Zimmdot, LLC
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommunityExpressNS
{
    using SteamAPICall_t = UInt64;

	// Input modes for the Big Picture gamepad text entry
	public enum EGamepadTextInputMode
	{
		k_EGamepadTextInputModeNormal = 0,
		k_EGamepadTextInputModePassword = 1
	};

	// Controls number of allowed lines for the Big Picture gamepad text entry
	public enum EGamepadTextInputLineMode
	{
		k_EGamepadTextInputLineModeSingleLine = 0,
		k_EGamepadTextInputLineModeMultipleLines = 1
	};

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct GamepadTextInputDismissed_t
	{
        internal const int k_iCallback = Events.k_iSteamUtilsCallbacks + 14;

		public Byte m_bSubmitted;			// true if user entered & accepted text (Call ISteamUtils::GetEnteredGamepadTextInput() for text), false if canceled input
		public UInt32 m_unSubmittedText;
	};

	public delegate void OnGamepadTextInputDismissed(Boolean submitted, String text);

	public class BigPicture
	{
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamUtils_ShowGamepadTextInput(EGamepadTextInputMode inputMode, EGamepadTextInputLineMode lineInputMode,
			[MarshalAs(UnmanagedType.LPStr)] String description, UInt32 maxCharacters);
		[DllImport("CommunityExpressSW")]
		private static extern UInt32 SteamUnityAPI_SteamUtils_GetEnteredGamepadTextLength();
		[DllImport("CommunityExpressSW")]
		private static extern Boolean SteamUnityAPI_SteamUtils_GetEnteredGamepadTextInput(IntPtr text, Int32 maxTextLength);

        private Events.OnEventHandler<GamepadTextInputDismissed_t> _onGamepadTextInputDismissedCallback;
        private CommunityExpress _cesdk;

        internal BigPicture(CommunityExpress cesdk)
        {
            _cesdk = cesdk;

            _onGamepadTextInputDismissedCallback = new Events.OnEventHandler<GamepadTextInputDismissed_t>(OnGamepadTextInputDismissedCallback);
            _cesdk.Events.AddEventHandler(GamepadTextInputDismissed_t.k_iCallback, _onGamepadTextInputDismissedCallback);
        }

        ~BigPicture()
        {
            _cesdk.Events.RemoveEventHandler(GamepadTextInputDismissed_t.k_iCallback, _onGamepadTextInputDismissedCallback);
        }

        public event OnGamepadTextInputDismissed GamepadTextInputDismissed;

		public Boolean ShowGamepadTextInput(EGamepadTextInputMode inputMode, EGamepadTextInputLineMode lineInputMode, String description, UInt32 maxCharacters)
		{
			bool ret = SteamUnityAPI_SteamUtils_ShowGamepadTextInput(inputMode, lineInputMode, description, maxCharacters);

            Console.WriteLine(ret);
            return ret;
		}

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
