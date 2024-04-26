using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConcreteLongRangeWeapon : ItemFactory
{
    [SerializeField] private ItemLongRangeWeapon[] itemLongRangeWeaponPrefabs;

    public override IItem SpawnItem(Vector3 position)
    {
        if (itemLongRangeWeaponPrefabs.Length > 0)
        {
            GameObject gameObject = Instantiate(itemLongRangeWeaponPrefabs[Random.Range(0, itemLongRangeWeaponPrefabs.Length)].gameObject, position, Quaternion.identity);

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Spawn();

            ItemLongRangeWeapon itemLongRangeWeapon = gameObject.GetComponent<ItemLongRangeWeapon>();
            itemLongRangeWeapon.Initialize();

            return itemLongRangeWeapon;
        }

        return null;
    }
}
