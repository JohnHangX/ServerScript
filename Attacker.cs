using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Physics.IgnoreLayerCollision(10, 11, true);
        Physics.IgnoreLayerCollision(10, 10, true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
