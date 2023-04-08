using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InvManager : InvDatabase
{
    public List<(InvItem, int)> stackedInventoryItems = new List<(InvItem, int)>();
    public Action onInventoryChangedCallback;

    void Start(){
        stackedInventoryItems = new List<(InvItem, int)>();
    }

    //Adds Item to the given Inventory
    public void AddItem(InvItem item, int amount)
    {
        int index = FindItemIndexInInventory(item);
        if(index >= 0){
            stackedInventoryItems[index] = (stackedInventoryItems[index].Item1,  stackedInventoryItems[index].Item2 + amount);
        } else {
            stackedInventoryItems.Add((item, amount));
        }
        return;
    }

    //Returns the amount of Items which can't be removed
    public int RemoveItem(InvItem item, int amount)
    {
        int index = FindItemIndexInInventory(item);
        if(index >= 0 && stackedInventoryItems[index].Item2 >= amount){
            stackedInventoryItems[index] = (stackedInventoryItems[index].Item1,  stackedInventoryItems[index].Item2 - amount);
            if(stackedInventoryItems[index].Item2 == 0){
                stackedInventoryItems.RemoveAt(index);
            }
            //onInventoryChangedCallback.Invoke();
            return 0;
        } else {
            return index == -1 ? -1 :amount - stackedInventoryItems[index].Item2;
        }
    }

    public int FindItemIndexInInventory(InvItem item)
    {
        int index = stackedInventoryItems.FindIndex(i => i.Item1.id == item.id);
        
        return index;
    }

    //FIXME
    public int GetItemCount(InvItem item){
        var index = FindItemIndexInInventory(item);
        if(index == -1) return 0;
        return stackedInventoryItems[index].Item2;
    }

    public bool IsEmpty(){
        return stackedInventoryItems.Count == 0;
    }
}