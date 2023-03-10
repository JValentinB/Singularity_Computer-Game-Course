using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBoss : Enemy
{
    private float rangeClose, rangeFar;
    private float[] rootSpikePos;
    [SerializeField] private GameObject rootSpike;
    [SerializeField] private GameObject rootSlash;
    [SerializeField] private GameObject rootSweep;
    
    void Start()
    {
        //from Damageable
        maxHealth = 1000;
        currentHealth = maxHealth;
        direction = 1;
        /* animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().mass = 32.5f; */
        gravitationalDirection = Vector3.down;
        //from Character
        jumpNumber = 0;
        currentSpeed = 2.5f;
        /* mass = GetComponent<Rigidbody>().mass; */
        jumpForce = 0;
        critChance = 0.2d;
        critMod = 1.3f;
        //from Enemy
        xp = 250;
        sightRange = 30;
        attackRange = 3;
        playerObject = GameObject.FindWithTag("Player");
        //from ForestBoss
        rangeClose = 3f;
        rangeFar = 8f;
        rootSpikePos = new float[3];
        rootSpikePos[0] = 10f;
        rootSpikePos[1] = 20f;
        rootSpikePos[2] = 30f;
        BranchSlash();
    }

    // Update is called once per frame
    void Update()
    {
        ChooseAttack();
    }

    private float DistanceToPlayer(){
        return Mathf.Abs(transform.position.x - playerObject.transform.position.x);
    }

    private void ChooseAttack(){
        if(DistanceToPlayer() < rangeClose){
            //Randomize all possible close range attacks
        } else if(DistanceToPlayer() > rangeFar){
            //Randomize all possible far range attacks
        } else {
            //Randomize all possible mid range attacks
        }
    }

    //Close range attack
    private void BranchSlash(){
        //Branch slashing downwards from above
        //Back off to avoid
        var spawnPos = new Vector3(transform.position.x, transform.position.y + 2.2f, transform.position.z);
        GameObject rootSlashObject = Instantiate(rootSlash, spawnPos, Quaternion.identity);
    }

    //Mid range attack
    private void RootSpikes(){
        //3 roots spiking upwards from the ground
        //Positions of spiking should be shown with earth rumbling at the pos
        for(int i = 0; i < rootSpikePos.Length; i++){
            var spawnPos = new Vector3(transform.position.x - rootSpikePos[i], transform.position.y, transform. position.z);
            GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.identity);
        }
        //set cooldown
    }

    //Mid range attack
    private void BranchSweep(){
        //Branch sweep at ground level, jump or back off to avoid
        var spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject rootSweepObject = Instantiate(rootSweep, spawnPos, Quaternion.identity);
    }

    //Far range attack
    private void ThrowProjectile(){
        //Projectile pulled out of the ground
        //Fly to player, can be manipulated
    }
}
