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
    public GameObject mainMenu;
    public string preGameScene;

    //Connects with Photon.
    public void Start()
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
            PhotonNetwork.player.NickName = nameInput.text;
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
        PhotonNetwork.player.NickName = nameInput.text;
        RoomOptions ro = new RoomOptions() { IsVisible = true, MaxPlayers = (byte)playerSlider.value };
        PhotonNetwork.JoinOrCreateRoom(roomInput.text, ro, TypedLobby.Default);
        PhotonNetwork.LoadLevel(preGameScene);
    }
}
