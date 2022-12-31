using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Prop
{
    void Start(){
        maxHealth = 1;
        currentHealth = maxHealth;
        gravitationalDirection = Vector3.down;
        direction = 1;
        rigidbody = GetComponent<Rigidbody>();
        prop_break = GetComponent<AudioSource>();
    }

    void FixedUpdate(){
        ApplyGravity();
        RotateGravity();
        OnDeath();
    }
}
