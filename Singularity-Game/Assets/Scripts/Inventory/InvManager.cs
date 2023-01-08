using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvManager : ItemDatabase
{
    public List<(InventoryItem, int)> stackedInventoryItems = new List<(InventoryItem, int)>();

    void Start(){
        stackedInventoryItems = new List<(InventoryItem, int)>();
    }

    //Adds Item to the given Inventory
    public void AddItem(InventoryItem item, int amount)
    {
        int index = FindItemIndexInInventory(item);
        if(index >= 0){
            stackedInventoryItems[index] = (stackedInventoryItems[index].Item1,  stackedInventoryItems[index].Item2 + amount);
            return;
        } else {
            stackedInventoryItems.Add((item, amount));
            return;
        }
    }

    //Returns the amount of Items which can't be removed
    public int RemoveItem(InventoryItem item, int amount)
    {
        int index = FindItemIndexInInventory(item);
        if(index >= 0 && stackedInventoryItems[index].Item2 >= amount){
            stackedInventoryItems[index] = (stackedInventoryItems[index].Item1,  stackedInventoryItems[index].Item2 - amount);
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