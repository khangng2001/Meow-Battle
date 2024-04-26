using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MeleeWeaponAudio : NetworkBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip missSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = missSound;
    }

    [ClientRpc]
    public void HitSoundClientRpc()
    {
        audioSource.Play();
    }
}
