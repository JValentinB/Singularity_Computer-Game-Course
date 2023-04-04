using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{   
    [Header("For all Damageables")]
    public int currentHealth;
    public int maxHealth;
    [HideInInspector] public int direction;
    public bool shift = false;
    public float gravityStrength = 27f;
    [HideInInspector] public Quaternion targetRotation;
    [HideInInspector] public Vector3 targetDirection, gravitationalDirection;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;
    [SerializeField] private HealthBar healthBar;
    public InvManager inventory = new InvManager();

    public void ApplyDamage(int damage){
        currentHealth -= damage;
        StartCoroutine(damageAnimation());
        if(healthBar){
            healthBar.UpdateHealth(currentHealth);
        }
    }

    public void ApplyGravity()
    {
        rb.AddForce(gravitationalDirection * gravityStrength, ForceMode.Acceleration);
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

    IEnumerator damageAnimation(){
        animator.SetTrigger("TakingDamage");
        animator.SetLayerWeight(2, 1);
        yield return new WaitForSeconds(1f);
        animator.ResetTrigger("TakingDamage");
        animator.SetLayerWeight(2, 0);
    }
}
