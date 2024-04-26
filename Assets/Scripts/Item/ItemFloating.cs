using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemFloating : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float magnitude;
    [SerializeField] private float speedRoll;
    //[SerializeField] private Vector3 pos;
    [SerializeField] private Vector3 posFix;

    public bool canFloat;
    private bool isLast = false;

    private void Start()
    {
        //pos = transform.position;
    }

    private void Update()
    {
        Floating();
    }

    public void Floating(bool canFloat)
    {
        this.canFloat = canFloat;

        if (canFloat)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            isLast = false;
            //pos = transform.position;
        }
    }

    private void Floating()
    {
        if (canFloat)
        {
            transform.localPosition = posFix + Vector3.up * Mathf.Sin(Time.time * speed) * magnitude;
            transform.Rotate(Vector3.up, speedRoll * Time.deltaTime);
        }
        else if (!canFloat && !isLast)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            isLast = true;
        }
    }
}
