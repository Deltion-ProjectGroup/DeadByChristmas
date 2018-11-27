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
    public string readyText, neutralText, waitForPlayerText, readyUpText, loadText;
    public int waitTime, remainingTime, minPlrRequired;
    public Text timerText;
    [Header("IntermissionStuff")]
    bool inIntermission;

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
        ReSort();
        GetComponent<PhotonView>().RPC("CheckReadyPlayers", PhotonTargets.MasterClient);
    }
    [PunRPC]
    //If a player left everyone will get the new playerlist
    public IEnumerator RemovePlayer()
    {
        yield return null;
        GetPlayers();
        GetComponent<PhotonView>().RPC("CheckReadyPlayers", PhotonTargets.MasterClient);
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
    [PunRPC]
    //Checks if everyone is ready and changes the readyText if needed
    public IEnumerator CheckReadyPlayers()
    {
        yield return null;
        if(allPlayers.Count + 1 >= minPlrRequired)
        {
            if (AllReady())
            {
                StartCoroutine(Intermission());
            }
            else
            {
                timerText.text = readyUpText;
                if (inIntermission)
                {
                    StopAllCoroutines();
                }
            }
        }
        else
        {
            timerText.text = waitForPlayerText;
            if (inIntermission)
            {
                StopAllCoroutines();
            }
        }
    }
    //this actually checks if everyone is ready
    bool AllReady()
    {
        if (!localPlayer.GetComponent<LobbyPlayer>().isReady)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if (!allPlayers[i].GetComponent<LobbyPlayer>().isReady)
                {
                    return false;
                }
            }
        }
        return true;
    }
    //The intermission countdown
    IEnumerator Intermission()
    {
        inIntermission = true;
        remainingTime = waitTime;
        while(remainingTime > 0)
        {
            yield return new WaitForSeconds(1);
            if(allPlayers.Count + 1 < minPlrRequired)
            {
                timerText.text = waitForPlayerText;
                StopAllCoroutines();
            }
            else
            {
                if (!AllReady())
                {
                    timerText.text = readyUpText;
                    StopAllCoroutines();
                }
            }
            remainingTime--;
            timerText.text = remainingTime.ToString();
        }
        timerText.text = loadText;
        inIntermission = false;
    }
    //Toggles the ready button
    public void ToggleReady(Image readyButton)
    {
        if (localPlayer.GetComponent<LobbyPlayer>().isReady)
        {
            localPlayer.GetComponent<LobbyPlayer>().isReady = false;
            readyButton.color = neutralColor;
            localPlayer.GetComponent<LobbyPlayer>().readyText.text = neutralText;
        }
        else
        {
            localPlayer.GetComponent<LobbyPlayer>().isReady = true;
            readyButton.color = readyColor;
            localPlayer.GetComponent<LobbyPlayer>().readyText.text = readyText;
        }
        GetComponent<PhotonView>().RPC("CheckReadyPlayers", PhotonTargets.MasterClient);
    }
    //What happens if the master client left
    public void OnMasterClientSwitched()
    {
        GetComponent<PhotonView>().RPC("RemovePlayer", PhotonTargets.MasterClient);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(timerText.text);
        }
        else
        {
            timerText.text = (string)stream.ReceiveNext();
        }
    }
}
