using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLobby : MonoBehaviour {
    [Header("PlayerData")]
    public GameObject localPlayer;
    public List<GameObject> allPlayers = new List<GameObject>();
    public Transform[] playerPlatformPositions;

    //Spawns the player in the lobby 
    public void OnJoinedRoom()
    {
        localPlayer = PhotonNetwork.Instantiate("LobbyPlayer", Vector3.zero, Quaternion.identity, 0);
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
    public IEnumerator AddPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        GetPlayers();
        print("GOT PLAYERS");
        ReSort();
        print("RESORTED");

    }
    [PunRPC]
    public IEnumerator RemovePlayer()
    {
        yield return null;
        GetPlayers();
    }
    public void GetPlayers()
    {
        allPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        allPlayers.Remove(localPlayer);
    }
    public void ReSort()
    {
        localPlayer.transform.position = playerPlatformPositions[0].position;
        for(int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].transform.position = playerPlatformPositions[i + 1].position;
        }
    }
}
