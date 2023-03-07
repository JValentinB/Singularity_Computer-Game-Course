using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBender : MonoBehaviour
{
    [Range(0f, 1f)]
    public float bendingAmount = 0.05f;
    [Range(0f, 1f)]
    public float bendingDistance = 1f; // is bending if the actual distance is smaller than bendingDistance * colliderWidth
    [HideInInspector] public float radius = 1f;
    [HideInInspector] public bool isMoving = false;

    private SphereCollider triggerCollider;
    private Rigidbody rb;
    private Vector3 lastPosition;


    void Start()
    {
        SphereCollider[] colliders = GetComponents<SphereCollider>();
        triggerCollider = colliders[0].isTrigger ? colliders[0] : colliders[1];
        radius = triggerCollider.radius;

        lastPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // void Update()
    // {
    //     if (lastPosition != transform.position)
    //     {
    //         isMoving = true;
    //         lastPosition = transform.position;
    //     }
    //     else isMoving = false;
    // }


    void OnTriggerEnter(Collider other)
    {
        LaserBeam beam = other.GetComponent<LaserBeam>();

        if (beam != null)
        {
            if (!beam.benders.Contains(this))
                beam.benders.Add(this);

            beam.beamInteraction = true;
        }
    }
}
