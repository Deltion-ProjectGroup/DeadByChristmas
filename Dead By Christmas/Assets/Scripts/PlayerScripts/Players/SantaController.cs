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
    bool canAttack = true;
    [Header("Abilities")]
    public Ability[] abilities;
    public bool canSpecial = true;
    public float specialCooldown;

	// Use this for initialization
	void Start () {
        damage = baseDamage;
        PlayerStart();
        bodyRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<PhotonView>().isMine)
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
        if (GetComponent<PhotonView>().isMine)
        {
            PlayerFixedUpdate();
        }
    }
    IEnumerator Attack()
    {
        if (canAttack)
        {
            canAttack = false;
            animator.SetTrigger("Attack");
            Ray shootRay = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            RaycastHit hitObj;
            if (Physics.Raycast(shootRay, out hitObj, attackRange, damageableObjects, QueryTriggerInteraction.Ignore))
            {
                if (hitObj.transform.tag == "Elf")
                {
                    hitObj.transform.GetComponent<PhotonView>().RPC("ReceiveDamage", PhotonTargets.All, damage);
                }
            }
            print("COOLDOWN");
            yield return new WaitForSeconds(attackCooldown);
            print("COOLDOWN DONE");
            canAttack = true;
        }
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
    public override void Death()
    {
        base.Death();
    }
}
