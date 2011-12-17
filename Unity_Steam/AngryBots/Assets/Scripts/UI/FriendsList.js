public var userOfflineTextStyle : GUIStyle;
public var userOnlineTextStyle : GUIStyle;
public var userBusyTextStyle : GUIStyle;
public var userAwayTextStyle : GUIStyle;
public var userSnoozeTextStyle : GUIStyle;

function OnGUI()
{
	var y : int = 0;

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