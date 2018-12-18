using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminTool : MonoBehaviour {
    public Dropdown userSelectForBan;
    public InputField banReason;
    PhotonPlayer banPlayer;

    public void RefreshPlayerList()
    {
        userSelectForBan.ClearOptions();
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            List<string> names = new List<string>();
            foreach (string admin in SaveDatabase.data.admins)
            {
                if(player.NickName != admin)
                {
                    names.Add(player.NickName);
                }
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
                print(player.NickName);
                SaveDatabase.data.Ban(banReason.text, player);
                break;
            }
        }
    }
}
