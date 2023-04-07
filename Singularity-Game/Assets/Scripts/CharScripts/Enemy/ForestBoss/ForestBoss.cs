using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBoss : Enemy
{
    private float rangeClose, rangeFar, sideRadius;
    [SerializeField] private float spikeCD, projectileCD, spikePause;
    [SerializeField] private int remainingSpikes;
    private float spikeCounter, projectileCounter, nextSpike;
    private float[] rootSpikePos = new float[10];
    [SerializeField] private GameObject rootSpike;
    [SerializeField] private GameObject manipulatableProjectile;
    [SerializeField] private Player playerScript; 

    [SerializeField] public float disToPlayer;

    //Positions of each side, has to be the middle and below the ground
    [SerializeField] private Vector3 bottomSideMidPos, topSideMidPos, rightSideMidPos, leftSideMidPos;
    
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
        rangeClose = 10f;
        rangeFar = 20f;

        spikeCD = 2f;
        projectileCD = 5f;

        playerScript = playerObject.GetComponent<Player>();
        bottomSideMidPos = transform.position;//Vector3.down;
        topSideMidPos = Vector3.up;
        rightSideMidPos = Vector3.right;
        leftSideMidPos = Vector3.left;
        sideRadius = 60f;
    }

    // Update is called once per frame
    void Update()
    {
        ChooseAttack();
    }

    private float DistanceToPlayer(){
        disToPlayer = Mathf.Abs(transform.position.x - playerObject.transform.position.x);
        return disToPlayer;
    }

    private void ChooseAttack(){
        var rand = Random.Range(0f, 1f);
        if(rand < 0.5f)         ThrowProjectile();
        else                    RootSpikes();
    }

    //Mid range attack (adjustable)
    private void RootSpikes(){
        if(remainingSpikes > 0){
            SpawnSpike();
            return;
        }
        
        if(spikeCounter > 0f){
            spikeCounter -= Time.deltaTime;
            return;
        }       
        
        spikeCounter = spikeCD;
        remainingSpikes = 20;
        Debug.Log("ROOT SPIKES!");
    }

    private void SpawnSpike(){
        if(spikePause > 0f){
            spikePause -= Time.deltaTime;
            return;
        }

        var spawnPos = transform.position;
        if(playerScript.gravitationalDirection == Vector3.right){
            spawnPos = new Vector3(rightSideMidPos.x, rightSideMidPos.y + Random.Range(-sideRadius, sideRadius), rightSideMidPos.z);
            GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.Euler(0f, 0f, 90f));
        } else if(playerScript.gravitationalDirection == Vector3.left){
            spawnPos = new Vector3(leftSideMidPos.x, leftSideMidPos.y + Random.Range(-sideRadius, sideRadius), leftSideMidPos.z);
            GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.Euler(0f, 0f, -90f));
        } else if(playerScript.gravitationalDirection == Vector3.up){
            spawnPos = new Vector3(topSideMidPos.x + Random.Range(-sideRadius, sideRadius), topSideMidPos.y, topSideMidPos.z);
            GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.Euler(0f, 0f, 180f));
        } else {
            spawnPos = new Vector3(bottomSideMidPos.x + Random.Range(-sideRadius, sideRadius), bottomSideMidPos.y, bottomSideMidPos.z);
            GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.identity);
        }
        
        remainingSpikes--;
        spikePause = 0.05f;
    }

    //Far range attack
    private void ThrowProjectile(){
        if(projectileCounter >= 0f){
            projectileCounter -= Time.deltaTime;
            //Debug.Log("Remaining cooldown: " + projectileCounter);
            return;
        }   

        //Projectile shouldn't spawn directly under player
        var spawnPosX = transform.position.x - Random.Range(5f, 40f);
        while(playerObject.transform.position.x - 5f <= spawnPosX && spawnPosX <= playerObject.transform.position.x + 5f){
            spawnPosX = transform.position.x - Random.Range(5f, 40f);
        }
        
        var spawnPos = new Vector3(transform.position.x - Random.Range(5f, 40f), transform.position.y, transform.position.z);
        GameObject projectileObject = Instantiate(manipulatableProjectile, spawnPos, Quaternion.identity);
        projectileCounter = projectileCD;
        Debug.Log("PROJECTILE!");
    }


    //-----------Unused attacks-----------------

    //Close range attack (maybe)
    /* private void BranchSlash(){
        if(branchCounter >= 0f){
            branchCounter -= Time.deltaTime;
            //Debug.Log("Remaining cooldown: " + branchCounter);
            return;
        }

        //Branch slashing downwards from above
        //Back off to avoid
        var spawnPos = new Vector3(transform.position.x, transform.position.y + 2.2f, transform.position.z);
        GameObject rootSlashObject = Instantiate(rootSlash, spawnPos, Quaternion.identity);
        branchCounter = slashCD;
        Debug.Log("BRANCH SLASH!");
    } */

    //Mid range attack
    /* private void BranchSweep(){
        if(branchCounter >= 0f){
            branchCounter -= Time.deltaTime;
            //Debug.Log("Remaining cooldown: " + branchCounter);
            return;
        }       

        //Branch sweep at ground level, jump or back off to avoid
        var spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject rootSweepObject = Instantiate(rootSweep, spawnPos, Quaternion.identity);
        branchCounter = sweepCD;
        Debug.Log("BRANCH SWEEP!");
    } */
    
}