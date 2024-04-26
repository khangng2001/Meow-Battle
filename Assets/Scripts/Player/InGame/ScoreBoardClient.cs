using Managers;
using Process;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreBoardClient : NetworkBehaviour
{
    private ScoreBoardServer scoreBoardServer;
    
    [Header("UI")]
    [SerializeField] private GameObject playerInfoPrefab;
    [SerializeField] private Transform bottom;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI timeCountText;

    [Header("LOGIC")]
    [SerializeField] private List<PlayerInfo> playerInfoList = new List<PlayerInfo>();

    [SerializeField] private List<GameObject> playerInfoObjectList = new List<GameObject>();
    private Dictionary<string, int> coinResult = new Dictionary<string, int>();

    private bool isCalCoin = false;

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsClient) return;

        scoreBoardServer = GetComponent<ScoreBoardServer>();
    }

    private void Start()
    {
        if (!NetworkManager.Singleton.IsClient) return;

        OnDestroyInfo = DestroyInfo;

    }

    private void Update()
    {
        // SHOW/HIDE SCORE BOARD
        if (!isCalCoin)
        {
            if (Input.GetKeyDown(KeyCode.Tab)) bottom.parent.gameObject.SetActive(true);
            else if (Input.GetKeyUp(KeyCode.Tab)) bottom.parent.gameObject.SetActive(false);
        }
        else
        {
            bottom.parent.gameObject.SetActive(true);
        }
    }

    [ClientRpc]
    public void ReadyReceiveNewListPlayerClientRpc()
    {
        playerInfoList.Clear();
    }

    [ClientRpc]
    public void AddEachPlayerToListClientRpc(string playerID, string playerName, int kill, int death, int coin)
    {
        playerInfoList.Add(new PlayerInfo
        {
            ID = playerID,
            Name = playerName,
            Kill = kill,
            Death = death,
            Coin = coin
        });
    }

    [ClientRpc]
    public void ReloadListClientRpc(int connectedClients)
    {
        // Delete All
        foreach (Transform child in bottom)
        {
            Destroy(child.gameObject);
        }
        playerInfoObjectList.Clear();

        // Add All Again
        int count = 0;
        foreach (PlayerInfo player in playerInfoList)
        {
            if (count >= connectedClients)
            {
                Debug.Log("A Lots");
                return;
            }
            count += 1;

            GameObject GO = Instantiate(playerInfoPrefab, bottom);
            playerInfoObjectList.Add(GO);

            UpdateInfoPlayerByIndex(playerInfoList.IndexOf(player));
        }
    }

    public void UpdateInfoPlayerByIndex(int index)
    {
        playerInfoObjectList[index].GetComponent<PlayerInfoInScoreBoard>().SetPosText((index + 1).ToString());
        playerInfoObjectList[index].GetComponent<PlayerInfoInScoreBoard>().SetNameText(playerInfoList[index].Name);
        playerInfoObjectList[index].GetComponent<PlayerInfoInScoreBoard>().SetKillText(playerInfoList[index].Kill.ToString(), playerInfoList[index].Death.ToString());
        playerInfoObjectList[index].GetComponent<PlayerInfoInScoreBoard>().SetCoinText(playerInfoList[index].Coin.ToString());
    }

    [ClientRpc]
    public void UpdatePlayerClientRpc(int index, int kill, int death, int coin)
    {
        PlayerInfo playerInfo = playerInfoList[index];

        playerInfo.Kill = kill;
        playerInfo.Death = death;
        //playerInfo.Coin = coin;

        if (playerInfo.Coin != coin)
        {
            PlusCoinInScoreBoard(index, coin);
        }

        playerInfoList[index] = playerInfo;

        UpdateInfoPlayerByIndex(index);
    }

    [ClientRpc]
    public void SetCountdownTextClientRpc(FixedString64Bytes text)
    {
        countdownText.text = text.ToString();
    }

    [ClientRpc]
    public void SendTimeCountClientRpc(float timeCount)
    {
        int minute = (int)timeCount / 60;
        int second = (int)timeCount % 60;

        string minuteText = minute < 10 ? "0" + minute.ToString() : minute.ToString();
        string secondText = second < 10 ? "0" + second.ToString() : second.ToString();

        timeCountText.text = minuteText + ":" + secondText;
    }

    [ClientRpc]
    public void SendCoinResultClientRpc(string id, int coin)
    {
        if (id != TranslateToPlayerNetcode.OnGetGameLauncher().GetPlayerDetail().unityID) return;

        isCalCoin = true;

        PlayerDetails playerDetails = TranslateToPlayerNetcode.OnGetGameLauncher().GetPlayerDetail();
        UpdateCoin(playerDetails, coin);
    }

    private async void UpdateCoin(PlayerDetails playerDetails, int coin)
    {
        await SingletonAPI.Instance.PutUpdateCoin(playerDetails, coin);
        playerDetails = await SingletonAPI.Instance.GetOnePlayer(playerDetails.unityID);
        TranslateToPlayerNetcode.OnGetGameLauncher().SetPlayerDetail(playerDetails);

        scoreBoardServer.SendAttendanceFinishCalCoinServerRpc(playerDetails.unityID);
    }

    // SHOW PLUS COIN IN SCORE BOARD
    private async void PlusCoinInScoreBoard(int indexPlayer, int coin)
    {
        int oldCoin = playerInfoList[indexPlayer].Coin;
        int newCoin = coin;
        int showCoin = oldCoin;

        while (showCoin < newCoin)
        {
            showCoin += 1;
            playerInfoObjectList[indexPlayer].GetComponent<PlayerInfoInScoreBoard>().SetCoinText(showCoin.ToString());
            await Task.Delay(50);
        }

        FinishPlusCoinForPlayer(playerInfoList[indexPlayer].ID);
    }

    private List<string> playerFinishPlusCoinList = new List<string>();
    private void FinishPlusCoinForPlayer(string id)
    {
        playerFinishPlusCoinList.Add(id);

        if (playerFinishPlusCoinList.Count == playerInfoList.Count) scoreBoardServer.SendAttendanceFinishCalCoinServerRpc(TranslateToPlayerNetcode.OnGetGameLauncher().GetPlayerDetail().unityID);
    }
    // =======================================

    // LOG OUT IN GAME

    public static Action OnDestroyInfo;
    private void DestroyInfo()
    {
        GameLauncher gameLauncher = TranslateToPlayerNetcode.OnGetGameLauncher?.Invoke();
        PlayerDetails playerDetails = gameLauncher.GetPlayerDetail();

        //int index = playerInfoList.IndexOf(playerInfoList.Find((player) => { if (player.ID == playerDetails.unityID) return true; return false; }));

        //playerInfoList.RemoveAt(index);
        //playerInfoObjectList.RemoveAt(index);

        scoreBoardServer.DeletePlayerServerRpc(playerDetails.unityID);
    }

    // =======================================
}
