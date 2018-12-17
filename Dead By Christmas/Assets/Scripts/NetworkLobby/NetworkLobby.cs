using System.Collections;
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
    public Slider playerSlider;
    [Header("Extra Infomration")]
    public GameObject mainMenu, nameSelectUI;
    public GameObject bannedUI;
    public Text reasonText;
    public string preGameScene;

    //Connects with Photon.
    public void Start()
    {
        if (!SaveDatabase.data.userData.banned)
        {
            PhotonNetwork.player.NickName = SaveDatabase.data.userData.username;
            if(PhotonNetwork.player.NickName == null)
            {
                StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
                nameSelectUI.SetActive(true);
            }
            else
            {
                PhotonNetwork.automaticallySyncScene = true;
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
        if (nameInput.text != "" && roomInput.text != "")
        {
            StartCoroutine(CreateARoom());
        }
    }

    //Joins an certain room.
    public void JoinCertainRoom(string roomName)
    {
        if (nameInput.text != "")
        {
            StartCoroutine(JoinRoom(roomName));
        }
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
        PhotonNetwork.player.NickName = nameInput.text;
        SaveDatabase.data.userData.username = PhotonNetwork.player.NickName;
        SaveDatabase.data.Save();
        PhotonNetwork.ConnectUsingSettings(version);
        mainMenu.SetActive(true);
        nameSelectUI.SetActive(false);
        StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
    }
}
