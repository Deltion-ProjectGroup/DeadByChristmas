using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerator : InteractableObject {
    public GameObject containedElf;
    public Transform elfPlacePosition;
    public float releaseDelay;
    // Use this for initialization
    void Start () {
		
	}
    private void Update()
    {
        if(Input.GetButtonUp("Fire1") && interactingPlayer != null)
        {
            if(interactingPlayer == GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().localPlayer)
            {
                GetComponent<PhotonView>().RPC("Cancel", PhotonTargets.All);
            }
        }
    }
    // Update is called once per frame
    public override void Interact(int interactorID)
    {
        base.Interact(interactorID);
        if(interactingPlayer.tag == "Santa")
        {
            if (containedElf == null)
            {
                if(interactingPlayer.GetComponent<SantaController>().carryingElf != null)
                {
                    GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>().RPC("ChangeStatusIcon", PhotonTargets.All, PhotonNetwork.player.NickName, 2);
                    GetComponent<PhotonView>().RPC("PlaceElf", PhotonTargets.All, interactingPlayer.GetComponent<PhotonView>().ownerId);
                }
            }
        }
        else
        {
            if (containedElf != null)
            {

                GetComponent<PhotonView>().RPC("ReleaseData", PhotonTargets.All, interactorID, interactingPlayer.GetComponent<PhotonView>().ownerId);
                StartCoroutine(ReleaseElf());
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
                containedElf.transform.position = elfPlacePosition.position;
                break;
            }
        }
    }
    [PunRPC]
    public void ReleaseData(int ownerID)
    {
        foreach (GameObject elf in GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().allElfs)
        {
            if (elf.GetComponent<PhotonView>().ownerId == ownerID)
            {
                interactingPlayer = elf;
                break;
            }
        }
    }
    [PunRPC]
    public void FinishRelease()
    {
        interactingPlayer = null;
        interactingPlayer.GetComponent<ElfController>().canInteract = true;
        containedElf.transform.SetParent(null);
        containedElf.GetComponent<ElfController>().currentState = ElfController.StruggleState.normal;
        containedElf = null;
    }
    public IEnumerator ReleaseElf()
    {
        interactingPlayer.GetComponent<ElfController>().canInteract = false;
        float process = 0;

        while(process < 1)
        {
            yield return new WaitForSeconds(releaseDelay);
            process += 0.01f;
        }
        interactingPlayer.GetComponent<ElfController>().canInteract = false;
        GetComponent<PhotonView>().RPC("FinishRelease", PhotonTargets.All);
    }
    public void OnTriggerEnter(Collider hit)
    {
        if(hit.tag == "Elf" && hit.gameObject == containedElf)
        {
            if (hit.gameObject == GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().localPlayer)
            {
                hit.gameObject.GetComponent<ElfController>().ActualDeath();
                GetComponent<PhotonView>().RPC("KillElf", PhotonTargets.All);
                GetComponent<PhotonView>().RPC("Cancel", PhotonTargets.All);
            }
        }
    }
    [PunRPC]
    void Cancel()
    {
        if(interactingPlayer != null)
        {
            StopAllCoroutines();
            interactingPlayer.GetComponent<ElfController>().canInteract = true;
            interactingPlayer = null;
        }
    }
    public override void CancelInteract()
    {
        Cancel();
    }
}
