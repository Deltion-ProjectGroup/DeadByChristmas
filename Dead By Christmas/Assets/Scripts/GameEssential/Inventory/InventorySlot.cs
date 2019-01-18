using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]public Inventory inventory;
    public GameObject holdingAbility;
    // Use this for initialization
    public void Awake()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        if(transform.childCount > 0)
        {
            holdingAbility = transform.GetChild(0).gameObject;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        inventory.hoveringObject = gameObject;
        if (!inventory.dragging && transform.childCount > 0)
        {
            holdingAbility.GetComponent<Animation>().Play("GrowAbility");
            inventory.description.text = holdingAbility.GetComponent<InventoryItem>().holdingAbility.description;
            inventory.abilityName.text = holdingAbility.GetComponent<InventoryItem>().holdingAbility.name;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        inventory.hoveringObject = null;
        if(!inventory.dragging && transform.childCount > 0)
        {
            holdingAbility.GetComponent<Animation>().Play("ShrinkAbility");
        }
    }
}
