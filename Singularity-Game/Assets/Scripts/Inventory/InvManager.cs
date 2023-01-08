using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvManager : ItemDatabase
{
    public int inventorySize;
    public int itemCount;
    public List<(InventoryItem, int)> stackedInventoryItems = new List<(InventoryItem, int)>();

    void Start(){
        stackedInventoryItems = new List<(InventoryItem, int)>();
    }

    //Returns the amount of items which were not added
    public int AddItem(InventoryItem item, int amount)
    {
        int index = FindItemIndexInInventory(item);
        Debug.Log(index + ", " + item.id);
        if(index >= 0 && itemCount + amount <= inventorySize && !item.pickedUp){
            stackedInventoryItems[index] = (stackedInventoryItems[index].Item1,  stackedInventoryItems[index].Item2 + amount);
            item.pickedUp = true;
            itemCount += amount;
            return 0;
        } else if(index == -1 && itemCount + amount <= inventorySize && !item.pickedUp) {
            stackedInventoryItems.Add((item, amount));
            item.pickedUp = true;
            itemCount += amount;
            return 0;
        } else if (!item.pickedUp){
            AddItem(item, inventorySize - itemCount);
            itemCount += amount;
            return itemCount + amount - inventorySize;
        }
        return -1;
    }

    //Returns the amount of Items which can't be removed
    public int RemoveItem(InventoryItem item, int amount)
    {
        int index = FindItemIndexInInventory(item);
        if(index >= 0 && stackedInventoryItems[index].Item2 >= amount){
            stackedInventoryItems[index] = (stackedInventoryItems[index].Item1,  stackedInventoryItems[index].Item2 - amount);
            itemCount -= amount;
            if(stackedInventoryItems[index].Item2 == 0){
                stackedInventoryItems.RemoveAt(index);
            }
            return 0;
        } else {
            return index == -1 ? -1 :amount - stackedInventoryItems[index].Item2;
        }
    }

    public int FindItemIndexInInventory(InventoryItem item)
    {
        return stackedInventoryItems.FindIndex(i => i.Item1.id == item.id);
    }

    public bool IsEmpty(){
        return stackedInventoryItems.Count == 0;
    }
}