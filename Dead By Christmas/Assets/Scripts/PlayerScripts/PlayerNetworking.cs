using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworking : MonoBehaviour {
	Vector3 position;
	[SerializeField] float smoothing;
	PhotonView photonView;
	
	void Start () {
		photonView = GetComponent<PhotonView>();
		if (photonView.isMine) {
			print("Is mine...");
			GetComponent<Player> ().enabled = true;

		} else {
			print("Is not mine...");
			StartCoroutine ("UpdateData");
		}
	}

	IEnumerator UpdateData () {
		while (true) {
			transform.position = Vector3.Lerp (transform.position, position, Time.deltaTime * smoothing);

			yield return null;
		}
	}

	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext (transform.position);
		} else {

			position = (Vector3) stream.ReceiveNext ();

		}
	}

}