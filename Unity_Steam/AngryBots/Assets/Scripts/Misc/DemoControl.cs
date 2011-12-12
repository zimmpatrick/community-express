using UnityEngine;
using System.Collections;


public class DemoControl : MonoBehaviour
// TODO: 640x480 splash screen for the loading level
// TODO: Set up the zip-for-installer script to strip the asset store folders
{
	public Texture2D pauseIcon, menuBackground, resumeButton, restartButton, fullscreenButton, muteButton, quitButton;
	
	private const float cornerTextureSize = 48.0f;
	private const float menuWidth = 200.0f, menuHeight = 241.0f, menuHeaderHeight = 26.0f, buttonWidth = 175.0f, buttonHeight = 30.0f;
	
	private bool fullScreenAvailable = false, quitEnabled = true, directKeyQuit = true;
	
	
	public bool AudioEnabled
	{
		get
		{
			return PlayerPrefs.GetInt ("Play audio", 1) != 0;
		}
		set
		{
			PlayerPrefs.SetInt ("Play audio", value ? 1 : 0);
			UpdateAudio ();
		}
	}
	
	
	void Start ()
	{
		UpdateAudio ();
		
		switch (Application.platform)
		{
			case RuntimePlatform.OSXWebPlayer:
			case RuntimePlatform.WindowsWebPlayer:
				fullScreenAvailable = true;
				quitEnabled = false;
				directKeyQuit = false;
			break;
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.WindowsPlayer:
				fullScreenAvailable = true;
				directKeyQuit = false;
			break;
		}
	}
	
	
	void UpdateAudio ()
	{
		AudioListener.volume = AudioEnabled ? 1.0f : 0.0f;
	}
	
	
	public void FlipFullscreen ()
	{
		Screen.fullScreen = !Screen.fullScreen;
	}
	
	
	public void FlipMute ()
	{
		AudioEnabled = !AudioEnabled;
	}
	
	
	public void FlipPause ()
	{
		Time.timeScale = Time.timeScale == 0.0f ? 1.0f : 0.0f;
	}
	
	
	void Update ()
	{
		if (directKeyQuit)
		{
			if (Input.GetKeyDown (KeyCode.Escape))
			{
				Application.Quit ();
			}
			else if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Menu))
			{
				Time.timeScale = 0.0f;
			}
		}
	}
	
	
	void OnGUI ()
	{
		Rect rightRect = new Rect (Screen.width - cornerTextureSize, 0.0f, cornerTextureSize, cornerTextureSize);
		
		switch (Event.current.type)
		{
			case EventType.Repaint:
				GUI.DrawTexture (rightRect, pauseIcon);
			break;
			case EventType.MouseDown:
				if (rightRect.Contains (Event.current.mousePosition))
				{
					FlipPause ();
					Event.current.Use ();
				}
			break;
		}
		
		if (Time.timeScale != 0.0f)
		{
			return;
		}
		
		Rect menuRect = new Rect (
			(Screen.width - menuWidth) * 0.5f,
			(Screen.height - menuHeight) * 0.5f,
			menuWidth,
			menuHeight
		);
		
		GUI.DrawTexture (menuRect, menuBackground);
		
		GUILayout.BeginArea (menuRect);
			GUILayout.Space (menuHeaderHeight);
		
			GUILayout.FlexibleSpace ();
			
			if (MenuButton (resumeButton))
			{
				Time.timeScale = 1.0f;
			}
			
			if (fullScreenAvailable)
			{
				GUILayout.FlexibleSpace ();
				if (MenuButton (fullscreenButton))
				{
					FlipFullscreen ();
				}
			}
			
			GUILayout.FlexibleSpace ();
			
			if (MenuButton (muteButton))
			{
				FlipMute ();
			}
			
			GUILayout.FlexibleSpace ();
			
			if (MenuButton (restartButton))
			{
				Time.timeScale = 1.0f;
				Application.LoadLevel (Application.loadedLevel);
			}
			
			if (quitEnabled)
			{
				GUILayout.FlexibleSpace ();
				if (MenuButton (quitButton))
				{
					Application.Quit ();
				}
			}
			GUILayout.FlexibleSpace ();
		GUILayout.EndArea ();
	}
	
	
	bool MenuButton (Texture2D icon)
	{
		bool wasPressed = false;
		
		GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
		
			Rect rect = GUILayoutUtility.GetRect (buttonWidth, buttonHeight, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight));
		
			switch (Event.current.type)
			{
				case EventType.MouseDown:
					if (rect.Contains (Event.current.mousePosition))
					{
						wasPressed = true;
					}
				break;
				case EventType.Repaint:
					GUI.DrawTexture (rect, icon);
				break;
			}
		
			GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		
		return wasPressed;
	}
}
