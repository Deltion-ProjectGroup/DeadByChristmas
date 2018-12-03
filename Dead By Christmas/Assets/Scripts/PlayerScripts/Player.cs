﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	//HEADER MOVEMENT
	//Movement vars
	[Header ("Movement")]

	//Walking
	[SerializeField] float baseSpeed; //The base speed
	float speed; //New speed (with multipliers etc.)

	//Rotating
	[SerializeField] float rotateMultiplier; //Sensitivity of camera

	//HEADER HEALTH
	//Health vars
	[Header ("Health")]
	[SerializeField] float baseHealth; //The base health
	float health; //Current health

	//HEADER INTERACCTION
	//Interaction vars
	[Header ("Interactions")]
	[SerializeField] float interactionRange; //Range of the interaction
	[SerializeField] string interactTag;
	bool carrying; //If player is currently carrying an item

	//HEADER CAMERA
	//Camera vars
	[Header ("Camera")]
	[SerializeField] Transform cam; //Transform of camera
	[SerializeField] Transform headBone; //Transform of head bone
	[SerializeField] float minX; //Minimum clamping of X
	[SerializeField] float maxX; //Maximum clamping of X
	float rotX; //Current X rot
	float rotY; //Current Y rot

	//CALL THESE IN THE INHERITING SCRIPTS
	public void PlayerStart () {
		//Assign variables
		speed = baseSpeed;
		health = baseHealth;

		rotX = headBone.rotation.x;
		rotY = transform.rotation.y;
	}

	public void PlayerUpdate () {
		CheckInteract ();
		CamRotate ();
	}

	public void PlayerFixedUpdate () {
		Walk ();
	}
	//^^^ CALL THESE IN THE INHERITING SCRIPTS ^^^

	void Walk () {
		//Make multiplier
		float multiplier = speed * Time.deltaTime;
		Vector3 verticalAxis = Input.GetAxis ("Vertical") * Vector3.forward * multiplier;
		Vector3 horizontalAxis = Input.GetAxis ("Horizontal") * Vector3.right * multiplier;

		//Translate the movement axis
		transform.Translate (verticalAxis); //Vertical axis
		transform.Translate (horizontalAxis); //Horizontal axis
	}

	//RaycastHit of camera
	RaycastHit hit;

	public void CheckInteract () {
		if (CanInteract ()) {
			if (Input.GetButtonDown ("Use")) {
				Interact ();
			}
		}
	}

	bool CanInteract () {
		//Shoot ray
		if (Physics.Raycast (cam.position, cam.forward, out hit, interactionRange)) {

			//Check if the hit object is interactable
			if (hit.transform.tag == interactTag) {
				return true;
			}
		}
		return false;
	}

	public void Interact () {
		//To do if interacting
		print ("Interacting...");
		if (hit.collider != null) {

			hit.transform.GetComponent<InteractableObject> ().Interact ();
		} else {
			print("Hit is null!");
		}
	}

	public void Death () {
		//To do if dying
		print ("Oof");
	}

	public void ReceiveDamage (int damageAmount) {
		//Subtract damage amount to health
		health -= damageAmount;

		//Check if health is under zero
		if (health <= 0) {
			Death ();
		}
	}

	void CamRotate () {
		//Make multiplier
		float multiplier = rotateMultiplier * Time.deltaTime;

		//Make X rotation
		float xToAdd = -Input.GetAxis ("Mouse Y") * multiplier;
		rotX += xToAdd;
		rotX = Clamped (rotX, minX, maxX);

		//Set X rotation
		headBone.localRotation = Quaternion.Euler (rotX, 0.0f, 0.0f);

		//Make Y rotation
		float yToAdd = Input.GetAxis ("Mouse X") * multiplier;
		rotY += yToAdd;

		//Set Y rotation
		transform.rotation = Quaternion.Euler (0.0f, rotY, 0.0f);

	}

	float Clamped (float input, float min, float max) {
		//Check if input is under the min
		if (input < min) {
			//Set float to min
			return min;
		}
		//Check if input is above the max
		else if (input > max) {
			return max;
		} else {
			return input;
		}
	}
}