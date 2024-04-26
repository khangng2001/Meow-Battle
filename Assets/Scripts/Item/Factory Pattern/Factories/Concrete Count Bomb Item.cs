using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConcreteCountBombItem : ItemFactory
{
    [SerializeField] private ItemCountBomb[] itemCountBombPrefabs;

    public override IItem SpawnItem(Vector3 position)
    {
        if (itemCountBombPrefabs.Length > 0)
        {
            GameObject gameObject = Instantiate(itemCountBombPrefabs[Random.Range(0, itemCountBombPrefabs.Length)].gameObject, position, Quaternion.identity);
            
            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Spawn();

            ItemCountBomb itemCountBomb = gameObject.GetComponent<ItemCountBomb>();
            itemCountBomb.Initialize();

            return itemCountBomb;
        }

        return null;
    }
}
