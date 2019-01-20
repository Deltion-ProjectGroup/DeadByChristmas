using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRoomJoin : MonoBehaviour {
	public string playerName;	

	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings ("0.1");
	}

	void OnJoinedLobby () {
		RoomOptions ro = new RoomOptions () { IsVisible = true, MaxPlayers = 10 };
		PhotonNetwork.JoinOrCreateRoom ("Mike", ro, TypedLobby.Default);
    }

	void OnJoinedRoom() {
		PhotonNetwork.Instantiate(playerName, Vector3.zero, Quaternion.Euler(Vector3.zero), 0);
	}

	// Update is called once per frame
	void Update () {

	}
}