using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class InvUI : MonoBehaviour
{
    public GameObject inventoryUI; // Reference to the inventory UI game object
    public Transform itemsParent; // Reference to the parent object of all the inventory items
    public GameObject inventoryItemUIPrefab; // Reference to the prefab for the UI element of an inventory item

    private InvManager playerInventory; // Reference to the player's inventory

    private void Start()
    {
        inventoryUI = GameObject.FindWithTag("InventoryUI");
        playerInventory = gameObject.GetComponent<InvManager>(); // Get the reference to the player's inventory
        playerInventory.onInventoryChangedCallback += UpdateUI;
    }

    void Update(){
        if(Input.GetKey(KeyCode.I) || (inventoryUI.activeSelf && Input.GetKey(KeyCode.Escape))){
            OpenCloseInventory();
        }
    }

    // call this method when the invnetory is updated
    private void UpdateUI()
    {
        // Add all the items from the inventory to the UI
        for (int i = 0; i < playerInventory.stackedInventoryItems.Count; i++)
        {
            InvItem item = playerInventory.stackedInventoryItems[i].Item1;
            int amount = playerInventory.stackedInventoryItems[i].Item2;

            // Instantiate the UI element for the item
            GameObject itemUI = Instantiate(inventoryItemUIPrefab, itemsParent);

            // Fill in the details for the UI element
            itemUI.GetComponent<Image>().sprite = item.icon;
            itemUI.transform.GetChild(0).GetComponent<Text>().text = item.itemName;
            itemUI.transform.GetChild(1).GetComponent<Text>().text = amount.ToString();

            // Add a button listener to the UI element to handle item interactions
            Button button = itemUI.GetComponent<Button>();
            button.onClick.AddListener(() => UseItem(item));
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
            UpdateUI();
        }
            
    }
    // Method to open and close the inventory
    public void OpenCloseInventory()
    {
        Debug.Log("Toggle Inv");
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        //UpdateUI();
    }
}
