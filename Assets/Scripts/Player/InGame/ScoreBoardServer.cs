using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreBoardServer : NetworkBehaviour
{
    private ScoreBoardClient scoreBoardClient;

    [Header("LOGIC")]
    [SerializeField] private List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
    [SerializeField] private List<GameObject> playerObjectList = new List<GameObject>();
    [SerializeField] private Action onAddPlayerOther;

    public static Action<string, string, int, int, int, Action, GameObject, string, int> OnAddPlayer;
    public static Action<string, int, int, int> OnUpdatePlayer;

    private string mapName;
    private int sizePlayer;

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        scoreBoardClient = GetComponent<ScoreBoardClient>();

        OnAddPlayer = AddPlayer;
        OnUpdatePlayer = UpdatePlayer;
    }

    public void AddPlayer(string playerID, string playerName, int kill, int death, int coin, Action OnAddPlayerOther, GameObject playerObject, string mapName, int sizePlayer)
    {
        // Set Condition For Load Scene
        this.mapName = mapName;
        this.sizePlayer = sizePlayer;

        // Set Player List
        playerInfoList.Add(new PlayerInfo
        {
            ID = playerID,
            Name = playerName,
            Kill = kill,
            Death = death,
            Coin = coin
        });
        playerObjectList.Add(playerObject);

        onAddPlayerOther += OnAddPlayerOther;
        onAddPlayerOther?.Invoke();

        scoreBoardClient.ReadyReceiveNewListPlayerClientRpc();
        foreach (PlayerInfo player in playerInfoList)
        {
            int index = playerInfoList.IndexOf(player);
            scoreBoardClient.AddEachPlayerToListClientRpc(playerInfoList[index].ID, playerInfoList[index].Name, playerInfoList[index].Kill, playerInfoList[index].Death, playerInfoList[index].Coin);
        }
        scoreBoardClient.ReloadListClientRpc(NetworkManager.Singleton.ConnectedClients.Count);

        // SHOW IN SERVER
        DisplayScoreBoard(this.playerInfoList);

        // ACTIVE ALL PLAYERS WHEN ALL PLAYERS BOTH ARE CONNECTED;
        ConditionOfPlayers(this.playerObjectList);

    }

    private void UpdatePlayer(string playerID, int kill, int death, int coin)
    {
        PlayerInfo playerInfo = playerInfoList.Find((PlayerInfo info) => { if (info.ID.Equals(playerID)) return true; return false; });
        int index = playerInfoList.IndexOf(playerInfo);

        if (index <= -1) return;

        playerInfo.Kill += kill;
        playerInfo.Death += death;
        playerInfo.Coin += coin;

        playerInfoList[index] = playerInfo;

        scoreBoardClient.UpdatePlayerClientRpc(index, playerInfoList[index].Kill, playerInfoList[index].Death, playerInfoList[index].Coin);


        // SHOW IN SERVER
        DisplayScoreBoard(this.playerInfoList);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeletePlayerServerRpc(string playerID)
    {
        PlayerInfo playerInfo = playerInfoList.Find((PlayerInfo info) => { if (info.ID.Equals(playerID)) return true; return false; });
        int index = playerInfoList.IndexOf(playerInfo);

        if (index <= -1) return;

        playerInfoList.RemoveAt(index);
        playerObjectList.RemoveAt(index);

        scoreBoardClient.ReadyReceiveNewListPlayerClientRpc();
        foreach (PlayerInfo player in playerInfoList)
        {
            int index2 = playerInfoList.IndexOf(player);
            scoreBoardClient.AddEachPlayerToListClientRpc(playerInfoList[index2].ID, playerInfoList[index2].Name, playerInfoList[index2].Kill, playerInfoList[index2].Death, playerInfoList[index2].Coin);
        }
        scoreBoardClient.ReloadListClientRpc(NetworkManager.Singleton.ConnectedClients.Count);

        // SHOW IN SERVER
        DisplayScoreBoard(this.playerInfoList);
    }

    // SHOW SCORE
    private void DisplayScoreBoard(List<PlayerInfo> playerInfoList)
    {
        if (playerInfoList.Count <= 0) return;

        Debug.Log("========== SCORE ==============");
        foreach (PlayerInfo player in playerInfoList)
        {
            Debug.Log("ID " + player.ID + "\n" +
                "Name " + player.Name + "\n" +
                "Kill " + player.Kill + "/" + player.Death + "\n" +
                "Coin " + player.Coin);
        }
        Debug.Log("============================");
    }
    // ============

    // ACTIVE PLAYER WHEN ALL CONNECTED
    private async void ConditionOfPlayers(List<GameObject> playerObjectList)
    {
        if (playerObjectList.Count < sizePlayer) return;
        if (playerObjectList.Count > sizePlayer) return;

        // Load Map
        //NetworkManager.Singleton.SceneManager.LoadScene(mapName, UnityEngine.SceneManagement.LoadSceneMode.Single);

        // === Find Location Of Players ===
        List<GameObject> standPlace = StandPlaceOfPlayers.OnGetStandPlaceList?.Invoke();

        try
        {
            int checkCount = standPlace.Count;

            foreach (GameObject player in playerObjectList)
            {
                player.transform.position = standPlace[playerObjectList.IndexOf(player)].transform.position;
                
                // SAVE FIRST LOCATION BORN TO REBORN
                player.GetComponent<PlayerControllerServer>().LocationBorn = standPlace[playerObjectList.IndexOf(player)].transform.position;
            }
        }
        catch
        {
            // If Get Location Fail, Use Loop To Get Again (Because Not Found, Find Again)
            await WaitFoundStandPlaceOfPlayers();

            try
            {
                standPlace = StandPlaceOfPlayers.OnGetStandPlaceList?.Invoke();

                foreach (GameObject player in playerObjectList)
                {
                    player.transform.position = standPlace[playerObjectList.IndexOf(player)].transform.position;
                    
                    // SAVE FIRST LOCATION BORN TO REBORN
                    player.GetComponent<PlayerControllerServer>().LocationBorn = standPlace[playerObjectList.IndexOf(player)].transform.position;
                }
            }
            catch
            {
                //playerObjectList[0].transform.position = new Vector3(-1, 1, 3);
            }
        }
        // === FOUND ===

        // Start Game Time
        StartCoroutine(CountdownStartGame(5f, playerObjectList));
    }

    private async Task WaitFoundStandPlaceOfPlayers()
    {
        try
        {
            await Task.Delay(1000);

            List<GameObject> standPlace = StandPlaceOfPlayers.OnGetStandPlaceList?.Invoke();
        }
        catch
        {
            await WaitFoundStandPlaceOfPlayers();
        }
    }

    IEnumerator CountdownStartGame(float time, List<GameObject> playerObjectList)
    {
        yield return new WaitForSeconds(0.1f);
        float timeCountdown = time;
        scoreBoardClient.SetCountdownTextClientRpc(timeCountdown.ToString());

        while (timeCountdown > 0)
        {
            yield return new WaitForSeconds(1f);
            timeCountdown -= 1;
            scoreBoardClient.SetCountdownTextClientRpc(timeCountdown.ToString());
        }

        yield return new WaitForSeconds(0.5f);
        scoreBoardClient.SetCountdownTextClientRpc("");

        foreach (GameObject player in playerObjectList)
        {
            player.GetComponent<PlayerControllerServer>().PlayerState = PlayerState.Normal;
        }

        // Start Countdown End Game Time Afer Started Game
        StartCoroutine(CountdownEndGame(playTime, playerObjectList));
    }
    // ===========

    // UNACTIVE PlAYER WHEN END GAME
    private float playTime = 60 * 3.5f;
    private IEnumerator CountdownEndGame(float time, List<GameObject> playerObjectList)
    {
        yield return new WaitForSeconds(0.1f);
        float timeCountdown = time;

        while (timeCountdown > 0)
        {
            yield return new WaitForSeconds(1f);
            timeCountdown -= 1;
            scoreBoardClient.SendTimeCountClientRpc(timeCountdown);
            if (timeCountdown <= 5) scoreBoardClient.SetCountdownTextClientRpc(timeCountdown.ToString());
        }

        yield return new WaitForSeconds(0.5f);
        scoreBoardClient.SetCountdownTextClientRpc("");

        foreach (GameObject player in playerObjectList)
        {
            player.GetComponent<PlayerControllerServer>().PlayerState = PlayerState.EndGame;
        }


        // Calculator Score
        foreach (PlayerInfo player in playerInfoList)
        {
            // OLD CALCULATOR COIN
            //int coinBase = 20;
            //int KDA;
            //if (player.Death == 0) KDA = player.Kill;
            //else if (player.Kill == 1) KDA = player.Kill;
            //else KDA = player.Kill / player.Death;
            //int coinResult = coinBase + coinBase * KDA;

            // NEW CALCULATOR COIN
            int coinResult = 0;
            int KDA = player.Kill - player.Death;
            switch (KDA)
            {
                case > 0:
                    coinResult += 20 + KDA * 5;
                    break;
                case <= 0:
                    switch (KDA)
                    {
                        case >= -1:
                            coinResult += 20;
                            break;
                        case >= -5:
                            coinResult += 10;
                            break;
                        case >= -10:
                            coinResult += 5;
                            break;
                    }
                    break;
            }

            scoreBoardClient.SendCoinResultClientRpc(player.ID, coinResult);
            
            int index = playerInfoList.IndexOf(player);
            scoreBoardClient.UpdatePlayerClientRpc(index, playerInfoList[index].Kill, playerInfoList[index].Death, coinResult);
        }
    }
    // ============

    [ServerRpc (RequireOwnership = false)]
    public void SendAttendanceFinishCalCoinServerRpc(string id)
    {
        SwitchBackScene.OnFinishCalCoin(id);
    }
}
