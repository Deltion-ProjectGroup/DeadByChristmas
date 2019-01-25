using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfController : Player {
    public float invisTimeAfterKO;
    public string effectText;
    [Range(0, 1)]
    public float walkSoundAmt, runSoundAmt;
	public enum StruggleState { normal, struggling, KnockedOut , Crafting , BeingDragged, Weapon}
    [Header("CurrentState")]
    public StruggleState currentState;

    [Header("StruggleInfo")]
    public string struggleInput;
    public float struggleMoveSpeed;
    public int struggleTime;
    public float pullBackSpeed;
    int struggling;

    [Header("CraftingInfo")]
    public float itemDetectRange;
    public LayerMask craftingItemsMask;
    bool canCraft;
    public int minimumItemAmount;
    public string craftingInput;
    public int craftingTime;
    IEnumerator currentCrafting;
    public float camBackwardsDistance;
    public string gun;

    [Header("InventoryInfo")]
    public Transform inventoryLocation;
    public bool hasItem;
    GameObject currentItem;
    public string ItemName;
    public string dropInput;

    [Header("KnockOutInfo")]
    public bool isKnockedOut = true;
    public float knockedOutTime;

    [Header("JumpInfo")]
    public float jumpForce;
    public bool addForce; // toggle it from at force, to setforce
    public float groundDetectionRange;
    public LayerMask groundMask;
    public bool canJump;
    public bool downardsVelocityEnabled;
    public float addDownwardsVelocity;

    [Header("ExtraCharacterInfo")]
    public GameObject fillBar;
    GameObject currentFillbar;
    public Transform currentCam;
    public float runMultiplier;
    public string runInput;
    public GameObject interactingObject;

    //StartFunction
    public void Start()
    {
        PlayerStart();
        isKnockedOut = false;
    }

    //Update Function
    public void Update()
    {
        if (GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().ingame && !paused)
        {
            CheckState();
        }
    }

    public void AddItem()
    {
        hasItem = true;
        currentItem = PhotonNetwork.Instantiate(ItemName, inventoryLocation.position, inventoryLocation.rotation, 0);
        currentItem.transform.parent = inventoryLocation;
        currentItem.GetComponent<Rigidbody>().isKinematic = true;
        currentItem.GetComponent<WeaponPart>().pickedUp = true;
        currentItem.GetComponent<WeaponPart>().hasCollider = false;
        currentItem.GetComponent<Collider>().enabled = false;
    }
    public void DropItem()
    {
        hasItem = false;
        currentItem.transform.position = transform.position + Vector3.up * 0.5f + transform.forward * 0.5f;
        currentItem.transform.parent = null;
        currentItem.GetComponent<Rigidbody>().isKinematic = false;
        currentItem.GetComponent<WeaponPart>().pickedUp = false;
        currentItem.GetComponent<WeaponPart>().hasCollider = true;
        currentItem.GetComponent<Collider>().enabled = true;
        currentItem = null;
        currentState = StruggleState.normal;
    }

    public void ExtraDownwardsVelocity()
    {
        if(rig.velocity.y <= 0)
        {
            rig.velocity -= addDownwardsVelocity * Time.deltaTime * Vector3.up;
        }
    }

    //Call this when you want to jump
    public void Jump()
    {
        if (canJump)
        {
            animator.SetBool("JumPland", false);
            animator.SetBool("Jump", true);
            rig = GetComponent<Rigidbody>();

            if (addForce)
                rig.AddForce(Vector3.up * jumpForce);
            else
                rig.velocity = Vector3.up * jumpForce;

            canJump = false;
        }
    }

    //Checks if you're standing on something.
    public void CheckGround()
    {
        if (Physics.CheckSphere(transform.position, groundDetectionRange, groundMask))
        {
            canJump = true;
            animator.SetBool("Jump", false);
            animator.SetBool("JumpLand", true);
        }
        else
        {
            canJump = false;
        }
    }

    //Checkes the state the elf is in
    public void CheckState()
    {
        switch (currentState)
        {
            case StruggleState.normal:
                Normal();
                break;

            case StruggleState.struggling:
                Struggling();
                break;

            case StruggleState.KnockedOut:
                KnockedOut();
                break;
            case StruggleState.Crafting:
                Crafting();
                break;
            case StruggleState.Weapon:
                Weapon();
                break;
        }
    }

    //The weapon State
    public void Weapon()
    {
        if (Input.GetButtonDown(dropInput) && !CanInteract() && hasItem)
            animator.SetBool("HasGun", false);
            DropItem();
        PlayerFixedUpdate();
        PlayerUpdate();
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetBool("HasGun", false);
            currentItem.GetComponent<BaseGun>().Fire();
            hasItem = false;
            currentItem = null;
        }
    }

    //The normal state
    public override void Walk()
    {
        float multiplier = speed * Time.deltaTime * extraMovmentMultiplier;
        Vector3 movePos = new Vector3();
        movePos.z = Input.GetAxis("Vertical");
        movePos.x = Input.GetAxis("Horizontal");
        if (movePos == Vector3.zero)
        {
            extraMovmentMultiplier = 0;
            audioSources[2].volume = 0;
        }
        else
        {
            if (Input.GetButton(runInput))
            {
                extraMovmentMultiplier = runMultiplier;
                audioSources[1].volume = runSoundAmt;
            }
            else
            {
                extraMovmentMultiplier = 1;
                audioSources[1].volume = walkSoundAmt;
            }
        }
        animator.SetFloat("MovementSpeed", extraMovmentMultiplier);
        transform.Translate(movePos * multiplier);
    }
    public void Normal()
    {
        CheckInteract();
        if(Input.GetButtonDown(dropInput) && !CanInteract() && hasItem)
            if (!currentItem.GetComponent<BaseGun>())
                DropItem();
        CheckGround();
        if (downardsVelocityEnabled)
            ExtraDownwardsVelocity();
        if (Input.GetButtonDown("Jump"))
            Jump();

        PlayerFixedUpdate();
        PlayerUpdate();
        CheckForItems();
        if (canCraft && Input.GetButtonDown(craftingInput))
        {
            currentCrafting = StartCrafting(craftingTime);
            animator.SetBool("Interact", true);
            StartCoroutine(ChangeAnimBool("Interact", false));
            animator.SetBool("Crafting", true);
            StartCoroutine(currentCrafting);
        }
    }

    //Checks if you are able to craft a weapon
    public void CheckForItems()
    {
        Collider[] itemsInRange = Physics.OverlapSphere(transform.position, itemDetectRange, craftingItemsMask);
        canCraft = (itemsInRange.Length >= minimumItemAmount)? true : false;
    }

    //The struggle state
    public void Struggling()
    {
        if (Input.GetButtonDown(struggleInput))
        {
            struggling += struggleTime;
            struggleMoveSpeed += struggleTime;
        }
        if (struggling != 0)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - transform.forward/2f, struggleMoveSpeed * Time.deltaTime);
            struggleMoveSpeed--;
            struggling--;
        }
        transform.Translate(Vector3.forward * Time.deltaTime * pullBackSpeed);
    }

    //the knocked out state
    public void KnockedOut()
    {
        if (!isKnockedOut)
        {
            PhotonView view = GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>();
            List<object> overloads = new List<object>();
            animator.SetBool("Death", true);
            view.RPC("ChangeStatusIcon", PhotonTargets.All, PhotonNetwork.player.NickName, 1);
            currentKnockedOutNumerator = KnockedOutTimer(knockedOutTime);
            StartCoroutine(KnockedOutTimer(knockedOutTime));
        }
        
    }

    public IEnumerator currentKnockedOutNumerator;

    public IEnumerator KnockedOutTimer(float time)
    {
        print("COUNTDING DOWN");
        isKnockedOut = true;
        yield return new WaitForSeconds(time);
        PhotonView view = GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>();
        List<object> overloads = new List<object>();
        overloads.Add(PhotonNetwork.player.NickName);
        overloads.Add(0);
        view.RPC("ChangeStatusIcon", PhotonTargets.All, overloads.ToArray());
        print("RETURNED TO NORMAL");
        animator.SetBool("Death", false);
        currentState = StruggleState.normal;
        isKnockedOut = false;
        health = baseHealth;
    }
    [PunRPC]
    public override void ReceiveDamage(int damageAmount)
    {
        if(currentState != StruggleState.KnockedOut && currentState != StruggleState.BeingDragged && currentState != StruggleState.struggling)
        {
            base.ReceiveDamage(damageAmount);
        }
    }
    //the crafting state
    public void Crafting()
    {
        if (Input.GetButtonUp(craftingInput))
        {
            currentState = StruggleState.normal;
            StopCoroutine(currentCrafting);
            currentCam.position += currentCam.forward * camBackwardsDistance;
            Destroy(currentFillbar);
            animator.SetBool("SitDown", false);
            animator.SetBool("Crafting", false);
            foreach (SkinnedMeshRenderer renderer in bodyRenderer)
            {
                renderer.enabled = false;
            }
        }
    }

    //Call this to start crafting.
    public IEnumerator StartCrafting(float time)
    {
        animator.SetBool("SitDown", true);
        currentFillbar = Instantiate(fillBar, transform.position + -currentCam.forward, Quaternion.identity);
        currentCam.position -= currentCam.forward * camBackwardsDistance;
        currentFillbar.transform.LookAt(currentCam);
        currentState = StruggleState.Crafting;
        FillbarValueSet fillBarComponent = currentFillbar.GetComponent<FillbarValueSet>();
        foreach (SkinnedMeshRenderer renderer in bodyRenderer)
        {
            renderer.enabled = true;
        }
        //sets the fillbar value.
        for (float i = 0; i < time; i += 0.05f)
        {
            fillBarComponent.fillbar.fillAmount = 1f / time * i;
            yield return new WaitForSeconds(0.05f);
        }
        //destroys the items in range.
        Collider[] itemsInRange = Physics.OverlapSphere(transform.position, itemDetectRange, craftingItemsMask);
        for (int i = 0; i < minimumItemAmount; i++)
        {
            itemsInRange[i].GetComponent<PhotonView>().RPC("DestroyThis", PhotonTargets.All);
        }
        currentState = StruggleState.normal;
        currentCam.position += currentCam.forward * camBackwardsDistance;
        Destroy(currentFillbar);
        animator.SetBool("SitDown", false);
        Crafted();
        foreach (SkinnedMeshRenderer renderer in bodyRenderer)
        {
            renderer.enabled = false;
        }
    }

    //This is called when youre done crafting an item.
    public void Crafted()
    {
        hasItem = true;
        currentItem = PhotonNetwork.Instantiate(gun, inventoryLocation.position, inventoryLocation.rotation, 0);
        currentItem.transform.SetParent(inventoryLocation);
        currentItem.GetComponent<Rigidbody>().isKinematic = true;
        currentItem.GetComponent<WeaponPart>().pickedUp = true;
        currentItem.GetComponent<WeaponPart>().hasCollider = false;
        currentItem.GetComponent<Collider>().enabled = false;
        currentItem.GetComponent<BaseGun>().controller = this;
        animator.SetBool("Crafting", false);
        animator.SetBool("HasGun", true);
        currentState = StruggleState.Weapon;
    }

    //gizmos
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, groundDetectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, itemDetectRange);
    }

    public override void Death()
    {
        currentState = StruggleState.KnockedOut;
    }
    [PunRPC]
    public void Kill()
    {
        audioSources[2].clip = audioClips[0];
    }
    public IEnumerator ActualDeath()
    {
        GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>().RPC("ChangeStatusIcon", PhotonTargets.All, PhotonNetwork.player.NickName, 3);
        GetComponent<PhotonView>().RPC("Kill", PhotonTargets.All);
        yield return new WaitForSeconds(1);
        GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>().RPC("GetElfs", PhotonTargets.All);
        PhotonNetwork.Destroy(gameObject);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(animator.GetBool("Crafting"));
            stream.SendNext(animator.GetBool("Sitting"));
            stream.SendNext(animator.GetBool("Death"));
            stream.SendNext(animator.GetBool("Jump"));
            stream.SendNext(animator.GetBool("Knock"));
            stream.SendNext(animator.GetBool("Interact"));
            stream.SendNext(animator.GetBool("SitDown"));
            stream.SendNext(animator.GetBool("JumpLand"));
            stream.SendNext(animator.GetBool("HasGun"));
            stream.SendNext(animator.GetBool("Emote"));
            stream.SendNext(animator.GetFloat("MovementSpeed"));
        }
        else
        {
            animator.SetBool("Crafting", (bool)stream.ReceiveNext());
            animator.SetBool("Sitting", (bool)stream.ReceiveNext());
            animator.SetBool("Death", (bool)stream.ReceiveNext());
            animator.SetBool("Jump", (bool)stream.ReceiveNext());
            animator.SetBool("Knock", (bool)stream.ReceiveNext());
            animator.SetBool("Interact", (bool)stream.ReceiveNext());
            animator.SetBool("SitDown", (bool)stream.ReceiveNext());
            animator.SetBool("JumpLand", (bool)stream.ReceiveNext());
            animator.SetBool("HasGun", (bool)stream.ReceiveNext());
            animator.SetBool("Emote", (bool)stream.ReceiveNext());
            animator.SetFloat("MovementSpeed", (float)stream.ReceiveNext());
        }
    }
}   