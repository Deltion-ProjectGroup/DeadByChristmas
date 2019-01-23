using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : InteractableObject {
	public bool[] seats;
	public Transform[] seatTransform;

	int index = 0;

	public override void Interact (int interactorID) {
		if (interactingPlayer.GetComponent<ElfController> ()) {
			foreach (bool seat in seats) {
				//False means not taken
				if (seat == false) {
					PutPlayerIn ();
					break;
				}
				index += 1;
			}
		}
	}

	IEnumerator WaitForInput () {
		while (true) {
			if (Input.GetButtonDown ("Use")) {
				PutPlayerOut();
				StopCoroutine("WaitForInput");
			}
			yield return null;
		}

	}

	void PutPlayerIn () {
		interactingPlayer.GetComponent<ElfController> ().enabled = false;
		interactingPlayer.transform.position = seatTransform[index].position;
		interactingPlayer.transform.parent = seatTransform[index];
		WaitForInput ();
		seats[index] = true;
	}

	void PutPlayerOut () {
		interactingPlayer.GetComponent<ElfController> ().enabled = true;
		interactingPlayer.transform.parent = null;
		seats[index] = false;
	}

	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {

		//Check if you are writing to the network
		if (stream.isWriting) {
			stream.SendNext (seats);
		} else {
			seats = (bool[]) stream.ReceiveNext ();
		}

	}

}