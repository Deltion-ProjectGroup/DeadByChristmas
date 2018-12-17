using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class SaveDatabase : MonoBehaviour {
    public static SaveDatabase data;
    [SerializeField] string[] admins; 
    [SerializeField] GameObject adminPanel;
    public PlayerData userData;
    private void Awake()
    {
        data = this;
        Load();
        ShowAdminTool();
    }
    public void Save()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        FileStream stream = new FileStream(Application.dataPath + "/Scripts/Saves/SaveData.xml", FileMode.Create);
        serializer.Serialize(stream, userData);
        stream.Close();
    }
    public void Load()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        FileStream stream = new FileStream(Application.dataPath + "/Scripts/Saves/SaveData.xml", FileMode.Open);
        userData = (PlayerData)serializer.Deserialize(stream);
        stream.Close();
    }
    [System.Serializable]
    public struct PlayerData
    {
        public string username;
        public bool banned;
        public string bannedReason;
        public List<string> friends;
    }
    void ShowAdminTool()
    {
        foreach(string admin in admins)
        {
            if (userData.username == admin)
            {
                adminPanel.SetActive(true);
            }
        }
    }
    public void OnApplicationQuit()
    {
        Save();
    }
}
