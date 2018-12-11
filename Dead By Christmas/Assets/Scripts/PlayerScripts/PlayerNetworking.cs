using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworking : MonoBehaviour {
	Vector3 position; //Player pos for the network
	Quaternion rotation; //Player rotation for the network
	[SerializeField] float smoothing; //Lerp smoothing of position and rotation
	public GameObject cam;
	PhotonView photonView; //PhotonView of player
	
	void Awake () {
		photonView = GetComponent<PhotonView>(); //Get the PhotonView of the player

		//Check if the PhotonView is yours
		if (photonView.isMine) { 
			print("Is mine...");
			GetComponent<Player> ().enabled = true; //Enable player behaviour script
			cam.SetActive(true);
		} else {
			StartCoroutine ("UpdateData"); //Update the data of the player
		}
	}
    

	IEnumerator UpdateData () {
		while (true) {
			transform.position = Vector3.Lerp (transform.position, position, Time.deltaTime * smoothing);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
			yield return null;
		}
	}

	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
		//Check if you are writing to the network
		if (stream.isWriting) {
			stream.SendNext (transform.position);
			stream.SendNext(transform.rotation);
		} else {
			position = (Vector3) stream.ReceiveNext ();
			rotation = (Quaternion) stream.ReceiveNext();
		}
	}

}