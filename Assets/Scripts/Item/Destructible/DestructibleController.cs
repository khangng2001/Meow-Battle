using Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DestructibleController : NetworkBehaviour, ICollide
{
    [SerializeField] ItemFactorySO factories;
    private ItemFactory factory;

    public void OnCollideByBullet(float timeFreeze, float pushForce, Vector3 directionPush)
    {
    }

    public void OnCollideByMelee(float timeStun, Vector3 pointHit, int strength)
    {
    }

    public void OnDestroyByBomb(GameObject owner)
    {
        // Spawn Item
        if (factories != null && factories.factories.Length > 0)
        {
            factory = factories.factories[Random.Range(0, factories.factories.Length)];

            factory.SpawnItem(new Vector3(transform.position.x, 0f, transform.position.z));
        }

        GetComponent<NetworkObject>().Despawn(true);
    }
}
