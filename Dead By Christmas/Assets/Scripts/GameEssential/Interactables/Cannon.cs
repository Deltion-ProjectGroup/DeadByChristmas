using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : InteractableObject {
	[SerializeField] float force;
	[SerializeField] float sensitivity;
	[SerializeField] Transform playerInCannonParent;
	[SerializeField] Transform cannonCamera;
	public bool hasPlayer;

	Material cannonMat;

	void Start () {
		cannonMat = GetComponent<Renderer> ().material;
	}

	void Update () {
		if (hasPlayer) {
			cannonMat.color = Color.red;
		} else {
			cannonMat.color = Color.green;
		}
	}

	public override void Interact () {
		base.Interact ();
		CheckForPlayer ();

	}

	void CheckForPlayer () {
		if (!hasPlayer) {
			PutPlayerIn ();
		}
	}

	void PutPlayerIn () {
		SetHasPlayer (true);
		interactingPlayer.parent = playerInCannonParent;
		interactingPlayer.GetComponent<Player> ().enabled = false;
		SetPlayerVars (interactingPlayer, false);

		StartCoroutine ("SetToZero");

		StartCoroutine ("WaitForShoot");

	}

	IEnumerator SetToZero () {
		while (interactingPlayer.localRotation != Quaternion.Euler (Vector3.zero)) {
			interactingPlayer.localPosition = Vector3.zero;
			interactingPlayer.localRotation = Quaternion.Euler (Vector3.zero);

			yield return null;
		}

		StopCoroutine ("SetToZero");
	}

	IEnumerator WaitForShoot () {
		while (true) {
			if (Input.GetButtonDown ("Fire1")) {
				Shoot ();
			}
			yield return null;
		}
	}

	void SetPlayerVars (Transform t, bool b) {

		t.GetComponent<Collider> ().enabled = b;
		t.GetComponent<Rigidbody> ().useGravity = b;
		t.GetComponent<Player> ().cam.gameObject.SetActive (b);
		cannonCamera.gameObject.SetActive (!b);
	}

	void Aim () {

	}

	Transform shotPlayer;

	void Shoot () {
		if (hasPlayer) {
			shotPlayer = interactingPlayer;
			interactingPlayer = null;

			print ("Pew");
			shotPlayer.SetParent (null);
			SetPlayerVars (shotPlayer, true);
			shotPlayer.GetComponent<Rigidbody> ().AddForce (shotPlayer.up * force);

			StartCoroutine ("CheckIfLanded");

			SetHasPlayer (false);
		}
	}

	bool hasLanded;
	[SerializeField] LayerMask mask;

	IEnumerator CheckIfLanded () {

		yield return new WaitForSecondsRealtime (2f);
		while (!Physics.CheckSphere (shotPlayer.position, 0.5f, mask)) {
			yield return null;
		}

		print ("Found!");
		shotPlayer.GetComponent<Player> ().enabled = true;
		hasLanded = true;
		shotPlayer = null;

		yield return null;
	}

	void SetHasPlayer (bool b) {
		hasPlayer = b;
	}

	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
		print ("Has player before serializing: " + hasPlayer);
		//Check if you are writing to the network
		if (stream.isWriting) {
			stream.SendNext (hasPlayer);
		} else {
			hasPlayer = (bool) stream.ReceiveNext ();
		}
		print ("Has player after serializing: " + hasPlayer);
	}

}