using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class InvUI : MonoBehaviour
{
    public GameObject inventoryOverlay; // Reference to the inventory UI game object
    public GameObject inventoryUI;
    public Transform itemsParent; // Reference to the parent object of all the inventory items
    public GameObject slotPrefab; // Reference to the prefab for the UI element of an inventory item

    private InvManager playerInventory; // Reference to the player's inventory

    public int slots;
    public int rows;
    public float slotSize, slotPadding;

    public float inventoryHeight, inventoryWidth, xSlotPosition, ySlotPosition;
    private RectTransform inventoryRect;
    private List<(GameObject, InvItem)> slotList;

    

    private void Start()
    {
        inventoryOverlay = GameObject.FindWithTag("InventoryUI");
        inventoryUI = inventoryOverlay.transform.Find("InventoryUI").gameObject;
        playerInventory = gameObject.GetComponent<Player>().inventoryManager; // Get the reference to the player's inventory
        slots = 50;
        slotSize = 100;
        rows = 5;
        slotPadding = 10;
        CreateLayout();
        UpdateInvUI();
        //playerInventory.onInventoryChangedCallback += UpdateUI;
    }

    private void CreateLayout()
    {
        slotList = new List<(GameObject, InvItem)>();
        int coulumns = slots / rows;
        inventoryWidth = coulumns * (slotSize + slotPadding) + slotPadding;
        inventoryHeight = rows * (slotSize + slotPadding) + slotPadding;
        inventoryRect = inventoryUI.GetComponent<RectTransform>();
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidth);
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHeight);

        for(int y = 0; y < rows; y++){
            for(int x = 0; x < coulumns; x++){
                GameObject newSlot = (GameObject)Instantiate(slotPrefab);
                newSlot.name = "Slot" + y + x;
                newSlot.transform.SetParent(inventoryUI.transform);
                RectTransform slotRect = newSlot.GetComponent<RectTransform>();
                xSlotPosition = slotPadding * (x + 1) + (slotSize * x) - (inventoryWidth / 2);
                ySlotPosition = - slotPadding * (y + 1) - (slotSize * y) + (inventoryHeight / 2);
                slotRect.localPosition = inventoryRect.localPosition + new Vector3(xSlotPosition, ySlotPosition, 0);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize);
                slotRect.localScale = new Vector3(1,1,1);
                slotList.Add((newSlot, null));
            }
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.I) || (inventoryOverlay.activeSelf && Input.GetKeyDown(KeyCode.Escape))){
            OpenCloseInventory(); 
        }
        
        if(Input.GetKeyDown(KeyCode.R)){
            Debug.Log(playerInventory.GetItem(0) != null);
            Debug.Log("TEST");
            slotList[0] = (slotList[0].Item1, playerInventory.GetItem(0));
            //UpdateInvUI();
        }
    }

    public void UpdateInvUI(){
        foreach (var entry in slotList)
        {
            if(entry.Item2){
                var icon = entry.Item1.transform.Find("InventoryItemIcon").gameObject.GetComponent<Image>();
                icon.sprite = entry.Item2.icon;
                var itemName = entry.Item1.transform.Find("ItemName").gameObject.GetComponent<TextMesh>();
                itemName.text = entry.Item2.itemName;
                foreach (Transform child in entry.Item1.transform)
                {
                    child.gameObject.SetActive(true);
                }
            } else {
                foreach (Transform child in entry.Item1.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    // Method to handle item interactions
    private void UseItem(InvItem item)
    {
        // Example usage: remove one of the item from the inventory when clicked
        int removed = playerInventory.RemoveItem(item, 1);
        if(removed == -1){
            Debug.Log("Item not found in inventory");
        } else if(removed > 0){
            Debug.Log("Not enough items in inventory");
        } else {
            //UpdateUI();
        }   
    }
    // Method to open and close the inventory
    public void OpenCloseInventory()
    {
        Debug.Log("Toggle Inv");
        inventoryOverlay.SetActive(!inventoryOverlay.activeSelf);
        //UpdateUI();
    }

    
}
