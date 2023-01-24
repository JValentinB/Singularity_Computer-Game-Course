using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InvItem
{ 
    public int id;
    public string itemName, description, iconPath;
    public bool equiped;

    public InvItem(int id, string itemName, string description, string iconPath){
        this.id = id;
        this.itemName = itemName;
        this.description = description;
        this.equiped = false;
        this.iconPath = iconPath;
    }
    public InvItem(InvItem item){
        this.id = item.id;
        this.itemName = item.itemName;
        this.description = item.description;
        this.equiped = false;
        this.iconPath = item.iconPath;
    }
}