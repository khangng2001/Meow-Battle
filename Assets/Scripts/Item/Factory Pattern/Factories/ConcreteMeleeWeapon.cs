using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConcreteMeleeWeapon : ItemFactory
{
    [SerializeField] private ItemMeleeWeapon[] itemMeleeWeaponPrefabs;

    public override IItem SpawnItem(Vector3 position)
    {
        if (itemMeleeWeaponPrefabs.Length > 0)
        {
            GameObject gameObject = Instantiate(itemMeleeWeaponPrefabs[Random.Range(0, itemMeleeWeaponPrefabs.Length)].gameObject, position, Quaternion.identity);

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Spawn();

            ItemMeleeWeapon itemMeleeWeapon = gameObject.GetComponent<ItemMeleeWeapon>();
            itemMeleeWeapon.Initialize();

            return itemMeleeWeapon;
        }

        return null;
    }
}
