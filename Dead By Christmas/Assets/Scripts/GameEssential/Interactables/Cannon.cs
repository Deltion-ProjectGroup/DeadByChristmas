using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : InteractableObject {
	[SerializeField] float force;
	[SerializeField] float sensitivity;
	[SerializeField] Transform playerInCannonParent;
	[SerializeField] Transform cannonCamera;
	public bool hasPlayer;
	public float timeToGetUp;

	void Start () { }

	void Update () { }

	public override void Interact (int id) {
		base.Interact (id);
		if (interactingPlayer.transform.GetComponent<ElfController> ()) {

			CheckForPlayer ();
		}

	}

	void CheckForPlayer () {
		if (!hasPlayer) {
			PutPlayerIn ();
		}
	}

	void PutPlayerIn () {
		//GetComponent<Collider> ().enabled = false;
		photonView.RPC ("SetHasPlayer", PhotonTargets.All, true);
		SetHasPlayer (true);
		interactingPlayer.transform.parent = playerInCannonParent;
		interactingPlayer.GetComponent<Player> ().enabled = false;

		SetPlayerVars (interactingPlayer.transform, false);

		//SetPlayerVars (interactingPlayer, false);

		StartCoroutine ("SetToZero");

		StartCoroutine ("WaitForShoot");

	}

	IEnumerator SetToZero () {
		if (interactingPlayer != null) {
			if (interactingPlayer.transform.parent != null) {
				while (interactingPlayer.transform.localRotation != Quaternion.Euler (Vector3.zero)) {
					interactingPlayer.transform.localPosition = Vector3.zero;
					interactingPlayer.transform.localRotation = Quaternion.Euler (90f, 0f, 0f);

					yield return null;
				}
			}
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

	Transform shotPlayer;

	void Shoot () {
		if (hasPlayer) {
			StopCoroutine ("SetToZero");
			shotPlayer = interactingPlayer.transform;
			interactingPlayer = null;

			print ("Pew");

			shotPlayer.SetParent (null);

			SetPlayerVars (shotPlayer.transform, true);
			//SetPlayerVars (shotPlayer, true);
			shotPlayer.GetComponent<Rigidbody> ().AddForce (shotPlayer.up * force);

			StartCoroutine ("CheckIfLanded");
			StartCoroutine ("WaitFor");

			photonView.RPC ("SetHasPlayer", PhotonTargets.All, true);
		}
	}

	IEnumerator WaitFor () {
		/*Rigidbody[] playerColider = shotPlayer.GetComponent<ElfController>().bones;
		foreach(Rigidbody rig in playerColider){
			rig.GetComponent<Collider>().isTrigger = true;
		}*/
		yield return new WaitForSeconds (0.5f);
		/*foreach(Rigidbody rig in playerColider){
			rig.GetComponent<Collider>().isTrigger = false;
		}*/

	}

	bool hasLanded = false;
	[SerializeField] LayerMask mask;

	IEnumerator CheckIfLanded () {
		yield return new WaitForSecondsRealtime (1f);
		while (!Physics.CheckSphere (shotPlayer.position, 0.5f, mask)) {
			yield return null;
		}

		print ("Found!");
		StartCoroutine (WaitForStand ());
		yield return null;
	}

	IEnumerator WaitForStand () {
		yield return new WaitForSecondsRealtime (timeToGetUp);
		shotPlayer.GetComponent<Player> ().enabled = true;
		hasLanded = true;
		shotPlayer = null;

		StopCoroutine (WaitForStand ());
		yield return null;
	}

	[PunRPC]
	void SetHasPlayer (bool b) {
		hasPlayer = b;
	}

	// void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
	// 	print ("Has player before serializing: " + hasPlayer);
	// 	//Check if you are writing to the network
	// 	if (stream.isWriting) {
	// 		stream.SendNext (hasPlayer);
	// 	} else {
	// 		hasPlayer = (bool) stream.ReceiveNext ();
	// 	}
	// 	print ("Has player after serializing: " + hasPlayer);
	// }

}