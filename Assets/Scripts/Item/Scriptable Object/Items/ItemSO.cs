using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDetail", menuName = "ScriptableObject/ItemSO")]
public class ItemSO : ScriptableObject
{
    public float valueSpeed;
    public int valueLengthBomb;
    public int valueCountBomb;
}
