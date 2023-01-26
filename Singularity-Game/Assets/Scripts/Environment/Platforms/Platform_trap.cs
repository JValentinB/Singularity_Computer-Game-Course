using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_trap : Platform
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = transform.GetComponent<Rigidbody>();
        timer = 0f;

        if (waypoints.Count != 0)
            transform.position = waypoints[0];
            
        //Making sure it doesn't get pushed away by the player if there are no waypoints
        if(waypoints.Count == 0) rb.isKinematic = true;
        else rb.isKinematic = false;
    }
    
    void OnTriggerStay(Collider col){
        if(col.GetComponent<Player>() &&
           anim.GetCurrentAnimatorStateInfo(0).IsName("idle")){
            anim.SetBool("active", true);
        }
    }

    void OnTriggerExit(Collider col){
        if(col.GetComponent<Player>()){
            anim.SetBool("active", false);
        }
    }
}
