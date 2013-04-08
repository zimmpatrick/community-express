using UnityEngine;
using System.Collections;

public class AppInfo : MonoBehaviour {
	
	private string language = "language";
	private string allLanguages = "language";
	private uint appID = 0000;
	private bool isAvailble = false;
	private string DLCName = "name";
	
	// Use this for initialization
	void Start () {
		language = UnityCommunityExpress.Instance.App.GetCurrentGameLanguage();
		allLanguages = UnityCommunityExpress.Instance.App.GetAvailableGameLanguages();
	}
	
	void OnGUI(){
		GUI.Label(new Rect(Screen.width-275, 150, 300, 30), "Current Language : " + language);
		GUI.Label(new Rect(Screen.width-275, 180, 300, 30), "Available Languages : " + allLanguages);
		GUI.Label(new Rect(Screen.width-275, 200, 300, 30), isAvailble + "  " + appID + "  " + DLCName);
		
		if(GUI.Button(new Rect(Screen.width - 100, 230, 100, 50), "GetDLC")){
			UnityCommunityExpress.Instance.App.GetDLCDataByIndex(0,out appID, out isAvailble, out DLCName, 255);
			UnityCommunityExpress.Instance.App.InstallDLC(appID);
		}
	}
}
