using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_enemy : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        //from Damageable
        maxHealth = 200;
        currentHealth = maxHealth;
        direction = 1;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().mass = 32.5f;
        gravitationalDirection = Vector3.down;
        //from Character
        jumpNumber = 0;
        currentSpeed = 2.5f;
        mass = GetComponent<Rigidbody>().mass;
        jumpForce = 0;
        critChance = 0.2d;
        critMod = 1.3f;
        //from Enemy
        xp = 100;
        sightRange = 7.5f;
        attackRange = 3;
        playerObject = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveEnemy();    
    }
}
