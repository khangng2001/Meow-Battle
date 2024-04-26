using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnItem : MonoBehaviour
{
    [SerializeField] ItemFactorySO factories;

    private ItemFactory factory;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.Z))
        {
            if (factories.factories.Length > 0)
            {
                factory = factories.factories[Random.Range(0, factories.factories.Length)];

                //factory.SpawnItem(new Vector3(-2.8f, 0f, -15f));
                factory.SpawnItem(new Vector3(-3f, 0f, -15f));
            }
        }
    }

    [ServerRpc]
    void AServerRpc()
    {
        Debug.Log("Server ne");
    }

    [ClientRpc]
    void AClientRpc()
    {
        Debug.Log("Client ne");
    }
}
