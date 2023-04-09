using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class WindUpstream : MonoBehaviour
{
    public float jumpForce = 2000f;
    public float gravity = 5f;
    public GameObject EnteringParticles;
    public GameObject ExitingParticles;

    private float prevGravityStrength;
    private float prevJumpForce;
    //Function will be called on leaving collider range
    private void OnTriggerEnter(Collider col)
    {
        var damageable = col.gameObject.GetComponent<Damageable>();

        if (damageable)
        {
            damageable.inWeightlessFields++;

            prevGravityStrength = damageable.gravityStrength;
            prevJumpForce = damageable.GetComponent<Character>().jumpForce;
            var rb = col.gameObject.GetComponent<Rigidbody>();
            damageable.gravityStrength = gravity;
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

            if (col.gameObject.tag == "Player")
            {
                GameObject particles = GameObject.Instantiate(EnteringParticles, col.transform.position, Quaternion.identity);
                var shape = particles.GetComponent<ParticleSystem>().shape;
                var character = col.transform.Find("Character");
                shape.skinnedMeshRenderer = character.GetComponent<SkinnedMeshRenderer>();
                Destroy(particles, 3f);
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        var damagbleObjectToShift = col.GetComponent<Damageable>();

        if (damagbleObjectToShift)
        {
            damagbleObjectToShift.inWeightlessFields--;

            // Debug.Log(damagbleObjectToShift.inWeightlessFields);
            if (damagbleObjectToShift.inWeightlessFields > 0)
                return;

            damagbleObjectToShift.gravityStrength = damagbleObjectToShift.standaradGravityStrength;

            Character character = col.GetComponent<Character>();
            if (character)
                character.jumpForce = character.standardJumpForce;

            if (col.gameObject.tag == "Player")
            {
                GameObject particles = GameObject.Instantiate(ExitingParticles, col.transform.position, Quaternion.identity);
                var shape = particles.GetComponent<ParticleSystem>().shape;
                var charac = col.transform.Find("Character");
                shape.skinnedMeshRenderer = charac.GetComponent<SkinnedMeshRenderer>();
                Destroy(particles, 3f);
            }
        }
    }
}
