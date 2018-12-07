using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScreen : MonoBehaviour {
    public static TransitionScreen transitionScreen;

    public void Awake()
    {
        transitionScreen = this;
    }
    public void FadeIn()
    {
        GetComponent<Animation>().Play("TransitionFadeIn");
    }
    public void FadeOut()
    {
        GetComponent<Animation>().Play("TransitionFadeOut");
    }
}
