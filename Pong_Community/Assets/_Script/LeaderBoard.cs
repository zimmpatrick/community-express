using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeaderBoard : MonoBehaviour {
	
	public List<int> scoreDetails = new List<int>();
	public CommunityExpressNS.Leaderboard pongLead;
	
	// Use this for initialization
	void Start () {
		scoreDetails.Add(3);
		scoreDetails.Add(3);
		Debug.Log("Leaderboard Requested");
		//UnityCommunityExpress.Instance.Leaderboards.FindOrCreateLeaderboard(OnLeaderBoardRetrieved, "Pong Lead", CommunityExpressNS.ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,CommunityExpressNS.ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNone);
		//CommunityExpressNS.OnLeaderboardRetrieved
		
	}
	
	void OnLeaderBoardRetrieved(CommunityExpressNS.Leaderboard leaderBoard){
		Debug.Log("LeaderBoard Retrieved");
		Debug.Log(leaderBoard.LeaderboardName);
		Debug.Log(UnityCommunityExpress.Instance.Leaderboards.Count);
		pongLead = leaderBoard;
		//pongLead.UploadLeaderboardScore(CommunityExpressNS.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate,24,scoreDetails);
		leaderBoard.UploadLeaderboardScore(CommunityExpressNS.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, 24, scoreDetails);
		leaderBoard.UploadLeaderboardScore(CommunityExpressNS.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, 44, scoreDetails);
		//leaderBoard.RequestLeaderboardEntries(0, 2, 1, OnLeaderBoardEntriesRetrieved);
	}
			
	// Update is called once per frame
	void Update () {
	
	}
	

    void OnConnectedToServer() {
		
    }
	
	void OnLeaderBoardEntriesRetrieved(CommunityExpressNS.LeaderboardEntries leadEntries){
		if (leadEntries!=null)
		{
			Debug.Log(leadEntries);
			Debug.Log(leadEntries.Count);
			Debug.Log(leadEntries[0].PersonaName);
			Debug.Log(leadEntries[0].ScoreDetails[0]);
		}
		else
		{
			Debug.Log("Failed to Retreive Leaderboard Entries");
		}
		
	}
	
	void OnGUI(){
		if(GUI.Button(new Rect(100, 100, 50, 50), "haha")){
			Debug.Log(UnityCommunityExpress.Instance.Leaderboards.Count);
			UnityCommunityExpress.Instance.Leaderboards.FindOrCreateLeaderboard(OnLeaderBoardRetrieved, "Pong Lead", CommunityExpressNS.ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,CommunityExpressNS.ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
			//pongLead.RequestLeaderboardEntries(0, 2, 1, OnLeaderBoardEntriesRetrieved);
			Debug.Log(UnityCommunityExpress.Instance.Leaderboards.Count);
			Debug.Log("Done");
		}
		if(GUI.Button(new Rect(200, 200, 50, 50), "check")){
			pongLead.RequestLeaderboardEntries(0, 30, 2, OnLeaderBoardEntriesRetrieved);
		}
	}
	
}
