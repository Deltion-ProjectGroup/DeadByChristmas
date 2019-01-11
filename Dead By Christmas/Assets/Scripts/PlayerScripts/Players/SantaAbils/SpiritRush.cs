using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpiritRush", menuName = "New SpiritRush")]
public class SpiritRush : Ability {

    public float duration;
    public float slowAmount;
    public override void Attack(Transform thisTransform)
    {
        for (int ab = 0; ab < GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().santa.GetComponent<SantaController>().abilities.Length; ab++)
        {
            if (GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().santa.GetComponent<SantaController>().abilities[ab] == this)
            {
                List<object> dataTransfer = new List<object>();
                dataTransfer.Add(ab);
                dataTransfer.Add(GameObject.FindGameObjectWithTag("Manager").GetComponent<GaemManager>().santa.GetComponent<PhotonView>().ownerId);
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
        foreach (GameObject target in targets)
        {
            Speed speedUp = target.AddComponent<Speed>();
            speedUp.duration = duration;
            speedUp.smooth = false;
            speedUp.speedChangeAmt = slowAmount;
            speedUp.smoothDelay = 1;
        }
    }
}
