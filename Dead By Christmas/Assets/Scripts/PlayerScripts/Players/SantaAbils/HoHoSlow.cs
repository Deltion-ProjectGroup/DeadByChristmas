using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HoHoSlow", menuName = "New HoHoSlow")]
public class HoHoSlow : Ability {
    public float duration;
    public float slowAmount;
    public float range;
    public LayerMask targets;
    public override void Attack(Transform thisTransform)
    {
        Collider[] availableTargets = Physics.OverlapSphere(thisTransform.position, range, targets, QueryTriggerInteraction.Ignore);
        foreach(Collider col in availableTargets)
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().localPlayer.GetComponent<PhotonView>().RPC("AddEffect", col.GetComponent<PhotonView>().owner);
        }
    }
    public override IEnumerator IEAttack(Transform thisTrans)
    {
        throw new System.NotImplementedException();
    }
    [PunRPC]
    public void AddEffect()
    {
        Slow slow = GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().localPlayer.AddComponent<Slow>();
        slow.duration = duration;
        slow.smooth = true;
        slow.slowAmount = slowAmount;
        slow.smoothDelay = 1;
    }
}
