using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullScenario : MonoBehaviour
{
    public Transform target;

    private FallingCliff fallingCliff;
    private Rigidbody rb;

    private bool pulledOut;

    void Start()
    {
        fallingCliff = target.GetComponent<FallingCliff>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(rb.velocity != Vector3.zero){
            fallingCliff.pullOut();
        }
    }
}
