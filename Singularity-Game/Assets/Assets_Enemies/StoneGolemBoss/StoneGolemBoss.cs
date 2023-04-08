using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class StoneGolemBoss : MonoBehaviour
{
    public float healthPoints = 100000f;
    private float currentHealthPoints;
    public bool dead = false;

    [Header("Black Hole")]
    public float blackHoleForce = 1f;
    public Transform platforms;

    [Header("Stampede Explosion")]
    public int stampedeDamage = 10;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public Vector3 explosionDirection;

    [Header("LaserAttack")]
    public LaserEmitter laserEmitter;
    public float laserWidth = 1f;
    public float laserMaxAngle = -110f;
    public float laserAttackTime = 8f;

    [Header("Particles")]
    public GameObject stampedeParticles;
    public GameObject heartDamageParticles;
    public GameObject heartExplosionParticles;
    public GameObject deathParticles;
    public GameObject blackHoleParticles;

    [HideInInspector] public bool bossFightStarted = false;
    private int fightPhase = 0;
    private LaserBeam laser;

    private Animator animator;
    private Rigidbody[] boneRigidbodies;
    private ObjectSounds objectSounds;

    Coroutine blackHoleCoroutine;
    bool blackHoleActive = false;
    Coroutine laserAttackCoroutine;
    bool laserAttackActive = false;
    bool phaseTimerActive = false;

    bool damageParticlesActive = false;
    bool protectHeartActive = false;

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
        // if R is pressed. Delete Before Release!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnDeath();
        }

        BossFightPhases();

        if (bossFightStarted && !phaseTimerActive)
        {
            StartCoroutine(PhaseTimer());
        }
    }

    public void ApplyDamage(float damage, LaserBeam laserBeam = null)
    {
        if (!bossFightStarted) return;

        if (laserBeam != null)
        {
            laser = laserBeam;
        }
        if (!damageParticlesActive)
            StartCoroutine(damageParticles());
        if (!protectHeartActive && !blackHoleActive)
            StartCoroutine(protectHeart());

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
        if (!bossFightStarted) return;

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
        Destroy(gameObject, 6f);
    }

    private void BossFightPhases()
    {
        if (!bossFightStarted) return;

        if (fightPhase == 0 && currentHealthPoints <= healthPoints * 0.833f && blackHoleCoroutine == null)
        {
            // blackHoleCoroutine = StartCoroutine(blackHole());
        }

        if (fightPhase == 0 && currentHealthPoints <= healthPoints * 0.666f)
        {
            fightPhase = 1;
            DestroyActiveBenders();
        }


    }

    IEnumerator PhaseTimer()
    {
        phaseTimerActive = true;
        yield return new WaitForSeconds(20f);

        if (!laserAttackActive)
            laserAttackCoroutine = StartCoroutine(LaserAttack());

        while (!dead)
        {
            yield return new WaitForSeconds(60f);

            if (!laserAttackActive)
                laserAttackCoroutine = StartCoroutine(LaserAttack());
        }

        phaseTimerActive = false;
    }

    // Particles and Animations _______________________________________________
    IEnumerator damageParticles()
    {
        damageParticlesActive = true;
        Transform heart = transform.Find("Heart");
        GameObject damageParticles = Instantiate(heartDamageParticles, heart.position, Quaternion.LookRotation(-heart.up));

        yield return new WaitForSeconds(1.5f);
        damageParticles.GetComponent<ParticleSystem>().Stop();
        Destroy(damageParticles, 1f);
        damageParticlesActive = false;
    }

    IEnumerator protectHeart()
    {
        protectHeartActive = true;

        float time = 0f;
        while (time < 1.5f)
        {
            time += Time.deltaTime;
            animator.SetLayerWeight(3, Mathf.Lerp(0f, 1f, time / 1.5f));
            yield return null;
        }

        while (true)
        {
            if (damageParticlesActive && !blackHoleActive)
                yield return new WaitForSeconds(2f);
            else break;
        }

        time = 0f;
        while (time < 1.5f)
        {
            time += Time.deltaTime;
            animator.SetLayerWeight(3, Mathf.Lerp(1f, 0f, time / 1.5f));
            yield return null;
        }

        protectHeartActive = false;
    }

    // ________________________________________________________________________

    // Skills _________________________________________________________________ 
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

    IEnumerator LaserAttack()
    {
        laserAttackActive = true;

        LaserBeam laserBeam = laserEmitter.transform.parent.GetComponentInChildren<LaserBeam>();

        laserEmitter.startEmitting(false);
        StartCoroutine(objectSounds.fadeInOut("LaserShooting", 0.7f, 1));
        StartCoroutine(objectSounds.risePitch("LaserShooting", 0.7f, 1));

        float time = 0f;
        while (time < 1f)
        {
            laserBeam.laserWidth = Mathf.Lerp(0f, 1, time);
            time += Time.deltaTime;
            yield return null;
        }
        laserBeam.laserWidth = 1f;

        yield return new WaitForSeconds(1f);

        blackHoleCoroutine = StartCoroutine(blackHole());

        time = 0f;
        while (time < laserAttackTime)
        {
            time += Time.deltaTime;
            float angle = Mathf.Lerp(0, laserMaxAngle, time / laserAttackTime);
            laserEmitter.rotateLaserEmitter(angle);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        time = 0f;
        while (time < 8f)
        {
            time += Time.deltaTime;
            float angle = Mathf.Lerp(laserMaxAngle, 0, time / laserAttackTime);
            laserEmitter.rotateLaserEmitter(angle);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        time = 0f;
        while (time < 1f)
        {
            laserBeam.laserWidth = Mathf.Lerp(1, 0, time);
            time += Time.deltaTime;
            yield return null;
        }
        laserBeam.laserWidth = 0f;
        laserEmitter.stopEmitting(false);

        laserAttackActive = false;
    }

    IEnumerator blackHole()
    {
        blackHoleActive = true;
        // Get Left Arm Rig from Rigbuilder
        Rig leftArmRig = GetComponent<RigBuilder>().layers.Find(layer => layer.name == "LeftArmRig").rig;
        float time = 0f;
        while (time < 1f)
        {
            leftArmRig.weight = Mathf.Lerp(0f, 1f, time);
            time += Time.deltaTime;
            yield return null;
        }

        // Create the black hole
        Transform target = leftArmRig.transform.Find("LeftArmTarget");
        GameObject particles = Instantiate(blackHoleParticles, Vector3.Scale(target.position, new Vector3(1, 1, 0)) + new Vector3(0, 0, -1), Quaternion.identity);
        Destroy(particles, 10f);

        Rigidbody player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        time = 0f;
        while (time < 8f)
        {
            time += Time.deltaTime;
            // pull the player towards the black hole
            player.AddForce((target.position - player.position) * 50f * blackHoleForce);

            if (laser != null)
            {
                List<LaserBender> benders = new List<LaserBender>(laser.benders);
                foreach (LaserBender bender in benders)
                {
                    bender.GetComponent<Rigidbody>().AddForce((target.position - bender.transform.position) * 1000f * blackHoleForce);
                }
            }
            List<Rigidbody> platformsRigidbodies = new List<Rigidbody>(platforms.GetComponentsInChildren<Rigidbody>());
            foreach (Rigidbody rb in platformsRigidbodies)
            {
                rb.AddForce((target.position - rb.transform.position) * 20000f * blackHoleForce);
            }

            yield return null;
        }

        Vector3 scale = particles.transform.localScale;
        float volume = particles.GetComponent<AudioSource>().volume;
        particles.GetComponent<ParticleSystem>().Stop();
        time = 0f;
        while (time < 1f)
        {
            Debug.Log(leftArmRig.weight);
            leftArmRig.weight = Mathf.Lerp(1f, 0f, time);
            particles.GetComponent<AudioSource>().volume = Mathf.Lerp(volume, 0f, time);
            particles.transform.localScale = Vector3.Lerp(scale, Vector3.zero, time);

            time += Time.deltaTime;
            yield return null;
        }
        blackHoleActive = false;
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

    // _________________________________________________________________ Skills
}
