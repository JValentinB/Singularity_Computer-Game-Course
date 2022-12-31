using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Prop
{
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
}
