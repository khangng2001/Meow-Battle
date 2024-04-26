using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimBehavioour : StateMachineBehaviour
{
    AimController aimController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        aimController = animator.GetComponentInChildren<AimController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (aimController == null) return;
        if (aimController.GetComponent<ItemLongRangeWeapon>().Owner == null) return;

        // Aim
        if (stateInfo.normalizedTime >= 0.8f)
        {
            aimController.ReturnTargetFromAim(Aim(animator.transform, Vector3.up * 0.743f, aimController.AimWithLayer));
            aimController.IsAim = true;
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (aimController == null) return;
        if (aimController.GetComponent<ItemLongRangeWeapon>().Owner == null) return;

        aimController.IsAim = false;
    }

    List<float> aimList = new List<float>() { 50, 30, 20};
    private Vector3 Aim(Transform pointAim, Vector3 PosYToAim, LayerMask AimWithLayer)
    {
        float length = 50f;
        
        Debug.DrawRay(pointAim.position + PosYToAim, pointAim.forward * length, Color.yellow, 0.1f);

        #region // Detect Rays
        Ray ray;

        ray = new Ray(pointAim.position + PosYToAim, pointAim.forward);
        if (Physics.Raycast(ray, out RaycastHit hit_mid, length, AimWithLayer))
        {
            if (IsTarget(hit_mid.collider.gameObject)) return hit_mid.point;
        }

        foreach (float aimChild in aimList)
        {
            Debug.DrawRay(pointAim.position + PosYToAim, (pointAim.forward * aimChild - pointAim.right).normalized * length, Color.blue, 0.1f);
            Debug.DrawRay(pointAim.position + PosYToAim, (pointAim.forward * aimChild + pointAim.right).normalized * length, Color.blue, 0.1f);

            ray = new Ray(pointAim.position + PosYToAim, pointAim.forward * aimChild + pointAim.right);
            if (Physics.Raycast(ray, out RaycastHit hit_right, length, AimWithLayer))
            {
                if (IsTarget(hit_right.collider.gameObject)) return hit_right.point;
            }

            ray = new Ray(pointAim.position + PosYToAim, pointAim.forward * aimChild - pointAim.right);
            if (Physics.Raycast(ray, out RaycastHit hit_left, length, AimWithLayer))
            {
                if (IsTarget(hit_left.collider.gameObject)) return hit_left.point;
            }
        }
        #endregion

        return hit_mid.point;
    }

    // Target To Aim (Priority Gradually Decrease)
    private bool IsTarget(GameObject objectAim)
    {
        if (objectAim.layer == LayerMask.NameToLayer("Player")) return true;
        if (objectAim.layer == LayerMask.NameToLayer("Bomb")) return true;
        //if (objectAim.layer == LayerMask.NameToLayer("Destructibles")) return true;
        //if (objectAim.layer == LayerMask.NameToLayer("Indestructibles")) return true;

        return false;
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
