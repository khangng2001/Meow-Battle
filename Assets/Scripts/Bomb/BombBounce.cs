using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BombBounce : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float magnitude;
    private Vector3 pos;

    private void Start()
    {
        pos = transform.localScale;
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        switch (GetComponentInParent<BombController>().TimeExist)
        {
            case >= 4:
                speed = 3f;
                break;
            case >= 3:
                speed = 5f;
                break;
            case >= 2:
                speed = 7f;
                break;
            case >= 1:
                speed = 10f;
                break;
            case >= 0:
                speed = 15f;
                break;
        }

        transform.localScale = pos + Vector3.forward * Mathf.Pow(Mathf.Sin(Time.time * speed), 2) * magnitude
                                   + Vector3.right * Mathf.Pow(Mathf.Sin(Time.time * speed), 2) * magnitude;
    }
}
