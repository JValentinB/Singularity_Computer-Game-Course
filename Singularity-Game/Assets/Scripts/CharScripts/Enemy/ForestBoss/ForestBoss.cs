using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBoss : Enemy
{
    private float rangeClose, rangeFar;
    [SerializeField] private float slashCD, sweepCD, spikeCD, projectileCD, spikePause;
    [SerializeField] private int remainingSpikes;
    private float branchCounter, spikeCounter, projectileCounter, nextSpike;
    private float[] rootSpikePos = new float[10];
    [SerializeField] private GameObject rootSpike;
    [SerializeField] private GameObject rootSlash;
    [SerializeField] private GameObject rootSweep;
    [SerializeField] private GameObject manipulatableProjectile;

    [SerializeField] public float disToPlayer;
    
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
        rootSpikePos[0] = 10f;
        rootSpikePos[1] = 20f;
        rootSpikePos[2] = 30f;

        slashCD = 3f;
        sweepCD = 2f;
        spikeCD = 2f;
        projectileCD = 5f;
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
        if(DistanceToPlayer() < rangeClose){
            //Randomize all possible close+ range attacks
            if(rand < 0.25f)        BranchSlash();
            else if(rand < 0.5f)    BranchSweep();
            else if(rand < 0.75f)   RootSpikes();
            else                    ThrowProjectile();
        } else if(DistanceToPlayer() < rangeFar){
            //Randomize all possible mid+ range attacks
            if(rand < 0.25f)        ThrowProjectile();
            else if(rand < 0.6f)    BranchSweep();
            else                    RootSpikes();
        } else {
            //Randomize all possible far range attacks
            if(rand < 0.5f)         ThrowProjectile();
            else                    RootSpikes();
        }
    }

    //ALL SPAWN POSITIONS HAVE TO BE SET MANUALLY; SINCE THEY SPAWN RELATIVE TO BOSS POSITION

    //Close range attack (maybe)
    private void BranchSlash(){
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
    }

    //Mid range attack
    private void BranchSweep(){
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
        var spawnPos = new Vector3(playerObject.transform.position.x + Random.Range(-30f, 30f), transform.position.y, transform.position.z);
        GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.identity);
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
        while(spawnPosX - 5f <= playerObject.transform.position.x && playerObject.transform.position.x <= spawnPosX + 5f){
            spawnPosX = transform.position.x - Random.Range(5f, 40f);
        }
        
        var spawnPos = new Vector3(transform.position.x - Random.Range(5f, 40f), transform.position.y, transform.position.z);
        GameObject projectileObject = Instantiate(manipulatableProjectile, spawnPos, Quaternion.identity);
        projectileCounter = projectileCD;
        Debug.Log("PROJECTILE!");
    }
}
