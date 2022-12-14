using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSlice : StateMachineBehaviour
{
    public float sliceStart;
    public float sliceStop;
    public float sliceForce;
    Player player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.GetComponent<Player>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= sliceStart && stateInfo.normalizedTime < sliceStop)
        {
            var velocity = Vector3.forward * sliceForce + player.rigidbody.velocity;
            player.transform.Translate(velocity * Time.deltaTime);
        }
        if (stateInfo.normalizedTime >= sliceStop)
        {
            player.rigidbody.velocity.Set(0, player.rigidbody.velocity.y, player.rigidbody.velocity.z);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    player.rigidbody.velocity = Vector3.zero;
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
