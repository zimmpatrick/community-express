/*
 * Copyright © 2012 Zimmdot, LLC. All Rights Reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 
 * 2. The name of the author may not be used to endorse or promote products 
 * derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY ZIMMDOT, LLC "AS IS" AND ANY EXPRESS OR 
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; 
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF 
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */


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
