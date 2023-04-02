using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvUI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private InvManager playerInventory;
    public int slots;
    private List<(GameObject, InvItem)> slotList;

    private void Start()
    {
        playerInventory = player.GetComponent<Player>().inventory;
        slots = 4;
        CeateSlotList();
        UpdateInvUI();
    }

    private void Update()
    {
        UpdateInvUI();
    }

    private void CeateSlotList()
    {
        slotList = new List<(GameObject, InvItem)>();
        var invUiTransform = gameObject.transform;
        foreach(Transform slot in invUiTransform){
            slotList.Add((slot.gameObject, null));
        }
    }

    private void UpdateInvUI(){
        foreach (var entry in slotList)
        {
            if(entry.Item2 != null){
                if(entry.Item2.iconPath != null){
                    var icon = entry.Item1.transform.Find("InventoryItemIcon").gameObject.GetComponent<Image>();
                    icon.sprite = Resources.Load<Sprite>(entry.Item2.iconPath);
                }
                var itemCount = entry.Item1.transform.Find("ItemCount").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
                itemCount.text = playerInventory.GetItemCount(entry.Item2).ToString();
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

    public bool AddItemToPlayerInventory(InvItem item, int amount){
        if(slotList.FindIndex(entry => (entry.Item2 != null && entry.Item2.id == item.id)) >= 0){
            playerInventory.AddItem(item, amount);
            UpdateInvUI();
            return true;
        }
        var firstEmptyIndex = slotList.FindIndex(entry => entry.Item2 == null);
        if(firstEmptyIndex >= 0){
            playerInventory.AddItem(item, amount);
            slotList[firstEmptyIndex] = (slotList[firstEmptyIndex].Item1, item);
            UpdateInvUI();
            return true;
        }
        return false;
    }

    public int GetItemIdInSlot(GameObject slot){
        foreach ((GameObject, InvItem) entry in slotList)
        {
            if(GameObject.ReferenceEquals(slot, entry.Item1)){
                return entry.Item2.id;
            }
        }
        return -1;
    }
}
