using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandPlaceOfPlayers : MonoBehaviour
{
    [SerializeField] private List<GameObject> standPlaceList = new List<GameObject>();

    public static Func<List<GameObject>> OnGetStandPlaceList;

    private void Awake()
    {
        OnGetStandPlaceList = StandPlaceList;
    }

    public List<GameObject> StandPlaceList()
    {
        standPlaceList.Clear();

        foreach (Transform child in transform)
        {
            standPlaceList.Add(child.gameObject);
        }

        return standPlaceList;
    }
}
