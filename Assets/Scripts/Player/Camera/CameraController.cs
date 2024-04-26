using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject owner;
    public GameObject Owner { get => owner; set => owner = value; }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (owner == null) Destroy(this.gameObject);
    }
}
