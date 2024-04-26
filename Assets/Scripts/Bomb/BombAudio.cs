using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BombAudio : NetworkBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip fuseSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip beatenSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.clip = fuseSound;
        audioSource.Play();
    }

    public void TranslateExplosionSound()
    {
        audioSource.clip = explosionSound;
        audioSource.loop = false;
        audioSource.Play();
    }

    [ClientRpc]
    public void BeatenSoundClientRpc()
    {
        audioSource.PlayOneShot(beatenSound);
    }
}
