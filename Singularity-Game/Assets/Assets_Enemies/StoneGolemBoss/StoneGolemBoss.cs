using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class StoneGolemBoss : MonoBehaviour
{
    public float healthPoints = 100000f;
    private float currentHealthPoints;
    public bool dead = false;

    [Header("Stampede Explosion")]
    public int stampedeDamage = 10;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public Vector3 explosionDirection;

    [Header("Particles")]
    public GameObject stampedeParticles;
    public GameObject heartExplosionParticles;
    public GameObject deathParticles;
    public GameObject blackHoleParticles;

    private int fightPhase = 0;
    private LaserBeam laser;

    private Animator animator;
    private Rigidbody[] boneRigidbodies;
    private ObjectSounds objectSounds;

    Coroutine blackHoleCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        currentHealthPoints = healthPoints;

        animator = GetComponent<Animator>();
        objectSounds = GetComponent<ObjectSounds>();

        Transform armature = transform.Find("Armature");
        boneRigidbodies = armature.GetComponentsInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // if R is pressed
        if (dead || Input.GetKeyDown(KeyCode.R))
        {
            OnDeath();
        }

        BossFightPhases();
    }

    public void ApplyDamage(float damage, LaserBeam laserBeam = null)
    {
        if (laserBeam != null)
        {
            laser = laserBeam;
        }
        currentHealthPoints -= damage;

        if (currentHealthPoints % 5000 == 0)
            Debug.Log("Boss health: " + currentHealthPoints);

        if (currentHealthPoints <= 0)
        {
            OnDeath();
        }
    }

    void OnDeath()
    {
        animator.enabled = false;
        foreach (Rigidbody rb in boneRigidbodies)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
        Transform heart = transform.Find("Heart");

        objectSounds.Play("RockExplosion");

        GameObject particles1 = Instantiate(deathParticles, heart.position, Quaternion.LookRotation(-heart.up));
        GameObject particles2 = Instantiate(heartExplosionParticles, heart.position, Quaternion.LookRotation(-heart.up));
        Destroy(particles1, 8f);
        Destroy(particles2, 8f);

        Destroy(transform.Find("Tendrils").gameObject);
        Destroy(heart.gameObject);
        Destroy(gameObject, 10f);

        dead = false;
    }

    private void BossFightPhases()
    {
        if (fightPhase == 0 && currentHealthPoints <= healthPoints * 0.833f && blackHoleCoroutine == null)
        {
            blackHoleCoroutine = StartCoroutine(blackHole());
        }

        if (fightPhase == 0 && currentHealthPoints <= healthPoints * 0.666f)
        {
            fightPhase = 1;
            DestroyActiveBenders();
        }


    }

    void StampedeExplosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(explosionDirection * explosionForce);
            }

            Player player = hit.GetComponent<Player>();
            if (player != null)
            {
                player.ApplyDamage(stampedeDamage);
            }
        }
        objectSounds.Play("GroundCrash");

        Vector3 position = new Vector3(-526.3f, -274.94f, 0);
        GameObject particles = Instantiate(stampedeParticles, position, Quaternion.LookRotation(Vector3.up));
        particles.GetComponent<ParticleSystem>().Play();
        Destroy(particles, 6f);
    }

    IEnumerator blackHole()
    {
        // Rise the weight of the left arm rig
        Rig leftArmRig = transform.Find("LeftArmRig").GetComponent<Rig>();
        float time = 0f;
        while (time < 1f)
        {
            leftArmRig.weight = Mathf.Lerp(0f, 1f, time);
            time += Time.deltaTime;
            yield return null;
        }

        // Create the black hole
        Transform target = leftArmRig.transform.Find("LeftArmTarget");
        GameObject particles = Instantiate(blackHoleParticles, Vector3.Scale(target.position, new Vector3(1,1,0)) + new Vector3(0,0,-1), Quaternion.identity);
        Destroy(particles, 5f);

        time = 0f;
        while (time < 5f)
        {
            time += Time.deltaTime;
            // pull the player towards the black hole
            Rigidbody player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
            player.AddForce((target.position - player.position) * 50f);

            List<LaserBender> benders = new List<LaserBender>(laser.benders);
            foreach (LaserBender bender in benders)
            {
                bender.GetComponent<Rigidbody>().AddForce((target.position - bender.transform.position) * 1000f);
            }

            yield return null;
        }

        time = 0f;
        while (time < 1f)
        {
            leftArmRig.weight = Mathf.Lerp(1f, 0f, time);
            time += Time.deltaTime;
            yield return null;
        }
    }

    void DestroyActiveBenders()
    {
        // Copy the list to avoid errors
        List<LaserBender> benders = new List<LaserBender>(laser.benders);
        foreach (LaserBender bender in benders)
        {
            bender.DestroyBender();
        }
    }
}
