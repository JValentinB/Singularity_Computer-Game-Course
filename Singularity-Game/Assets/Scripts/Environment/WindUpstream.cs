using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class WindUpstream : MonoBehaviour
{   
    public float jumpForce = 2000f;

    private float prevGravityStrength;
    private float prevJumpForce;
    //Function will be called on leaving collider range
    private void OnTriggerEnter(Collider col)
    {
        var damageableObjectToShift = col.gameObject.GetComponent<Damageable>();

        if (damageableObjectToShift)
        {
            prevGravityStrength = damageableObjectToShift.gravityStrength;
            prevJumpForce = damageableObjectToShift.GetComponent<Character>().jumpForce;
            var rb = col.gameObject.GetComponent<Rigidbody>();
            damageableObjectToShift.gravityStrength = 5f;
            damageableObjectToShift.GetComponent<Character>().jumpForce = jumpForce;
            rb.velocity = new Vector3(rb.velocity.x, -10f ,rb.velocity.z); 
        }
    }

    private void OnTriggerExit(Collider col)
    {
        var damagbleObjectToShift = col.gameObject.GetComponent<Damageable>();
        
        if (damagbleObjectToShift)
        {
            damagbleObjectToShift.gravityStrength = prevGravityStrength;
            damagbleObjectToShift.GetComponent<Character>().jumpForce = prevJumpForce;
        }

    }
}
