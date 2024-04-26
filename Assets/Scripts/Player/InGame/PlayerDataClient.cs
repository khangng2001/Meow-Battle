using Managers;
using Process;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerDataClient : NetworkBehaviour
{
    private PlayerDataServer playerDataServer;
    private PlayerSkin playerSkin;

    [SerializeField] private NetworkVariable<FixedString64Bytes> playerIDNetwork = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Owner);
    [SerializeField] private NetworkVariable<FixedString64Bytes> playerSkinNetwork = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Owner);
    [SerializeField] private NetworkVariable<FixedString64Bytes> playerExpressionNetwork = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Owner);

    string idTemp;
    string nameTemp;
    private string skinNameTemp;
    private string expressionNameTemp;
    string mapNameTemp;
    int sizePlayerTemp;

    private void Awake()
    {
        playerDataServer = GetComponent<PlayerDataServer>();
        playerSkin = GetComponent<PlayerSkin>();

        gameObject.name = gameObject.name + " " + Random.Range(100, 999);
    }

    private async void Start()
    {
        if (!NetworkManager.Singleton.IsClient) return;
        if (!IsOwner) return;

        // FIND ID AND NAME HERE
        PlayerDetails playerDetails = GetPlayerDetails();
        this.idTemp = playerDetails.unityID;
        this.nameTemp = playerDetails.name;

        // FIND MAP NAME, SIZE PLAYERS, MY SKIN HERE
        Lobby lobby = null;
        while (true)
        {
            try
            {
                await Task.Delay(1000);
                Debug.Log(GetLobbyCurrent().Id);
                lobby = await LobbyService.Instance.GetLobbyAsync(GetLobbyCurrent().Id);
                if (lobby != null) break;
            }
            catch
            {
                Debug.Log("Again");
            }
        }
        string mapName = lobby.Data["Map"].Value;
        int sizePlayer = lobby.MaxPlayers;
        foreach (Player player in lobby.Players)
        {
            if (player.Id != idTemp) continue;

            skinNameTemp = player.Data["PlayerSkin"].Value;
            expressionNameTemp = player.Data["PlayerExpression"].Value;
            break;
        }

        this.mapNameTemp = mapName;
        this.sizePlayerTemp = sizePlayer;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;

        // SEND TO SERVER
        playerDataServer.SendDataServerRpc(this.idTemp, this.nameTemp, this.mapNameTemp, this.sizePlayerTemp, NetworkManager.Singleton.LocalClientId);
    }

    private async void SceneManager_OnLoadComplete(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        if (sceneName == this.mapNameTemp)
        {
            // SEND TO SERVER PART 2
            playerDataServer.SendDataToScoreBoardServerRpc();
            TranslateToPlayerNetcode.OnGetGameLauncher().UpdatePageState(GameLauncher.PageState.InGame);
        }

        if (sceneName == "Main Game")
        {
            TranslateToPlayerNetcode.OnGetGameLauncher().UpdatePageState(GameLauncher.PageState.InRoom);
            TranslateToPlayerNetcode.OnGetGameLauncher().GetComponentInChildren<ListRoomManager>().OnCheckStateButtonInRoom?.Invoke();
            Lobby lobby = null;
            while (lobby == null)
            {
                try
                {
                    lobby = await LobbyService.Instance.GetLobbyAsync(GetLobbyCurrent().Id);
                }
                catch { }
            }
            TranslateToPlayerNetcode.OnGetGameLauncher().GetComponentInChildren<ListRoomManager>().ShowLobby(lobby);
            Debug.Log("Out");
            NetworkManager.Singleton.Shutdown();
        }
    }

    [ClientRpc]
    public void SendPlayerIDToOtherClientRpc()
    {
        if (IsOwner)
        {
            // FIND ID HERE
            playerIDNetwork.Value = idTemp;
            playerSkinNetwork.Value = skinNameTemp;
            playerExpressionNetwork.Value = expressionNameTemp;
        }

        if (!IsOwner)
        {
            StartCoroutine(AwaitReceivedPlayerID());
        }
    }

    // Wait Receive PlayerID (NetworkVarible) From Other Player Object Just Appeared
    IEnumerator AwaitReceivedPlayerID()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (this.playerIDNetwork.Value.ToString().Trim().Length > 0 &&
                this.playerSkinNetwork.Value.ToString().Trim().Length > 0 &&
                this.playerExpressionNetwork.Value.ToString().Trim().Length > 0)
            {
                break;
            }
        }

        // DO SOMETHING AFTER HAVE PLAYERIDNETWORK
        playerSkin.LoadSkinForOtherPlayer(this.playerSkinNetwork.Value.ToString(), this.playerExpressionNetwork.Value.ToString());
    }

    // GET
    public string PlayerIDNetwork
    {
        get
        {
            return playerIDNetwork.Value.ToString();
        }
    }

    public PlayerDetails GetPlayerDetails()
    {
        return TranslateToPlayerNetcode.OnGetInfoPlayer?.Invoke() ?? new PlayerDetails();
    }

    public Lobby GetLobbyCurrent()
    {
        return TranslateToPlayerNetcode.OnGetInfoLobby?.Invoke() ?? null;
    }

    public List<ScriptableObject> GetInfoSkinList()
    {
        return TranslateToPlayerNetcode.OnGetInfoSkinList?.Invoke() ?? new List<ScriptableObject>();
    }

    public List<ScriptableObject> GetInfoExpression()
    {
        return TranslateToPlayerNetcode.OnGetInfoExpressionList?.Invoke() ?? new List<ScriptableObject>();
    }
}
