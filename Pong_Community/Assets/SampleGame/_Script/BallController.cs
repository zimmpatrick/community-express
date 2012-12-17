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

public class BallController : MonoBehaviour {
	
	public bool justStart = false;
	public bool goLeft = false;
	public bool gameOver = true;
	
	public float ballSpeedX = 5;
	public float ballSpeedY = 5;
	
	// Use this for initialization
	void Start () {
		//NetworkView.
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Network.isServer){
			networkView.RPC("moveBall", RPCMode.Server, transform.position.x, transform.position.y);
		}
		if(Menu.Instance.isStart &&Network.isServer){
			if(!justStart){
				int tempInt = 0;
				tempInt = Random.Range(0, 10);
				if(tempInt>=5){
					ballSpeedX*=-1;
				}
				tempInt = Random.Range(0, 10);
				if(tempInt>=5){
					ballSpeedY*=-1;
				}
				justStart=true;
			}
			this.transform.position = new Vector3(transform.position.x+ballSpeedX*Time.deltaTime, transform.position.y+ballSpeedY*Time.deltaTime, transform.position.z);
			if(transform.position.x<-137||transform.position.x>137){
				Menu.Instance.GameOver();
			}
		}
	}
	
	[RPC]void moveBall(float ballX, float ballY){
		transform.position = new Vector3(ballX, ballY, transform.position.z);
	}
	
	
	
	void OnTriggerEnter(Collider obj){
		if(obj.tag=="wall"&&Network.isServer){
			ballSpeedY*=-1;
		}
		if(obj.tag=="paddle"&&Network.isServer){
			ballSpeedX*=-1;
		}
	}
}
