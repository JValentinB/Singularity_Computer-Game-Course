using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Character : Damageable
{
    //FIXME
    public int jumpFactor, jumpNumber;
    public float currentSpeed, walkSpeed, runSpeed, sprintSpeed, mass, jumpForce, critMod;
    public double critChance;

    public void Jump(){
        if (animator.GetBool("Jumping") && animator.GetBool("Falling")) 
            animator.SetBool("Jumping", false);
        if (Input.GetKeyDown(KeyCode.Space) && jumpNumber > 0)
        {
            animator.SetTrigger("Jumping");
            animator.SetBool("Falling", true);
            rigidbody.AddForce(-1 * gravitationalDirection * jumpForce);
            jumpNumber--;
        }
    }

    public void ChangeLineOfSight(Vector3 vecHor, Vector3 vecVer){
        if(!shift){
            if(gravitationalDirection == Vector3.down) transform.rotation = Quaternion.LookRotation(vecHor * direction, gravitationalDirection * (-1));
            else if(gravitationalDirection == Vector3.up) transform.rotation = Quaternion.LookRotation(vecHor * direction , gravitationalDirection * (-1));
            else if(gravitationalDirection == Vector3.right) transform.rotation = Quaternion.LookRotation(vecVer * direction, gravitationalDirection * (-1));
            else if(gravitationalDirection == Vector3.left) transform.rotation = Quaternion.LookRotation(vecVer * direction * (-1), gravitationalDirection * (-1));
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
