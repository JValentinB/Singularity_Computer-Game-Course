using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcornBranchTarget : MonoBehaviour
{   
    public float smoothTime = 0.3f;
    public Vector3 startPosLocal;

    private Vector3 startPos;
    private Vector3 velocity = Vector3.zero;
    private Rigidbody rb;

    private bool collided;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startPosLocal = transform.localPosition;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        float distance = (transform.position - startPos).magnitude;
        if(distance > 0.1f){
            Debug.Log(distance);
            transform.position = Vector3.SmoothDamp(transform.position, startPos, ref velocity, smoothTime);
        }
        
        if((transform.position - startPos).magnitude > 1f)
        {
            rb.velocity = Vector3.zero;
            // collided = false;
        }
    }

    // void OnCollisionEnter(Collision collision)
    // {   
    //     collided = true;
    // }
}
