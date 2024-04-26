using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BombWarning : NetworkBehaviour
{
    [SerializeField] private GameObject warningZonePrefab;

    void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        int length = (int)GetComponent<BombController>().ExplosionLength;
        SpawnWarningZone(length);
    }

    private void SpawnWarningZone(int length)
    {
        GameObject GOFirst;

        GOFirst = Spawn(warningZonePrefab, transform.position, this.transform);
    }
    private GameObject Spawn(GameObject prefab, Vector3 location, Transform parent)
    {
        GameObject GO = Instantiate(prefab, location, Quaternion.identity, parent);
        return GO;
    }
}
