using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantaController : Player {
    public int damage;
    [SerializeField] int baseDamage;
    [SerializeField] float attackCooldown;
    [SerializeField] float attackRange;
    [SerializeField] LayerMask damageableObjects;
    bool canAttack = true;

	// Use this for initialization
	void Start () {
        damage = baseDamage;
        PlayerStart();
	}
	
	// Update is called once per frame
	void Update () {
        PlayerUpdate();
        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(Attack());
        }
	}
    private void FixedUpdate()
    {
        PlayerFixedUpdate();
    }
    IEnumerator Attack()
    {
        if (canAttack)
        {
            canAttack = false;
            yield return null;
            Ray shootRay = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            RaycastHit hitObj;
            if (Physics.Raycast(shootRay, out hitObj, attackRange, damageableObjects, QueryTriggerInteraction.Ignore))
            {
                if (hitObj.transform.tag == "Player")
                {
                    hitObj.transform.GetComponent<ElfController>().ReceiveDamage(damage);
                    print("COOLDOWN");
                    yield return new WaitForSeconds(2);
                    print("COOLDOWN DONE");
                }
            }
            else
            {
                print("NOTHING");
            }
            canAttack = true;
        }
    }
    public override void Death()
    {
        throw new System.NotImplementedException();
    }
}
