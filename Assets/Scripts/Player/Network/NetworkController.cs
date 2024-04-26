using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : NetworkBehaviour
{
    public Button serverBtn;
    public Button hostBtn;
    public Button clientBtn;
    public Button exitBtn;
    public TextMeshProUGUI roleText;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        AddEventForButtons();
    }

    private void Update()
    {
        ShowOrHideExitBtn();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideCursor(false);
        }
    }

    private void HideCursor(bool isHide)
    {
        if (isHide) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }

    private void AddEventForButtons()
    {
        serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            roleText.text = "Server";
            HideCursor(false);
        });

        hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            roleText.text = "Host";
            HideCursor(false);
        });

        clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            roleText.text = "Client";
            HideCursor(false);
        });

        exitBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
        });
    }

    private void ShowOrHideExitBtn()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
        {
            serverBtn.gameObject.SetActive(false);
            hostBtn.gameObject.SetActive(false);
            clientBtn.gameObject.SetActive(false);
            exitBtn.gameObject.SetActive(true);
            roleText.gameObject.SetActive(true);
        }
        else
        {
            serverBtn.gameObject.SetActive(true);
            hostBtn.gameObject.SetActive(true);
            clientBtn.gameObject.SetActive(true);
            exitBtn.gameObject.SetActive(false);
            roleText.gameObject.SetActive(false);
        }
    }
}
