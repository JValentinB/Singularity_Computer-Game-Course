using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGolemBoss : MonoBehaviour
{
    public bool dead = false;

    [Header("Stampede Explosion")]
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public Vector3 explosionDirection;

    [Header("Particles")]
    public GameObject stampedeParticles;
    public GameObject heartExplosionParticles;
    public GameObject deathParticles;

    private Animator animator;
    private Rigidbody[] boneRigidbodies;
    private ObjectSounds objectSounds;

    // Start is called before the first frame update
    void Start()
    {
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
        }
        objectSounds.Play("GroundCrash");

        Vector3 position = new  Vector3(-526.3f,-274.94f,0);
        GameObject particles = Instantiate(stampedeParticles, position, Quaternion.LookRotation(Vector3.up));
        particles.GetComponent<ParticleSystem>().Play();
        Destroy(particles, 6f);
    }


}
