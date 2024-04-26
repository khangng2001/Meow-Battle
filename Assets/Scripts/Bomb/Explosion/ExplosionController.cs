using Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ExplosionController : NetworkBehaviour
{
    private GameObject owner;
    private GameObject root;

    private void Start()
    {
        if (!GetComponentInChildren<ParticleSystem>().isPlaying) GetComponentInChildren<ParticleSystem>().Play();
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (root == null) Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        other.GetComponent<ICollide>()?.OnDestroyByBomb(owner);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        other.GetComponent<PlayerControllerServer>()?.OnDestroyByBomb(owner);
    }

    // GET - SET
    public GameObject Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = value;
        }
    }
    public GameObject Root
    {
        get
        {
            return root;
        }
        set
        {
            root = value;
        }
    }
}
