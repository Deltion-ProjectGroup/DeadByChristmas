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
	[SerializeField] float minX;
	[SerializeField] float maxX;
	float rotX;
	float rotY;

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
		if (canInteract ()) {
			if (Input.GetButtonDown ("Use")) {
				Interact ();
			}
		}
	}

	bool canInteract () {
		RaycastHit hit;

		if (Physics.Raycast (cam.position, cam.forward, out hit, interactionRange)) {
			if (hit.transform.tag == interactTag) {
				return true;
			}
		}

		return false;
	}

	public void Interact () {
		print ("Interacting...");
	}

	public void Death () {
		print ("Oof");
	}

	public void ReceiveDamage (int damageAmount) {
		health -= damageAmount;

		if (health <= 0) {
			Death ();
		}
	}

	void CamRotate () {
		//Make multiplier
		float multiplier = rotateMultiplier * Time.deltaTime;

		float xToAdd = Input.GetAxis ("Mouse Y") * -1f * multiplier;
		rotX += xToAdd;
		rotX = clamped (rotX, minX, maxX);


		float yToAdd = Input.GetAxis ("Mouse X") * multiplier;
		rotY += yToAdd;
	
		headBone.localRotation = Quaternion.Euler (rotX, 0.0f, 0.0f);
		transform.rotation = Quaternion.Euler(0.0f, rotY, 0.0f);

	}

	float clamped (float input, float min, float max) {
		if (input < min) {
			return min;
		} else if (input > max) {
			return max;
		} else {
			return input;
		}
	}
}