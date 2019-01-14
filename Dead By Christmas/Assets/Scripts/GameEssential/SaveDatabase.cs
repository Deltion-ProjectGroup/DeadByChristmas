using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class SaveDatabase : MonoBehaviour {
    public static SaveDatabase data;
    public string[] admins;
    bool panelToggled;
    [SerializeField] GameObject adminPanel;
    public PlayerData userData;
    private void Awake()
    {
        data = this;
        Load();
    }
    public void Update()
    {
        if (Input.GetButtonDown("ToggleAdminPanel"))
        {
            ToggleAdmin();
        }
    }
    public void Save()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        FileStream stream = new FileStream(Application.persistentDataPath + "/SaveData.xml", FileMode.Create);
        serializer.Serialize(stream, userData);
        stream.Close();
    }
    public void ToggleAdmin()
    {
        foreach(string admin in admins)
        {
            if(admin == PhotonNetwork.player.NickName)
            {
                if (panelToggled)
                {
                    panelToggled = false;
                    adminPanel.SetActive(false);
                    if(GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>() != null)
                    {
                        Cursor.lockState = CursorLockMode.Confined;
                    }
                }
                else
                {
                    panelToggled = true;
                    adminPanel.SetActive(true);
                    adminPanel.GetComponent<AdminTool>().RefreshPlayerList();
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }
    public void Load()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        if (File.Exists(Application.persistentDataPath + "/SaveData.xml"))
        {
            print("Yes");
            FileStream stream = new FileStream(Application.persistentDataPath + "/SaveData.xml", FileMode.Open);
            userData = (PlayerData)serializer.Deserialize(stream);
            stream.Close();
        }
    }
    [System.Serializable]
    public struct PlayerData
    {
        public string username;
        public bool banned;
        public string bannedReason;
        public List<string> friends;
    }
    public void OnApplicationQuit()
    {
        Save();
    }
    public void Ban(string reason, PhotonPlayer banPlayer)
    {
        GetComponent<PhotonView>().RPC("SendBan", banPlayer, reason);
    }
    [PunRPC]
    public void SendBan(string reason)
    {
        userData.banned = true;
        userData.bannedReason = reason;
        Save();
        PhotonNetwork.LeaveRoom();
    }
}
