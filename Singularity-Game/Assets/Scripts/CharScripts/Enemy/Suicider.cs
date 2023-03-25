using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy y = -2.069685
//Player y = -0.01290846
//Diff = 2,05677654

public class Suicider : Enemy
{
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip explosionClip, inflateClip;
    private float explForce, explRadius, explUplift, cooldown, done;
    private bool attacking;
    public bool triggered;

    void Start(){
        //from Damageable
        maxHealth = 50;
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
        xp = 50;
        sightRange = 5;
        attackRange = 3;
        playerObject = GameObject.FindWithTag("Player");
        //from Suicider
        explosionEffect.GetComponent<AudioSource>().clip = inflateClip;
        explForce = 50000f;
        explRadius = 3f;
        explUplift = 55f;
        attacking = false;
        triggered = false;
        cooldown = 2f;
        done = 0f;
    }

    void FixedUpdate(){
        ChangeLineOfSight();
        RotateGravity();
        ApplyGravity();
        MoveEnemy();
    }

    void Update(){
        BoomAttack();
        OnDeath();
    }

    public void BoomAttack(){
        explosionEffect.transform.position = transform.position;
        
        if(InRange(attackRange) || done >= 1.4f || triggered){
            if(done >= cooldown) {
                //Speichernutzung? OverlapSphereNoAlloc...?
                var hitColliders = Physics.OverlapSphere(transform.position, attackRange);
                foreach (var hitCollider in hitColliders){
                    var hitObject = hitCollider.gameObject;
                    if(hitObject.GetComponent<Damageable>()){
                        var forceDir = hitObject.transform.position - transform.position;
                        var enemyPos = hitObject.transform.position + forceDir;
                        hitObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        hitObject.GetComponent<Rigidbody>().AddExplosionForce(explForce, enemyPos, explRadius, explUplift);
                        if(hitObject.GetComponent<Suicider>()){
                            if(!hitObject.GetComponent<Suicider>().triggered && hitObject.GetComponent<Suicider>().done < 1.4f){
                                hitObject.GetComponent<Suicider>().xp = 0;
                                hitObject.GetComponent<Suicider>().done = 1;
                                hitObject.GetComponent<Suicider>().triggered = true;
                            }
                        } else {
                            xp = InRange(attackRange) ? 0 : xp;
                            hitObject.GetComponent<Damageable>().ApplyDamage((int)Mathf.Floor(Crit() * 50));
                        }
                    }
                }
                explosionEffect.GetComponent<ParticleSystem>().Play(true);
                explosionEffect.GetComponent<AudioSource>().clip = explosionClip;
                explosionEffect.GetComponent<AudioSource>().Play();
                currentHealth = 0;
            }
            if(!explosionEffect.GetComponent<AudioSource>().isPlaying) explosionEffect.GetComponent<AudioSource>().Play();
            done += Time.deltaTime;
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f)*(Mathf.Max(done, 1f));
        }
        else {
            if(explosionEffect.GetComponent<AudioSource>().isPlaying) explosionEffect.GetComponent<AudioSource>().Stop();
            done = 0;
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }
}

