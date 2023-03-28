using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGolemBoss : MonoBehaviour
{
    public bool dead = false;
    public GameObject deathParticles;

    private Animator animator;
    private Rigidbody[] boneRigidbodies;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        Transform armature = transform.Find("Armature");
        boneRigidbodies = armature.GetComponentsInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
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

        GameObject particles = Instantiate(deathParticles, heart.position, Quaternion.LookRotation(-heart.up));
        Destroy(particles, 10f);
        Destroy(transform.Find("Tendrils").gameObject);
        Destroy(heart.gameObject);
        Destroy(gameObject, 10f);

        dead = false;
    }
}
