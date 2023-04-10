using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreeBoss : Enemy
{ 
    [Header("Tree Boss")]
    public List<MovingBossRoom> bossRoomParts;
    public List<GameObject> toggleObjectsAtDeath;
    [HideInInspector] public List<SpikeSpawnZone> spikeSpawnZones = new List<SpikeSpawnZone>();
    public float timeoutAtStart = 7f;
    public float stunnedTime = 10f;
    public bool secondPhase, dead, freeze;

    [Header("Attacks")]
    public float attackInterval = 4f;
    [Range(0,1)]
    public float spikeProbability = 0.75f;
    [Range(0,1)]
    public float rockProbability = 0.5f;
    public GameObject rootSpike, manipulatableProjectile;


    private Player playerScript;
    [HideInInspector] public float distanceToPlayer;
    [HideInInspector] public bool startFight;
    private bool fightStarted = false;

    bool roomPartsMoved = false;
    [HideInInspector] public bool stunned = false;

    private RockSpawner rockSpawner;
    private ObjectSounds objectSounds;

    private Coroutine spikeCoroutine;
    private Coroutine projectileCoroutine;
    private bool stunTimerRunning = false;

    void Start()
    {
        //from Damageable
        animator = GetComponent<Animator>();
        maxHealth = 1500;
        currentHealth = maxHealth;
        direction = 1;
        gravitationalDirection = Vector3.down;
        healthBar.maxHealth = maxHealth;
        healthBar.currentHealth = currentHealth;
        //from Enemy
        playerObject = GameObject.FindWithTag("Player");
        playerScript = playerObject.GetComponent<Player>();

        rockSpawner = transform.parent.Find("BossGround").GetComponentInChildren<RockSpawner>();
        objectSounds = GetComponent<ObjectSounds>();
        freeze = true;
    }

    // Update is called once per frame
    void Update()
    {   
        Debug.Log(freeze);
        if(!startFight || freeze || dead) return;

        if(startFight && !fightStarted){
            fightStarted = true;
            StartCoroutine(startTimeOut());
        }

        if (startFight && currentHealth <= 0f) OnDeath();

        if(stunned && !stunTimerRunning){
            StartCoroutine(stunnedTimer());
            return;
        }
        else if(stunTimerRunning) return;
            
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
            float rand = Random.Range(0f, 1f);
            if (rand < rockProbability)
                rockSpawner.spawnRock(manipulatableProjectile);

            rand = Random.Range(0f, 1f);
            if(rand < spikeProbability)
            {   
                foreach (var spikeSpawnZone in spikeSpawnZones)
                {
                    if (spikeCoroutine != null)
                        StopCoroutine(spikeCoroutine);
                    spikeCoroutine = StartCoroutine(spikeSpawnZone.spawnRootSpikes());
                }
            }
            yield return new WaitForSeconds(attackInterval);
        }
        OnDeath();
    }

    private void SecondPhase()
    {
        if (currentHealth > Mathf.Floor(maxHealth / 2) && !secondPhase) return;

        secondPhase = true;

        // rootSpike.GetComponent<rootSpike>().increaseDamage(5);
        // manipulatableProjectile.GetComponent<m_Projectile>().increaseDamage(5);

        if (!roomPartsMoved)
        {   
            StartCoroutine(objectSounds.PlayForTime("MovingRoom", 1, 5));
            roomPartsMoved = true;
            foreach (var part in bossRoomParts)
            {
                StartCoroutine(part.moveRoom());
            }
        }
    }

    new private void OnDeath()
    {
        startFight = false;
        dead = true;
        //Defeat animation
        //Open up escape path
        GetComponent<Animator>().SetBool("Dead", true);

        foreach(GameObject gameObject in toggleObjectsAtDeath){
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }

    IEnumerator stunnedTimer()
    {   
        stunTimerRunning = true;
        yield return new WaitForSeconds(stunnedTime);
        stunTimerRunning = false;
        stunned = false;
    }

    IEnumerator startTimeOut(){
        while(freeze){
            yield return null;
        }
        /* freeze = true;
        yield return new WaitForSeconds(timeoutAtStart);
        freeze = false; */
        StartCoroutine(ChooseAttack());
    }
}
