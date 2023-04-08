using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Iron_Dude : Enemy
{

    [SerializeField] private bool attacking;
    [SerializeField] private GameObject fireball;
    [SerializeField] private float cool_time = 3.5f;
    [SerializeField] private float CASTING_TIME = 0.5f;
    private float cool_down;
    private bool dead;
    private float dying_time = 3.0f;



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
        attackRange = 12;
        playerObject = GameObject.FindWithTag("Player");
        //from Tree_enemy
        attacking = false;
        cool_down = 0.0f;
        dead = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead)
        {
            ChangeLineOfSight();
            RotateGravity();
            ApplyGravity();
            MoveEnemy();
            OnDeath();
        }

    }

    void Update()
    {
        if (!dead)
        {
            Attacking();
            IronDudeAttack();
            coolDown();
        }
        if (dead)
        {
            Dying();
        }
    }





    void IronDudeAttack()
    {

        if (cool_down <= 0)
        {
            if (attacking)
            {
                animator.SetTrigger("CastAttack");
                Instantiate(fireball, instantiatePosition(), Quaternion.identity);
                cool_down = cool_time;
                attacking = false;
            }

        }
    }

    void coolDown()
    {
        if (cool_down > 0)
        {
            cool_down -= 1 * Time.deltaTime;
            if (cool_down < 0)
            {
                cool_down = 0;
            }
        }
    }

    Vector3 instantiatePosition()
    {
        return new Vector3(transform.position.x + 2 * direction, transform.position.y, transform.position.z);
    }
    public new void OnDeath()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animator.SetTrigger("Die");
            dead = true;
        }
    }
    public new void MoveEnemy()
    {
        var velocity = Vector3.zero;
        if (InRange(sightRange) && !InRange(attackRange - 0.5f))
        {
            if (gravitationalDirection.x == 1)
            {
                direction = playerObject.transform.position.y - transform.position.y > 0 ? 1 : -1;
            }
            else if (gravitationalDirection.x == -1)
            {
                direction = playerObject.transform.position.y - transform.position.y > 0 ? -1 : 1;
            }
            else
            {
                direction = playerObject.transform.position.x - transform.position.x > 0 ? 1 : -1;
            }
            velocity = Vector3.forward * currentSpeed;
        }
        transform.Translate(velocity * Time.deltaTime);
        animator.SetFloat("EnemySpeed", velocity.magnitude);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    void Attacking()
    {
        if (cool_down <= 0)
        {
            attacking = InRange(attackRange);
        }
    }

    void Dying()
    {
        if(dying_time > 0)
        {
            dying_time -= 1 * Time.deltaTime;
        } else
            if(dying_time <= 0)
        {
            Destroy(gameObject);
        }
    }
}



