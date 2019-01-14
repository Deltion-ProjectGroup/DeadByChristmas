using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaemManager : MonoBehaviour {
    [Header("RoleDistribution")]
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
    [Header("Initializers")]
    public int partAmount;
    [Header("GameData")]
    public GameObject[] allElfs;
    public GameObject santa;
    public List<GameObject> inGamePlayers = new List<GameObject>();
	// Use this for initialization
    //Spawns weapons and showsRoles
	void Start () {
        GetComponent<GameUIManager>().CreateElfStatuses();
        if (PhotonNetwork.isMasterClient)
        {
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
        string santaUserName = allPlayers[Random.Range(0, allPlayers.Length)].NickName;
        isSanta[santaUserName] = true;
        GetComponent<PhotonView>().RPC("RemoveSanta", PhotonTargets.All, santaUserName);
    }
    //shows the roles
    [PunRPC]
    public IEnumerator ShowRoles()
    {
        yield return new WaitForSeconds(1);
        if (PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("RandomizePlayers", PhotonTargets.MasterClient);
        }
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
            StartCoroutine(SpawnPlayer());
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
    [PunRPC]
    public void GetElfs()
    {
        print("FINDING ELFS");
        allElfs = GameObject.FindGameObjectsWithTag("Elf");
        if(allElfs.Length == 0)
        {
            print("U WON");
        }
    }
    [PunRPC]
    public void GetSanta()
    {
        print("FINDING SANTA");
        santa = GameObject.FindGameObjectWithTag("Santa");
    }
    [PunRPC]
    public void GetInGamePlayers()
    {
        print("FINDING PLAYERS");
        inGamePlayers = new List<GameObject>();
        GetElfs();
        GetSanta();
        inGamePlayers.Add(santa);
        for(int elf = 0; elf < allElfs.Length; elf++)
        {
            inGamePlayers.Add(allElfs[elf]);
        }
    }
    //spawns the player
    public IEnumerator SpawnPlayer()
    {
        if((bool)isSanta[PhotonNetwork.player.NickName] == true)
        {
            localPlayer = PhotonNetwork.Instantiate(santaPrefab, santaSpawns[Random.Range(0, santaSpawns.Length)].position, Quaternion.identity, 0);
        }
        else
        {
            localPlayer = PhotonNetwork.Instantiate(elfPrefab, elfSpawns[Random.Range(0, elfSpawns.Length)].position, Quaternion.identity, 0);
        }
        yield return null;
        GetInGamePlayers();
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
    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (isSanta.ContainsKey(otherPlayer.NickName))
        {
            if ((bool)isSanta[otherPlayer.NickName])
            {
                print("SANTA LEFT");
            }
            else
            {
                GetComponent<GameUIManager>().ChangeStatusIcon(otherPlayer.NickName, 4);
            }
        }
        else
        {
            GetComponent<GameUIManager>().ChangeStatusIcon(otherPlayer.NickName, 4);
        }
    }
}
