﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkLobby : Photon.MonoBehaviour {

    [Header("Build Version")]
    public string version;
    [Header("Room Information")]
    public Transform roomHolder;
    public GameObject roomButton;
    public InputField nameInput, roomInput;
    public Text welcomeText;
    public string welcomeMessage;
    public Slider playerSlider;
    [Header("Extra Infomration")]
    public GameObject mainMenu, nameSelectUI;
    public GameObject bannedUI;
    public Text reasonText;
    public string preGameScene;
    public Text pathText;
    [Header("Friends")]
    public GameObject friendPanel;
    public Transform friendHolder;
    public string ingameStatus, lobbyStatus;


    public void Update()
    {
        pathText.text = Application.persistentDataPath;
        if (PhotonNetwork.connectedAndReady)
        {
            if (SaveDatabase.data.userData.friends.Count > 0)
            {
                PhotonNetwork.FindFriends(SaveDatabase.data.userData.friends.ToArray());
            }
        }
    }
    //Connects with Photon.
    public void Start()
    {
        if (!SaveDatabase.data.userData.banned)
        {
            PhotonNetwork.playerName = SaveDatabase.data.userData.username;
            PhotonNetwork.automaticallySyncScene = true;
            if (PhotonNetwork.player.NickName == null || PhotonNetwork.player.NickName == "")
            {
                StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
                nameSelectUI.SetActive(true);
            }
            else
            {
                welcomeText.text = welcomeMessage + PhotonNetwork.player.NickName;
                if (PhotonNetwork.connected)
                {
                    StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
                    mainMenu.SetActive(true);
                    nameInput.text = PhotonNetwork.player.NickName;
                }
                else
                {
                    PhotonNetwork.ConnectUsingSettings(version);
                }
            }
        }
        else
        {
            bannedUI.SetActive(true);
            reasonText.text = SaveDatabase.data.userData.bannedReason;
            StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
        }
    }

    //Makes the ui visible when you're connected.
    public void OnConnectedToPhoton()
    {
        StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
        mainMenu.SetActive(true);
    }

    public void OnReceivedRoomListUpdate()
    {
        //Clears the roomlayout.
        foreach (Transform child in roomHolder)
        {
            Destroy(child.gameObject);
        }
        //Sets the new rooms
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach(RoomInfo room in rooms)
        {
            GameObject panel = Instantiate(roomButton, roomHolder);
            panel.GetComponent<RoomButton>().SetInfo(room, this);
        }
    }

    //Creates a new room if the room doest excist, or it joins an already excisting one.
    public void CreateRoom()
    {
        if (roomInput.text != "")
        {
            StartCoroutine(CreateARoom());
        }
    }

    //Joins an certain room.
    public void JoinCertainRoom(string roomName)
    {
        StartCoroutine(JoinRoom(roomName));
    }
    IEnumerator JoinRoom(string name)
    {
        StartCoroutine(TransitionScreen.transitionScreen.FadeIn());
        yield return new WaitForSeconds(TransitionScreen.transitionScreen.GetComponent<TransitionScreen>().screen.GetComponent<Animation>().GetClip("TransitionFadeIn").length);
        PhotonNetwork.JoinRoom(name);
    }
    IEnumerator CreateARoom()
    {
        StartCoroutine(TransitionScreen.transitionScreen.FadeIn());
        yield return new WaitForSeconds(TransitionScreen.transitionScreen.GetComponent<TransitionScreen>().screen.GetComponent<Animation>().GetClip("TransitionFadeIn").length);
        RoomOptions ro = new RoomOptions() { IsVisible = true, MaxPlayers = (byte)playerSlider.value };
        PhotonNetwork.JoinOrCreateRoom(roomInput.text, ro, TypedLobby.Default);
        PhotonNetwork.LoadLevel(preGameScene);
    }
    public void SetName()
    {
        PhotonNetwork.playerName = nameInput.text;
        SaveDatabase.data.userData.username = PhotonNetwork.player.NickName;
        welcomeText.text = welcomeMessage + PhotonNetwork.player.NickName;
        SaveDatabase.data.Save();
        PhotonNetwork.ConnectUsingSettings(version);
        mainMenu.SetActive(true);
        nameSelectUI.SetActive(false);
        StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
    }
    public void OnUpdatedFriendList()
    {
        print(PhotonNetwork.Friends.Count);
        print(PhotonNetwork.Friends[0].IsOnline);
        foreach(Transform t in friendHolder)
        {
            Destroy(t.gameObject);
        }
        foreach(FriendInfo friend in PhotonNetwork.Friends)
        {
            if (friend.IsOnline)
            {
                GameObject data = Instantiate(friendPanel, friendHolder);
                data.GetComponentInChildren<Text>().text = friend.Name;
                if (friend.IsInRoom)
                {
                    data.GetComponent<FriendButton>().roomName = friend.Room;
                    data.GetComponent<FriendButton>().gameStatus.text = ingameStatus;
                    data.GetComponent<FriendButton>().inGame = true;
                }
                else
                {
                    data.GetComponent<FriendButton>().gameStatus.text = lobbyStatus;
                }
            }
        }
    }
    public void OnPhotonJoinRoomFailed()
    {
        StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
    }
}
