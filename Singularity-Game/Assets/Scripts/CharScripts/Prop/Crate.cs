using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Prop
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
        inventoryManager.AddItem(inventoryManager.GetItem(0), 69);
        inventoryManager.AddItem(inventoryManager.GetItem(1), 420);

        GameObject lootObject = Instantiate(lootPrefab, transform.position, transform.rotation);
        lootObject.GetComponent<Loot>().inventory = inventoryManager;
    }
}
