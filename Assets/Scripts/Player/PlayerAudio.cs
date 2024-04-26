using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAudio : NetworkBehaviour
{
    public enum MoveType
    {
        Walk,
        Run,
        None
    }
    
    [SerializeField] private AudioSource audioMainTain;
    [SerializeField] private AudioSource audioOneShot;

    [Header("All can hear")]
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip beatenSound;
    [SerializeField] private AudioClip placeBombSound;

    [Header("Only me")]
    [SerializeField] private AudioClip equipSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip dazeSound;
    [SerializeField] private AudioClip shieldCreatedSound;
    [SerializeField] private AudioClip shieldBrokenSound;
    [SerializeField] private AudioClip dieSound;

    //===================== ALL CAN HEAR ========================

    [ClientRpc]
    public void MainTainSoundClientRpc(MoveType type)
    {
        switch (type)
        {
            case MoveType.Walk:
                audioMainTain.clip = walkSound;
                if (!audioMainTain.isPlaying) audioMainTain.Play();
                break;
            case MoveType.Run:
                audioMainTain.clip = runSound;
                if (!audioMainTain.isPlaying) audioMainTain.Play();
                break;
            case MoveType.None:
                if (audioMainTain.isPlaying) audioMainTain.Stop();
                break;
        }
    }

    [ClientRpc]
    public void BeatenSoundClientRpc()
    {
        audioOneShot.PlayOneShot(beatenSound);
    }

    [ClientRpc]
    public void PlaceBombSoundClientRpc()
    {
        audioOneShot.PlayOneShot(placeBombSound);
    }

    //============================================================

    //===================== ALL CAN HEAR ========================

    [ClientRpc]
    public void EquipSoundClientRpc()
    {
        if (!IsOwner) return;
        audioOneShot.PlayOneShot(equipSound);
    }

    [ClientRpc]
    public void DropSoundClientRpc()
    {
        if (!IsOwner) return;
        audioOneShot.PlayOneShot(dropSound);
    }

    [ClientRpc]
    public void DazeSoundClientRpc()
    {
        if (!IsOwner) return;
        audioOneShot.PlayOneShot(dazeSound);
    }

    [ClientRpc]
    public void DieSoundClientRpc()
    {
        if (!IsOwner) return;
        audioOneShot.PlayOneShot(dieSound);
    }

    [ClientRpc]
    public void ShieldCreatedSoundClientRpc()
    {
        if (!IsOwner) return;
        audioOneShot.PlayOneShot(shieldCreatedSound);
    }

    [ClientRpc]
    public void ShieldBrokenSoundClientRpc()
    {
        if (!IsOwner) return;
        audioOneShot.PlayOneShot(shieldBrokenSound);
    }

    //============================================================
}
