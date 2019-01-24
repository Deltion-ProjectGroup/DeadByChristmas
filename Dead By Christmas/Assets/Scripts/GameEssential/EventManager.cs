using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    public void Awake()
    {
        PhotonNetwork.OnEventCall += OnEvent;
    }
    public void OnEvent(byte eventCode, object content, int senderId)
    {
        object[] data = (object[])content;
        GaemManager gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>();
        switch (eventCode)
        {
            case 0://AbilityCast
                List<GameObject> targets = new List<GameObject>();
                for (int i = 1; i < data.Length; i++)
                {
                    for (int possibilities = 0; possibilities < gameManager.inGamePlayers.Count; possibilities++)
                    {
                        if (gameManager.inGamePlayers[possibilities].GetComponent<PhotonView>().ownerId == (int)data[i])
                        {
                            targets.Add(gameManager.inGamePlayers[possibilities]);
                            break;
                        }
                    }
                }
                gameManager.santa.GetComponent<SantaController>().abilities[(int)data[0]].AddEffect(targets.ToArray());
                break;
        }
    }
}
