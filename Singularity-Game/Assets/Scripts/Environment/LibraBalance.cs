using System.Collections;
using UnityEngine;

public class LibraBalance : MonoBehaviour
{
    public bool spinning = false;
    public float leftLimit = 30f;
    public float rightLimit = 30f;
    public bool freezeX = false;
    public bool freezeY = false;
    public bool freezeZ = false;

    private Vector3 initialPosition;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY; ;
        // if(freezeAllPositions)
        //     rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;; 
        initialPosition = transform.position;
    }

    void Update()
    {
        if(spinning)
            spinningFreely();
        else libra();
    }

    void libra(){
        float xRotation  = transform.localEulerAngles.x;
        if(360 - leftLimit > xRotation && xRotation > rightLimit){
            if(xRotation < 180)
                xRotation = rightLimit;
            else xRotation = 360 - leftLimit;
        }
        transform.localEulerAngles = new Vector3(xRotation, 90,0);
        
        float x = freezeX ? initialPosition.x : transform.position.x; 
        float y = freezeY ? initialPosition.y : transform.position.y; 
        float z = freezeZ ? initialPosition.z : transform.position.z; 
        transform.position = new Vector3(x, y, z);
    }
    void spinningFreely(){
        transform.rotation = Quaternion.Euler(transform.rotation.x, 90, 0);
    }
}
