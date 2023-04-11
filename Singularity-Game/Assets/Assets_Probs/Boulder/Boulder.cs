using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : Damageable
{   
    [Header("Boulder")]
    public GameObject impactParticles;

    private AudioSource audioSource;

    private Vector3 startingPosition;
    private float velocity;

    [HideInInspector] public bool destroyedRoots = false;
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 999999999;
        currentHealth = maxHealth;

        mass = 2500;
        gravitationalDirection = Vector3.down;
        targetDirection = Vector3.down;
        direction = 1;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;


        startingPosition = transform.position;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();
        UpdateCheckpoint();
        velocity = rb.velocity.magnitude;
    }

    public void ResetBoulder(){
        transform.position = startingPosition;
        rb.velocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {  
        if (collision.gameObject.tag != "Player" && velocity > 10 && !destroyedRoots)
        {  
            if(audioSource != null){
                audioSource.volume = velocity / 50;
                audioSource.Play();
            }
            
            if(impactParticles != null){
                Vector3 impactPosition = collision.contacts[0].point;
                Quaternion impactRotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal);
                GameObject impact = Instantiate(impactParticles, impactPosition, impactRotation);

                impact.GetComponent<ParticleSystem>().Play();
                Destroy(impact, 5);
            }
        }
    }

    private void UpdateCheckpoint(){
        if(destroyedRoots) Checkpoint.treeBossEntryOpened = true;
    }
}
