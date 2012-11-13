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
