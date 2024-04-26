using Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletControllerServer : MonoBehaviour
{
    [SerializeField] private GameObject owner;
    public LongRangeWeaponSO longRangeWeaponDetail;

    private float speed = 50f;
    private bool isMove = true;

    private void Update()
    {
        // if (!NetworkManager.Singleton.IsServer) return;
        if (!isMove) return;

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // if (!NetworkManager.Singleton.IsServer) return;
        if (CheckCollide(other.gameObject))
        {
            isMove = false;

            if (longRangeWeaponDetail != null)  other.GetComponent<ICollide>()?.OnCollideByBullet(longRangeWeaponDetail.timeFreeze, longRangeWeaponDetail.pushForce, Vector3.zero);

            Destroy(this.gameObject);
        }
    }

    private bool CheckCollide(GameObject GODetect)
    {
        if (GODetect.layer == LayerMask.NameToLayer("Player")) return true;
        if (GODetect.layer == LayerMask.NameToLayer("Bomb")) return true;
        if (GODetect.layer == LayerMask.NameToLayer("Destructibles")) return true;
        if (GODetect.layer == LayerMask.NameToLayer("Indestructibles")) return true;

        return false;
    }

    // GET - SET
    public GameObject Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = value;
        }
    }
    public LongRangeWeaponSO LongRangeWeaponDetail
    {
        get
        {
            return longRangeWeaponDetail;
        }
        set
        {
            longRangeWeaponDetail = value;
        }
    }
}
