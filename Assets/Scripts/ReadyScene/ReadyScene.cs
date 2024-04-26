using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReadyScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI readyText;
    private float opacityTextValue = 1f;
    private bool opacityTextTrigger = false;

    [SerializeField] private Image blackBackground;
    public float opacityBlackValue = 0f;
    private bool opacityBlackTrigger = true;
    
    private void Update()
    {
        // ================== RUN ANIMATION TEXT ====================
        readyText.color = new Color(1, 1, 1, opacityTextValue);
        switch (opacityTextTrigger)
        {
            case false:
                opacityTextValue -= Time.deltaTime;
                if (opacityTextValue <= 0f) opacityTextTrigger = true;
                break;
            case true:
                opacityTextValue += Time.deltaTime;
                if (opacityTextValue >= 1f) opacityTextTrigger = false;
                break;
        }
        // ======================================================
        
        // ================= CHECK CONTINUE =====================
        if (Input.anyKeyDown) opacityBlackTrigger = false;
        // ======================================================

        // ================== BLACK BACKGROUND ==================
        switch (opacityBlackTrigger)
        {
            case true:
                // if (opacityBlackValue > 0f)
                // {
                //     opacityBlackValue -= Time.deltaTime * 0.2f;
                //     blackBackground.color = new Color(0, 0, 0, opacityTextValue);
                // }
                break;
            case false:
                if(opacityBlackValue <= 1f) opacityBlackValue += Time.deltaTime * 0.2f;
                
                if (opacityBlackValue >= 1f) SceneManager.LoadScene("Main Game", LoadSceneMode.Single);
                break;
        }
        // ======================================================
        
    }
}
