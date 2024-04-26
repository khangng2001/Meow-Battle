using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponDetail", menuName = "ScriptableObject/MeleeWeaponSO")]
public class MeleeWeaponSO : ScriptableObject
{
    public float timeStun;
    public int strength;
}
