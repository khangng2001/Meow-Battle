using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemFactoryDetail", menuName = "ScriptableObject/ItemFactorySO")]
public class ItemFactorySO : ScriptableObject
{
    public ItemFactory[] factories;
}
