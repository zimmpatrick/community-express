public var userOfflineTextStyle : GUIStyle;
public var userOnlineTextStyle : GUIStyle;
public var userBusyTextStyle : GUIStyle;
public var userAwayTextStyle : GUIStyle;
public var userSnoozeTextStyle : GUIStyle;

private var showFriendsList : boolean = false;

function OnGUI()
{
	var y : int = 20;
	
	if (GUI.Button(Rect(0, 0, 150, 20), "Toggle Friends List"))
	{
		showFriendsList = !showFriendsList;
		Debug.Log("Showing Friends List "+showFriendsList);
	}

	if (showFriendsList)
	{
		for (var f : CommunityExpressNS.Friend in UnityCommunityExpress.Instance.Friends)
		{
			switch (f.PersonaState)
			{
				case 0:
					//GUI.Label(Rect(0, y, 200, 20), f.PersonaName, userOfflineTextStyle);
					//break;
					continue;
	
				case 1:
					GUI.Label(Rect(0, y, 200, 20), f.PersonaName, userOnlineTextStyle);
					break;
	
				case 2:
					GUI.Label(Rect(0, y, 200, 20), f.PersonaName, userBusyTextStyle);
					break;
	
				case 3:
					GUI.Label(Rect(0, y, 200, 20), f.PersonaName, userAwayTextStyle);
					break;
	
				case 4:
					GUI.Label(Rect(0, y, 200, 20), f.PersonaName, userSnoozeTextStyle);
					break;
			}
			
			y += 20;
		}
	}
}
