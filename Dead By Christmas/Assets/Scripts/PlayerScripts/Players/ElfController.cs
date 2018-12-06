using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElfController : Player {

	public enum StruggleState { normal, struggling, KnockedOut , Crafting , BeingDragged}
    [Header("CurrentState")]
    public StruggleState struggleState;

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

    [Header("KnockOutInfo")]
    public Rigidbody[] bones;
    public bool isKnockedOut = true;

    [Header("ExtraCharacterInfo")]
    public GameObject fillBar;
    GameObject currentFillbar;
    [SerializeField] Transform currentCam;

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

    //Checkes the state the elf is in
    public void CheckState()
    {
        switch (struggleState)
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
        }
    }

    //The normal state
    public void Normal()
    {
        PlayerFixedUpdate();
        PlayerUpdate();
        if (isKnockedOut)
            GetComponent<PhotonView>().RPC("ToggleElfRagdoll", PhotonTargets.All, false);
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
        if (isKnockedOut)
            GetComponent<PhotonView>().RPC("ToggleElfRagdoll", PhotonTargets.All, false);
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
            GetComponent<PhotonView>().RPC("ToggleElfRagdoll", PhotonTargets.All, true);
    }

    //Toggle the ragdoll
    public void ToggleRagdoll(bool onOrOf)
    {
        isKnockedOut = onOrOf;
        GetComponent<Animator>().enabled = !onOrOf;
        foreach (Rigidbody joint in bones)
        {
            joint.isKinematic = !onOrOf;
        }
    }

    //the crafting state
    public void Crafting()
    {
        if (Input.GetButtonUp(craftingInput))
        {
            struggleState = StruggleState.normal;
            StopCoroutine(currentCrafting);
            currentCam.position += currentCam.forward * camBackwardsDistance;
            Destroy(currentFillbar);
        }
    }

    //Call this to start crafting.
    public IEnumerator StartCrafting(float time)
    {
        currentFillbar = Instantiate(fillBar, transform.position + -currentCam.forward, Quaternion.identity);
        currentCam.position -= currentCam.forward * camBackwardsDistance;
        currentFillbar.transform.LookAt(currentCam);
        struggleState = StruggleState.Crafting;
        FillbarValueSet fillBarComponent = currentFillbar.GetComponent<FillbarValueSet>();
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
            Destroy(itemsInRange[i].gameObject);
        }
        struggleState = StruggleState.normal;
        currentCam.position += currentCam.forward * camBackwardsDistance;
        Destroy(currentFillbar);
        Crafted();
    }

    //This is called when youre done crafting an item.
    public void Crafted()
    {
        Debug.Log("Crafted Weapon");
    }

    //gizmos
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, itemDetectRange);
    }
    public override void Death()
    {
        struggleState = StruggleState.KnockedOut;
    }
}   