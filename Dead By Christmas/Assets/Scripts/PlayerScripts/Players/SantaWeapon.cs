using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaWeapon : MonoBehaviour {
    public string hitTag;
    public bool enabled;
    // Use this for initialization



    public void OnTriggerEnter(Collider hit)
    {
        if(hit.tag == hitTag && enabled)
        {
            hit.GetComponent<PhotonView>().RPC("ReceiveDamage", PhotonTargets.All, 1);
            hit.GetComponentInParent<PhotonView>().RPC("DealDamage", PhotonTargets.All);
        }
    }
}
