using System.Collections;
using UnityEngine;

public class Slow : Buff {
    public float slowAmount;
    public bool smooth;
    public float smoothModifier = 0.1f;
    public float smoothDelay = 1;
    public float smoothModifierDelay;
    public override IEnumerator Effect(float duration)
    {
        Player player = gameObject.GetComponent<Player>();
        float baseBackup = player.baseSpeed;
        if (smooth)
        {
            while (player.baseSpeed != baseBackup - slowAmount)
            {
                yield return new WaitForSeconds(smoothModifierDelay);
                player.baseSpeed = Mathf.Lerp(player.baseSpeed, baseBackup - slowAmount, smoothDelay);
                player.speed = player.baseSpeed;
            }
        }
        else
        {
            player.baseSpeed = baseBackup - slowAmount;
            player.speed = player.baseSpeed;
        }
        if (smooth)
        {
            yield return new WaitForSeconds(duration);
            while (player.baseSpeed != baseBackup)
            {
                yield return new WaitForSeconds(smoothModifierDelay);
                player.baseSpeed = Mathf.Lerp(player.baseSpeed, baseBackup, smoothDelay);
            }
        }
        else
        {
            yield return new WaitForSeconds(duration);
            player.baseSpeed = baseBackup;
            player.speed = player.baseSpeed;
        }
        Destroy(this);
    }
}
