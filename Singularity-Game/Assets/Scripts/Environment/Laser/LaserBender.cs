using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBender : MonoBehaviour
{   
    [Header("Time until the bender is moving back to initial position")]
    public float timeUntilReset = 10f;
    [Range(0f, 1f)]
    public float bendingAmount = 0.05f;
    [Range(0f, 1f)]
    public float bendingDistance = 1f; // is bending if the actual distance is smaller than bendingDistance * colliderWidth
    [HideInInspector] public float radius = 1f;
    [HideInInspector] public bool isMoving = false;

    private SphereCollider triggerCollider;
    private Rigidbody rb;
    private LaserBeam laserBeam;

    private Vector3 initialPosition;
    Coroutine timerCoroutine;
    bool timerTicking = false;

    void Start()
    {
        SphereCollider[] colliders = GetComponents<SphereCollider>();
        triggerCollider = colliders[0].isTrigger ? colliders[0] : colliders[1];
        radius = triggerCollider.radius;

        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(!timerTicking && IsMoving())
        {
            timerCoroutine = StartCoroutine(Timer());
        }
    }


    void OnTriggerEnter(Collider other)
    {
        LaserBeam beam = other.GetComponent<LaserBeam>();

        if (beam != null)
        {   
            laserBeam = beam;
            if (!beam.benders.Contains(this))
                beam.benders.Add(this);

            beam.beamInteraction = true;
        }
        else
            laserBeam = null;
    }

    // check if the bender is moving
    bool IsMoving()
    {
        if (rb.velocity.magnitude > 0.01f)
            return true;
        else
            return false;
    }

    IEnumerator Timer()
    {
        timerTicking = true;
        Debug.Log("Timer started");
        yield return new WaitForSeconds(timeUntilReset);
        Debug.Log("Timer ended");
        // move back to initial position with high velocity
        transform.position = initialPosition;
        timerTicking = false;

        if(laserBeam != null)
            laserBeam.benderReset = true;
    }
}
