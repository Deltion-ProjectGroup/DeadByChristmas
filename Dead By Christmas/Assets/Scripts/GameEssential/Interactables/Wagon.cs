using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : InteractableObject {
	public bool[] seats;
	public Transform[] seatTransform;

	int index = 0;

	public override void Interact (int interactorID) {
		base.Interact(interactorID);
		print("Interacting with wagon");

		if (interactingPlayer.GetComponent<ElfController> ()) {
			foreach (bool seat in seats) {

				//False means not taken
				if (seat == false) {
					photonView.RPC("PutPlayerIn", PhotonTargets.All);
					break;
				}
				index += 1;
			}
		}
	}

	IEnumerator WaitForInput () {
		while (true) {
			if (Input.GetButtonDown ("Use")) {
				photonView.RPC("PutPlayerOut", PhotonTargets.All);
				StopCoroutine("WaitForInput");
			}
			yield return null;
		}

	}

	[PunRPC]
	void PutPlayerIn () {
		print("Putting player in.");
		interactingPlayer.GetComponent<ElfController> ().enabled = false;
		interactingPlayer.transform.position = seatTransform[index].position;
		interactingPlayer.transform.parent = seatTransform[index];
		WaitForInput ();
		seats[index] = true;
	}

	[PunRPC]
	void PutPlayerOut () {
		print("Putting player out.");
		interactingPlayer.GetComponent<ElfController> ().enabled = true;
		interactingPlayer.transform.parent = null;
		seats[index] = false;
	} 
}