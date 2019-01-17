using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfAnimTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Timer());	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    IEnumerator Timer()
    {
        Animator animator = GetComponent<Animator>();
        yield return new WaitForSeconds(2);
        animator.SetBool("Walking", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("Walking", false);
        animator.SetBool("Running", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("Running", false);
        animator.SetBool("Jump", true);
        yield return new WaitForSeconds(3.5f);
        animator.SetBool("Jump", false);
        animator.SetBool("JumpLand", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("JumpLand", false);
        animator.SetBool("Sitting", true);
        animator.SetBool("SitDown", true);
        yield return new WaitForSeconds(2.5f);
        animator.SetBool("SitDown", false);
        animator.SetBool("Sitting", false);
        animator.SetBool("Crafting", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("Crafting", false);
        animator.SetBool("Walking", true);
        animator.SetBool("Emote", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("Emote", false);
        animator.SetBool("HasGun", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("HasGun", false);
        animator.SetBool("Interact", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("Interact", false);
        yield return new WaitForSeconds(2);
        animator.SetBool("Death", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("Death", false);
    }
}
