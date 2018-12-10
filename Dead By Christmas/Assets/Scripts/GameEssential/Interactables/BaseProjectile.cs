using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{

    public float speed;
    public float detectionRange;


    public void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Physics.CheckSphere(transform.position,detectionRange))
        {
            OnHit();
        }
    }

    public void OnHit()
    {
        Destroy(gameObject);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
