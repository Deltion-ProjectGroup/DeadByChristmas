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
    [Header("CustomProperties")]
    public GameObject masterOptions;
    public bool visible = true;
    public bool toggledOpt;
    [Header("Friending")]
    public GameObject friendRequest;
    public Text requesterName;
    public List<PhotonPlayer> remaingRequests = new List<PhotonPlayer>();

    public GameObject friendRequestAnswer;
    public Text receiverName;
    public Text receiveMessageText;
    public List<PhotonPlayer> remaingReceives = new List<PhotonPlayer>();
    //Spawns the player in the lobby 
    void OnJoinedRoom()
    {
        print("JOINED");
        localPlayer = PhotonNetwork.Instantiate("LobbyPlayer", Vector3.zero, Quaternion.identity, 0);
        localPlayer.GetComponent<LobbyPlayer>().friendButton.SetActive(false);
        if (localPlayer != null)
        {
            localPlayer.GetComponent<LobbyPlayer>().readyText.text = neutralText;
            localPlayer.GetComponent<LobbyPlayer>().userName.text = PhotonNetwork.player.NickName;
        }
        GetComponent<PhotonView>().RPC("AddPlayer", PhotonTargets.All);
        if (PhotonNetwork.isMasterClient)
        {
            masterOptions.SetActive(true);
        }
        StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
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
        yield return new WaitForSeconds(0.1f);
        GetPlayers();
        GetComponent<PhotonView>().RPC("CheckReadyPlayers", PhotonTargets.MasterClient);
    }
    //Gets all the players in the game and sets the list to it
    public void GetPlayers()
    {
        allPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        allPlayers.Remove(localPlayer);
        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < allPlayers.Count; i++)
            {
                allPlayers[i].GetComponent<LobbyPlayer>().kickButton.SetActive(true);
            }
        }
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
        yield return new WaitForSeconds(0.1f);
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
            GetPlayers();
            if (allPlayers.Count + 1 < minPlrRequired)
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
                else
                {
                    remainingTime--;
                    timerText.text = remainingTime.ToString();
                }
            }
        }
        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.room.IsVisible = false;
        timerText.text = loadText;
        inIntermission = false;
        TransitionScreen.transitionScreen.GetComponent<PhotonView>().RPC("FadeIn", PhotonTargets.All);
        yield return new WaitForSeconds(TransitionScreen.transitionScreen.GetComponent<TransitionScreen>().screen.GetComponent<Animation>().GetClip("TransitionFadeIn").length);
        
        PhotonNetwork.LoadLevel("Game");
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
    public void ToggleOptions(RectTransform toggleIcon)
    {
        Vector3 newAngle = toggleIcon.transform.localEulerAngles;
        if (toggledOpt)
        {
            toggledOpt = false;
            masterOptions.GetComponent<Animation>().Play("MasterOptDisappear");
            newAngle.z -= 180;
        }
        else
        {
            toggledOpt = true;
            masterOptions.GetComponent<Animation>().Play("MasterOptAppear");
            newAngle.z += 180;
        }
        toggleIcon.localEulerAngles = newAngle;
    }
    //Toggles the lock of the room
    public void ToggleLock(GameObject img)
    {
        if (PhotonNetwork.room.IsOpen)
        {
            img.SetActive(true);
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.room.IsVisible = false;
        }
        else
        {
            img.SetActive(false);
            PhotonNetwork.room.IsOpen = true;
            if (visible)
            {
                PhotonNetwork.room.IsVisible = true;
            }
        }
    }
    //Toggles the visibility of the room
    public void ToggleVisible(GameObject img)
    {
        if (visible)
        {
            visible = false;
            img.SetActive(true);
            PhotonNetwork.room.IsVisible = false;
        }
        else
        {
            visible = true;
            img.SetActive(false);
            if (PhotonNetwork.room.IsOpen)
            {
                PhotonNetwork.room.IsVisible = true;
            }
        }
    }
    //What happens if the master client left
    public void OnMasterClientSwitched()
    {
        if (PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("RemovePlayer", PhotonTargets.MasterClient);
            masterOptions.SetActive(true);
        }
    }
    //Kicks a player
    [PunRPC]
    public void KickPlayer(PhotonPlayer playerToKick)
    {
        if (PhotonNetwork.isMasterClient)
        { 
            PhotonNetwork.CloseConnection(playerToKick);
        }
    }
    //When a player leaves
    public void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenuScene");
    }
    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        GetPlayers();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(timerText.text);
            stream.SendNext(visible);
        }
        else
        {
            timerText.text = (string)stream.ReceiveNext();
            visible = (bool)stream.ReceiveNext();
        }
    }
    [PunRPC]
    public void ReceiveFriendRequest(PhotonPlayer requester)
    {
        remaingRequests.Add(requester);
        if(remaingRequests.Count == 1)
        {
            currentCoroutine = ShowFriendRequest(1);
            StartCoroutine(currentCoroutine);
        }
    }
    [PunRPC]
    public void SendRequestAnswerBack(bool accepted, PhotonPlayer receiver, bool timedOut = true)
    {

        friendRequestAnswer.SetActive(true);
        receiverName.text = receiver.NickName;
        if (accepted)
        {
            SaveDatabase.data.userData.friends.Add(receiver.NickName);
            receiveMessageText.text = "Has accepted your request";
        }
        else
        {
            if (timedOut)
            {
                receiveMessageText.text = "Didn't respond in time";
            }
            else
            {
                receiveMessageText.text = "Has denied your request";
            }
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject target in targets)
            {
                if(target.GetComponent<LobbyPlayer>().userName.text == receiver.NickName)
                {
                    target.GetComponent<LobbyPlayer>().friendButton.SetActive(true);
                }
            }
        }
    }
    public void FriendRequestDecision(bool accepted)
    {
        if (accepted)
        {
            SaveDatabase.data.userData.friends.Add(remaingRequests[0].NickName);
            GetComponent<PhotonView>().RPC("SendRequestAnswerBack", remaingRequests[0], true, PhotonNetwork.player, false);
            StopCoroutine("ShowFriendRequest");
            friendRequest.SetActive(false);
        }
        else
        {
            GetComponent<PhotonView>().RPC("SendRequestAnswerBack", remaingRequests[0], false, PhotonNetwork.player, false);
            StopCoroutine(currentCoroutine);
            friendRequest.SetActive(false);
        }
        remaingRequests.RemoveAt(0);
        if(remaingRequests.Count > 0)
        {
            currentCoroutine = ShowFriendRequest(1);
            StartCoroutine(currentCoroutine);
        }
    }
    IEnumerator currentCoroutine;
    public IEnumerator ShowFriendRequest(int version)
    {
        friendRequest.SetActive(true);
        while(remaingRequests.Count > 0)
        {
            yield return new WaitForSeconds(5);
            GetComponent<PhotonView>().RPC("SendRequestAnswerBack", remaingRequests[0], false, PhotonNetwork.player, true);
            remaingRequests.RemoveAt(0);
        }
        friendRequest.SetActive(false);
    }
}
