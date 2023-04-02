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
        var damageable = col.gameObject.GetComponent<Damageable>();

        if (damageable)
        {
            prevGravityStrength = damageable.gravityStrength;
            prevJumpForce = damageable.GetComponent<Character>().jumpForce;
            var rb = col.gameObject.GetComponent<Rigidbody>();
            damageable.gravityStrength = 5f;
            damageable.GetComponent<Character>().jumpForce = jumpForce;


            Vector3 direction = damageable.targetDirection;
            float axisSpeed = Mathf.Abs(direction.x) == 0 ? rb.velocity.y : rb.velocity.x;
            float axisDirection = Mathf.Abs(direction.x) == 0 ? direction.y : direction.x;

            bool highSpeed = axisDirection > 0 ? axisSpeed > 35 : axisSpeed < -35;
            if (highSpeed)
            {
                Vector3 InverseDirection = new Vector3(Mathf.Abs(direction.x) == 1 ? 0.1f : 1,
                                                   Mathf.Abs(direction.y) == 1 ? 0.1f : 1,
                                                   Mathf.Abs(direction.z) == 1 ? 0.1f : 1);
                rb.velocity = Vector3.Scale(rb.velocity, InverseDirection); // + direction * 10f;
                                                                            // new Vector3(rb.velocity.x, -10f ,rb.velocity.z); 
            }

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
