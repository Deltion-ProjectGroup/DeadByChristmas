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
    public SantaWeapon weapon;
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
        if (GetComponent<PhotonView>().isMine && GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().ingame && !paused)
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
        if (GetComponent<PhotonView>().isMine && GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().ingame && !paused)
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
            weapon.GetComponent<SantaWeapon>().enabled = true;
            yield return new WaitForSeconds(1);
            animator.SetBool("Attack", false);
            yield return new WaitForSeconds(4);
            weapon.GetComponent<SantaWeapon>().enabled = false;
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
        elfToPickUp.GetComponent<ElfController>().StopAllCoroutines();
        elfToPickUp.transform.position = carryPosition.position;
        elfToPickUp.transform.SetParent(carryPosition);
        elfToPickUp.GetComponent<Collider>().enabled = false;
        elfToPickUp.GetComponent<Rigidbody>().isKinematic = true;
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
            health = 0;
            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameUIManager>().UpdateSantaHealth();
            StartCoroutine(gameManager.EndGame(false));
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
                //GameObject.FindGameObjectWithTag("Manager").GetComponent<GameUIManager>().IndicatorAppear(interactInput, hit.transform.GetComponent<InteractableObject>().interactEffect);
                return true;
            }
            else
            {
                if(hit.transform.tag == "Elf")
                {
                    //GameObject.FindGameObjectWithTag("Manager").GetComponent<GameUIManager>().IndicatorAppear(interactInput, hit.transform.GetComponent<ElfController>().effectText);
                    return true;
                }
            }
        }
        //GameObject.FindGameObjectWithTag("Manager").GetComponent<GameUIManager>().IndicatorDissapear();
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
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(animator.GetFloat("MovementSpeed"));
            stream.SendNext(animator.GetBool("Death"));
            stream.SendNext(animator.GetBool("Attack"));
        }
        else
        {
            animator.SetFloat("MovementSpeed", (float)stream.ReceiveNext());
            animator.SetBool("Death", (bool)stream.ReceiveNext());
            animator.SetBool("Attack", (bool)stream.ReceiveNext());
        }
    }
}
