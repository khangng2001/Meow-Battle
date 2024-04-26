using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    private Vector3 targetPoint = Vector3.zero;
    [SerializeField] private LayerMask aimWithLayer;
    [SerializeField] private LineRenderer lazer;

    Transform transformWeapon;

    private bool isAim = false;

    private void Start()
    {
        transformWeapon = GetComponentInChildren<ItemFloating>().transform;
    }

    private void Update()
    {
        // If Not Have Gun
        if (!GetComponentInParent<PlayerWeapon>())
        {
            isAim = false;
            VisualizeLazer(this.lazer, false, Vector3.zero, Vector3.zero);
            return;
        }

        if (isAim)
        {
            // Need To Fix Direction Aim
            //transformWeapon.rotation = Quaternion.Slerp(transformWeapon.rotation, Quaternion.LookRotation(targetPoint - (transformWeapon.parent.position + Vector3.up * 0.743f)), Time.deltaTime * 20f);

            Vector3 startPoint = GetComponent<ItemLongRangeWeapon>().PointShoot.position;
            Vector3 endPoint = targetPoint;

            VisualizeLazer(this.lazer, true, startPoint, endPoint);

            transformWeapon.rotation = Quaternion.Slerp(transformWeapon.rotation, Quaternion.LookRotation(endPoint - startPoint), Time.deltaTime * 20f);
        }
        else if (!isAim)
        {
            VisualizeLazer(this.lazer, false, Vector3.zero, Vector3.zero);

            transformWeapon.localRotation = Quaternion.Slerp(transformWeapon.localRotation, GetComponentInChildren<FindHand>().transform.GetChild(0).localRotation, Time.deltaTime * 20f);
        }
    }


    public void ReturnTargetFromAim(Vector3 targetPoint)
    {
        this.targetPoint = targetPoint;
    }

    private void VisualizeLazer(LineRenderer lazer, bool isCasting, Vector3 startPoint, Vector3 endPoint)
    {
        if (GetComponent<ItemLongRangeWeapon>().Owner != null 
            && GetComponent<ItemLongRangeWeapon>().Owner.GetComponent<PlayerControllerClient>() 
            && !GetComponent<ItemLongRangeWeapon>().Owner.GetComponent<PlayerControllerClient>().IsOwner) return;

        if (isCasting)
        {
            lazer.enabled = true;
            //lazer.SetPosition(0, startPoint);
            //lazer.SetPosition(1, endPoint);

            lazer.SetPosition(1, new Vector3(0f, 0f, Vector3.Distance(startPoint, endPoint)));
        }
        else
        {
            lazer.enabled = false;
        }
    }

    // GET - SET
    public Vector3 TargetPoint
    {
        get
        {
            return targetPoint;
        }
        set
        {
            targetPoint = value;
        }
    }
    public LayerMask AimWithLayer
    {
        get
        {
            return aimWithLayer;
        }
        set
        {
            aimWithLayer = value;
        }
    }
    public bool IsAim
    {
        get
        {
            return isAim;
        }
        set
        {
            isAim = value;
        }
    }
}
