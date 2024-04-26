using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LongRangeBehaviour : StateMachineBehaviour
{
    bool canBang = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        animator.GetComponent<PlayerWeapon>().UsingWeaponState = UsingWeaponState.Using;
        canBang = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        // Continue Aim While Shoot
        //animator.GetComponentInChildren<AimController>().IsAim = true;

        // Moment Shoot
        if (stateInfo.normalizedTime >= 0.1f && canBang)
        {
            canBang = false;

            Vector3 target = animator.GetComponentInChildren<AimController>().TargetPoint;
            animator.GetComponentInChildren<ItemLongRangeWeapon>().Bang(target);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        // Stop Aim After Shoot
        //animator.GetComponentInChildren<AimController>().IsAim = false;

        // Decrease Number Of Bullet And Change State Weapon
        animator.GetComponent<PlayerWeapon>().UsingWeaponState = UsingWeaponState.NotUsing;
        animator.GetComponent<PlayerWeapon>().NumberOfUse -= 1;
    }

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
