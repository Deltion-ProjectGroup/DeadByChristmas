using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {
    public Sprite[] elfStatusIcons;
    public Hashtable icons;
    public GameObject elfStatusHolder;
    public Transform elfStatuses;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public enum ElfStatus
    {
        Alive, Knocked, Incinerator, Dead, Disconnected
    }
    [PunRPC]
    public void ChangeStatusIcon(string username, ElfStatus newStatus)
    {
        GameObject target = (GameObject)icons[username];
        switch (newStatus)
        {
            case ElfStatus.Alive:
                target.GetComponent<Image>().sprite = elfStatusIcons[0];
                break;
            case ElfStatus.Knocked:
                target.GetComponent<Image>().sprite = elfStatusIcons[1];
                break;
            case ElfStatus.Incinerator:
                target.GetComponent<Image>().sprite = elfStatusIcons[2];
                break;
            case ElfStatus.Dead:
                target.GetComponent<Image>().sprite = elfStatusIcons[3];
                break;
            case ElfStatus.Disconnected:
                target.GetComponent<Image>().sprite = elfStatusIcons[4];
                break;
        }
    }
    public void CreateElfStatuses()
    {
        GaemManager gameManager = GetComponent<GaemManager>();
        for(int key = 0; key < PhotonNetwork.playerList.Length; key++)
        {
            if (gameManager.isSanta.ContainsKey(PhotonNetwork.playerList[key].NickName))
            {
                if((bool)gameManager.isSanta[PhotonNetwork.playerList[key].NickName] == false)
                {
                    GameObject spawnedIcon = Instantiate(elfStatusHolder, elfStatuses);
                    spawnedIcon.GetComponent<Text>().text = PhotonNetwork.playerList[key].NickName;
                    icons.Add(PhotonNetwork.playerList[key].NickName, spawnedIcon);
                }
            }
        }
    }
}
