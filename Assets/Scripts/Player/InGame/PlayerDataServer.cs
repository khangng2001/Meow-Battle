using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataServer : NetworkBehaviour
{
    private PlayerDataClient playerDataClient;

    private string playerID;
    private string playerName;
    private string mapName;
    private int sizePlayer;
    private ulong localClientID;

    private void Awake()
    {
        playerDataClient = GetComponent<PlayerDataClient>();
    }

    [ServerRpc]
    public void SendDataServerRpc(string id, string name, string mapName, int sizePlayer, ulong localClientID)
    {
        this.playerID = id;
        this.playerName = name;
        this.mapName = mapName;
        this.sizePlayer = sizePlayer;
        this.localClientID = localClientID;

        SwitchSceneService.OnAttendance?.Invoke(id, sizePlayer, mapName);
    }

    [ServerRpc]
    public void SendDataToScoreBoardServerRpc()
    {
        // Send To Score Board In Server
        ScoreBoardServer.OnAddPlayer?.Invoke(this.playerID, this.playerName, 0, 0, 0, playerDataClient.SendPlayerIDToOtherClientRpc, this.gameObject, this.mapName, this.sizePlayer);
    }

    public void UpdateScorePlayer(string playerID, int kill, int death, int coin)
    {
        ScoreBoardServer.OnUpdatePlayer?.Invoke(playerID, kill, death, coin);
    }

    // GET - SET
    public string PlayerID
    {
        get
        {
            return playerID;
        }
    }

    public ulong LocalClientID
    {
        get
        {
            return localClientID;
        }
    }

    public string Name
    {
        get
        {
            return playerName;
        }
    }
}
