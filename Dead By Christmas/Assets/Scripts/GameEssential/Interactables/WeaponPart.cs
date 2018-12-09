using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPart : InteractableObject {

    public Vector3 position;
    public Quaternion rotation;
    public Rigidbody rig;
    public float smoothing;
    public bool pickedUp;
    public bool hasCollider = true;

    public void Awake()
    {
        if (!photonView.isMine)
        {
            rig.isKinematic = true;
            StartCoroutine(UpdateData());
        }
    }

    public override void Interact()
    {
        if (!pickedUp && !interactingPlayer.GetComponent<ElfController>().hasItem)
        {
            interactingPlayer.GetComponent<ElfController>().AddItem();
            photonView.RPC("DestroyThis", PhotonTargets.All);
        }
    }

    [PunRPC]
    public void DestroyThis()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);

    }

    IEnumerator UpdateData()
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
            yield return null;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(pickedUp);
            stream.SendNext(hasCollider);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
            pickedUp = (bool)stream.ReceiveNext();
            GetComponent<Collider>().enabled = (bool)stream.ReceiveNext();
        }
    }
}
