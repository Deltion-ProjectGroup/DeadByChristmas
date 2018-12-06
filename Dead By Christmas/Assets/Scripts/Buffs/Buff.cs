using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : MonoBehaviour {
    public float duration;
    // Use this for initialization
    private void Start()
    {
        StartCoroutine(Effect(duration));
    }

    // Update is called once per frame
    public abstract IEnumerator Effect(float duration)
}
