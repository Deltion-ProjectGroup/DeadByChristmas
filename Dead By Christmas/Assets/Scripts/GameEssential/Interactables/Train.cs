using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : InteractableObject {
	public Transform nodeParent;
	List<Transform> node = new List<Transform> ();
	Vector3 nextNodePos;
	public float speed;

	int trackIndex;

	void Start () {
		foreach (Transform child in nodeParent) {
			node.Add (child);
		}
		print ("Track parent has " + node.Count + " children.");
		transform.position = node[0].position;
		trackIndex = 1;
	}

	void Update () {
		nextNodePos = node[trackIndex].position;

		if (Vector3.Distance (transform.position, node[trackIndex].position) > 0.1f) {
			transform.position = Vector3.MoveTowards (transform.position, nextNodePos, speed * Time.deltaTime);
			transform.LookAt (node[trackIndex].position);
		} else {
			trackIndex += 1;
			if(trackIndex == node.Count) {
				trackIndex = 0;
			}
			print("Switching to " + trackIndex + ". Count is " + node.Count);

		}
	}
}