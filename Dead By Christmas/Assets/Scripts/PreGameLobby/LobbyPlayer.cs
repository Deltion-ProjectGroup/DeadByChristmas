using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour {

    public Text userName;
    public Text readyText;
    public GameObject kickButton;
    public GameObject friendButton;
    public bool isReady;
    public bool gameLoaded;



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(userName.text);
            stream.SendNext(readyText.text);
            stream.SendNext(isReady);
            stream.SendNext(gameLoaded);
        }
        else
        {
            userName.text = (string)stream.ReceiveNext();
            readyText.text = (string)stream.ReceiveNext();
            isReady = (bool)stream.ReceiveNext();
            gameLoaded = (bool)stream.ReceiveNext();
        }
    }
    public void KickThis()
    {
        PhotonPlayer thisPlayer = new PhotonPlayer(false, GetComponent<PhotonView>().ownerId, "KICKEDPLAYER");
        GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>().RPC("KickPlayer", PhotonTargets.MasterClient, thisPlayer);//PHOTONPLAYER
    }
    public void SendFriendRequest()
    {
        PhotonPlayer target = GetComponent<PhotonView>().owner;
        GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>().RPC("ReceiveFriendRequest", target, PhotonNetwork.player);
        friendButton.SetActive(false);
    }
}
