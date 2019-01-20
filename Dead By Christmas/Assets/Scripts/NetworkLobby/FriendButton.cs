using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendButton : MonoBehaviour {
    public string roomName;
    [HideInInspector]
    public bool inGame;
    public Text gameStatus;

    public void Join()
    {
        if (inGame && !GameObject.FindGameObjectWithTag("Manager").GetComponent<NetworkLobby>().loading)
        {
            PhotonNetwork.JoinRoom(roomName);
            GameObject.FindGameObjectWithTag("Manager").GetComponent<NetworkLobby>().loading = true;
        }
    }
}
