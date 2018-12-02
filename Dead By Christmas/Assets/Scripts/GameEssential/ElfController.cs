using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfController : MonoBehaviour {

	public enum StruggleState { normal, struggling, knockedOut , Crafting}
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
    private bool canCraft;
    public int minimumItemAmount;
    public string craftingInput;
    public int craftingTime;
    private IEnumerator currentCrafting;
    [Header("KnockOutInfo")]
    public Rigidbody[] bones;
    public bool isKnockedOut = true;

    public void Update()
    {
        CheckState();
    }

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

            case StruggleState.knockedOut:
                KnockedOut();
                break;
            case StruggleState.Crafting:
                Crafting();
                break;
        }
    }

    public void CheckForItems()
    {
        Collider[] itemsInRange = Physics.OverlapSphere(transform.position, itemDetectRange, craftingItemsMask);
        canCraft = (itemsInRange.Length >= minimumItemAmount)? true : false;
    }

    public IEnumerator StartCrafting(float time)
    {
        for (float i = 0; i < time; i += 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Collider[] itemsInRange = Physics.OverlapSphere(transform.position, itemDetectRange, craftingItemsMask);
        for (int i = 0; i < minimumItemAmount; i++)
        {
            Destroy(itemsInRange[i].gameObject);
        }
        struggleState = StruggleState.normal;
    }

    public void Normal()
    {
        if (isKnockedOut)
            ToggleRagdoll(false);
        CheckForItems();
        if (canCraft && Input.GetButtonDown(craftingInput))
        {
            currentCrafting = StartCrafting(craftingTime);
            StartCoroutine(currentCrafting);
            struggleState = StruggleState.Crafting;
        }
    }

    public void ToggleRagdoll(bool onOrOf)
    {
        isKnockedOut = onOrOf;
        GetComponent<Animator>().enabled = !onOrOf;
        foreach (Rigidbody joint in bones)
        {
            joint.isKinematic = !onOrOf;
        }
        if (onOrOf)
        {
            foreach (Rigidbody joint in bones)
            {
                joint.AddExplosionForce(1000,transform.position + transform.forward + Vector3.up * 0.5f,30);
            }
        }
    }

    public void Struggling()
    {
        if (isKnockedOut)
            ToggleRagdoll(false);
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

    public void KnockedOut()
    {
        if (!isKnockedOut)
            ToggleRagdoll(true);
    }

    public void Crafting()
    {
        if (Input.GetButtonUp(craftingInput))
        {
            struggleState = StruggleState.normal;
            StopCoroutine(currentCrafting);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, itemDetectRange);
    }
}
