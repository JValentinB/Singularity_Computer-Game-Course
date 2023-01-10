using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationVariation : StateMachineBehaviour
{
    public List<AnimationClip> animations;
    AnimatorOverrideController animatorOverrideController;
    public string motionName;
    public int min;
    public int max = 10;
    public int threshold = 9;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        randomizeAnimation();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //randomizeAnimation();
    }

    void randomizeAnimation()
    {
        int random = Random.Range(min, max);
        if(random < threshold)
        {
            animatorOverrideController[motionName] = animations[0];
        }
        else animatorOverrideController[motionName] = animations[1];
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
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
