using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTreeBoss : MonoBehaviour
{
    [Header("Time Between Punches")]
    public float timeBetweenPunches = 5f;
    public AnimationCurve curve = new AnimationCurve();
    public AnimationClip clip;
    [Header("Explosion")]
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public Vector3 explosionPosition;

    private Animator animator;
    private SoundsTreeBoss sounds;
    private Transform player;
    private RigControlTreeBoss rigControl;
    private Coroutine weightCoroutine;

    bool punchTimeOut = false;
    Coroutine punchTimerCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        // get rigcontrol script from child
        rigControl = transform.Find("Rig 1").GetComponent<RigControlTreeBoss>();
        sounds = GetComponent<SoundsTreeBoss>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
            punchIfClose();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject.transform;
            sounds.groaningSoundRandom();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = null;
        }
    }

    void punchIfClose()
    {
        if (punchTimeOut)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < 9)
        {
            StartCoroutine(punch());
        }
    }

    IEnumerator punch()
    {
        animator.SetBool("Punch", true);
        if (punchTimerCoroutine != null)
        {
            StopCoroutine(punchTimerCoroutine);
        }
        punchTimerCoroutine = StartCoroutine(punchTimer());
        yield return new WaitForSeconds(clip.length);
        animator.SetBool("Punch", false);
    }

    IEnumerator punchTimer()
    {
        punchTimeOut = true;
        yield return new WaitForSeconds(timeBetweenPunches);
        punchTimeOut = false;
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + explosionPosition, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {   
                rb.AddForce(Vector3.up * explosionForce);
                // rb.AddExplosionForce(explosionForce, transform.position + explosionPosition, explosionRadius);
            }
        }
    }

    public void setWeight(float clipLength)
    {
        if (weightCoroutine != null)
        {
            StopCoroutine(weightCoroutine);
        }
        weightCoroutine = StartCoroutine(rigControl.weightCurve(curve, clipLength));
    }
}
