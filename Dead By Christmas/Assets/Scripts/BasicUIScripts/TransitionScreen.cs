using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScreen : MonoBehaviour {
    public static TransitionScreen transitionScreen;
    [SerializeField]GameObject screen;

    public void Awake()
    {
        transitionScreen = this;
    }
    //The fadein animation
    [PunRPC]
    public IEnumerator FadeIn()
    {
        screen.SetActive(true);
        screen.GetComponent<Animation>().Play("TransitionFadeIn");
        yield return new WaitForSeconds(screen.GetComponent<Animation>().GetClip("TransitionFadeIn").length);
    }
    //The fadeout animation
    [PunRPC]
    public IEnumerator FadeOut()
    {
        screen.GetComponent<Animation>().Play("TransitionFadeOut");
        yield return new WaitForSeconds(screen.GetComponent<Animation>().GetClip("TransitionFadeOut").length);
        screen.SetActive(false);
    }
}
