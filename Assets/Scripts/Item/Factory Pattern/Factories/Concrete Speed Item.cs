using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConcreteSpeedItem : ItemFactory
{
    [SerializeField] private ItemSpeed[] itemSpeedPrefabs;

    public override IItem SpawnItem(Vector3 position)
    {
        if (itemSpeedPrefabs.Length > 0)
        {
            GameObject gameObject = Instantiate(itemSpeedPrefabs[Random.Range(0, itemSpeedPrefabs.Length)].gameObject, position, Quaternion.identity);

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Spawn();

            ItemSpeed itemSpeed = gameObject.GetComponent<ItemSpeed>();
            itemSpeed.Initialize();

            return itemSpeed;
        }

        return null;
    }
}
