using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : InteractableObject {
	[SerializeField] float force;
	[SerializeField] float sensitivity;
	public bool hasPlayer;

	public override void Interact() {
		base.Interact();
	}

	void Aim() {

	}

	void Shoot() {

	}
}
