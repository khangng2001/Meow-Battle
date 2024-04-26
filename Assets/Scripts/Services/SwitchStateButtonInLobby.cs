using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class SwitchStateButtonInLobby : MonoBehaviour
{
    private GameLauncher gameLauncher;
    
    [Header("EVENTS TO SUBSCRIBE")]
    private ListRoomManager listRoomManager;
    private InRoomManager inRoomManager;
    private FetchPlayer fetchPlayer;
    
    private void Awake()
    {
        gameLauncher = GetComponentInParent<GameLauncher>();
        listRoomManager = gameLauncher?.GetComponentInChildren<ListRoomManager>();
        fetchPlayer = gameLauncher?.GetComponentInChildren<FetchPlayer>();
        inRoomManager = gameLauncher?.GetComponentInChildren<InRoomManager>();
    
        RegisterEvents();
    }
    
    private void OnDestroy()
    {
        UnRegisterEvents();
    }
    
    private void RegisterEvents()
    {
        fetchPlayer.OnCheckStateButtonInRoom += Check;
        listRoomManager.OnCheckStateButtonInRoom += Check;
        inRoomManager.OnReadyClick += UpdatePlayerReadyData;
    }
    
    private void UnRegisterEvents()
    {
        fetchPlayer.OnCheckStateButtonInRoom -= Check;
        listRoomManager.OnCheckStateButtonInRoom -= Check;
        inRoomManager.OnReadyClick -= UpdatePlayerReadyData;
    }
    
    public async void Check()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(listRoomManager.GetLobbyCurrent().Id);
            if (lobby == null) return;

            if (lobby.HostId == gameLauncher.GetPlayerDetail().unityID)
            {
                // Find Host And Check Ready (Host Auto Ready)
                Dictionary<string, GameObject> playerList = GetComponent<FetchPlayer>().PLayerList;
                playerList.TryGetValue(gameLauncher.GetPlayerDetail().unityID, out GameObject playerGameObject);
                if (playerGameObject != null)
                {
                    // Ready Check
                    playerGameObject.transform.GetChild(2).gameObject.SetActive(true);
                
                }
                
                // Send Ready Value Of Host If Value Of Host Is UnReady
                UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions();
                updatePlayerOptions.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "ReadyState", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: "Ready")
                    }
                };
                lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, lobby.HostId, updatePlayerOptions);
                
                // Check Value Of Other Players Is Ready?
                foreach (Player player in lobby.Players)
                {
                    if (player.Data["ReadyState"].Value == "UnReady")
                    {
                        inRoomManager.SwitchButtonState(StartGameButtonState.UnPlay);
                        return;
                    }
                }
                
                // If All Ready, Then Play
                inRoomManager.SwitchButtonState(StartGameButtonState.Play);
            }
            else // If You Are Other Player
            {
                Player player = lobby.Players.Find((player) =>
                {
                    if (player.Id == gameLauncher.GetPlayerDetail().unityID) return true;
                    return false;
                });
                
                if (player.Data["ReadyState"].Value == "Ready") inRoomManager.SwitchButtonState(StartGameButtonState.UnReady);
                if (player.Data["ReadyState"].Value == "UnReady") inRoomManager.SwitchButtonState(StartGameButtonState.Ready);
            }
        }
        catch
        {
            await Task.Delay(1000);
            Check();
        }
    }
    
    public async void UpdatePlayerReadyData(StartGameButtonState buttonState)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.GetLobbyAsync(listRoomManager.GetLobbyCurrent().Id);

            UpdatePlayerOptions options = new UpdatePlayerOptions();

            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {
                    "ReadyState", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member,
                        value: buttonState.ToString())
                }
            };

            await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, gameLauncher.GetPlayerDetail().unityID, options);

            bool isReady;
            switch (buttonState)
            {
                case StartGameButtonState.Ready:
                    inRoomManager.SwitchButtonState(StartGameButtonState.UnReady);
                    isReady = true;
                    break;
                case StartGameButtonState.UnReady:
                    inRoomManager.SwitchButtonState(StartGameButtonState.Ready);
                    isReady = false;
                    break;
                default:
                    inRoomManager.SwitchButtonState(StartGameButtonState.Ready);
                    isReady = false;
                    break;
            }
            
            // Find Host And Check Ready (Host Auto Ready)
            Dictionary<string, GameObject> playerList = GetComponent<FetchPlayer>().PLayerList;
            playerList.TryGetValue(gameLauncher.GetPlayerDetail().unityID, out GameObject playerGameObject);
            if (playerGameObject != null)
            {
                // Ready Check
                playerGameObject.transform.GetChild(2).gameObject.SetActive(isReady);
                
            }

        }
        catch
        {
            UpdatePlayerReadyData(StartGameButtonState.UnReady);
        }
    }
}
