using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinDetail", menuName = "ScriptableObject/SkinListSO")]
public class SkinListSO : ScriptableObject
{
    [Header("SkinSOList")]
    public List<ScriptableObject> skinSOList = new List<ScriptableObject>();

    [Header("ExpressionSOList")]
    public List<ScriptableObject> expressionSOList = new List<ScriptableObject>();
}
