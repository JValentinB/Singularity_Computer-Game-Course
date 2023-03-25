using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Character : Damageable
{
    //FIXME
    [Header("For all Characters")]
    public int jumpNumber;
    public float currentSpeed, walkSpeed, runSpeed, sprintSpeed, mass, jumpForce, critMod;
    public double critChance;

    [HideInInspector]public int jumpsRemaining;
    [HideInInspector]public bool isGrounded;

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

    public IEnumerator playAnimationForTime(string animationName, float time){
        animator.SetBool(animationName, true);
        yield return new WaitForSeconds(time);
        animator.SetBool(animationName, false);
    }
}
