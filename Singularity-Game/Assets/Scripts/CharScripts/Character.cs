using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Character : Damageable
{
    //FIXME
    public int jumpNumber;
    public float currentSpeed, walkSpeed, runSpeed, sprintSpeed, mass, jumpForce, critMod;
    public double critChance;

    public void Jump(){
        if (animator.GetBool("Jumping") && rb.velocity.y < -0.2f) {
            animator.SetBool("Falling", true);
            animator.SetBool("Jumping", false);
        }
        if (Input.GetKeyDown(KeyCode.Space) && jumpNumber > 0)
        {
            animator.SetTrigger("Jumping");
            //animator.SetBool("Falling", true);
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce((-1) * gravitationalDirection * jumpForce, ForceMode.Impulse);
            jumpNumber--;
        }
    }

    public void ChangeLineOfSight(){
        if(!shift){
            if(gravitationalDirection == Vector3.right) transform.rotation = Quaternion.LookRotation(Vector3.up * direction, gravitationalDirection * (-1));
            else if(gravitationalDirection == Vector3.left) transform.rotation = Quaternion.LookRotation(Vector3.down * direction, gravitationalDirection * (-1));
            else transform.rotation = Quaternion.LookRotation(Vector3.right * direction, gravitationalDirection * (-1));
        }
    }

    public float Crit()
    {
        var rnd = Random.value;
        if(rnd <= critChance){
            return critMod;
        }
        return 1;
    }
}
