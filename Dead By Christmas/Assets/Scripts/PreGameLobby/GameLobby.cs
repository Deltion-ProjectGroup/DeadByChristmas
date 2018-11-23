using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLobby : MonoBehaviour {
    [Header("PlayerData")]
    public GameObject localPlayer;
    public List<GameObject> allPlayers = new List<GameObject>();
    public Transform[] playerPlatformPositions;
    [Header("ReadyData")]
    public Color readyColor, neutralColor;
    public string readyText, neutralText, waitForPlayerText, readyUpText;
    public int waitTime, minPlrRequired;
    public Text timerText;

    //Spawns the player in the lobby 
    public void OnJoinedRoom()
    {
        localPlayer = PhotonNetwork.Instantiate("LobbyPlayer", Vector3.zero, Quaternion.identity, 0);
        localPlayer.GetComponent<LobbyPlayer>().readyText.text = neutralText;
        localPlayer.GetComponent<LobbyPlayer>().userName.text = PhotonNetwork.player.NickName;
        GetComponent<PhotonView>().RPC("AddPlayer", PhotonTargets.All);
    }
    //The void that makes sure the game starts
    public void StartGame()
    {

    }
    public void Leave()
    {

    }
    [PunRPC]
    //If the player joins it gets all the players and resorts them
    public IEnumerator AddPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        GetPlayers();
        print("GOT PLAYERS");
        ReSort();
        print("RESORTED");

    }
    [PunRPC]
    //If a player left everyone will get the new playerlist
    public IEnumerator RemovePlayer()
    {
        yield return null;
        GetPlayers();
    }
    //Gets all the players in the game and sets the list to it
    public void GetPlayers()
    {
        allPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        allPlayers.Remove(localPlayer);
    }
    //Places everyone on their pedestal like they should be
    public void ReSort()
    {
        localPlayer.transform.position = playerPlatformPositions[0].position;
        for(int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].transform.position = playerPlatformPositions[i + 1].position;
        }
    }
    //Toggles the ready button
    public void ToggleReady(Image readyButton)
    {
        if (localPlayer.GetComponent<LobbyPlayer>().isReady)
        {
            localPlayer.GetComponent<LobbyPlayer>().isReady = false;
            readyButton.color = neutralColor;
            localPlayer.GetComponent<LobbyPlayer>().readyText.text = readyText;
        }
        else
        {
            localPlayer.GetComponent<LobbyPlayer>().isReady = true;
            readyButton.color = readyColor;
            localPlayer.GetComponent<LobbyPlayer>().readyText.text = neutralText;
        }
    }
}
