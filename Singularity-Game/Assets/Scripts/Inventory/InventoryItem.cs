using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryItem : MonoBehaviour
{ 
    public int id;
    public string itemName, description;
    public bool equiped;
    public Sprite icon;

    public InventoryItem(int id, string itemName, string description, Sprite icon){
        this.id = id;
        this.itemName = itemName;
        this.description = description;
        this.equiped = false;
        this.icon = icon;
    }
    public InventoryItem(InventoryItem item){
        this.id = item.id;
        this.itemName = item.itemName;
        this.description = item.description;
        this.equiped = false;
        this.icon = item.icon;
    }
}