using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SwitchBackScene : MonoBehaviour
{
    private List<string> playerFinishUpdateCoinToServerList = new List<string>();
    private List<string> playerFinishPlusCoinList = new List<string>();
    
    public static Action<string> OnFinishCalCoin;
    
    private void Awake()
    {
        OnFinishCalCoin = FinishCalCoin;
    }
    
    private void FinishCalCoin(string id)
    {
        playerFinishUpdateCoinToServerList.Add(id);
        
        if (playerFinishUpdateCoinToServerList.Count >= (NetworkManager.Singleton.ConnectedClients.Count * 2))
        {
            StartCoroutine(SwitchSceneAfterTime(5f));
        }
    }

    IEnumerator SwitchSceneAfterTime(float time)
    {
        float maxTime = time;
        float timing = 0f;

        while (timing < maxTime)
        {
            timing += 1f;
            Debug.Log(timing);
            yield return new WaitForSeconds(1f);
        }

        // Switch Scene Map Game
        NetworkManager.Singleton.SceneManager.LoadScene("Main Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
