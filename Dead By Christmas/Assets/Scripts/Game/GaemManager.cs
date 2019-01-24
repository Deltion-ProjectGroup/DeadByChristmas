using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GaemManager : MonoBehaviour {
    [Header("RoleDistribution")]
    object[] emptyData;
    public AudioSource[] audioSources;
    public AudioClip[] audioClips;
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
    public bool ingame;
    public bool finished;
	// Use this for initialization
    //Spawns weapons and showsRoles
	void Start () {
        GetComponent<GameUIManager>().CreateElfStatuses();
        StartCoroutine(StartGame());
	}
    public IEnumerator StartGame()
    {
        if (PhotonNetwork.isMasterClient)
        {
            yield return new WaitForSeconds(2);
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
    public IEnumerator EndGame(bool santaWon)
    {
        finished = true;
        if (santaWon)
        {
            if ((bool)isSanta[PhotonNetwork.player.NickName])
            {
                audioSources[0].clip = audioClips[0];
            }
            else
            {
                audioSources[0].clip = audioClips[1];
            }
        }
        else
        {
            if ((bool)isSanta[PhotonNetwork.player.NickName])
            {
                audioSources[0].clip = audioClips[1];
            }
            else
            {
                audioSources[0].clip = audioClips[0];
            }
        }
        audioSources[0].Play();
        yield return new WaitForSeconds(audioSources[0].clip.length + 1);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenuScene");
    }
    [PunRPC]
    public IEnumerator GetElfs()
    {
        yield return null;
        print("FINDING ELFS");
        allElfs = GameObject.FindGameObjectsWithTag("Elf");
        if(allElfs.Length == 0 && !finished)
        {
            StartCoroutine(EndGame(true));
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
        yield return new WaitForSeconds(1.5f);
        if (PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("GetInGamePlayers", PhotonTargets.All);
        }
        yield return new WaitForSeconds(1);
        if (santa.GetComponent<PhotonView>().isMine)
        {
            List<int> send = new List<int>();
            foreach(int id in SaveDatabase.data.userData.equippedAbilities)
            {
                send.Add(id);
            }
            GetComponent<PhotonView>().RPC("UpdateSantaHealth", PhotonTargets.All);
            GetComponent<PhotonView>().RPC("LoadSantaAbilities", PhotonTargets.All, send.ToArray());
        }
        StartCoroutine(TransitionScreen.transitionScreen.FadeOut());
        ingame = true;
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
        if (!finished)
        {
            if (isSanta.ContainsKey(otherPlayer.NickName))
            {
                if ((bool)isSanta[otherPlayer.NickName])
                {
                    santa.GetComponent<SantaController>().health = 0;
                    GetComponent<GameUIManager>().UpdateSantaHealth();
                    StartCoroutine(EndGame(false));
                }
                else
                {
                    GetComponent<GameUIManager>().ChangeStatusIcon(otherPlayer.NickName, 4);
                    StartCoroutine(GetElfs());
                }
            }
            else
            {
                GetComponent<GameUIManager>().ChangeStatusIcon(otherPlayer.NickName, 4);
                StartCoroutine(GetElfs());
            }
        }
    }
    [PunRPC]
    public void LoadSantaAbilities(int[] abilityIDs)
    {
        SaveDatabase database = GameObject.FindGameObjectWithTag("Database").GetComponent<SaveDatabase>();
        for(int id = 0; id < abilityIDs.Length; id++)
        {
            for(int ability = 0; ability < database.abilities.Length; ability++)
            {
                if(abilityIDs[id] == database.abilities[ability].abilityID)
                {
                    santa.GetComponent<SantaController>().abilities[id] = database.abilities[ability];
                    break;
                }
            }
        }
    }
}
