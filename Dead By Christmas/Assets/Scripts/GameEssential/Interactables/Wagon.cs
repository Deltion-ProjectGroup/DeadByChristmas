using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : InteractableObject {
	public bool[] seats;
	public Transform[] seatTransform;

	public override void Interact (int interactorID) {
		if (interactingPlayer.GetComponent<ElfController> ()) {
			int index = 0;

			foreach (bool seat in seats) {
				//False means not taken
				if (seat == false) {
					PutPlayerIn (index);
					break;
				}
				index += 1;
			}
		}
	}

	void PutPlayerIn (int index) {
		interactingPlayer.GetComponent<ElfController> ().enabled = false;
		interactingPlayer.transform.position = seatTransform[index].position;
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