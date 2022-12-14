using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy y = -2.069685
//Player y = -0.01290846
//Diff = 2,05677654

public class Shiftcider : Enemy
{
    private float explForce, explRadius, explUplift, cooldown, done;
    private bool attacking;
    public bool triggered;

    void Start(){
        //from Damageable
        maxHealth = 50;
        currentHealth = maxHealth;
        direction = 1;
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().mass = 32.5f;
        gravitationalDirection = Vector3.down;
        //from Character
        jumpFactor = 0;
        jumpNumber = 0;
        currentSpeed = 2.5f;
        mass = GetComponent<Rigidbody>().mass;
        jumpForce = mass * jumpFactor;
        //from Enemy
        xp = 50;
        sightRange = 5;
        attackRange = 3;
        playerObject = GameObject.FindWithTag("Player");
        //from Suicider
        explForce = 100000f;
        explRadius = 3f;
        explUplift = 100f;
        attacking = false;
        triggered = false;
        cooldown = 2f;
        done = 0f;
    }

    void FixedUpdate(){
        RotateGravity();
        ChangeLineOfSight(Vector3.forward, Vector3.up);
        ApplyGravity();
        MoveEnemy();
    }

    void Update(){
        attacking = InRange(attackRange);
        BoomAttack();
    }

    public void BoomAttack(){
        if(attacking || done >= 1.4f || triggered){
            if(done >= cooldown || triggered) {
                var hitColliders = Physics.OverlapSphere(transform.position, attackRange);
                foreach (var hitCollider in hitColliders){
                    var hitObject = hitCollider.gameObject;
                    if(hitObject.GetComponent<Damageable>()){
                        var forceDir = hitObject.transform.position - transform.position;
                        var enemyPos = hitObject.transform.position + forceDir;
                        hitObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        hitObject.GetComponent<Rigidbody>().AddExplosionForce(explForce, enemyPos, explRadius, explUplift);
                        if(hitObject.GetComponent<Suicider>()){
                            hitObject.GetComponent<Suicider>().xp = 0;
                            hitObject.GetComponent<Suicider>().triggered = true;
                        } else {
                            xp = hitObject.GetComponent<Player>() ? 0 : xp;
                            hitObject.GetComponent<Damageable>().ApplyDamage(50);
                        }
                    }
                }
                currentHealth = 0;
                OnDeath();               
            }
            done += Time.deltaTime;
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f)*(Mathf.Max(done, 1f));
        }
        else {
            done = 0;
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }
}

