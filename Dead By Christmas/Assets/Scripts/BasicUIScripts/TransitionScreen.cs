using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScreen : MonoBehaviour {
    public static TransitionScreen transitionScreen;
    GameObject screen;

    public void Awake()
    {
        transitionScreen = this;
        screen = gameObject;
    }
    //The fadein animation
    [PunRPC]
    public IEnumerator FadeIn()
    {
        screen.SetActive(true);
        GetComponent<Animation>().Play("TransitionFadeIn");
        yield return new WaitForSeconds(GetComponent<Animation>().GetClip("TransitionFadeIn").length);
    }
    //The fadeout animation
    [PunRPC]
    public IEnumerator FadeOut()
    {
        GetComponent<Animation>().Play("TransitionFadeOut");
        yield return new WaitForSeconds(GetComponent<Animation>().GetClip("TransitionFadeOut").length);
        screen.SetActive(false);
    }
}
