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

    [Header("Punching")]
    public int punchDamage = 10;
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
    bool isStunned = false;
    bool isDead = false;
    bool secondPhaseStarted = false;

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
        if(treeBoss.freeze || !treeBoss.startFight) return;

        if(treeBoss.dead)
        {
            if (!isDead){
                isDead = true;
                rigControl.setWeight(0);
            }
            return;
        }

        if(treeBoss.secondPhase && !secondPhaseStarted){
            secondPhaseStarted = true;

            punchDamage += 5;
        }
        
        if (treeBoss.stunned)
        {
            if (!isStunned)
                StartCoroutine(stunnedAnimation());
            return;
        }

        if (player != null)
            punchIfClose();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!treeBoss.freeze && other.gameObject.tag == "Player")
        {
            player = other.transform;
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

    // void punchFoeToDeath(){
    //     if (punchDirection == PunchDirection.None)
    //         return;

    //     bool punchedPlayer = punchDirection == PunchDirection.Middle ? 
    // }

    void Explode()
    {
        Vector3 explosionPosition = punchDirection == PunchDirection.Left ? explosionPositionLeft :
                                        punchDirection == PunchDirection.Right ? explosionPositionRight : explosionPositionMiddle;
        float randomSign = (Random.value < 0.5f) ? -1 : 1;
        
        Collider[] colliders = Physics.OverlapSphere(transform.position + explosionPosition, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = punchDirection == PunchDirection.Middle ? new Vector3(randomSign,0.05f,0) : (rb.transform.position - transform.position).normalized;
                rb.AddForce((Vector3.up + direction) * explosionForce);
                // rb.AddExplosionForce(explosionForce, transform.position + explosionPosition, explosionRadius);
            }
            if(hit.gameObject.tag == "Player"){
                hit.gameObject.GetComponent<Player>().ApplyDamage(punchDamage);
            }
        }

        GameObject explosion = Instantiate(explosionParticles, transform.position + explosionPosition, Quaternion.Euler(-90, 0, 0));
        Destroy(explosion, 5f);
    }

    IEnumerator stunnedAnimation(){
        isStunned = true;
        setWeightWithCurve(treeBoss.stunnedTime, curve2);

        animator.SetBool("Startled", true);
        animator.SetBool("Stunned", true);

        yield return new WaitForSeconds(treeBoss.stunnedTime);

        animator.SetBool("Stunned", false);
        isStunned = false;
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

    void toggleAnimatorBool(string name)
    {
        animator.SetBool(name, !animator.GetBool(name));
    }
}
