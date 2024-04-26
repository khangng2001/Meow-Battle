using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemAudio : NetworkBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip equipSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    [ClientRpc]
    public void EquipSoundClientRpc()
    {
        audioSource.PlayOneShot(equipSound);
    }
}
