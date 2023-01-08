using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum bulletMode{
    Red,
    Green,
    Blue
}

public class Damageable : MonoBehaviour
{
    public int currentHealth, maxHealth, direction;
    public bool shift = false;
    public float gravityStrength = 18f;
    public Quaternion targetRotation;
    public Vector3 targetDirection, gravitationalDirection;
    public Animator animator;
    public Rigidbody rigidbody;

    
    public void ApplyDamage(int damage){
        currentHealth -= damage;
    }

    public void ApplyGravity()
    {
        rigidbody.AddForce(gravitationalDirection * gravityStrength, ForceMode.Acceleration);
    }

    public void ShiftGravity(Vector3 shiftDirection){
        if(shiftDirection != gravitationalDirection){
            targetDirection = shiftDirection;
            shift = true;
            gravitationalDirection = shiftDirection;
        }
    }

    public void RotateGravity(){
        if(shift){
            if(targetDirection == Vector3.down) targetRotation = Quaternion.LookRotation(Vector3.right * direction, gravitationalDirection * (-1));
            else if(targetDirection == Vector3.up) targetRotation = Quaternion.LookRotation(Vector3.left * direction, gravitationalDirection * (-1));
            else if(targetDirection == Vector3.right) targetRotation = Quaternion.LookRotation(Vector3.up * direction, gravitationalDirection * (-1));
            else if(targetDirection == Vector3.left) targetRotation = Quaternion.LookRotation(Vector3.down * direction, gravitationalDirection * (-1));
            
            var rotSpeed = 5f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed);
            if(transform.rotation == targetRotation){
                shift = false;
            }
        }
    }

    public void Wait(float time){
        var counter = 0f;
        while (counter < time) {
            counter += Time.deltaTime;
        }
    }
}
