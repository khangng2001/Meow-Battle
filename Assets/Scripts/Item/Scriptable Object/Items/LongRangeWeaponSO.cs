using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LongRangeWeaponDetail", menuName = "ScriptableObject/LongRangeWeaponSO")]
public class LongRangeWeaponSO : ScriptableObject
{
    public float timeFreeze;
    public float pushForce;
}
