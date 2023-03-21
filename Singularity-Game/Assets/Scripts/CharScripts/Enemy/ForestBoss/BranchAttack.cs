using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchAttack : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        transform.Rotate(0f, -90f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        AttackFinished();
    }

    private void AttackFinished(){
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("End")){
            Destroy(gameObject, 3f);
        }
    }

    //OnTriggerEnter and dmg is on 5th bone, since I had to put the collider on them
}
