using UnityEngine;
using System.Collections;

public class PaddleControl : MonoBehaviour {

	public float paddleSpeed = 30;
	public bool cantMove = false;
	private bool firstTimeHit = false;

	void FixedUpdate () {
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
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
{
    if (stream.isWriting)
    {
        Vector3 pos = transform.position;
        stream.Serialize(ref pos);
    }
    else
    {
        Vector3 posRec = Vector3.zero;
        stream.Serialize(ref posRec);
        transform.position = posRec;
    }
}


	
	
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
