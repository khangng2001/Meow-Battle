using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemLengthBomb : NetworkBehaviour, IItem
{
    // Objects
    private ParticleSystem particle;

    // Detail
    [SerializeField] private ItemSO itemDetail;
    [SerializeField] private float positionYWorldSpace;

    ItemFloating itemFloating;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        Initialize();
        InitializedClientRpc();
    }

    public void Initialize()
    {
        // Call
        particle = GetComponentInChildren<ParticleSystem>();
        itemFloating = GetComponentInChildren<ItemFloating>();

        // First Implement
        ParticleSystemTrigger(true);
        itemFloating.Floating(true);
        transform.position = new Vector3(transform.position.x, positionYWorldSpace, transform.position.z);
    }
    [ClientRpc]
    private void InitializedClientRpc()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        itemFloating = GetComponentInChildren<ItemFloating>();

        ParticleSystemTrigger(true);
        itemFloating.Floating(true);
        transform.position = new Vector3(transform.position.x, positionYWorldSpace, transform.position.z);
    }


    private void ParticleSystemTrigger(bool turn)
    {
        if (turn) particle.Play();
        else particle.Stop();
    }

    private void Equip(PlayerItem ownerPlayerItem)
    {
        // Myself
        ParticleSystemTrigger(false);
        GetComponent<SphereCollider>().enabled = false;
        GetComponentInChildren<ItemFloating>().GetComponent<MeshRenderer>().enabled = false;

        // PlayerItem Uprade
        ownerPlayerItem.UpgradeLengthBomb(itemDetail.valueLengthBomb);

        Destroy(this.gameObject, 2f);
    }
    [ClientRpc]
    private void BehaviourEquipClientRpc()
    {
        ParticleSystemTrigger(false);
        GetComponent<SphereCollider>().enabled = false;
        GetComponentInChildren<ItemFloating>().GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (other.GetComponent<PlayerItem>())
        {
            Equip(other.GetComponent<PlayerItem>());
            BehaviourEquipClientRpc();
        }
    }
}
