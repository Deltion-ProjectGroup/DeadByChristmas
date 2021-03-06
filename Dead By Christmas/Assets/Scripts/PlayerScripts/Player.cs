﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour {
    //HEADER MOVEMENT
    [HideInInspector] public bool paused;
    public float animTransSmooth;
    public AudioSource[] audioSources;
    public AudioClip[] audioClips;
    //Movement vars
    [Header ("Movement")]

	//Walking
	public float baseSpeed; //The base speed
    public float speed; //New speed (with multipliers etc.)
    [HideInInspector] public float extraMovmentMultiplier = 1;

	//Rotating
	[SerializeField] float rotateMultiplier; //Sensitivity of camera

	//HEADER HEALTH
	//Health vars
	[Header ("Health")]
	public float baseHealth; //The base health
	public float health; //Current health

	//HEADER INTERACCTION
	//Interaction vars
	[Header ("Interactions")]
	public float interactionRange; //Range of the interaction
	public string interactTag;
    public string interactInput;
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
    public SkinnedMeshRenderer[] bodyRenderer;
    public SkinnedMeshRenderer[] allRenderer;
    public Animator animator;
    public bool canInteract = true;
    //CALL THESE IN THE INHERITING SCRIPTS
    public void PlayerStart () {
        //Assign variables
        Cursor.lockState = CursorLockMode.Locked;
		speed = baseSpeed;
		health = baseHealth;

		rotX = headBone.rotation.x;
		rotY = transform.rotation.y;

        foreach(SkinnedMeshRenderer renderer in bodyRenderer)
        {
            renderer.enabled = false;
        }
	}

	public void PlayerUpdate () {
        if (!paused)
        {
            CheckInteract();
            CamRotate();
        }
	}

	public void PlayerFixedUpdate () {
        if (!paused)
        {
            Walk();
        }
	}
	//^^^ CALL THESE IN THE INHERITING SCRIPTS ^^^

	public virtual void Walk () {
        //Make multiplier
        float multiplier = speed * Time.deltaTime * extraMovmentMultiplier;
        Vector3 movePos = new Vector3();
        movePos.z = Input.GetAxis("Vertical");
        movePos.x = Input.GetAxis("Horizontal");
        if (movePos == Vector3.zero)
        {
            extraMovmentMultiplier = Mathf.MoveTowards(extraMovmentMultiplier, 0, animTransSmooth);
        }
        else
        {
            extraMovmentMultiplier = Mathf.MoveTowards(extraMovmentMultiplier, 1, animTransSmooth);
        }
        animator.SetFloat("MovementSpeed", extraMovmentMultiplier * multiplier);

        //Translate the movement axis
        transform.Translate(movePos * multiplier * Time.deltaTime); //Vertical axis
    }

	//RaycastHit of camera
	public RaycastHit hit;

	public void CheckInteract () {
		if (CanInteract ()) {
            if (Input.GetButtonDown ("Use") && canInteract) {
                animator.SetBool("Interact", true);
                StartCoroutine(ChangeAnimBool("Interact", false));
				Interact ();
			}
        }
	}

	public virtual bool CanInteract () {
		//Shoot ray
		if (Physics.Raycast (cam.position, cam.forward, out hit, interactionRange)) {

			//Check if the hit object is interactable
			if (hit.transform.tag == interactTag) {

                return true;
            }
		}
        return false;
	}

	public virtual void Interact () {
		//To do if interacting
		print ("Interacting...");
		if (hit.collider != null) {
			hit.transform.GetComponent<InteractableObject> ().interactingPlayer = gameObject;
			hit.transform.GetComponent<InteractableObject> ().Interact (GetComponent<PhotonView>().ownerId);
		} else {
			print ("Hit is null!");
		}
	}

	public virtual void Death()
    {
        foreach (SkinnedMeshRenderer renderer in bodyRenderer)
        {
            renderer.enabled = false;
        }
        animator.SetBool("Death", true);
    }
    [PunRPC]
	public virtual void ReceiveDamage (int damageAmount) {
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

    public IEnumerator ChangeAnimBool(string parameterName, bool value)
    {
        yield return new WaitForEndOfFrame();
        animator.SetBool(parameterName, value);
    }
}