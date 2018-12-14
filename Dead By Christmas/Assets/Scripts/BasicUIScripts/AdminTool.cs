using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminTool : MonoBehaviour {
    public Dropdown userSelectForBan;
    public InputField banReason;
    PhotonPlayer banPlayer;

    public void Start()
    {
        RefreshPlayerList();
    }
    public void OnLeftRoom()
    {
        RefreshPlayerList();
    }
    public void OnJoinedRoom()
    {
        RefreshPlayerList();
    }
    public void RefreshPlayerList()
    {
        userSelectForBan.ClearOptions();
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            List<string> names = new List<string>();
            if(player != PhotonNetwork.player)
            {
                names.Add(player.NickName);
            }
            userSelectForBan.AddOptions(names);
        }
    }
    public void Ban()
    {
        PhotonPlayer[] players;
        players = PhotonNetwork.playerList;
        foreach(PhotonPlayer player in players)
        {
            if(player.NickName == userSelectForBan.options[userSelectForBan.value].text)
            {
                banPlayer = player;
                break;
            }
        }
        GetComponent<PhotonView>().RPC("SendBan", banPlayer, banReason.text);
    }
    [PunRPC]
    public void SendBan(string reason)
    {
        SaveDatabase.data.userData.banned = true;
        SaveDatabase.data.userData.bannedReason = reason;
        PhotonNetwork.LeaveRoom();
    }
}
