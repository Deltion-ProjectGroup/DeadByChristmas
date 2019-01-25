﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonOptions : MonoBehaviour {

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenuScene");
    }
    public void Continue(bool ingame)
    {
        if (ingame)
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().optToggled = false;
            GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().localPlayer.GetComponent<Player>().paused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameLobby>().optToggled = false;
        }
        gameObject.SetActive(false);
    }
}
