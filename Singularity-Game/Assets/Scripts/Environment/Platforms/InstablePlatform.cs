using System.Collections;
using UnityEngine;

public class InstablePlatform : Platform
{
    private Animator animator;
    private bool isShaking = false;
    private bool isBroken = false;

    [Header("Instable")]
    public float shakeDuration = 1f;
    public float breakDuration = 0.5f;
    public float fixDuration = 5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isShaking && !isBroken)
        {
            shakeDuration -= Time.deltaTime;
            if (shakeDuration <= 0f)
            {
                isBroken = true;
                animator.SetBool("isBroken", true);
                isShaking = false;
                animator.SetBool("isShaking", false);
                StartCoroutine(Break());
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody other_rigidbody = other.GetComponent<Rigidbody>();
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            other_rigidbody.velocity += rb.velocity;
            if (!isShaking)
            {
                isShaking = true;
                animator.SetBool("isShaking", true);
            }
        }
    }

    IEnumerator Break()
    {
        animator.SetBool("isBroken", true);
        foreach (Collider collider in GetComponents<Collider>())
        {
            if (!collider.isTrigger)
            {
                collider.enabled = false;
                break;
            }
        }
        yield return new WaitForSeconds(breakDuration);
        StartCoroutine(Fix());
    }

    IEnumerator Fix()
    {
        yield return new WaitForSeconds(fixDuration);
        animator.SetBool("isBroken", false);
    
        foreach (Collider collider in GetComponents<Collider>())
        {
            if (!collider.isTrigger)
            {
                collider.enabled = true;
                break;
            }
        }
        // isShaking = false;
        isBroken = false;
        shakeDuration = 1f;
    }
}
