using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crate : Prop
{
    private InvManager inventory = new InvManager();

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
        inventory.AddItem(inventory.GetItem(0), 69);
        inventory.AddItem(inventory.GetItem(1), 420);

        GameObject lootObject = Instantiate(lootPrefab, transform.position, transform.rotation);
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
