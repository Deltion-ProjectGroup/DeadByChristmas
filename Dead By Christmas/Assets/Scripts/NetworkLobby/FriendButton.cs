using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendButton : MonoBehaviour {
    public string roomName;

    public void Join()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
