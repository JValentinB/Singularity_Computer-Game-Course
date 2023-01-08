using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateItem : Prop
{
    private InvManager inventoryManager = new InvManager();
    [SerializeField] GameObject lootPrefab;

    void Start(){
        //Components
        rigidbody = GetComponent<Rigidbody>();

        //Variables
        maxHealth = 1;
        currentHealth = maxHealth;
        gravitationalDirection = Vector3.down;
        direction = 1;
    }

    void FixedUpdate(){
        ApplyGravity();
        RotateGravity();
        OnDeath();
    }

    public override void createLoot(){
        inventoryManager.inventorySize = 6;
        InventoryItem item1 = inventoryManager.GetItem(0);
        InventoryItem item2 = inventoryManager.GetItem(1);
        int first = inventoryManager.AddItem(item1, 2);
        int second = inventoryManager.AddItem(item2, 4);

        GameObject lootObject = Instantiate(lootPrefab, transform.position, transform.rotation);
        lootObject.GetComponent<Loot>().inventory = inventoryManager;
    }
}
