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
        for(int ab = 0; ab < GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().santa.GetComponent<SantaController>().abilities.Length; ab++)
        {
            if(GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().santa.GetComponent<SantaController>().abilities[ab] == this)
            {
                List<object> dataTransfer = new List<object>();
                dataTransfer.Add(ab);
                foreach(Collider col in availableTargets)
                {
                    dataTransfer.Add(col.GetComponent<PhotonView>().ownerId);
                }
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                PhotonNetwork.RaiseEvent(0, dataTransfer.ToArray(), true, options);
                break;
            }
        }
    }
    public override IEnumerator IEAttack(Transform thisTrans)
    {
        throw new System.NotImplementedException();
    }
    public override void AddEffect(GameObject[] targets)
    {
        foreach(GameObject target in targets)
        {
            Slow slow = target.AddComponent<Slow>();
            slow.duration = duration;
            slow.smooth = true;
            slow.slowAmount = slowAmount;
            slow.smoothDelay = 1;
        }
    }
}
