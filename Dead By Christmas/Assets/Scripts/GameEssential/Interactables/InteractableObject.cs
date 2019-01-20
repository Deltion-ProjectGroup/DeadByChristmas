using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : Photon.MonoBehaviour {
	public GameObject interactingPlayer;
	public virtual void Interact (int interactorID) {
		print("Interacting with " + transform.name);
        if(interactorID == GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().santa.GetComponent<PhotonView>().ownerId)
        {
            interactingPlayer = GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().santa;
        }
        else
        {
            foreach(GameObject elf in GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().allElfs)
            {
                if(elf.GetComponent<PhotonView>().ownerId == interactorID)
                {
                    interactingPlayer = elf;
                    break;
                }
            }
        }
	}

    public virtual void CancelInteract()
    {

    }
}
