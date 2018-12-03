using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntertactableObject : MonoBehaviour {
    public virtual void Interact () {
        print ("Interacting with " + transform.name);
    }
}