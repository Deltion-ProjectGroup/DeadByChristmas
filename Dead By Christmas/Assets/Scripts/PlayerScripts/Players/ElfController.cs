using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfController : Player {

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
    public Transform InventoryLocation;
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
    [SerializeField] Transform currentCam;
    public float runMultiplier;
    public string runInput;

    //StartFunction
    public void Start()
    {
        PlayerStart();
    }

    //Update Function
    public void Update()
    {
        CheckState();
    }

    public void AddItem()
    {
        hasItem = true;
        currentItem = PhotonNetwork.Instantiate(ItemName, InventoryLocation.position, InventoryLocation.rotation, 0);
        currentItem.transform.parent = InventoryLocation;
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
            canJump = true;
        else
            canJump = false;
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
            DropItem();
        PlayerFixedUpdate();
        PlayerUpdate();
        if (Input.GetButtonDown("Fire1"))
        {
            currentItem.GetComponent<BaseGun>().Fire();
            hasItem = false;
            currentItem = null;
        }
    }

    //The normal state
    public void Normal()
    {
        extraMovmentMultiplier = (Input.GetButton(runInput)) ? runMultiplier : 1;
        CheckInteract();
        if(Input.GetButtonDown(dropInput) && !CanInteract() && hasItem)
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
            transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward/2f, struggleMoveSpeed * Time.deltaTime);
            struggleMoveSpeed--;
            struggling--;
        }
        transform.Translate(-Vector3.forward * Time.deltaTime * pullBackSpeed);
    }

    //the knocked out state
    public void KnockedOut()
    {
        if (!isKnockedOut)
        {
            PhotonView view = GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>();
            List<object> overloads = new List<object>();
            overloads.Add(PhotonNetwork.player.NickName);
            overloads.Add(GameUIManager.ElfStatus.Knocked);
            view.RPC("ChangeStatusIcon", PhotonTargets.All, overloads);
            currentKnockedOutNumerator = KnockedOutTimer(knockedOutTime);
            StartCoroutine(currentKnockedOutNumerator);
        }
        
    }

    public IEnumerator currentKnockedOutNumerator;

    public IEnumerator KnockedOutTimer(float time)
    {
        isKnockedOut = true;
        yield return new WaitForSeconds(time);
        currentState = StruggleState.normal;
        isKnockedOut = false;
        PhotonView view = GameObject.FindGameObjectWithTag("Manager").GetComponent<PhotonView>();
        List<object> overloads = new List<object>();
        overloads.Add(PhotonNetwork.player.NickName);
        overloads.Add(GameUIManager.ElfStatus.Knocked);
        view.RPC("ChangeStatusIcon", PhotonTargets.All, overloads);
        health = baseHealth;
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
            foreach (SkinnedMeshRenderer renderer in bodyRenderer)
            {
                renderer.enabled = false;
            }
        }
    }

    //Call this to start crafting.
    public IEnumerator StartCrafting(float time)
    {
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
        currentItem = PhotonNetwork.Instantiate(gun, InventoryLocation.position, InventoryLocation.rotation, 0);
        currentItem.transform.parent = InventoryLocation;
        currentItem.GetComponent<Rigidbody>().isKinematic = true;
        currentItem.GetComponent<WeaponPart>().pickedUp = true;
        currentItem.GetComponent<WeaponPart>().hasCollider = false;
        currentItem.GetComponent<Collider>().enabled = false;
        currentItem.GetComponent<BaseGun>().controller = this;
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
}   