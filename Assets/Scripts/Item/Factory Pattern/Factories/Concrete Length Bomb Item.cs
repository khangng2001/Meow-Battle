using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConcreteLengthBombItem : ItemFactory
{
    [SerializeField] private ItemLengthBomb[] itemLengthPrefabs;

    public override IItem SpawnItem(Vector3 position)
    {
        if (itemLengthPrefabs.Length > 0)
        {
            GameObject gameObject = Instantiate(itemLengthPrefabs[Random.Range(0, itemLengthPrefabs.Length)].gameObject, position, Quaternion.identity);
            
            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Spawn();

            ItemLengthBomb itemLengthBomb = gameObject.GetComponent<ItemLengthBomb>();
            itemLengthBomb.Initialize();

            return itemLengthBomb;
        }

        return null;
    }
}
