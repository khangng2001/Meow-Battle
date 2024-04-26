using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemFactory : MonoBehaviour
{
    public abstract IItem SpawnItem(Vector3 position);
}
