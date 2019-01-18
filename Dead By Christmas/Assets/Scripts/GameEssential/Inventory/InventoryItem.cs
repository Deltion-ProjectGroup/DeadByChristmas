using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Inventory inventory;
    public Ability holdingAbility;
    [HideInInspector]public Canvas canvasComp;

    public void Awake()
    {
        canvasComp = GetComponent<Canvas>();
        GetComponent<Image>().sprite = holdingAbility.icon;
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
    }

}
