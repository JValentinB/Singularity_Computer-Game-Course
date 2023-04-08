using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTreeBoss : MonoBehaviour
{
    [Header("Time Between Punches")]
    public float timeBetweenPunches = 5f;
    public AnimationCurve curve1 = new AnimationCurve();
    public AnimationCurve curve2 = new AnimationCurve();
    public AnimationClip clip;
    [Header("Explosion")]
    public GameObject explosionParticles;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public Vector3 explosionPositionMiddle;
    public Vector3 explosionPositionLeft;
    public Vector3 explosionPositionRight;

    enum PunchDirection { Left, Right, Middle, None };
    private PunchDirection punchDirection;

    private TreeBoss treeBoss;
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
        treeBoss = GetComponent<TreeBoss>();
        animator = GetComponent<Animator>();
        // get rigcontrol script from child
        rigControl = transform.Find("Rig 1").GetComponent<RigControlTreeBoss>();
        sounds = GetComponent<SoundsTreeBoss>();
    }

    // Update is called once per frame
    void Update()
    {   
        if(transform.GetComponent<TreeBoss>().freeze) return;

        if(treeBoss.stunned){
            if(!animator.GetBool("Stunned"))
                setWeightWithCurve(treeBoss.stunnedTime, curve2);
            animator.SetBool("Stunned", true);
            return;
        }
        else{
            animator.SetBool("Stunned", false);
        } 

        if (player != null)
            punchIfClose();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !transform.GetComponent<TreeBoss>().freeze)
        {
            player = other.gameObject.transform;
            sounds.groaningSoundRandom();

            if (punchTimerCoroutine != null)
                StopCoroutine(punchTimerCoroutine);
            punchTimerCoroutine = StartCoroutine(punchTimer(6f));
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
        setPunchDirection();

        float time = punchDirection == PunchDirection.None ? 2 : timeBetweenPunches;
        if (punchTimerCoroutine != null)
            StopCoroutine(punchTimerCoroutine);
        punchTimerCoroutine = StartCoroutine(punchTimer(time));

        yield return new WaitForSeconds(clip.length);
        animator.SetBool("PunchMiddle", false);
        animator.SetBool("PunchLeft", false);
        animator.SetBool("PunchRight", false);
    }

    IEnumerator punchTimer(float time)
    {
        punchTimeOut = true;
        yield return new WaitForSeconds(time);
        punchTimeOut = false;
    }

    void setPunchDirection()
    {
        PunchZone punchZoneMiddle = transform.parent.Find("PunchZones").Find("PunchZoneMiddle").GetComponent<PunchZone>();
        if (punchZoneMiddle.playerInZone)
        {
            punchDirection = PunchDirection.Middle;
            animator.SetBool("PunchMiddle", true);
            return;
        }
        PunchZone punchZoneLeft = transform.parent.Find("PunchZones").Find("PunchZoneLeft").GetComponent<PunchZone>();
        if (punchZoneLeft.playerInZone)
        {
            punchDirection = PunchDirection.Left;
            animator.SetBool("PunchLeft", true);
            return;
        }
        PunchZone punchZoneRight = transform.parent.Find("PunchZones").Find("PunchZoneRight").GetComponent<PunchZone>();
        if (punchZoneRight.playerInZone)
        {
            punchDirection = PunchDirection.Right;
            animator.SetBool("PunchRight", true);
            return;
        }
        punchDirection = PunchDirection.None;
    }

    void Explode()
    {
        Vector3 explosionPosition = punchDirection == PunchDirection.Left ? explosionPositionLeft :
                                        punchDirection == PunchDirection.Right ? explosionPositionRight : explosionPositionMiddle;
        Collider[] colliders = Physics.OverlapSphere(transform.position + explosionPosition, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {   
                Vector3 direction = (rb.transform.position - transform.position).normalized;
                rb.AddForce((Vector3.up + direction) * explosionForce);
                // rb.AddExplosionForce(explosionForce, transform.position + explosionPosition, explosionRadius);
            }
        }

        GameObject explosion = Instantiate(explosionParticles, transform.position + explosionPosition, Quaternion.Euler(-90, 0, 0));
        Destroy(explosion, 5f);
    }

    public void setWeight(float clipLength)
    {
        if (weightCoroutine != null)
        {
            StopCoroutine(weightCoroutine);
        }
        weightCoroutine = StartCoroutine(rigControl.weightCurve(curve1, clipLength));
    }

    void setWeightWithCurve(float clipLength, AnimationCurve curve)
    {
        if (weightCoroutine != null)
        {
            StopCoroutine(weightCoroutine);
        }
        weightCoroutine = StartCoroutine(rigControl.weightCurve(curve, clipLength));
    }
}
