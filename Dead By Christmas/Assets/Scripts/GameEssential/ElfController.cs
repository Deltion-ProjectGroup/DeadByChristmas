using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfController : MonoBehaviour {

	public enum StruggleState { normal, struggling, knockedOut}
    public StruggleState struggleState;
    [Header("StruggleInfo")]
    public string struggleInput;
    public float struggleMoveSpeed;
    public int struggleTime;
    public float pullBackSpeed;
    int struggling;
    [Header("CraftingInfo")]
    public Transform itemLocation;
    public float itemDetectRange;
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
        }
    }

    public void Normal()
    {
        if (isKnockedOut)
        {
            ToggleRagdoll(false);
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
        {
            ToggleRagdoll(false);
        }
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
        {
            ToggleRagdoll(true);
        }
    }
}
