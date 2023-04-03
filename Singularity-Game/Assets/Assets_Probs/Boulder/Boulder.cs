using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : Damageable
{   
    [Header("Boulder")]
    public float speed = 10f;

    private Vector3 startingPosition;
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 999999999;
        currentHealth = maxHealth;

        mass = 2500;
        gravitationalDirection = Vector3.down;
        targetDirection = Vector3.down;
        direction = 1;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;


        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();
    }

    public void ResetBoulder(){
        transform.position = startingPosition;
        rb.velocity = Vector3.zero;
    }
}
