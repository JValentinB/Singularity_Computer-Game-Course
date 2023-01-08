using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateItem : Prop
{
    private InvManager inventoryManager = new InvManager();

    void Start(){
        //Components
        rigidbody = GetComponent<Rigidbody>();

        //Variables
        maxHealth = 1;
        currentHealth = maxHealth;
        gravitationalDirection = Vector3.down;
        direction = 1;

        inventoryManager.inventorySize = 6;
        InventoryItem item1 = inventoryManager.GetItem(0);
        InventoryItem item2 = inventoryManager.GetItem(1);
        int first = inventoryManager.AddItem(item1, 2);
        int second = inventoryManager.AddItem(item2, 4);
    }

    void FixedUpdate(){
        ApplyGravity();
        RotateGravity();
        OnDeath();
    }
}
