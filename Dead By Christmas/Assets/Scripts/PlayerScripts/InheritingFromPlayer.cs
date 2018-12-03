using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InheritingFromPlayer : Player {

	// Use this for initialization
	void Start () {
		PlayerStart();
	}
	
	// Update is called once per frame
	void Update () {
		PlayerUpdate();
	}

	void FixedUpdate() {
		PlayerFixedUpdate();
	}
}
