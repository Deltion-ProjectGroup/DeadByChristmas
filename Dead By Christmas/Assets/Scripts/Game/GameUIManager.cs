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
    public GameObject indicatorText;
    // Use this for initialization

    private void Awake()
    {
        //Instantiate(elfStatusHolder, elfStatuses);
    }
    public enum ElfStatus
    {
        Alive, Knocked, Incinerator, Dead, Disconnected
    }
    public void IndicatorAppear(string inputButton, string effect)
    {
        indicatorText.SetActive(true);
        indicatorText.GetComponent<Text>().text = "Press " + "[" + inputButton + "] " + "to " + effect;
    }
    public void IndicatorDissapear()
    {
        indicatorText.SetActive(false);
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
            Image statusIcon = target.GetComponentInChildren<Image>();
            switch (newStatus)
            {
                case 0:
                    statusIcon.sprite = elfStatusIcons[0];
                    break;
                case 1:
                    statusIcon.sprite = elfStatusIcons[1];
                    break;
                case 2:
                    statusIcon.sprite = elfStatusIcons[2];
                    break;
                case 3:
                    statusIcon.sprite = elfStatusIcons[3];
                    break;
                case 4:
                    statusIcon.sprite = elfStatusIcons[4];
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
