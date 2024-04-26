using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Managers;
using Process;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Services
{
    public class FetchListRooms : MonoBehaviour
    {
        [SerializeField] private GameObject roomPrefab;
        [SerializeField] private GameObject roomListContent;

        [Header("EVENTS TO SUBSCRIBE")]
        private ListRoomManager listRoomManager;
        private MainMenuManager mainMenuManagerSubject;
        private LogOutRoom logOutRoomSubject;
        private InRoomManager inRoomManager;
        
        private GameLauncher gameLauncher;

        [Header("EVENTS TO PUBLISH")]
        public Action<string> OnJoinRoomByIDMessage;
        public Action<string> OnFetchPlayer;

        public Sprite map_grassy;
        public Sprite map_old_tarvren;
        public Sprite map_arctic;
        
        private void Awake()
        {
            gameLauncher = GetComponentInParent<GameLauncher>();
            listRoomManager = GetComponent<ListRoomManager>();
            mainMenuManagerSubject = GetComponentInParent<MainMenuManager>();
            logOutRoomSubject = GetComponentInChildren<LogOutRoom>();
            inRoomManager = gameLauncher.GetComponentInChildren<InRoomManager>();
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            mainMenuManagerSubject.OnBattleGame += FetchListRoomCallBack;
            listRoomManager.OnRefreshRoom += FetchListRoomCallBack;
            logOutRoomSubject.OnExitRoomEvent += FetchListRoomCallBack;
           
        }

        private void UnsubscribeEvents()
        {
            mainMenuManagerSubject.OnBattleGame -= FetchListRoomCallBack;
            listRoomManager.OnRefreshRoom -= FetchListRoomCallBack;
            logOutRoomSubject.OnExitRoomEvent -= FetchListRoomCallBack;
        }

        private async void FetchListRoomCallBack()
        {
           await FetchListLobbies();
        }
        
        public async Task FetchListLobbies()
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
                {
                    Count = 10,
                    Filters = new List<QueryFilter>()
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0" , QueryFilter.OpOptions.GT)
                    },
                    Order = new List<QueryOrder>()
                    {
                        new QueryOrder(false, QueryOrder.FieldOptions.Created)
                    }
                };
                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
                for (int i = 0; i < roomListContent.transform.childCount; i++)
                {
                    Destroy(roomListContent.transform.GetChild(i).gameObject);
                }

                foreach (Lobby lobbyExisted in queryResponse.Results)
                {
                    GameObject lobbyCreated = Instantiate(roomPrefab, roomListContent.transform);
                    switch(lobbyExisted.Data["Map"].Value)
                    {
                        case "grassy":
                            lobbyCreated.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = map_grassy;
                            break;
                        case "old tarvern":
                            lobbyCreated.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = map_old_tarvren;
                            break;
                        case "arctic":
                            lobbyCreated.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = map_arctic;
                            break;
                    }
                    lobbyCreated.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text =
                        lobbyExisted.Name;
                    lobbyCreated.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text =
                        lobbyExisted.Data["GameMode"].Value;
                    lobbyCreated.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text =
                        $"{lobbyExisted.Players.Count.ToString()}/{lobbyExisted.MaxPlayers}";
                    lobbyCreated.GetComponent<Button>().onClick.AddListener( async ()   => 
                    {
                        try
                        {
                            gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
                            Lobby lobby = await JoinRoomByID(lobbyExisted.Id);
                            if (lobby == null)
                            {
                                // if cannot join the clicked room, refresh the room list and update to lisRoom state again
                                await FetchListLobbies();
                                gameLauncher.UpdatePageState(GameLauncher.PageState.ListRoom);
                                return;
                            }
                            listRoomManager.SetCurrentHostLobby(null, lobby);
                            await listRoomManager.SubscribeLobbyEvents(true);
                            OnFetchPlayer?.Invoke(lobby.Id);
                            inRoomManager.SetCodeRoom(lobby.LobbyCode);
                            gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
                        }
                        catch (Exception e)
                        {
                            OnJoinRoomByIDMessage?.Invoke(e.Message);
                            inRoomManager.SetCodeRoom("");
                            await listRoomManager.SubscribeLobbyEvents(false);
                            listRoomManager.SetCurrentHostLobby(lobbyHost: null, lobbyCurrent: null);
                            gameLauncher.UpdatePageState(GameLauncher.PageState.ListRoom);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                OnJoinRoomByIDMessage?.Invoke(e.Message);
                await FetchListLobbies();
                gameLauncher.UpdatePageState(GameLauncher.PageState.ListRoom);
            }
        }

        private async Task<Lobby> JoinRoomByID(string lobbyExistedId)
        {
            try
            {
                //PLAYER DETAILS
                PlayerDetails playerDetails =  gameLauncher.GetPlayerDetail();
                JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions();
                joinLobbyByIdOptions.Player = new Unity.Services.Lobbies.Models.Player(
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
                return await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId: lobbyExistedId, joinLobbyByIdOptions);
            }
            catch (Exception e)
            {
                await FetchListLobbies();
                OnJoinRoomByIDMessage?.Invoke(e.Message);
                return null;
            }
        }
    }
}
