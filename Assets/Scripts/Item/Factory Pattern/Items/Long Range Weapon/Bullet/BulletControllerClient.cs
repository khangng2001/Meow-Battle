using Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletControllerClient : MonoBehaviour
{
    private float speed = 50f;
    private bool isMove = true;

    private float timeExisted = 1.5f;

    private void Update()
    {
        // if (!NetworkManager.Singleton.IsClient) return;
        if (!isMove) return;
        if (timeExisted < 0f) Destroy(this.gameObject);

        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        timeExisted -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (!NetworkManager.Singleton.IsClient) return;
        if (CheckCollide(other.gameObject))
        {
            isMove = false;

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
}
