using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaemManager : MonoBehaviour {
    public ExitGames.Client.Photon.Hashtable isSanta = new ExitGames.Client.Photon.Hashtable();
    public PhotonPlayer[] allPlayers;
    public Text roleText;
    [TextArea]
    public string santaText, elfText;
	// Use this for initialization
	void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(RandomizePlayers());
            GetComponent<PhotonView>().RPC("ShowRoles", PhotonTargets.All);
        }
	}
	
	// Update is called once per frame
	void Update () {

	}
    public IEnumerator RandomizePlayers()
    {
        yield return new WaitForSeconds(1);
        allPlayers = PhotonNetwork.playerList;
        foreach(PhotonPlayer player in allPlayers)
        {
            isSanta.Add(player.NickName, false);
        }
        isSanta[allPlayers[Random.Range(0, allPlayers.Length)].NickName] = true;
    }
    [PunRPC]
    public IEnumerator ShowRoles()
    {
        yield return new WaitForSeconds(2);
        if (isSanta.ContainsKey(PhotonNetwork.player.NickName))
        {
            if ((bool)isSanta[PhotonNetwork.player.NickName] == true)
            {
                roleText.color = Color.red;
                roleText.text = santaText;
            }
            else
            {
                roleText.color = Color.green;
                roleText.text = elfText;
            }
        }
        else
        {
            print("DOESNT CONTAIN KEY");
        }
        yield return new WaitForSeconds(2);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(isSanta);
        }
        else
        {
            isSanta = (ExitGames.Client.Photon.Hashtable)stream.ReceiveNext();
        }
    }
}
