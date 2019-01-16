using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour
{
    public Inventory inventory;
    public Ability holdingAbility;
    [HideInInspector]public Canvas canvasComp;

    public void Awake()
    {
        canvasComp = GetComponent<Canvas>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

}
