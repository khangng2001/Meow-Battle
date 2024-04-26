using Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscOptionController : MonoBehaviour
{
    [Header("Option")]
    [SerializeField] private GameObject setting;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button closeOptionBtn;

    [Header("Alert")]
    [SerializeField] private GameObject alert;
    [SerializeField] private Button acceptQuitBtn;
    [SerializeField] private Button closeAlertBtn;

    [Header("Music")]
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private AudioSource audioMusic;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorial;
    [SerializeField] private Button openTutorialBtn;
    [SerializeField] private Button closeTutorialBtn;

    private bool escMode = false;

    private void Awake()
    {
        quitBtn.onClick.AddListener(() => { alert.SetActive(true); EmitSoundClick.Emit(); });
        closeOptionBtn.onClick.AddListener(() => { escMode = false; EmitSoundClick.Emit(); });
        openTutorialBtn.onClick.AddListener(() => { tutorial.SetActive(true); EmitSoundClick.Emit(); });
        closeTutorialBtn.onClick.AddListener(() => { tutorial.SetActive(false); EmitSoundClick.Emit(); });

        acceptQuitBtn.onClick.AddListener(() => 
        {
            EmitSoundClick.Emit();

            ScoreBoardClient.OnDestroyInfo?.Invoke();
            NetworkManager.Singleton.Shutdown();
            TranslateToPlayerNetcode.OnGetGameLauncher?.Invoke().GetComponentInChildren<InRoomManager>().OnLogOutRoomAction?.Invoke();
            SceneManager.LoadScene("Main Game", LoadSceneMode.Single);
        });
        closeAlertBtn.onClick.AddListener(() => { alert.SetActive(false); EmitSoundClick.Emit(); });


        // HANDLE MUSIC
        sliderMusic.onValueChanged.AddListener((floatValue) =>
        {
            ChangeVolumeMusic(floatValue);
        });
    }

    private void Update()
    {
        // SHOW/HIDE CURSOR AND ESC BOARD
        if (Input.GetKeyDown(KeyCode.Escape)) escMode = !escMode;
        if (escMode)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            setting.SetActive(true);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            setting.SetActive(false);
            alert.SetActive(false);
            tutorial.SetActive(false);
        }
    }

    private void ChangeVolumeMusic(float volume)
    {
        audioMusic.volume = volume;
    }
}
