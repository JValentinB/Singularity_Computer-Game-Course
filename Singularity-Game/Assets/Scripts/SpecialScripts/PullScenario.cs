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
        // if (pulledOut) PullOut();
        if(rb.velocity != Vector3.zero){
            Debug.Log("pulled out");
            fallingCliff.pullOut();
        }
    }

    // void OnTriggerEnter(Collider col)
    // {
    //     var proj = col.GetComponent<Projectile>();
    //     if (proj)
    //     {
    //         if (proj.mode == 0) pulledOut = true;
    //     }
    // }

    // private void PullOut()
    // {
    //     transform.Translate((transform.right).normalized * Time.deltaTime * 5f);
    // }
}
