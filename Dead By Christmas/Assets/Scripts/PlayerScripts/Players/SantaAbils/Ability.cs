using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "New Ability")]
public abstract class Ability : ScriptableObject {
    public float cooldown;



    public abstract void Attack(Transform thisTransform);
    public abstract IEnumerator IEAttack(Transform thisTrans);
}
