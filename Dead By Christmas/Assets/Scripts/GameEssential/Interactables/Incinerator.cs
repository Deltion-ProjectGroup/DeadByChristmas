using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerator : InteractableObject {
    public GameObject containedElf;
    GaemManager gameManager;
    public Transform elfPlacePosition;
    public float releaseDelay;
    GameObject currentFillbar;
    [Tooltip("Lower is smoother")]
    public float releaseSmoothness;
    public Camera incineratorCam;
    // Use this for initialization
    void Start () {
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>();
	}
    private void Update()
    {
        if(Input.GetButtonUp("Fire1") && interactingPlayer != null)
        {
            if(interactingPlayer.GetComponent<PhotonView>().isMine)
            {
                if(interactingPlayer.GetComponent<ElfController>() != null)
                {
                    Destroy(currentFillbar);
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
        interactingPlayer = null;
        GameObject elfToPlace = gameManager.santa.GetComponent<SantaController>().carryingElf;
        gameManager.santa.GetComponent<SantaController>().carryingElf = null;
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
        foreach (GameObject elf in gameManager.allElfs)
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
        ElfController interactorController = interactingPlayer.GetComponent<ElfController>();
        interactorController.canInteract = true;
        interactingPlayer = null;
        containedElf.transform.SetParent(null);
        containedElf.GetComponent<Rigidbody>().isKinematic = false;
        containedElf.GetComponent<ElfController>().currentState = ElfController.StruggleState.normal;
        containedElf.GetComponent<ElfController>().animator.SetBool("Death", false);
        if (containedElf.GetComponent<PhotonView>().isMine)
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>().RPC("ChangeStatusIcon", PhotonTargets.All, PhotonNetwork.player.NickName, 0);
            containedElf.GetComponent<Player>().cam.GetComponent<Camera>().enabled = true;
            incineratorCam.enabled = false;
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
        ElfController elfController = interactingPlayer.GetComponent<ElfController>();
        elfController.canInteract = false;
        currentFillbar = Instantiate(elfController.fillBar, elfController.transform.position + -elfController.currentCam.forward, Quaternion.identity);
        elfController.currentCam.position -= elfController.currentCam.forward * elfController.camBackwardsDistance;
        currentFillbar.transform.LookAt(elfController.currentCam);
        FillbarValueSet fillBarComponent = currentFillbar.GetComponent<FillbarValueSet>();
        float process = 0;

        while(process < releaseDelay)
        {

            yield return new WaitForSeconds(releaseSmoothness);
            process += releaseSmoothness;
            fillBarComponent.fillbar.fillAmount = process;
        }
        Destroy(currentFillbar);
        elfController.canInteract = false;
        elfController.currentCam.position += elfController.currentCam.forward * elfController.camBackwardsDistance;
        GetComponent<PhotonView>().RPC("FinishRelease", PhotonTargets.All);
    }
    public void OnTriggerEnter(Collider hit)
    {
        if(hit.tag == "Elf" && hit.gameObject == containedElf)
        {
            if (hit.gameObject.GetComponent<PhotonView>().isMine)
            {
                GetComponent<PhotonView>().RPC("Kill", PhotonTargets.All);
                GetComponent<PhotonView>().RPC("Cancel", PhotonTargets.All);
                StartCoroutine(hit.gameObject.GetComponent<ElfController>().ActualDeath());
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
            if (interactingPlayer.GetComponent<PhotonView>().isMine)
            {
                ElfController elfController = interactingPlayer.GetComponent<ElfController>();
                Destroy(currentFillbar);
                elfController.canInteract = false;
                elfController.currentCam.position += elfController.currentCam.forward * elfController.camBackwardsDistance;
            }
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
