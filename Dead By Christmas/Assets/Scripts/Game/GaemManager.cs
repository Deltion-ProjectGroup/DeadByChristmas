﻿using System.Collections;
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
	// Use this for initialization
	void Start () {
        if (PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("ShowRoles", PhotonTargets.All);
            GetComponent<PhotonView>().RPC("SpawnWeaponParts", PhotonTargets.MasterClient);
        }
	}

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

    [PunRPC]
    public IEnumerator ShowRoles()
    {
        yield return new WaitForSeconds(1);
        RandomizePlayers();
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
            TransitionScreen.transitionScreen.FadeOut();
        }
    }
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
    public void SpawnPlayer()
    {
        if((bool)isSanta[PhotonNetwork.player.NickName] == true)
        {
            localPlayer = PhotonNetwork.Instantiate(santaPrefab, santaSpawns[Random.Range(0, santaSpawns.Length)].position, Quaternion.identity, 0);
        }
        else
        {
            localPlayer = PhotonNetwork.Instantiate(elfPrefab, elfSpawns[Random.Range(0, elfSpawns.Length)].position, Quaternion.identity, 0);
        }
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
