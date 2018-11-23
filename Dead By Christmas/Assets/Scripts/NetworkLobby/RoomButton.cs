using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour {
    
    public Text lobbyNameInput, lobbyPlayerInput;
    private RoomInfo info;
    private NetworkLobby lobbyManager;

    //Sets the Information of a RoomButton.
    public void SetInfo(RoomInfo aInfo, NetworkLobby aLobbyManager)
    {
        info = aInfo;
        lobbyManager = aLobbyManager;

        lobbyNameInput.text = info.Name;
        lobbyPlayerInput.text = "Players : " + info.PlayerCount + " / " + info.MaxPlayers;
    }

    //Starts the JoinCertainRoom on the NetworkLobby once the button is pressed.
    public void OnButtonPressed()
    {
        lobbyManager.JoinCertainRoom(info.Name);
    }
}
