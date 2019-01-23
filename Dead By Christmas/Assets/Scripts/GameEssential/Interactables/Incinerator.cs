using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerator : InteractableObject {
    public GameObject containedElf;
    public Transform elfPlacePosition;
    public float releaseDelay;
    public Camera incineratorCam;
    // Use this for initialization
    void Start () {
		
	}
    private void Update()
    {
        if(Input.GetButtonUp("Fire1") && interactingPlayer != null)
        {
            if(interactingPlayer == GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().localPlayer)
            {
                if(interactingPlayer.GetComponent<ElfController>() != null)
                {
                    GetComponent<PhotonView>().RPC("Cancel", PhotonTargets.All);
                }
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
                    GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>().RPC("ChangeStatusIcon", PhotonTargets.All, interactingPlayer.GetComponent<SantaController>().carryingElf.GetComponent<PhotonView>().owner.NickName, 2);
                    GetComponent<PhotonView>().RPC("PlaceElf", PhotonTargets.All);
                }
            }
        }
        else
        {
            if (containedElf != null)
            {

                GetComponent<PhotonView>().RPC("ReleaseData", PhotonTargets.All, interactingPlayer.GetComponent<PhotonView>().ownerId);
                StartCoroutine(ReleaseElf());
            }
        }
    }
    [PunRPC]
    public void PlaceElf()
    {
        GameObject elfToPlace = GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().santa.GetComponent<SantaController>().carryingElf;
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().santa.GetComponent<SantaController>().carryingElf = null;
        containedElf = elfToPlace;
        if (containedElf.GetComponent<PhotonView>().isMine)
        {
            containedElf.GetComponent<Player>().cam.GetComponent<Camera>().enabled = false;
            foreach(SkinnedMeshRenderer renderer in containedElf.GetComponent<Player>().allRenderer)
            {
                renderer.enabled = true;
            }
            incineratorCam.enabled = true;
        }
        containedElf.GetComponent<Collider>().enabled = true;
        containedElf.GetComponent<ElfController>().currentState = ElfController.StruggleState.struggling;
        containedElf.transform.position = elfPlacePosition.position;
        containedElf.transform.LookAt(new Vector3(transform.position.x, containedElf.transform.position.y, transform.position.z));
        containedElf.transform.SetParent(null);
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
        interactingPlayer.GetComponent<ElfController>().canInteract = true;
        interactingPlayer = null;
        containedElf.transform.SetParent(null);
        containedElf.GetComponent<Rigidbody>().isKinematic = false;
        containedElf.GetComponent<ElfController>().currentState = ElfController.StruggleState.normal;
        if (containedElf.GetComponent<PhotonView>().isMine)
        {
            containedElf.GetComponent<Player>().cam.GetComponent<Camera>().enabled = true;
            foreach (SkinnedMeshRenderer renderer in containedElf.GetComponent<Player>().allRenderer)
            {
                renderer.enabled = false;
            }
            incineratorCam.enabled = false;
        }
        containedElf = null;
    }
    public IEnumerator ReleaseElf()
    {
        interactingPlayer.GetComponent<ElfController>().canInteract = false;
        float process = 0;

        while(process < 1)
        {
            yield return new WaitForSeconds(releaseDelay);
            process += 0.1f;
        }
        interactingPlayer.GetComponent<ElfController>().canInteract = false;
        GetComponent<PhotonView>().RPC("FinishRelease", PhotonTargets.All);
    }
    public void OnTriggerEnter(Collider hit)
    {
        if(hit.tag == "Elf" && hit.gameObject == containedElf)
        {
            if (hit.gameObject.GetComponent<PhotonView>().isMine)
            {
                GetComponent<PhotonView>().RPC("Kill", PhotonTargets.All);
                StartCoroutine(hit.gameObject.GetComponent<ElfController>().ActualDeath());
                GetComponent<PhotonView>().RPC("Cancel", PhotonTargets.All);
            }
        }
    }
    [PunRPC]
    public void Kill()
    {
        GetComponent<AudioSource>().Play();
        containedElf.tag = "Untagged";
    }
    [PunRPC]
    void Cancel()
    {
        if(interactingPlayer != null)
        {
            StopAllCoroutines();
            print(interactingPlayer);
            interactingPlayer.GetComponent<Player>().canInteract = true; //Interactingplayer could be santa???
            interactingPlayer = null;
        }
    }
    public override void CancelInteract()
    {
        Cancel();
    }
}
