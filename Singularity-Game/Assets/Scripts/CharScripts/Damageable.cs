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
    [HideInInspector] public float standaradGravityStrength = 27f;
    public float mass;

    [HideInInspector] public Quaternion targetRotation;
    [HideInInspector] public Vector3 targetDirection, gravitationalDirection;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public int inWeightlessFields = 0;

    [SerializeField] public HealthBar healthBar;
    public InvManager inventory = new InvManager();

    private List<Vector3> gravityShifts = new List<Vector3>();
    // private bool inverting = false;

    public void ApplyDamage(int damage){
        currentHealth -= damage;
        Debug.Log(transform.name + " taking Damage");
        if(animator != null)
            StartCoroutine(damageAnimation());
        if(healthBar){
            healthBar.UpdateHealth(currentHealth);
        }
    }

    public void ApplyGravity()
    {
        rb.AddForce(gravitationalDirection * gravityStrength, ForceMode.Acceleration);
    }

    public void ShiftGravity(){
        Vector3 oldDirection = targetDirection;
        if(gravityShifts.Count == 0){
            targetDirection = Vector3.down;
            shift = true;
            gravitationalDirection = Vector3.down;
        }
        else if(gravityShifts[0] != gravitationalDirection){
            targetDirection = gravityShifts[0];
            shift = true;
            gravitationalDirection = gravityShifts[0];
        }

        if(GetComponent<Player>()){
            // if(!inverting)
            //     StartCoroutine(invertPlayerControl(GetComponent<Player>(), oldDirection, targetDirection == Vector3.up ? -1 : 1));

            if(Camera.main.GetComponent<CameraControl>().downDirection != targetDirection)
                Camera.main.GetComponent<CameraControl>().turnCameraWithShift(oldDirection, targetDirection, 0.75f);
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

    public void AddGravityShift(Vector3 shiftDirection){
        if(!gravityShifts.Contains(shiftDirection))
            gravityShifts.Add(shiftDirection);
    }

    public void RemoveGravityShift(Vector3 shiftDirection){
        if(gravityShifts.Contains(shiftDirection))
            gravityShifts.Remove(shiftDirection);
    }

    public Vector3 getGravityShift(){
        if(gravityShifts.Count == 0) return Vector3.down;
        return gravityShifts[0];
    }

    public int getGravityShiftCount(){
        return gravityShifts.Count;
    }

    public bool isFirstShifter(Vector3 shifterDirection){
        return shifterDirection == gravityShifts[0];
    }

    public void Wait(float time){
        var counter = 0f;
        while (counter < time) {
            counter += Time.deltaTime;
        }
    }

    IEnumerator damageAnimation(){
        Debug.Log(transform.name);
        animator.SetTrigger("TakingDamage");
        animator.SetLayerWeight(2, 1);
        yield return new WaitForSeconds(1f);
        animator.ResetTrigger("TakingDamage");
        animator.SetLayerWeight(2, 0);
    }
}
