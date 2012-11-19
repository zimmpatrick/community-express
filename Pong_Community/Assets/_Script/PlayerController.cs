using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed = 5; 
	// Use this for initialization 
	void Start () {
	    if(!networkView.isMine)
	    {
	        enabled = false;
	    }
	}
// Update is called once per frame 
	void Update () { 
		if(networkView.isMine) { 
			Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0 ); 
			transform.Translate(speed * moveDir * Time.deltaTime);
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

}
