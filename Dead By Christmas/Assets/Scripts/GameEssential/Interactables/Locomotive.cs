using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotive : Photon.MonoBehaviour {
	public Transform nodeParent;
	List<Transform> node = new List<Transform> ();
	Vector3 currentNodePos;
	public float speed;
	public float rotateSpeed;
	int trackIndex;

	void Start () {
		int index = 0;
		foreach (Transform child in nodeParent) {
			node.Add (child);
			child.name = "Node (" + index + ")";
			index += 1;
		}
		print ("Track parent has " + node.Count + " children.");
		transform.position = node[0].position;
		trackIndex = 1;
	}

	void Update () {
		currentNodePos = node[trackIndex].position;

		if (Vector3.Distance (transform.position, currentNodePos) > 0.1f) {
			transform.position = Vector3.MoveTowards (transform.position, currentNodePos, speed * Time.deltaTime);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (currentNodePos - transform.position), rotateSpeed * Time.deltaTime);
		} else {
			trackIndex += 1;
			if (trackIndex == node.Count) {
				trackIndex = 0;
			}
			print ("Switching to " + trackIndex + ". Count is " + node.Count);
		}
	}

}