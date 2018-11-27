using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	//HEADER MOVEMENT
	//Movement vars
	[Header ("Movement")]

	//Walking
	[SerializeField] float baseSpeed; //The base speed
	float speed; //New speed (with multipliers etc.)

	//Jumping
	[SerializeField] float baseJumpPower; //The base jump power
	float jumpPower; //New jump power (with multipliers etc.)
	bool canJump;

	//Rotating
	[SerializeField] float rotateMultiplier; //Sensitivity of camera

	//Other
	Rigidbody playerRigidbody; //Rigidbody of player
	[SerializeField] string jumpTag;

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

	void Start () {
		//Assign variables
		speed = baseSpeed;
		jumpPower = baseJumpPower;
		health = baseHealth;
		playerRigidbody = GetComponent<Rigidbody> ();

		rotX = headBone.rotation.x;
		rotY = transform.rotation.y;
	}

	void Update () {
		//Get jump input
		if (Input.GetButtonDown ("Jump")) {
			if (canJump) {
				Jump ();
			}
		}

		CheckInteract ();
		CamRotate ();
	}

	void FixedUpdate () {
		//Walk
		Walk ();
	}

	void OnCollisionEnter (Collision c) {
		//Check if the entered object is jumpable
		if (c.transform.tag == jumpTag) {
			canJump = true;
		}
	}

	void OnCollisionExit (Collision c) {
		//Check if the exited object is jumpable
		if (c.transform.tag == jumpTag) {
			canJump = false;
		}
	}

	void Jump () {
		//Add upward force
		playerRigidbody.AddForce (Vector3.up * jumpPower);
	}

	void Walk () {
		//Make multiplier
		float multiplier = speed * Time.deltaTime;
		Vector3 verticalAxis = Input.GetAxis ("Vertical") * Vector3.forward * multiplier;
		Vector3 horizontalAxis = Input.GetAxis ("Horizontal") * Vector3.right * multiplier;

		//Translate the movement axis
		transform.Translate (verticalAxis); //Vertical axis
		transform.Translate (horizontalAxis); //Horizontal axis
	}

	public void CheckInteract () {
		//Check if canInteract is true
		if (canInteract ()) {
			//Check for use input
			if (Input.GetButtonDown ("Use")) {
				Interact ();
			}
		}
	}

	bool canInteract () {
		//RaycastHit of camera
		RaycastHit hit;

		//Shoot ray
		if (Physics.Raycast (cam.position, cam.forward, out hit, interactionRange)) {

			//Check if the hit object is interactable
			if (hit.transform.tag == interactTag) {
				//Set bool to true
				return true;
			}
		}
		//Set bool to false
		return false;
	}

	public void Interact () {
		//To do if interacting
		print ("Interacting...");
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
		float xToAdd = Input.GetAxis ("Mouse Y") * -1f * multiplier;
		rotX += xToAdd;
		rotX = clamped (rotX, minX, maxX);

		//Set X rotation
		headBone.localRotation = Quaternion.Euler (rotX, 0.0f, 0.0f);

		//Make Y rotation
		float yToAdd = Input.GetAxis ("Mouse X") * multiplier;
		rotY += yToAdd;

		//Set Y rotation
		transform.rotation = Quaternion.Euler (0.0f, rotY, 0.0f);

	}

	float clamped (float input, float min, float max) {
		//Check if input is under the min
		if (input < min) {
			//Set float to min
			return min;
		}
		//Check if input is above the max
		else if (input > max) {
			//Set float to max
			return max;
		} else {
			//Set float to input
			return input;
		}
	}
}