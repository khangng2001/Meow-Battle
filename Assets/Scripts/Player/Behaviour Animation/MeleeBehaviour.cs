using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MeleeBehaviour : StateMachineBehaviour
{
    bool canPonk = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        animator.GetComponent<PlayerWeapon>().UsingWeaponState = UsingWeaponState.Using;
        canPonk = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (stateInfo.normalizedTime >= 0.1f && canPonk)
        {
            canPonk = false;

            ItemMeleeWeapon itemMeleeWeapon = animator.GetComponentInChildren<ItemMeleeWeapon>();

            Aim(animator.transform, Vector3.up * 0.743f, itemMeleeWeapon.AimWithLayer, itemMeleeWeapon.Ponk);
            
            // AUDIO
            itemMeleeWeapon.GetComponent<MeleeWeaponAudio>().HitSoundClientRpc();
        }
    }

    // Because PointAim is Tranform.Position Of PLayer => Wrong PointAim
    // So PosYToAim Fix It, It Make PointAim Raised => Right PointAim
    private void Aim(Transform pointAim, Vector3 PosYToAim, LayerMask layerMask, Action<GameObject, Vector3> OnDetect = null)
    {
        Debug.DrawRay(pointAim.position + PosYToAim, pointAim.forward * 1.25f, Color.yellow, 2f);
        Debug.DrawRay(pointAim.position + PosYToAim, (pointAim.forward + pointAim.right).normalized * 1f, Color.red, 2f);
        Debug.DrawRay(pointAim.position + PosYToAim, (pointAim.forward - pointAim.right).normalized * 1f, Color.red, 2f);
        Debug.DrawRay(pointAim.position + PosYToAim, (pointAim.forward + pointAim.right + pointAim.forward).normalized * 1f, Color.red, 2f);
        Debug.DrawRay(pointAim.position + PosYToAim, (pointAim.forward - pointAim.right + pointAim.forward).normalized * 1f, Color.red, 2f);


        #region // Detect Rays
        Ray ray;

        ray = new Ray(pointAim.position + PosYToAim, pointAim.forward);
        if (Physics.Raycast(ray, out RaycastHit hit_1, 1.25f, layerMask))
        {
            OnDetect(hit_1.collider.gameObject, hit_1.point);
        }

        ray = new Ray(pointAim.position + PosYToAim, (pointAim.forward + pointAim.right).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit_2, 1f, layerMask))
        {
            OnDetect(hit_2.collider.gameObject, hit_2.point);
        }

        ray = new Ray(pointAim.position + PosYToAim, (pointAim.forward - pointAim.right).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit_3, 1f, layerMask))
        {
            OnDetect(hit_3.collider.gameObject, hit_3.point);
        }

        ray = new Ray(pointAim.position + PosYToAim, (pointAim.forward + pointAim.right + pointAim.forward).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit_4, 1f, layerMask))
        {
            OnDetect(hit_4.collider.gameObject, hit_4.point);
        }

        ray = new Ray(pointAim.position + PosYToAim, (pointAim.forward - pointAim.right + pointAim.forward).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit_5, 1f, layerMask))
        {
            OnDetect(hit_5.collider.gameObject, hit_5.point);
        }
        #endregion
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!NetworkManager.Singleton.IsServer) return;

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
