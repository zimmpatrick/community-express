using UnityEngine;
using System.Collections;

public class PaddleControl : MonoBehaviour {

	public float paddleSpeed = 30;
	public bool cantMove = false;
	private bool firstTimeHit = false;
	private string curColor = "hjk";
	private UnityCommunityExpress communityExpress;
	public string myName = "";
	public Rect myRect;
	public float randX = 0;
	public float randY = 0;
	public GameObject gText;
	private GameObject tempObj;
	
	void Start(){
		communityExpress = UnityCommunityExpress.Instance;
		randX = Random.Range(0.2f, 0.5f);
		randY = Random.Range(0.2f, 0.5f);
		//tempObj = Network.Instantiate(gText, new Vector3(randX, randY , 0), Quaternion.identity,0) as GameObject;
		
		myName=communityExpress.User.PersonaName;
		Debug.Log("running");
		//tempObj.guiText.text = communityExpress.User.PersonaName;
	}
	

	
	
	void FixedUpdate () {
		if(networkView.isMine){
			networkView.RPC("ChangeNetworkColor", RPCMode.Server, Menu.Instance.userColor);
			switch(Menu.Instance.userColor){
				case "white":
					renderer.material.color = Color.white;
				break;
				case "blue":
					renderer.material.color = Color.blue;
				break;
				case "red":
					renderer.material.color = Color.red;
				break;
				case "green":
					renderer.material.color = Color.green;
				break;
			}
			if(curColor!=Menu.Instance.userColor){
				
				
			}
			curColor = Menu.Instance.userColor;
		}
		if(Menu.Instance.isStart&&networkView.isMine){
			
			cantMove = false;
			paddleSpeed = 60;
			paddleSpeed*=Input.GetAxis("Vertical");
			if(transform.position.y<-77&&paddleSpeed<0){
				cantMove=true;
			}else if(transform.position.y>77&&paddleSpeed>0){
				cantMove=true;
			}
			if(!cantMove){
				transform.position = new Vector3(transform.position.x, transform.position.y+paddleSpeed*Time.deltaTime, transform.position.z);
			}
		}
	}
	
	[RPC]void ChangeNetworkColor(string newColor){
		switch(newColor){
				case "white":
					renderer.material.color = Color.white;
				break;
				case "blue":
					renderer.material.color = Color.blue;
				break;
				case "red":
					renderer.material.color = Color.red;
				break;
				case "green":
					renderer.material.color = Color.green;
				break;
			}
	}
	
	void OnGUI(){
		//float offset = tempObj.guiText.pixelOffset.x+1*Time.deltaTime;
		
		//networkView.RPC("OnReceiveState", RPCMode.Server, myName, offset);
		
		//GUI.Label(new Rect(randX, randY, 100, 100), myName);
	}
	
//	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
//	{
//	    if (stream.isWriting)
//	    {
//	        char theName = myName;
//	        stream.Serialize(ref theName);
//			
//	    }
//	    else
//	    {
//	        char[] theName = "";
//	        stream.Serialize(ref theName);
//	        myName = theName.ToString();
//	    }
//	}


	//[RPC]void OnReceiveState(string theName, float offset){
	//	if(networkView.isMine){
			//tempObj.guiText.text = theName;
			//tempObj.guiText.pixelOffset = new Vector2(offset, gText.guiText.pixelOffset.y);
			//myName=communityExpress.User.PersonaName;
	//	}
//	}
	
	void OnTriggerEnter(Collider obj){
		if(obj.tag=="ball"){
			
				
			Menu.Instance.AddHitCounter();
			if(!firstTimeHit){
				Menu.Instance.UnlockAchievement();
				firstTimeHit=true;
			}
		}
	}
}
