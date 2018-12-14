using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaemManager : MonoBehaviour {
    object[] emptyData;
    public ExitGames.Client.Photon.Hashtable isSanta;
    public PhotonPlayer[] allPlayers;
    public GameObject roleText;
    public GameObject mainCam;
    public GameObject localPlayer;
    [TextArea]
    public string santaText, elfText;
    public string santaPrefab, elfPrefab;
    public Transform[] santaSpawns, elfSpawns, weaponPartSpots;
    [Range(3, 15)]
    public int partAmount;
    public Text timerText;
    int remainingSeconds;
    [SerializeField] int roundTime;
	// Use this for initialization
    //Spawns weapons and showsRoles
	void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            remainingSeconds = roundTime;
            timerText.text = CalcRemainingTime();
            GetComponent<PhotonView>().RPC("ShowRoles", PhotonTargets.All);
            GetComponent<PhotonView>().RPC("SpawnWeaponParts", PhotonTargets.MasterClient);
        }
	}
    //Randomizes the roles
    [PunRPC]
    public void RandomizePlayers()
    {
        isSanta = new ExitGames.Client.Photon.Hashtable();
        allPlayers = PhotonNetwork.playerList;
        foreach(PhotonPlayer player in allPlayers)
        {
            isSanta.Add(player.NickName, false);
        }
        isSanta[allPlayers[Random.Range(0, allPlayers.Length)].NickName] = true;
    }
    //shows the roles
    [PunRPC]
    public IEnumerator ShowRoles()
    {
        yield return new WaitForSeconds(1);
        GetComponent<PhotonView>().RPC("RandomizePlayers", PhotonTargets.MasterClient);
        yield return new WaitForSeconds(2);
        if (isSanta.ContainsKey(PhotonNetwork.player.NickName))
        {
            if ((bool)isSanta[PhotonNetwork.player.NickName] == true)
            {
                roleText.GetComponent<Text>().color = Color.red;
                roleText.GetComponent<Text>().text = santaText;
            }
            else
            {
                roleText.GetComponent<Text>().color = Color.green;
                roleText.GetComponent<Text>().text = elfText;
            }
            roleText.SetActive(true);
            roleText.GetComponent<Animation>().Play();
            yield return new WaitForSeconds(roleText.GetComponent<Animation>().clip.length);
            roleText.SetActive(false);
            SpawnPlayer();
            StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
        }
    }
    //spawns the weapon parts
    [PunRPC]
    public void SpawnWeaponParts()
    {
        List<Transform> availableSpawnSpots = new List<Transform>();
        foreach(Transform trans in weaponPartSpots)
        {
            availableSpawnSpots.Add(trans);
        }
        for(int spawnedAmt = 0; spawnedAmt < partAmount; spawnedAmt++)
        {
            int randomizer = Random.Range(0, availableSpawnSpots.Count);
            PhotonNetwork.InstantiateSceneObject("WeaponPart", availableSpawnSpots[randomizer].position, Quaternion.identity, 0, emptyData);
            availableSpawnSpots.RemoveAt(randomizer);
        }

    }
    IEnumerator Timer()
    {
        while(remainingSeconds > 0)
        {
            yield return new WaitForSeconds(1);
            remainingSeconds--;
            CalcRemainingTime();
        }
        timerText.text = "GAME DONE";
    }
    //spawns the player
    public void SpawnPlayer()
    {
        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(Timer());
        }
        if((bool)isSanta[PhotonNetwork.player.NickName] == true)
        {
            localPlayer = PhotonNetwork.Instantiate(santaPrefab, santaSpawns[Random.Range(0, santaSpawns.Length)].position, Quaternion.identity, 0);
        }
        else
        {
            localPlayer = PhotonNetwork.Instantiate(elfPrefab, elfSpawns[Random.Range(0, elfSpawns.Length)].position, Quaternion.identity, 0);
        }
    }
    public void OnMasterClientSwitched()
    {
        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(Timer());
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(isSanta);
            stream.SendNext(remainingSeconds);
            stream.SendNext(timerText.text);
        }
        else
        {
            isSanta = (ExitGames.Client.Photon.Hashtable)stream.ReceiveNext();
            remainingSeconds = (int)stream.ReceiveNext();
            timerText.text = (string)stream.ReceiveNext();
        }
    }
    string CalcRemainingTime()
    {
        int minutes = Mathf.FloorToInt(remainingSeconds / 60);
        float seconds = ((remainingSeconds / 60) - minutes) * 60;
        return minutes.ToString() + ":" + seconds.ToString();
    }
}
