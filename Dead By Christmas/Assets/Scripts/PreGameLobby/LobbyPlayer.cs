using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour {

    public Text userName;
    public Text readyText;
    public bool isReady;



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(userName.text);
            stream.SendNext(readyText.text);
            stream.SendNext(isReady);
        }
        else
        {
            userName.text = (string)stream.ReceiveNext();
            readyText.text = (string)stream.ReceiveNext();
            isReady = (bool)stream.ReceiveNext();
        }
    }
}
