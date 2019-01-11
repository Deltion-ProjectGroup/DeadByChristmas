using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour {
	//HEADER MOVEMENT
	//Movement vars
	[Header ("Movement")]

	//Walking
	public float baseSpeed; //The base speed
    [HideInInspector] public float speed; //New speed (with multipliers etc.)

	//Rotating
	[SerializeField] float rotateMultiplier; //Sensitivity of camera

	//HEADER HEALTH
	//Health vars
	[Header ("Health")]
	[SerializeField] float baseHealth; //The base health
	public float health; //Current health

	//HEADER INTERACCTION
	//Interaction vars
	[Header ("Interactions")]
	[SerializeField] float interactionRange; //Range of the interaction
	[SerializeField] string interactTag;
	bool carrying; //If player is currently carrying an item

	//HEADER CAMERA
	//Camera vars
	[Header ("Camera")]
	public Transform cam; //Transform of camera
	[SerializeField] Transform headBone; //Transform of head bone
	[SerializeField] float minX; //Minimum clamping of X
	[SerializeField] float maxX; //Maximum clamping of X
	float rotX; //Current X rot
	float rotY; //Current Y rot
    [Header ("Body")]
    public Rigidbody rig;
    public SkinnedMeshRenderer bodyRenderer;
    public Animator animator;
    //CALL THESE IN THE INHERITING SCRIPTS
    public void PlayerStart () {
        //Assign variables
        Cursor.lockState = CursorLockMode.Locked;
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
        Vector3 movePos = new Vector3();
		movePos.z = Input.GetAxis ("Vertical") * 1 * multiplier;
		movePos.x = Input.GetAxis ("Horizontal") * 1 * multiplier;

		//Translate the movement axis
        if(movePos.x == 0 && movePos.z == 0)
        {
            animator.SetBool("Walking", false);
        }
        else
        {
            animator.SetBool("Walking", true);
            transform.Translate(movePos); //Vertical axis
        }
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

	public bool CanInteract () {
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
			hit.transform.GetComponent<InteractableObject> ().interactingPlayer = transform;
			hit.transform.GetComponent<InteractableObject> ().Interact ();
		} else {
			print ("Hit is null!");
		}
	}

	public virtual void Death()
    {
        bodyRenderer.enabled = true;
        animator.SetTrigger("Death");
    }
    [PunRPC]
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {

        }
        else
        {

        }
    }
}