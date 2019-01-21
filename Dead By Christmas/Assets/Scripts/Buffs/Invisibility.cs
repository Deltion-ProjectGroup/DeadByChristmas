using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisibility : Buff {
    public float timeBeforeFullyInvis;
    public float tickInvisibilityChange;

    public override IEnumerator Effect(float duration)
    {
        foreach (SkinnedMeshRenderer renderer in GetComponent<Player>().bodyRenderer)
        {
            Material fader = new Material(renderer.material);
            fader.SetFloat("_Mode", 2);
            fader.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            fader.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            fader.SetInt("_ZWrite", 0);
            fader.DisableKeyword("_ALPHATEST_ON");
            fader.EnableKeyword("_ALPHABLEND_ON");
            fader.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            fader.renderQueue = 3000;
            renderer.material = fader;
        }
        while(GetComponent<Player>().bodyRenderer[0].material.color.a > 0)
        {
            foreach(SkinnedMeshRenderer renderer in GetComponent<Player>().bodyRenderer)
            {
                Color newColor = renderer.material.color;
                newColor.a -= tickInvisibilityChange;
                renderer.material.color = newColor;
            }
            yield return new WaitForSeconds(timeBeforeFullyInvis * tickInvisibilityChange);
        }
        yield return new WaitForSeconds(duration);
        while (GetComponent<Player>().bodyRenderer[0].material.color.a < 1)
        {
            foreach (SkinnedMeshRenderer renderer in GetComponent<Player>().bodyRenderer)
            {
                Color newColor = renderer.material.color;
                newColor.a += tickInvisibilityChange;
                renderer.material.color = newColor;
            }
            yield return new WaitForSeconds(timeBeforeFullyInvis * tickInvisibilityChange);
        }
        foreach (SkinnedMeshRenderer renderer in GetComponent<Player>().bodyRenderer)
        {
            Material fader = new Material(renderer.material);
            fader.SetFloat("_Mode", 0);
            fader.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            fader.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            fader.SetInt("_ZWrite", 1);
            fader.DisableKeyword("_ALPHATEST_ON");
            fader.DisableKeyword("_ALPHABLEND_ON");
            fader.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            fader.renderQueue = -1;
            renderer.material = fader;
        }
        Destroy(this);
    }
}
