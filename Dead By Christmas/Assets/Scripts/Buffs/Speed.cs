using System.Collections;
using UnityEngine;

public class Speed : Buff {
    public float speedChangeAmt;
    public bool smooth;
    public float smoothModifier = 0.1f;
    public float smoothDelay = 0.125f;
    public float smoothModifierDelay;
    public override IEnumerator Effect(float duration)
    {
        Player player = gameObject.GetComponent<Player>();
        float baseBackup = player.baseSpeed;
        if (smooth)
        {
            while (player.baseSpeed != baseBackup + speedChangeAmt)
            {
                yield return new WaitForSeconds(smoothModifierDelay);
                player.baseSpeed = Mathf.Lerp(player.baseSpeed, baseBackup + speedChangeAmt, smoothDelay);
                player.speed = player.baseSpeed;
            }
        }
        else
        {
            player.baseSpeed = baseBackup + speedChangeAmt;
            player.speed = player.baseSpeed;
        }
        yield return new WaitForSeconds(duration);
        if (smooth)
        {
            while (player.baseSpeed != baseBackup)
            {
                yield return new WaitForSeconds(smoothModifierDelay);
                player.baseSpeed = Mathf.Lerp(player.baseSpeed, baseBackup, smoothDelay);
            }
        }
        else
        {
            player.baseSpeed = baseBackup;
            player.speed = player.baseSpeed;
        }
        Destroy(this);
    }
}
