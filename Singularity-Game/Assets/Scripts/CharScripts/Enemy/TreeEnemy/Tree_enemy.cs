using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Tree_enemy : Enemy
{

    [SerializeField] private bool attacking;
    [SerializeField] private GameObject[] rocks;
    [SerializeField] private float cool_time = 3.0f;
    [SerializeField] private float CASTING_TIME = 3.0f;
    private float casting_time;
    private float cool_down = 0.0f;
    private bool isWalking;
    




    // Start is called before the first frame update
    void Start()
    {
        //from Damageable
        maxHealth = 200;
        currentHealth = maxHealth;
        direction = 1;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().mass = 80.0f;
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
        sightRange = 15;
        attackRange = 5;
        playerObject = GameObject.FindWithTag("Player");
        //from Tree_enemy
        attacking = false;
        isWalking = false;
        casting_time = CASTING_TIME;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ChangeLineOfSight();
        RotateGravity();
        ApplyGravity();
        MoveEnemy();
        Tree_enemy_attack();
        ToggleAnimation();
    }

    void Update()
    {
        attacking = InRange(attackRange);
        CoolDown();
        OnDeath();
    }

    void Tree_enemy_attack()
    {
        if (attacking)
        {
            if (cool_down <= 0)
            {
                Instantiate(rocks[Random.Range(0, rocks.Length)], transform);
                cool_down = cool_time;
                casting_time = CASTING_TIME;
                animator.SetTrigger("CastAttack");
            }

        }
    }

    void ToggleAnimation()
    {
        if (InRange(sightRange) && !isWalking)
        {
            animator.SetTrigger("Walk");
            isWalking = true;
        }
        if (!InRange(sightRange) && isWalking)
        {
            animator.SetTrigger("StopWalk");
            isWalking = false;
        }
    }

    void CoolDown()
    {
        if(cool_down > 0)
        {
            cool_down -= 1.0f * Time.deltaTime;
            

            if (cool_down < 0)
            {
                cool_down = 0;
            }
        }
        if(casting_time > 0)
        {
            casting_time -= 1.0f * Time.deltaTime;

            if (casting_time <= 0)
            {
                animator.SetTrigger("StopCasting");
                casting_time = CASTING_TIME;
            }
            
                
    
        }
    }

}


