using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Managers;
using Process;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Services
{
    public class SearchRoom : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomCodeInputField;

        private GameLauncher gameLauncher;
        private ListRoomManager listRoomManager;
        private FetchListRooms fetchListRooms;
        private InRoomManager inRoomManager;

        [Header("EVENTS TO PUBLISH")]
        public Action<string> OnJoinRoomByCodeMessage;
        public Action<string> OnFetchPlayer;

        private void Awake()
        {
            gameLauncher = GetComponentInParent<GameLauncher>();
            listRoomManager = GetComponent<ListRoomManager>();
            inRoomManager = gameLauncher.GetComponentInChildren<InRoomManager>();
            fetchListRooms = GetComponent<FetchListRooms>();

            SubscribeEvents();
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            listRoomManager.OnSearchRoom += OnSearchRoomConfirm;
            
        }

        private void UnSubscribeEvents()
        {
            listRoomManager.OnSearchRoom -= OnSearchRoomConfirm;
            
        }
        private async void OnSearchRoomConfirm()
        {
            //Hide UI
            listRoomManager.ShowHideSearchRoomCanvasUI(false);
            
            if (roomCodeInputField.text.Trim().Length <= 0)
            {
               listRoomManager.SetMessageAlert("Please input code room");
               listRoomManager.ShowHideSearchRoomCanvasUI(true);
               return;
            }

            try
            {
                gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
                Lobby lobby = await JoinRoomByCode(roomCodeInputField.text);
                if (lobby == null)
                {
                    // if cannot join the clicked room, refresh the room list and update to lisRoom state again
                    listRoomManager.SetMessageAlert("Not Found Room");
                    await fetchListRooms.FetchListLobbies();
                    gameLauncher.UpdatePageState(GameLauncher.PageState.ListRoom);
                    listRoomManager.ShowHideSearchRoomCanvasUI(true);
                    return;
                }
                listRoomManager.SetCurrentHostLobby(null, lobby);
                await listRoomManager.SubscribeLobbyEvents(true);
                OnFetchPlayer?.Invoke(lobby.Id);
                inRoomManager.SetCodeRoom(lobby.LobbyCode);
                listRoomManager.ShowHideSearchRoomCanvasUI(false);
                gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
            }
            catch (Exception e)
            {
                listRoomManager.SetMessageAlert(e.Message);
                inRoomManager.SetCodeRoom("");
                await listRoomManager.SubscribeLobbyEvents(false);
                listRoomManager.SetCurrentHostLobby(lobbyHost: null, lobbyCurrent: null);
                gameLauncher.UpdatePageState(GameLauncher.PageState.ListRoom);
                listRoomManager.ShowHideSearchRoomCanvasUI(true);
            }
        }
        
        private async Task<Lobby> JoinRoomByCode(string code)
        {
            try
            {
                PlayerDetails playerDetails = gameLauncher.GetPlayerDetail();
                JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions();
                joinLobbyByCodeOptions.Player = new Unity.Services.Lobbies.Models.Player(
                    id: playerDetails.unityID, data:new Dictionary<string, PlayerDataObject>()
                    {
                        {
                            "PlayerID" , new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerDetails.unityID)
                        },
                        {
                            "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerDetails.name)
                        }, 
                        { 
                            "PlayerSkin", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Skin_00") 
                        },
                        {
                            "PlayerExpression", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Expression_00")
                        },
                        {
                            "ReadyState", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "UnReady")
                        }
                    });
                return await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinLobbyByCodeOptions);
            }
            catch
            {
              
                return null;
            }
        }

        public async void JoinRoomByInviteWithCode(string code)
        {
            try
            {
                gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
                Lobby lobby = await JoinRoomByCode(code);
                if (lobby == null)
                {
                    // if cannot join the clicked room, refresh the room list and update to lisRoom state again
                    listRoomManager.SetMessageAlert("Not Found Room");
                    await fetchListRooms.FetchListLobbies();
                    gameLauncher.UpdatePageState(GameLauncher.PageState.ListRoom);
                    return;
                }
                listRoomManager.SetCurrentHostLobby(null, lobby);
                await listRoomManager.SubscribeLobbyEvents(true);
                OnFetchPlayer?.Invoke(lobby.Id);
                inRoomManager.SetCodeRoom(lobby.LobbyCode);
                //listRoomManager.OnDisplaySearchRoomCanvas();
                gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
            }
            catch (Exception e)
            {
                listRoomManager.SetMessageAlert(e.Message);
                inRoomManager.SetCodeRoom("");
                await listRoomManager.SubscribeLobbyEvents(false);
                listRoomManager.SetCurrentHostLobby(lobbyHost: null, lobbyCurrent: null);
                gameLauncher.UpdatePageState(GameLauncher.PageState.ListRoom);
            }
        }

        public void RefeshRoomCode()
        {
            roomCodeInputField.text = "";
        }
    }
}
