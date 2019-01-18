using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerator : InteractableObject {
    public bool containsElf;
    public GameObject containedElf;
    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    public override void Interact()
    {
        base.Interact();
        if(interactingPlayer.tag == "Santa")
        {
            if (containedElf == null)
            {
                if(interactingPlayer.GetComponent<SantaController>().carryingElf != null)
                {
                    GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>().RPC("ChangeStatusIcon", PhotonTargets.All, PhotonNetwork.player.NickName, 2);
                    GetComponent<PhotonView>().RPC("PlaceElf", PhotonTargets.All);
                }
            }
        }
        else
        {
            if (containedElf != null)
            {
                GetComponent<PhotonView>().RPC("ReleaseElf", PhotonTargets.All);
            }
        }
    }
    [PunRPC]
    public void PlaceElf(int ownerID)
    {
        foreach(GameObject elf in GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().allElfs)
        {
            if(elf.GetComponent<PhotonView>().ownerId == ownerID)
            {
                containedElf = elf;
                containedElf.GetComponent<ElfController>().currentState = ElfController.StruggleState.struggling;
                break;
            }
        }
    }
    [PunRPC]
    public void ReleaseElf()
    {
        containedElf.transform.SetParent(null);
        containedElf.GetComponent<ElfController>().currentState = ElfController.StruggleState.normal;
        containedElf = null;
    }
    public void KillElf(GameObject elf)
    {
        elf.GetComponent<ElfController>().ActualDeath();
    }
    public void OnTriggerEnter(Collider hit)
    {
        if(hit.tag == "Elf" && hit.gameObject == containedElf)
        {
            KillElf(hit.gameObject);
        }
    }
}
