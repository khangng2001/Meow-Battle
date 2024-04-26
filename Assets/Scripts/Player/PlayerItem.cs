using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerItem : NetworkBehaviour
{
    // ITEM
    public void UpgradeSpeed(float value)
    {
        // AUDIO
        GetComponent<PlayerAudio>().EquipSoundClientRpc();

        if (value < 0 && GetComponent<PlayerMovement>().FastSpeed <= 2.5f) return;
        if (value > 0 && GetComponent<PlayerMovement>().FastSpeed >= 3.75f) return;

        GetComponent<PlayerMovement>().FastSpeed += value;
    }

    public void UpgradeLengthBomb(int value)
    {
        // AUDIO
        GetComponent<PlayerAudio>().EquipSoundClientRpc();

        if (value < 0 && GetComponent<PlayerBomb>().BombLength <= 1) return;

        GetComponent<PlayerBomb>().BombLength += value;
    }

    public void UpgradeCountBomb(int value)
    {
        // AUDIO
        GetComponent<PlayerAudio>().EquipSoundClientRpc();

        if (value < 0 && GetComponent<PlayerBomb>().BombCount <= 1) return;

        GetComponent<PlayerBomb>().BombCount += value;
    }

    // WEAPON
    public void UseMeleeWeapon(float numberOfUse)
    {
        // AUDIO
        GetComponent<PlayerAudio>().EquipSoundClientRpc();

        GetComponent<PlayerWeapon>().HavingWeaponState = HavingWeaponState.HaveMeleeWeapon;
        GetComponent<PlayerWeapon>().NumberOfUse = numberOfUse;
    }

    public void UseLongRangeWeapon(float numberOfUse)
    {
        // AUDIO
        GetComponent<PlayerAudio>().EquipSoundClientRpc();

        GetComponent<PlayerWeapon>().HavingWeaponState = HavingWeaponState.HaveLongRangeWeapon;
        GetComponent<PlayerWeapon>().NumberOfUse = numberOfUse;
    }

    public bool CanReceiveWeapon()
    {
        if (GetComponent<PlayerWeapon>().HavingWeaponState == HavingWeaponState.NotHave
            && !GetComponent<Animator>().GetBool("IsHaveWeapon")) return true;

        return false;
    }

    public void Unequip(float numberOfUse)
    {
        PlayerWeapon playerWeapon = GetComponent<PlayerWeapon>();
        Vector3 locationInFrontOf = transform.position + transform.forward * 2f;

        switch (playerWeapon.HavingWeaponState)
        {
            case HavingWeaponState.HaveMeleeWeapon:
                GetComponentInChildren<ItemMeleeWeapon>().Unequip(locationInFrontOf, numberOfUse);
                break;
            case HavingWeaponState.HaveLongRangeWeapon:
                GetComponentInChildren<ItemLongRangeWeapon>().Unequip(locationInFrontOf, numberOfUse);
                break;
        }

        // AUDIO
        GetComponent<PlayerAudio>().DropSoundClientRpc();

        playerWeapon.HavingWeaponState = HavingWeaponState.NotHave;
    }
}
