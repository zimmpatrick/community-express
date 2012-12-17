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
