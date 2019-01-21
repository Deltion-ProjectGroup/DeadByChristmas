using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UsefulAttributes;

public class SantaController : Player {
    [Header("Damage")]
    public int damage;
    [SerializeField] int baseDamage;
    [SerializeField] float attackCooldown;
    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] LayerMask damageableObjects;
    public bool canAttack = true;
    [Header("Abilities")]
    public Ability[] abilities;
    public bool canSpecial = true;
    public float specialCooldown;
    public Transform carryPosition;
    public GameObject carryingElf;

	// Use this for initialization
	void Start () {
        damage = baseDamage;
        PlayerStart();
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<PhotonView>().isMine && GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().ingame)
        {
            PlayerUpdate();
            if (Input.GetButtonDown("Fire1"))
            {
                StartCoroutine(Attack());
            }
            if (Input.GetButtonDown("Fire2"))
            {
                StartCoroutine(SpecialAttack(2));
            }
        }
	}
    private void FixedUpdate()
    {
        if (GetComponent<PhotonView>().isMine && GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().ingame)
        {
            PlayerFixedUpdate();
        }
    }
    IEnumerator Attack()
    {
        if (canAttack)
        {
            canAttack = false;
            animator.SetBool("Attack", true);
            Ray shootRay = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            RaycastHit hitObj;
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length - 2.5f);
            if (Physics.Raycast(shootRay, out hitObj, attackRange, damageableObjects, QueryTriggerInteraction.Ignore))
            {
                if (hitObj.transform.tag == "Elf")
                {
                    hitObj.transform.GetComponent<PhotonView>().RPC("ReceiveDamage", PhotonTargets.All, damage);
                    GetComponent<PhotonView>().RPC("DealDamage", PhotonTargets.All);
                }
            }
            animator.SetBool("Attack", false);
            yield return new WaitForSeconds(1);
            print("COOLDOWN DONE");
            canAttack = true;
        }
    }
    [PunRPC]
    public void DealDamage()
    {
        audioSources[0].clip = audioClips[0];
        audioSources[0].Play();
    }
    public IEnumerator SpecialAttack(float time)
    {
        if (canSpecial)
        {
            canSpecial = false;
            abilities[0].Attack(transform);
            yield return new WaitForSeconds(time);
            canSpecial = true;
        }
    }
    [PunRPC]
    public void PickUpElf(int elfID)
    {
        GameObject elfToPickUp = null;
        foreach(GameObject elf in GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().allElfs)
        {
            if(elf.GetComponent<PhotonView>().ownerId == elfID)
            {
                elfToPickUp = elf;
                break;
            }
        }
        carryingElf = elfToPickUp;
        elfToPickUp.transform.SetParent(carryPosition);
        elfToPickUp.transform.position = Vector3.zero;
        elfToPickUp.GetComponent<Collider>().enabled = false;
        elfToPickUp.GetComponent<ElfController>().currentState = ElfController.StruggleState.BeingDragged;

    }
    [PunRPC]
    public override void ReceiveDamage(int damageAmount)
    {
        base.ReceiveDamage(damageAmount);
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GameUIManager>().UpdateSantaHealth();
    }
    public override void Death()
    {
        base.Death();
        GaemManager gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>();
        if (!gameManager.finished)
        {
            StartCoroutine(gameManager.EndGame());
        }
    }
    public override bool CanInteract()
    {
        //Shoot ray
        if (Physics.Raycast(cam.position, cam.forward, out hit, interactionRange))
        {

            //Check if the hit object is interactable
            if (hit.transform.tag == interactTag)
            {
                return true;
            }
            else
            {
                if(hit.transform.tag == "Elf")
                {
                    return true;
                }
            }
        }
        return false;
    }
    public override void Interact()
    {
        //To do if interacting
        print("Interacting...");
        if (hit.collider != null)
        {
            if(hit.transform.GetComponent<InteractableObject>() != null)
            {
                hit.transform.GetComponent<InteractableObject>().interactingPlayer = gameObject;
                hit.transform.GetComponent<InteractableObject>().Interact(GetComponent<PhotonView>().ownerId);
            }
            else
            {
                GetComponent<PhotonView>().RPC("PickUpElf", PhotonTargets.All, hit.transform.GetComponent<PhotonView>().ownerId);
            }
        }
        else
        {
            print("Hit is null!");
        }
    }
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(animator.GetBool("Walking"));
            stream.SendNext(animator.GetBool("Death"));
            stream.SendNext(animator.GetBool("Attack"));
        }
        else
        {
            animator.SetBool("Walking", (bool)stream.ReceiveNext());
            animator.SetBool("Death", (bool)stream.ReceiveNext());
            animator.SetBool("Attack", (bool)stream.ReceiveNext());
        }
    }
}
