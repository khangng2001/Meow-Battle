using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeaponAudio : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip shootSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = shootSound;
    }

    public void Shoot()
    {
        audioSource.Play();
    }
}
