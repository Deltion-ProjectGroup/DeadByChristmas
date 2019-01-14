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
                target.GetComponentInChildren<Image>().sprite = elfStatusIcons[0];
                break;
            case ElfStatus.Knocked:
                target.GetComponentInChildren<Image>().sprite = elfStatusIcons[1];
                break;
            case ElfStatus.Incinerator:
                target.GetComponentInChildren<Image>().sprite = elfStatusIcons[2];
                break;
            case ElfStatus.Dead:
                target.GetComponentInChildren<Image>().sprite = elfStatusIcons[3];
                break;
            case ElfStatus.Disconnected:
                target.GetComponentInChildren<Image>().sprite = elfStatusIcons[4];
                break;
        }
        target.GetComponent<Animation>().Play();
    }

    public void CreateElfStatuses()
    {
        GaemManager gameManager = GetComponent<GaemManager>();
        for(int key = 0; key < PhotonNetwork.playerList.Length; key++)
        {
            GameObject elfStatus = Instantiate(elfStatusHolder, elfStatuses);
            elfStatus.GetComponentInChildren<Text>().text = PhotonNetwork.playerList[key].NickName;
            icons.Add(PhotonNetwork.playerList[key].NickName, elfStatus);
        }
    }
    [PunRPC]
    public void RemoveSanta(string username)
    {
        Destroy((GameObject)icons[username]);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {

        }
        else
        {

        }
    }
}
