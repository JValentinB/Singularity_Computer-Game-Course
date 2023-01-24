using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class WindUpstream : MonoBehaviour
{
    private float prevGravityStrength;
    //Function will be called on leaving collider range
    private void OnTriggerEnter(Collider col)
    {
        var damagbleObjectToShift = col.gameObject.GetComponent<Damageable>();

        if (damagbleObjectToShift)
        {
            prevGravityStrength = damagbleObjectToShift.gravityStrength;
            var rg = col.gameObject.GetComponent<Rigidbody>();
            damagbleObjectToShift.gravityStrength = 5f;
            damagbleObjectToShift.GetComponent<Character>().jumpForce = 3050f;
            rg.velocity = new Vector3(rg.velocity.x, -10f ,rg.velocity.z); 
        }
    }

    private void OnTriggerExit(Collider col)
    {
        var damagbleObjectToShift = col.gameObject.GetComponent<Damageable>();
        
        if (damagbleObjectToShift)
        {
            damagbleObjectToShift.gravityStrength = prevGravityStrength;
            damagbleObjectToShift.GetComponent<Character>().jumpForce = 1050f;
        }

    }
}
