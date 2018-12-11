using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScreen : MonoBehaviour {
    public static TransitionScreen transitionScreen;

    public void Awake()
    {
        transitionScreen = this;
    }
    //The fadein animation
    [PunRPC]
    public void FadeIn()
    {
        GetComponent<Animation>().Play("TransitionFadeIn");
    }
    //The fadeout animation
    [PunRPC]
    public void FadeOut()
    {
        GetComponent<Animation>().Play("TransitionFadeOut");
    }
}
