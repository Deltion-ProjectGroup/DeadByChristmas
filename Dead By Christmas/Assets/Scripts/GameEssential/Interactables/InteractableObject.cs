using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : Photon.MonoBehaviour {
	public Transform interactingPlayer;
	public virtual void Interact () {
		print("Interacting with " + transform.name);
	}
}
