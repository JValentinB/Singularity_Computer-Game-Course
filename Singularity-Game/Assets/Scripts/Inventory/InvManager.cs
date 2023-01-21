using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
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
            onInventoryChangedCallback.Invoke();
            return;
        } else {
            stackedInventoryItems.Add((item, amount));
            onInventoryChangedCallback.Invoke();
            return;
        }
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
            onInventoryChangedCallback.Invoke();
            return 0;
        } else {
            return index == -1 ? -1 :amount - stackedInventoryItems[index].Item2;
        }
    }

    public int FindItemIndexInInventory(InvItem item)
    {
        return stackedInventoryItems.FindIndex(i => i.Item1.id == item.id);
    }

    public bool IsEmpty(){
        return stackedInventoryItems.Count == 0;
    }
}