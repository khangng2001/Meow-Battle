using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SwitchSceneService : MonoBehaviour
{
    private List<string> playerlist = new List<string>();

    public static Action<string, int, string> OnAttendance;

    private void Awake()
    {
        OnAttendance = Attendance;
    }

    private void Attendance(string id, int maxPlayer, string mapName)
    {
        playerlist.Add(id);

        if (playerlist.Count == maxPlayer)
        {
            // Switch Scene Map Game
            NetworkManager.Singleton.SceneManager.LoadScene(mapName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
