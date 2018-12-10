using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : MonoBehaviour {

    public ElfController controller;
    public string projectile;
    public Transform firePosition;

    public void Fire()
    {
        PhotonNetwork.Instantiate(projectile, firePosition.position, controller.cam.rotation, 0);
        Destroy(gameObject);
        controller.currentState = ElfController.StruggleState.normal;
    }
}
