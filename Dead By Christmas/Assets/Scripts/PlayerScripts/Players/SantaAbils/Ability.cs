using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Ability", menuName = "New Ability")]
public abstract class Ability : ScriptableObject {
    public Sprite icon;
    public float cooldown;
    [TextArea(1, 2)]
    public string description;
    public int abilityID;



    public abstract void Attack(Transform thisTransform);
    public abstract IEnumerator IEAttack(Transform thisTrans);
    public abstract void AddEffect(GameObject[] targets);
}
