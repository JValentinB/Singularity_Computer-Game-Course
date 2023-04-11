using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crate : Prop
{

    public bool dropSingularityCrystal = true;
    public int singularityCrystalAmmo = 2;

    public bool dropAetherCrystal = true;
    public int aetherCrystalAmmo = 5;

    void Start(){
        //Components
        rb = GetComponent<Rigidbody>();

        //Variables
        maxHealth = 1;
        currentHealth = maxHealth;
        gravitationalDirection = Vector3.down;
        direction = 1;

        singularityCrystalAmmo = 2;
        aetherCrystalAmmo = 5;
    }

    void FixedUpdate(){
        ApplyGravity();
        RotateGravity();
        OnDeath();

        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    public override void createLoot()
    {
        

        if(dropSingularityCrystal) inventory.AddItem(inventory.GetItem(2), singularityCrystalAmmo);

        if(dropAetherCrystal) inventory.AddItem(inventory.GetItem(3), aetherCrystalAmmo);

        var fixedPos = new Vector3(transform.position.x, transform.position.y-1f, transform.position.z);
        GameObject lootObject = Instantiate(lootPrefab, fixedPos, transform.rotation);
        lootObject.GetComponent<Loot>().inventory = inventory;
    }

    public override void createPieces(){
        Vector3 pos = transform.position;
        for(int i = 0; i < 5; i++){
            Vector3 piecePos = new Vector3(pos.x+(Random.value%10)/10, pos.y+(Random.value%10)/10, pos.z+(Random.value%10)/10);
            GameObject pieceClone = Instantiate(cratePiece, piecePos, transform.rotation);
            Destroy(pieceClone, 5);
        }
    }
}
