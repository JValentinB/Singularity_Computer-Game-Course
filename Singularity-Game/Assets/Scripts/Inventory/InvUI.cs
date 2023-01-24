using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class InvUI : MonoBehaviour
{
    public GameObject inventoryUI, slotPrefab;
    private InvManager playerInventory; // Reference to the player's inventory
    public int slots;
    private List<(GameObject, InvItem)> slotList;

    

    private void Start()
    {
        inventoryUI = GameObject.FindWithTag("InventoryUI");
        playerInventory = gameObject.GetComponent<Player>().inventory; // Get the reference to the player's inventory
        slots = 20;
        CreateLayout();
        UpdateInvUI();
        OpenCloseInventory();
        //playerInventory.onInventoryChangedCallback += UpdateUI;
    }

    private void CreateLayout()
    {
        slotList = new List<(GameObject, InvItem)>();

        for(int i = 0; i < slots; i++){
            GameObject newSlot = (GameObject)Instantiate(slotPrefab);
            newSlot.name = "Slot" + i;
            newSlot.transform.SetParent(inventoryUI.transform, false);
            slotList.Add((newSlot, null));
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.I) || (inventoryUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))){
            OpenCloseInventory(); 
        }
        /* if(Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("TEST ALL ITEMS IN LIST:");
            foreach (var entry in slotList){
                if(entry.Item2 != null){
                    Debug.Log(entry.Item2.itemName);
                }
            }
        } */
    }

    public void UpdateInvUI(){
        foreach (var entry in slotList)
        {
            if(entry.Item2 != null){
                if(playerInventory.GetItemCount(entry.Item2) >= 0)//>0
                {
                    if(entry.Item2.iconPath != null){
                        Debug.Log("T3");
                        var icon = entry.Item1.transform.Find("InventoryItemIcon").gameObject.GetComponent<Image>();
                        icon.sprite = Resources.Load<Sprite>(entry.Item2.iconPath);
                    }
                    if(entry.Item2.itemName != null){
                        Debug.Log("T4");
                        var itemName = entry.Item1.transform.Find("ItemName").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
                        itemName.text = entry.Item2.itemName;
                    }
                    var itemCount = entry.Item1.transform.Find("ItemCount").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
                    itemCount.text = playerInventory.GetItemCount(entry.Item2).ToString();
                }
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

    public bool AddItemToList(InvItem item){
        if(slotList.FindIndex(entry => (entry.Item2 != null && entry.Item2.id == item.id)) >= 0){
            UpdateInvUI();
            return true;
        }
        var firstEmptyIndex = slotList.FindIndex(entry => entry.Item2 == null);
        if(firstEmptyIndex >= 0){
            slotList[firstEmptyIndex] = (slotList[firstEmptyIndex].Item1, item);
            UpdateInvUI();
            return true;
        }
        Debug.Log("Das soll nicht passieren");
        return false;
    }

    /* // Method to handle item interactions
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
    } */
    // Method to open and close the inventory
    public void OpenCloseInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        //UpdateUI();
    }

    
}
