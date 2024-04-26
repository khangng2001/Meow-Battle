using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Loading : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject page;

    private float time = 3f;
    
    private void Update()
    {
        if (!page.activeSelf) return;

        time -= Time.deltaTime * 5f;

        switch (time)
        {
            case <= -1:
                time = 3f;
                break;
            case <= 0:
                loadingText.text = "Loading...";
                break;
            case <= 1f:
                loadingText.text = "Loading..";
                break;
            case <= 2f:
                loadingText.text = "Loading.";
                break;
            case <= 3f:
                loadingText.text = "Loading";
                break;
        }
    }
}
