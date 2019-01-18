using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
    public List<GameObject> activeSlots = new List<GameObject>();
    public List<GameObject> allSlots =  new List<GameObject>();
    public Transform activeSlotHolder;
    public Transform inactiveSlotHolder;
    public bool dragging;
    public GameObject hoveringObject;
    public Transform startSlot;
    public Transform draggingAbility;
    public Text description;
    public Text abilityName;
    Vector3 backupPos;
    public SaveDatabase database;
    public ScrollRect scrollbar;
    const float iconFollowSpeed = 1000;
    // Use this for initialization
    void Start () {
        database = GameObject.FindGameObjectWithTag("Database").GetComponent<SaveDatabase>();
        foreach(Transform slot in activeSlotHolder)
        {
            activeSlots.Add(slot.gameObject);
            allSlots.Add(slot.gameObject);
        }
        foreach(Transform slot in inactiveSlotHolder)
        {
            allSlots.Add(slot.gameObject);
        }
        Load();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1") && dragging == false && hoveringObject != null)
        {
            if(hoveringObject.transform.childCount > 0)
            {
                draggingAbility = hoveringObject.transform.GetChild(0);
                dragging = true;
                draggingAbility.GetComponent<Image>().raycastTarget = false;
                draggingAbility.GetComponent<InventoryItem>().canvasComp.sortingOrder++;
                startSlot = draggingAbility.parent;
                backupPos = draggingAbility.position;
                scrollbar.enabled = false;
            }
        }
        if(Input.GetButtonUp("Fire1") && dragging == true)
        {
            draggingAbility.GetComponent<Image>().raycastTarget = true;
            draggingAbility.GetComponent<InventoryItem>().canvasComp.sortingOrder--;
            scrollbar.enabled = true;
            dragging = false;
            if(hoveringObject != null)
            {
                if (hoveringObject == startSlot.gameObject)
                {
                    draggingAbility.transform.SetParent(startSlot);
                    draggingAbility.transform.position = backupPos;
                }
                else
                {
                    Swap(startSlot.gameObject, hoveringObject);
                    Save();
                }
            }
            else
            {
                draggingAbility.transform.SetParent(startSlot);
                draggingAbility.transform.position = backupPos;
                draggingAbility.GetComponent<Animation>().Play("ShrinkAbility");
            }
            draggingAbility = null;
            startSlot = null;
        }
        if (dragging)
        {
            draggingAbility.position = Vector3.MoveTowards(draggingAbility.position, Input.mousePosition, iconFollowSpeed);
        }
    }
    public void Swap(GameObject item, GameObject item2, bool ingame = true)
    {
        if (!ingame)
        {
            backupPos = item.transform.GetChild(0).position;
        }
        item2.GetComponent<InventorySlot>().holdingAbility = item.transform.GetChild(0).gameObject;
        item.GetComponent<InventorySlot>().holdingAbility = item2.transform.GetChild(0).gameObject;
        item.transform.GetChild(0).position = item2.transform.GetChild(0).position;
        item2.transform.GetChild(0).position = backupPos;
        item.transform.GetChild(0).SetParent(item2.transform);
        item2.transform.GetChild(0).SetParent(item.transform);
    }
    public void Save()
    {
        List<int> equippedAbilityIDs = new List<int>();
        for(int i = 0; i < activeSlots.Count; i++)
        {
            equippedAbilityIDs.Add(activeSlots[i].GetComponent<InventorySlot>().holdingAbility.GetComponent<InventoryItem>().holdingAbility.abilityID);
        }
        database.userData.equippedAbilities = equippedAbilityIDs;
        database.Save();
    }
    public void Load()
    {
        if(database.userData.equippedAbilities.Count > 0)
        {
            for (int i = 0; i < database.userData.equippedAbilities.Count; i++)
            {
                if (i < activeSlots.Count)
                {
                    if (activeSlots[i].GetComponent<InventorySlot>().holdingAbility.GetComponent<InventoryItem>().holdingAbility.abilityID != database.userData.equippedAbilities[i])
                    {
                        print("SHUD SWAP");
                        for (int ability = 0; ability < allSlots.Count; ability++)
                        {
                            if (allSlots[ability].GetComponent<InventorySlot>().holdingAbility.GetComponent<InventoryItem>().holdingAbility.abilityID == database.userData.equippedAbilities[i])
                            {
                                print("FOUND");
                                Swap(allSlots[ability], activeSlots[i], false);
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            print("CUNT");
        }
    }
}
