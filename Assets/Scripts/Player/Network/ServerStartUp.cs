using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using UnityEngine;

public class ServerStartUp : MonoBehaviour
{
    private ushort _serverPort;
    private string _internalServerIp = "0.0.0.0";

    private IMultiplayService _multiplayService;
    private IServerQueryHandler m_ServerQueryHandler;

    private bool hasServer = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private async void Start()
    {
        string[] arg = Environment.GetCommandLineArgs();

        for (int i = 0; i < arg.Length; i++)
        {
            if (arg[i] == "-dedicatedServer")
            {
                hasServer = true;
            }

            if (arg[i] == "-port" && i+1 < arg.Length)
            {
                _serverPort = ushort.Parse(arg[i+1]);   
            }
        }

        if (hasServer)
        {
            //Start Server
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            StartServer();
            await StartServerHandler();
            Debug.Log("Has server");
        }
    }

    private void Update()
    {
        if (m_ServerQueryHandler != null)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                m_ServerQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClients.Count;

                // if (NetworkManager.Singleton.ConnectedClients.Count == 2)
                // {
                //     NetworkManager.Singleton.SceneManager.LoadScene("Map_01", UnityEngine.SceneManagement.LoadSceneMode.Single);
                // }
                //if (m_ServerQueryHandler.CurrentPlayers == 1)
                //{
                //    NetworkManager.Singleton.SceneManager.LoadScene("Map_01_Te", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                //}
            }
            m_ServerQueryHandler.UpdateServerCheck();
        }
    }

    private void StartServer()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(_internalServerIp, _serverPort);
        NetworkManager.Singleton.StartServer();
    }

    private async Task StartServerHandler()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        try
        {
            _multiplayService = MultiplayService.Instance;

            m_ServerQueryHandler = await _multiplayService.StartServerQueryHandlerAsync(10, "n/a", "n/a", "0", "n/a");

            LogServerConfig();
        }
        catch (Exception ex)
        {
            Debug.LogError("Something went wrong trying to setup the ServerQueryHandler " + ex.Message);
        }
    }

    private static void LogServerConfig()
    {
        var serverConfig = MultiplayService.Instance.ServerConfig;
        Debug.Log($"Server ID[{serverConfig.ServerId}]");
        Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        Debug.Log($"Port[{serverConfig.Port}]");
        Debug.Log($"QueryPort[{serverConfig.QueryPort}]");
        Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");
    }
}
