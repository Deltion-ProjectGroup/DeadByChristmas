using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {
    public Sprite[] elfStatusIcons;
    public List<GameObject> icons = new List<GameObject>();
    public GameObject elfStatusHolder;
    public Transform elfStatuses;
    public GameObject santaStatus;
    // Use this for initialization

    private void Awake()
    {
        //Instantiate(elfStatusHolder, elfStatuses);
    }
    public enum ElfStatus
    {
        Alive, Knocked, Incinerator, Dead, Disconnected
    }
    [PunRPC]
    public void UpdateSantaHealth()
    {
        santaStatus.GetComponent<Animation>().Play();
        santaStatus.GetComponentInChildren<Text>().text = GetComponent<GaemManager>().santa.GetComponent<Player>().health.ToString();
    }
    [PunRPC]
    public void ChangeStatusIcon(string username, int newStatus)
    {
        GameObject target = null;
        foreach (GameObject icon in icons)
        {
            if (icon.GetComponentInChildren<Text>().text == username)
            {
                target = icon;
                break;
            }
        }
        if(target != null)
        {
            switch (newStatus)
            {
                case 0:
                    target.GetComponentInChildren<Image>().sprite = elfStatusIcons[0];
                    break;
                case 1:
                    target.GetComponentInChildren<Image>().sprite = elfStatusIcons[1];
                    break;
                case 2:
                    target.GetComponentInChildren<Image>().sprite = elfStatusIcons[2];
                    break;
                case 3:
                    target.GetComponentInChildren<Image>().sprite = elfStatusIcons[3];
                    break;
                case 4:
                    target.GetComponentInChildren<Image>().sprite = elfStatusIcons[4];
                    break;
            }
            target.GetComponent<Animation>().Play();
        }
    }

    public void CreateElfStatuses()
    {
        GaemManager gameManager = GetComponent<GaemManager>();
        for(int key = 0; key < PhotonNetwork.playerList.Length; key++)
        {
            GameObject elfStatus = Instantiate(elfStatusHolder, elfStatuses);
            elfStatus.GetComponentInChildren<Text>().text = PhotonNetwork.playerList[key].NickName;
            icons.Add(elfStatus);
        }
    }
    [PunRPC]
    public void RemoveSanta(string username)
    {
        foreach(GameObject icon in icons)
        {
            if(icon.GetComponentInChildren<Text>().text == username)
            {
                icons.Remove(icon);
                Destroy(icon);
                break;
            }
        }
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
