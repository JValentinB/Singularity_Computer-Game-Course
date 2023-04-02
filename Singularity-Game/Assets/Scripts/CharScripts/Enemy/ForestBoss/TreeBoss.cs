using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBoss : Enemy
{
    private float rangeClose, rangeFar, sideRadiusTB, sideRadiusLR, spikeCounter, projectileCounter,
    nextSpike, projSpawnRadius;
    private bool secondPhase;
    [SerializeField] private float spikeCD, projectileCD, spikePause;
    [SerializeField] private int remainingSpikes;
    [SerializeField] private GameObject rootSpike, manipulatableProjectile,
    bottomSide, topSide, rightSide, leftSide;
    [SerializeField] private Player playerScript; 
    [SerializeField] public float disToPlayer;
    [SerializeField] private Vector3 bottomSideMidPos, topSideMidPos, rightSideMidPos, 
    leftSideMidPos;
    
    void Start()
    {
        //from Damageable
        maxHealth = 1000;
        currentHealth = maxHealth;
        direction = 1;
        gravitationalDirection = Vector3.down;
        //from Character
        jumpNumber = 0;
        currentSpeed = 2.5f;
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

        spikeCD = 3f;
        projectileCD = 2f;

        playerScript = playerObject.GetComponent<Player>();
        bottomSideMidPos = bottomSide.transform.position;
        topSideMidPos = topSide.transform.position;
        rightSideMidPos = rightSide.transform.position;
        leftSideMidPos = leftSide.transform.position;
        sideRadiusTB = 18f;
        sideRadiusLR = 25f;
        projSpawnRadius = 13f;
    }

    // Update is called once per frame
    void Update()
    {
        ChooseAttack();
        SecondPhase();
    }

    private float DistanceToPlayer(){
        disToPlayer = Mathf.Abs(transform.position.x - playerObject.transform.position.x);
        return disToPlayer;
    }

    private void ChooseAttack(){
        var rand = Random.Range(0f, 1f);
        if(rand < 0.65f)         ThrowProjectile();
        else                    RootSpikes();
    }

    private void SecondPhase(){
        if(currentHealth > Mathf.Floor(maxHealth/2) && !secondPhase) return;
        projectileCD = 3f;
        var rootBridges = GameObject.FindGameObjectsWithTag("RootBridge");
        foreach(var bridge in rootBridges){
            bridge.GetComponent<RootBridge>().destroyBridge = true;
        }
        secondPhase = true;
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
        remainingSpikes = 15;
        Debug.Log("ROOT SPIKES!");
    }

    private void SpawnSpike(){
        if(spikePause > 0f){
            spikePause -= Time.deltaTime;
            return;
        }

        var spawnPos = transform.position;
        if(playerScript.gravitationalDirection == Vector3.right){
            spawnPos = new Vector3(rightSideMidPos.x, rightSideMidPos.y + Random.Range(-sideRadiusLR, sideRadiusLR), rightSideMidPos.z);
            GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.Euler(0f, 0f, 90f));
        } else if(playerScript.gravitationalDirection == Vector3.left){
            spawnPos = new Vector3(leftSideMidPos.x, leftSideMidPos.y + Random.Range(-sideRadiusLR, sideRadiusLR), leftSideMidPos.z);
            GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.Euler(0f, 0f, -90f));
        } else if(playerScript.gravitationalDirection == Vector3.up){
            spawnPos = new Vector3(topSideMidPos.x + Random.Range(-sideRadiusTB, sideRadiusTB), topSideMidPos.y, topSideMidPos.z);
            GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.Euler(0f, 0f, 180f));
        } else {
            spawnPos = new Vector3(bottomSideMidPos.x + Random.Range(-sideRadiusTB, sideRadiusTB), bottomSideMidPos.y, bottomSideMidPos.z);
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
        var spawnPosX = transform.position.x - Random.Range(-projSpawnRadius, projSpawnRadius);
        while(playerObject.transform.position.x - 5f <= spawnPosX && spawnPosX <= playerObject.transform.position.x + 5f){
            spawnPosX = transform.position.x - Random.Range(-projSpawnRadius, projSpawnRadius);
        }
        
        var spawnPos = new Vector3(transform.position.x - Random.Range(-projSpawnRadius, projSpawnRadius), transform.position.y, 0f);
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
