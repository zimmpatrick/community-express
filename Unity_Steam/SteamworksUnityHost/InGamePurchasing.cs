using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace CommunityExpressNS
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct MicroTxnAuthorizationResponse_t
	{
		public UInt32 m_unAppID;	// App Purchase originated in
		public UInt64 m_ulOrderID;	// Order number specified for this purchase
		public Byte m_bAuthorized;	// Whether or not the user authorized the purchase
	}

	delegate void OnTransactionAuthorizationReceivedFromSteam(ref MicroTxnAuthorizationResponse_t CallbackData);
	public delegate void OnInGamePurchaseComplete(InGamePurchase purchase, bool successful);

	public class InGamePurchasing
	{
		[DllImport("CommunityExpressSW.dll")]
		private static extern bool SteamUnityAPI_SetTransactionAuthorizationCallback(IntPtr OnTransactionAuthorizationReceivedCallback);
		[DllImport("CommunityExpressSW.dll")]
		private static extern IntPtr SteamUnityAPI_SteamUser();
		[DllImport("CommunityExpressSW.dll")]
		private static extern UInt64 SteamUnityAPI_SteamUser_GetSteamID(IntPtr user);
		[DllImport("CommunityExpressSW.dll")]
		private static extern IntPtr SteamUnityAPI_SteamApps();
		[DllImport("CommunityExpressSW.dll")]
		[return: MarshalAs(UnmanagedType.LPStr, SizeParamIndex = 0)]
		private static extern String SteamUnityAPI_SteamApps_GetCurrentGameLanguage(IntPtr apps);
		[DllImport("CommunityExpressSW.dll")]
		private static extern UInt32 SteamUnityAPI_SteamUtils_GetAppID();

		private IntPtr _user;
		private IntPtr _apps;
		private SteamID _steamID;
		private String _language = "en";
		private RegionInfo _regionInfo = null;
		private List<InGamePurchase> _outstandingPurchases = new List<InGamePurchase>();

		private OnTransactionAuthorizationReceivedFromSteam _internalOnTransactionAuthorizationReceived = null;

		internal InGamePurchasing()
		{
			_user = SteamUnityAPI_SteamUser();
			_apps = SteamUnityAPI_SteamApps();

			if (_internalOnTransactionAuthorizationReceived == null)
			{
				_internalOnTransactionAuthorizationReceived = new OnTransactionAuthorizationReceivedFromSteam(OnTransactionAuthorizationReceived);
				SteamUnityAPI_SetTransactionAuthorizationCallback(Marshal.GetFunctionPointerForDelegate(_internalOnTransactionAuthorizationReceived));
			}
		}

		public InGamePurchase NewPurchase(bool useTestMode, String webAPIKey, UInt64 orderID)
		{
			if (_regionInfo == null)
			{
				_steamID = new SteamID(SteamUnityAPI_SteamUser_GetSteamID(_user));
				
				// Find the ISO 2-Letter language id for this use's Steam Language setting
				String steamLanguage = SteamUnityAPI_SteamApps_GetCurrentGameLanguage(_apps);
				foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
				{
					if (culture.DisplayName.Equals(steamLanguage, StringComparison.OrdinalIgnoreCase) ||
						culture.EnglishName.Equals(steamLanguage, StringComparison.OrdinalIgnoreCase) ||
						culture.NativeName.Equals(steamLanguage, StringComparison.OrdinalIgnoreCase))
					{
						_language = culture.TwoLetterISOLanguageName;
						break;
					}
				}

				FetchRegionInfo(useTestMode, webAPIKey);
			}

			return new InGamePurchase(useTestMode, webAPIKey, orderID);
		}

		// Returns true if the Purchase process was started successfully
		public bool StartPurchase(InGamePurchase purchase, OnInGamePurchaseComplete callback)
		{
			if (_regionInfo != null)
			{
				purchase.InGamePurchaseCompleteCallback = callback;

				SteamWebAPIRequest request = CommunityExpress.Instance.SteamWebAPI.NewRequest(GetWebInterface(purchase.UseTestMode), "InitTxn", "v0002");
				request.AddPostValue("key", purchase.WebAPIKey);
				request.AddPostValue("orderid", purchase.OrderID.ToString());
				request.AddPostValue("steamid", _steamID.ToUInt64().ToString());
				request.AddPostValue("appid", SteamUnityAPI_SteamUtils_GetAppID().ToString());
				request.AddPostValue("itemcount", purchase.ItemList.Count.ToString());
				request.AddPostValue("language", _language);
				request.AddPostValue("currency", _regionInfo.ISOCurrencySymbol);

				int index = 0;
				foreach(InGamePurchase.Item item in purchase.ItemList)
				{
					request.AddPostValue("itemid%5B" + index + "%5D", item.ID.ToString());
					request.AddPostValue("qty%5B" + index + "%5D", item.Quantity.ToString());
					request.AddPostValue("amount%5B" + index + "%5D", item.Amount.ToString());
					request.AddPostValue("description%5B" + index + "%5D", item.Description);

					if (item.Category != "")
					{
						request.AddPostValue("category%5B" + index + "%5D", item.Category);
					}

					index++;
				}

				_outstandingPurchases.Add(purchase);
				request.Execute(OnPurchaseStarted);

				return true;
			}

			return false;
		}

		private void OnPurchaseStarted(SteamWebAPIRequest request, String response)
		{
			// Find the InGamePurchase item from the request's OrderID
			InGamePurchase purchase = null;
			UInt64 orderid = UInt64.Parse(request.GetPostValue("orderid"));
			foreach (InGamePurchase ingamepurchase in _outstandingPurchases)
			{
				if (ingamepurchase.OrderID == orderid)
				{
					purchase = ingamepurchase;
					break;
				}
			}

			if (purchase != null)
			{
				// Fetch the result from the returned JSON data
				JsonReader json = new JsonTextReader(new StringReader(response));
				while (json.Read())
				{
					if (json.TokenType == JsonToken.PropertyName && json.Value.Equals("result"))
					{
						if (json.ReadAsString().Equals("OK", StringComparison.OrdinalIgnoreCase))
						{
							return;
						}
					}
				}

				purchase.InGamePurchaseCompleteCallback(purchase, false);
				_outstandingPurchases.Remove(purchase);
			}
		}

		private void OnTransactionAuthorizationReceived(ref MicroTxnAuthorizationResponse_t callbackData)
		{
			foreach (InGamePurchase purchase in _outstandingPurchases)
			{
				if (purchase.OrderID == callbackData.m_ulOrderID)
				{
					if (callbackData.m_bAuthorized == 1)
					{
						SteamWebAPIRequest request = CommunityExpress.Instance.SteamWebAPI.NewRequest(GetWebInterface(purchase.UseTestMode), "FinalizeTxn");
						request.AddPostValue("key", purchase.WebAPIKey);
						request.AddPostValue("orderid", callbackData.m_ulOrderID.ToString());
						request.AddPostValue("appid", SteamUnityAPI_SteamUtils_GetAppID().ToString());
						request.Execute(OnTransactionFinalized);
					}
					else
					{
						purchase.InGamePurchaseCompleteCallback(purchase, false);
						_outstandingPurchases.Remove(purchase);
					}

					return;
				}
			}
		}

		private void OnTransactionFinalized(SteamWebAPIRequest request, String response)
		{
			// Find the InGamePurchase item from the request's OrderID
			InGamePurchase purchase = null;
			UInt64 orderid = UInt64.Parse(request.GetPostValue("orderid"));
			foreach (InGamePurchase ingamepurchase in _outstandingPurchases)
			{
				if (ingamepurchase.OrderID == orderid)
				{
					purchase = ingamepurchase;
					break;
				}
			}

			if (purchase != null)
			{
				// Fetch the result from the returned JSON data
				bool success = false;
				JsonReader json = new JsonTextReader(new StringReader(response));
				while (json.Read())
				{
					if (json.TokenType == JsonToken.PropertyName && json.Value.Equals("result"))
					{
						if (json.ReadAsString().Equals("OK", StringComparison.OrdinalIgnoreCase))
						{
							success = true;
							break;
						}
					}
				}

				purchase.InGamePurchaseCompleteCallback(purchase, success);
				_outstandingPurchases.Remove(purchase);
			}
		}

		private void FetchRegionInfo(bool useTestURL, String webAPIKey)
		{
			SteamWebAPIRequest request = CommunityExpress.Instance.SteamWebAPI.NewRequest(GetWebInterface(useTestURL), "GetUserInfo");
			request.AddGetValue("key", webAPIKey);
			request.AddGetValue("steamid", _steamID.ToUInt64().ToString());
			request.Execute(OnRegionInfoRetrieved);
		}

		private void OnRegionInfoRetrieved(SteamWebAPIRequest request, String response)
		{
			// Find the user's country in the returned JSON data
			JsonReader json = new JsonTextReader(new StringReader(response));
			while (json.Read())
			{
				if (json.TokenType == JsonToken.PropertyName && json.Value.Equals("country"))
				{
					_regionInfo = new RegionInfo(json.ReadAsString());
					break;
				}
			}
		}

		private String GetWebInterface(bool useTestURL)
		{
			if (useTestURL)
			{
				return "ISteamMicroTxnSandbox";
			}

			return "ISteamMicroTxn";
		}

		public RegionInfo RegionInfo
		{
			get { return _regionInfo; }
		}
	}
}
