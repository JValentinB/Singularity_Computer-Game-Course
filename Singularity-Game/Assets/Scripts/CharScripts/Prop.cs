using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : Damageable
{
    void Start(){
        maxHealth = 1;
        currentHealth = maxHealth;
        gravitationalDirection = Vector3.down;
        direction = 1;
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate(){
        ApplyGravity();
        RotateGravity();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public void onDeath(){
        //destroy prop
        //spawn all items from inventory
        gameObject.SetActive(false);
    }
}
