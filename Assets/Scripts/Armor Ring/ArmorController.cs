using Unity.Netcode;
using UnityEngine;

public class ArmorController : NetworkBehaviour
{
    private GameObject owner;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        owner = this.gameObject;
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (owner == null) Destroy(this.gameObject);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
        TurnOnClientRpc();
    }
    [ClientRpc]
    private void TurnOnClientRpc()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
        TurnOffClientRpc();
    }
    [ClientRpc]
    private void TurnOffClientRpc()
    {
        gameObject.SetActive(false);
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
}
