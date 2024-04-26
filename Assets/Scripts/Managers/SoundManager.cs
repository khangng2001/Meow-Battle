using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static Func<SoundManager> OnGetSoundManager;

    private void Awake()
    {
        OnGetSoundManager = () =>
        {
            return this;
        };
    }

    public static SoundManager instance;

    [SerializeField] private List<AudioClip> soundEffecct;
    /*
     0: Walking
     1: Run
     2: Slash no Hit
     3: Hit Player
     4: Hit Bomb
     5: Shooting
     6: Collecting
     7: Fuse
     8: Explosion
     9: End Match
     */

    public void PlaySoundEffect(int indexSoundEffect, Vector3 point, float volume)
    {
        AudioSource.PlayClipAtPoint(soundEffecct[indexSoundEffect], point, volume);
    }

    public AudioClip GetAudioClip(int index)
    {
        return soundEffecct[index];
    }
}
