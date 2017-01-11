using UnityEngine;
using System.Collections;

public class AnimStateBehaviour : StateMachineBehaviour 
{
    //ID of this state
    public int ID;
    //Refrence of animation item
    [HideInInspector]
	public AnimationItem animationitem = null;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animationitem == null)
        {
            animationitem = animator.GetComponent<AnimationItem>();
        }
        animationitem.CurrentState = ID;
        animationitem.StartState(ID);
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animationitem == null)
        {
            animationitem = animator.GetComponent<AnimationItem>();
        }
        animationitem.EndState(ID);
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
