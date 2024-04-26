using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPageManager : MonoBehaviour
{
    [Header("Exit")]
    [SerializeField] private Button exitBtn;
    [SerializeField] private int m_exitStateIndex;      //0: main menu      1: game-setting

    [Header("ChangePage")]
    [SerializeField] private Button changePageBtn;
    [SerializeField] private GameObject page1;
    [SerializeField] private GameObject page2;
    private int m_pageIndex = 0;                        //0: page 1         1: page 2

    [SerializeField] private GameLauncher gameLauncher;

    private void Start()
    {
        m_pageIndex = 0;
        exitBtn.onClick.AddListener(exitTutorialPage);
        changePageBtn.onClick.AddListener(UpdatePageUI);
    }

    private void exitTutorialPage()
    {
        if (m_exitStateIndex == 0)
        {
            gameLauncher.UpdatePageState(GameLauncher.PageState.MenuGame);
        }
        if (m_exitStateIndex == 1)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void UpdatePageUI()
    {
        page1.SetActive(false);
        page2.SetActive(false);
        if (m_pageIndex == 0)
        {
            page1.SetActive(true);
            m_pageIndex = 1;
            return;
        }
        if (m_pageIndex == 1)
        {
            page2.SetActive(true);
            m_pageIndex = 0;
            return;
        }
    }
}
