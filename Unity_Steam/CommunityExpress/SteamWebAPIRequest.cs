using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.IO;

namespace CommunityExpressNS
{
    /// <summary>
    /// When the request is completed
    /// </summary>
    /// <param name="request">API request</param>
    /// <param name="response">Response to show</param>
	public delegate void OnRequestComplete(SteamWebAPIRequest request, String response);

    /// <summary>
    /// Reqeust for Web API
    /// </summary>
	public class SteamWebAPIRequest
	{
		private Uri _url;
		private OnRequestComplete _onRequestComplete;
		private HttpWebRequest _request;
		private String _postValues = "";

		internal SteamWebAPIRequest(String url)
		{
			_url = new Uri(url);
		}
        /// <summary>
        /// Adds get value
        /// </summary>
        /// <param name="key">API key</param>
        /// <param name="value">Value to add</param>
		public void AddGetValue(String key, String value)
		{
			// Tack the new value onto the end of our current URL
			if (_url.Query == "")
			{
				_url = new Uri(_url.ToString() + "?" + key + "=" + value);
			}
			else
			{
				_url = new Uri(_url.ToString() + "&" + key + "=" + value);
			}
		}
        /// <summary>
        /// Adds post value
        /// </summary>
        /// <param name="key">API key</param>
        /// <param name="value">Value to add</param>
		public void AddPostValue(String key, String value)
		{
			if (_postValues == "")
			{
				_postValues = key + "=" + value;
			}
			else
			{
				_postValues += "&" + key + "=" + value;
			}
		}
        /// <summary>
        /// Gets post value
        /// </summary>
        /// <param name="key">API key</param>
        /// <returns>true if gotten</returns>
		public String GetPostValue(String key)
		{
			int start = _postValues.IndexOf(key, 0, StringComparison.OrdinalIgnoreCase);
			if (start != -1)
			{
				start += key.Length + 1;
				int end = _postValues.IndexOf("&", start);
				if (end == -1)
					return _postValues.Substring(start, _postValues.Length);

				return _postValues.Substring(start, end - start);
			}

			return "";
		}
        /// <summary>
        /// Executes API request
        /// </summary>
        /// <param name="onRequestComplete">When request is completed</param>
		public void Execute(OnRequestComplete onRequestComplete)
		{
			_onRequestComplete = onRequestComplete;

			// Due to the requirement to fetch DNS data synchronously, we need to execute the request in a thread
			Thread t = new Thread(new ThreadStart(internalExecute));
			t.IsBackground = true;
			t.Start();
		}

		private void internalExecute()
		{
			// Create our request object with the desired URL
			_request = (HttpWebRequest)HttpWebRequest.Create(_url);

			// If there is post data
			if (_postValues != "")
			{
				_request.ContentType = "application/x-www-form-urlencoded";
				_request.Method = "POST";
				_request.AllowWriteStreamBuffering = true;
				_request.ContentLength = _postValues.Length;
				StreamWriter writer = new StreamWriter(_request.GetRequestStream()); // Here is the WebException thrown
				writer.Write(_postValues);
				writer.Close();
			}

			// Kick off the asynchronous fetch
			IAsyncResult asyncResult = _request.BeginGetResponse(new AsyncCallback(OnResponseReceived), null);

			// Make sure we don't wait too long for a response(10 seconds)
			ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(ScanTimeoutCallback), this, 10000, true);
		}

		private void OnResponseReceived(IAsyncResult asyncResult)
		{
			// Get response string
			HttpWebResponse response = (HttpWebResponse)_request.EndGetResponse(asyncResult);
			Encoding enc = System.Text.Encoding.GetEncoding(1252);
			StreamReader responseStream = new StreamReader(response.GetResponseStream(), enc);
			string responseString = responseStream.ReadToEnd();

			// Close our responses
			responseStream.Close();
			response.Close();

			// Call the delegate callback
			_onRequestComplete(this, responseString);
		}

		private static void ScanTimeoutCallback(object steamWebAPIRequest, bool timedOut)
		{
			// Abort our request if we actually timed out
			if (timedOut && steamWebAPIRequest != null)
			{
				((HttpWebRequest)((SteamWebAPIRequest)steamWebAPIRequest)._request).Abort();

				((SteamWebAPIRequest)steamWebAPIRequest)._onRequestComplete((SteamWebAPIRequest)steamWebAPIRequest, "");
			}
		}
	}
}
