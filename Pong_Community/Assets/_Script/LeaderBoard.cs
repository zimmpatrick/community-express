using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaderBoard : MonoBehaviour {
	
	public List<int> scoreDetails = new List<int>();
	public CommunityExpressNS.Leaderboard pongLead;
	public CommunityExpressNS.LeaderboardEntries pongEntries;
	public bool ShowLeaderboard = false;
	public static LeaderBoard Instance;
	
	// Use this for initialization
	void Start () {
		Instance = this;
		//UnityCommunityExpress.Instance.Leaderboards.FindOrCreateLeaderboard(OnLeaderBoardRetrieved, "Pong Lead", CommunityExpressNS.ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,CommunityExpressNS.ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNone);
		//CommunityExpressNS.OnLeaderboardRetrieved
		
	}
	
	void OnLeaderBoardRetrieved(CommunityExpressNS.Leaderboard leaderBoard){
		Debug.Log("LeaderBoard Retrieved");
		Debug.Log(leaderBoard.LeaderboardName);
		Debug.Log(UnityCommunityExpress.Instance.Leaderboards.Count);
		pongLead = leaderBoard;
		
		pongLead.RequestLeaderboardEntries(0, 30, 2, OnLeaderBoardEntriesRetrieved);
	}
			
	// Update is called once per frame
	void Update () {
	
	}
	

    void OnConnectedToServer() {
		
    }
	
	void OnLeaderBoardEntriesRetrieved(CommunityExpressNS.LeaderboardEntries leadEntries){
		if (leadEntries!=null)
		{
			pongEntries=leadEntries;
			ShowLeaderboard=!ShowLeaderboard;
		}
		else
		{
			Debug.Log("Failed to Retreive Leaderboard Entries");
		}
		
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(400, 15, 150, 30), "Leaderboards")){
			UnityCommunityExpress.Instance.Leaderboards.FindOrCreateLeaderboard(OnLeaderBoardRetrieved, 
																				"Pong Lead", 
																				CommunityExpressNS.ELeaderboardSortMethod.k_ELeaderboardSortMethodAscending,
																				CommunityExpressNS.ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
		}
		
		if(ShowLeaderboard){
			
			int ledY = 80;
			foreach(CommunityExpressNS.LeaderboardEntry led in pongEntries){
				GUI.Label(new Rect(400,50,150, 30), "Player");
				GUI.Label(new Rect(500,50,150, 30), "Score");
				GUILayout.BeginHorizontal();
					GUI.Label(new Rect(400,ledY,150, 30), led.PersonaName);
					GUI.Label(new Rect(500,ledY,150, 30), led.Score.ToString());
				GUILayout.EndHorizontal();
				ledY+=30;
			}
		}
	}
	
	public void UpdateLeaderboard(int highestScore){
		pongLead.UploadLeaderboardScore(CommunityExpressNS.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, highestScore, scoreDetails);
	}
	
}
