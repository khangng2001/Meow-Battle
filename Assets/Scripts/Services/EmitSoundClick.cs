using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitSoundClick : MonoBehaviour
{
    [SerializeField] private static AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void Emit()
    {
        audioSource.Play();
    }
}
