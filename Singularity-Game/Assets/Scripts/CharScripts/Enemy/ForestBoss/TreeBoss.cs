using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBoss : Enemy
{ 
    [Header("Tree Boss")]
    public List<MovingBossRoom> bossRoomParts;
    public List<SpikeSpawnZone> spikeSpawnZones = new List<SpikeSpawnZone>();
    public float stunnedTime = 10f;
    public bool secondPhase, dead, freeze;


    [SerializeField] private float spikeCooldown;
    [SerializeField] private float projectileCD, spikePause, hitPhaseTimer;
    [SerializeField] private int remainingSpikes;
    [SerializeField]
    private GameObject rootSpike, manipulatableProjectile,
    bottomSide, rightSide, leftSide;
    private Player playerScript;
    [HideInInspector] public float distanceToPlayer;
    [HideInInspector] public bool startFight;
    [SerializeField]
    // private Vector3 bottomSideMidPos, rightSideMidPos,
    // leftSideMidPos;

    bool roomPartsMoved = false;
    [HideInInspector] public bool stunned = false;

    private RockSpawner rockSpawner;

    private Coroutine spikeCoroutine;
    private Coroutine projectileCoroutine;
    private bool stunTimerRunning = false;

    void Start()
    {
        //from Damageable
        maxHealth = 1000;
        currentHealth = maxHealth;
        direction = 1;
        gravitationalDirection = Vector3.down;
        healthBar.maxHealth = maxHealth;
        healthBar.currentHealth = currentHealth;
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
        // rangeClose = 10f;
        // rangeFar = 20f;

        remainingSpikes = 15;
        spikeCooldown = 3f;
        projectileCD = 2f;

        playerScript = playerObject.GetComponent<Player>();
        // bottomSideMidPos = bottomSide.transform.position;
        // rightSideMidPos = rightSide.transform.position;
        // leftSideMidPos = leftSide.transform.position;
        // sideRadiusTB = 18f;
        // sideRadiusLR = 25f;
        // projSpawnRadius = 13f;
        hitPhaseTimer = 30f;

        rockSpawner = transform.parent.Find("BossGround").GetComponentInChildren<RockSpawner>();

        StartCoroutine(ChooseAttack());
    }

    // Update is called once per frame
    void Update()
    {
        if(freeze) return;

        if(stunned && !stunTimerRunning)
            StartCoroutine(stunnedTimer());
            
        if (!startFight || stunned) return;

        if (startFight && currentHealth <= 0f) OnDeath();
        ChooseAttack();
        SecondPhase();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            startFight = true;
        }
    }

    private float DistanceToPlayer()
    {
        distanceToPlayer = Mathf.Abs(transform.position.x - playerObject.transform.position.x);
        return distanceToPlayer;
    }

    private IEnumerator ChooseAttack()
    {
        while (!dead)
        {
            if (rockSpawner.playerOnIsland) yield return new WaitForSeconds(1f);

            var rand = Random.Range(0f, 1f);
            if (rand < 0.5f)
            {
                rockSpawner.spawnRock(manipulatableProjectile);
            }
            else
            {   
                foreach (var spikeSpawnZone in spikeSpawnZones)
                {
                    if (spikeCoroutine != null)
                        StopCoroutine(spikeCoroutine);
                    spikeCoroutine = StartCoroutine(spikeSpawnZone.spawnRootSpikes());
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private void SecondPhase()
    {
        if (currentHealth > Mathf.Floor(maxHealth / 2) && !secondPhase) return;

        secondPhase = true;
        projectileCD = 3f;
        // var rootBridges = GameObject.FindGameObjectsWithTag("RootBridge");
        // foreach (var bridge in rootBridges)
        // {
        //     bridge.GetComponent<RootBridge>().destroyBridge = true;
        // }
        if (!roomPartsMoved)
        {   
            roomPartsMoved = true;
            foreach (var part in bossRoomParts)
            {
                StartCoroutine(part.moveRoom());
            }
        }
    }

    private IEnumerator HitPhase()
    {
        yield return new WaitForSeconds(hitPhaseTimer);

        //Animation
        //Collider activation
    }

    new private void OnDeath()
    {
        startFight = false;
        dead = true;
        //Defeat animation
        //Open up escape path
    }

    IEnumerator stunnedTimer()
    {   
        stunTimerRunning = true;
        yield return new WaitForSeconds(stunnedTime);
        stunTimerRunning = false;
        stunned = false;
    }

    //Mid range attack (adjustable)
    // private IEnumerator SpawnSpikes()
    // {

    //     for (int spike = 0; spike < remainingSpikes; spike++)
    //     {
    //         var spawnPos = transform.position;
    //         if (playerScript.gravitationalDirection == Vector3.right)
    //         {
    //             spawnPos = new Vector3(rightSideMidPos.x, rightSideMidPos.y + Random.Range(-sideRadiusLR, sideRadiusLR), rightSideMidPos.z);
    //             GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.Euler(0f, 0f, 90f));
    //             rootSpikeObject.GetComponent<rootSpike>().growingDirection = Vector3.left;
    //         }
    //         else if (playerScript.gravitationalDirection == Vector3.left)
    //         {
    //             spawnPos = new Vector3(leftSideMidPos.x, leftSideMidPos.y + Random.Range(-sideRadiusLR, sideRadiusLR), leftSideMidPos.z);
    //             GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.Euler(0f, 0f, -90f));
    //             rootSpikeObject.GetComponent<rootSpike>().growingDirection = Vector3.right;
    //         }
    //         else
    //         {
    //             spawnPos = new Vector3(bottomSideMidPos.x + Random.Range(-sideRadiusTB, sideRadiusTB), bottomSideMidPos.y, bottomSideMidPos.z);
    //             GameObject rootSpikeObject = Instantiate(rootSpike, spawnPos, Quaternion.identity);
    //             rootSpikeObject.GetComponent<rootSpike>().growingDirection = Vector3.up;
    //         }

    //         yield return new WaitForSeconds(0.1f);
    //     }
    // }

    //Far range attack
    // private void ThrowProjectile()
    // {
    //     if (projectileCounter >= 0f)
    //     {
    //         projectileCounter -= Time.deltaTime;
    //         return;
    //     }

    //     //Projectile shouldn't spawn directly under player
    //     // var spawnPosX = transform.position.x - Random.Range(-projSpawnRadius, projSpawnRadius);
    //     // while (playerObject.transform.position.x - 5f <= spawnPosX && spawnPosX <= playerObject.transform.position.x + 5f)
    //     // {
    //     //     spawnPosX = transform.position.x - Random.Range(-projSpawnRadius, projSpawnRadius);
    //     // }

    //     // var spawnPos = new Vector3(transform.position.x - Random.Range(-projSpawnRadius, projSpawnRadius), transform.position.y, 0f);

    //     rockSpawner.spawnRock(manipulatableProjectile);

    //     projectileCounter = projectileCD;
    // }


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
