using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public class ListRoomManager : MonoBehaviour
    {

        [Header("CREATE ROOM")]
        [SerializeField] private GameObject createRoomCanvas;
        [SerializeField] private Button displayCreateRoomCanvasBtn;
        [SerializeField] private Button closeCreateRoomCanvasBtn;
        [SerializeField] private Button onConfirmCreateRoomBtn;

        [Header("REFRESH AND SEARCH ROOM")]
        [SerializeField] private Button refreshListRoom;
        
        [SerializeField] private Button displaySearchRoomCanvas;
        [SerializeField] private GameObject searchRoomCanvas;
        [SerializeField] private Button confirmSearchRoom;
        [SerializeField] private Button closeSearchRoomCanvasBtn;
        [SerializeField] private TextMeshProUGUI messageAlert;
        [SerializeField] private Button switchBackToMainMenuButton;
        
        private Lobby lobbyHost;
        private Lobby lobbyCurrent;
        private float heartbeatTimer = 15f;

        private LobbyEventCallbacks lobbyEventCallbacks = new LobbyEventCallbacks();

        private GameLauncher gameLauncher;
        
        [Header("EVENTS TO PUBLISH")]
        public Action OnCreateRoom;
        public Action OnRefreshRoom;
        public Action OnSearchRoom;
        public Action<string> OnNotifyLobbyHeartBeat;
        public Action<string> OnFetchPlayer;
        public Action OnCancelTicket;
        public Action OnCheckStateButtonInRoom;
        

        public void SetCurrentHostLobby(Lobby lobbyHost, Lobby lobbyCurrent)
        {
            this.lobbyHost = lobbyHost;
            this.lobbyCurrent = lobbyCurrent;
        }
        public Lobby GetLobbyCurrent()
        {
            return lobbyCurrent;
        }

        public Lobby GetLobbyHost()
        {
            return lobbyHost;
        }

        public async Task SubscribeLobbyEvents(bool condition)
        {
            if (condition)
            {
                lobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
                await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyCurrent.Id, lobbyEventCallbacks);
            }
            else
            {
                lobbyEventCallbacks.LobbyChanged -= OnLobbyChanged;
            }
        }

        private async void OnLobbyChanged(ILobbyChanges obj)
        {
            if (obj.PlayerJoined.Changed || obj.PlayerLeft.Changed)
            {
                // CANCEL CREATE TICKET IF CREATING TICKET
                //if (obj.PlayerLeft.Changed) OnCancelTicket?.Invoke();
                
                //OnCheckStateButtonInRoom?.Invoke();
                
                //fetch list player
                OnFetchPlayer?.Invoke(lobbyCurrent.Id);
            }

            if (obj.PlayerData.Changed)
            {
                OnCancelTicket?.Invoke();
                OnCheckStateButtonInRoom?.Invoke();

                ChangedLobbyValue<Dictionary<int, LobbyPlayerChanges>> data = obj.PlayerData;

                foreach (var player in data.Value)
                {
                    int key = player.Key; // Key is index Player in Data (Maybe)

                    await GetComponentInChildren<InRoomManager>().UpdateSkinOtherPlayer(key);
                }
            }

            if (obj.Data.Changed)
            {
                Debug.Log("Data Changed");
                
                // Setting Room

                // Setting IP, Port
                if (obj.Data.Value["Port"].Changed || obj.Data.Value["Ip"].Changed)
                {
                    gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);

                    if (obj.Data.Value.TryGetValue("Port", out ChangedOrRemovedLobbyValue<DataObject> portValue) &&
                        obj.Data.Value.TryGetValue("Ip", out ChangedOrRemovedLobbyValue<DataObject> ipValue))
                    {
                        Debug.Log("Port And Ip Changed");
                        
                        if (portValue.Value.Value != "" && ipValue.Value.Value != "")
                        {
                            Debug.Log("Receive Ticket ID");
                            while (true)
                            {
                                try
                                {
                                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(GetLobbyCurrent().Id);
                                    GetComponentInChildren<StartGameInRoom>().CurrentLobbyTemp = lobby;

                                    break;
                                }
                                catch
                                {
                                    Debug.Log("Receive Ticket ID Again");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Awake()
        {
            SetEventsButton();
            gameLauncher = GetComponentInParent<GameLauncher>();
        }

        private void Update()
        {
            if (lobbyHost != null)
            {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f)
                {
                    float heartbeatTimerMax = 15;
                    heartbeatTimer = heartbeatTimerMax;
                    try
                    {
                        Debug.Log("HEART BEAT: " + heartbeatTimer);
                        LobbyService.Instance.SendHeartbeatPingAsync(lobbyId: lobbyHost.Id);
                    }
                    catch (Exception ex)
                    {
                       OnNotifyLobbyHeartBeat?.Invoke(ex.Message);
                    }

                }
            }
        }

        private void SearchRoomByCode()
        {
            OnSearchRoom.Invoke();
        }

        private void RefreshListRoom()
        {
            OnRefreshRoom.Invoke();
        }

        private void CreateRoom()
        {
            OnCreateRoom.Invoke();
        }

        #region DISPLAY UI
        public void OnDisplaySearchRoomCanvas()
        {
            searchRoomCanvas.SetActive(!searchRoomCanvas.activeSelf);
            messageAlert.text = "";
            GetComponent<SearchRoom>().RefeshRoomCode();
        }
        public void OnDisplayCreateRoomCanvas()
        {
            createRoomCanvas.SetActive(!createRoomCanvas.activeSelf);
		}
		#endregion

		public void SetMessageAlert(string message)
        {
            messageAlert.text = message;
        }
        
        private void SetEventsButton()
        {
            displayCreateRoomCanvasBtn.onClick.AddListener(() => { OnDisplayCreateRoomCanvas(); EmitSoundClick.Emit(); });
            closeCreateRoomCanvasBtn.onClick.AddListener(() => { OnDisplayCreateRoomCanvas(); EmitSoundClick.Emit(); });
            onConfirmCreateRoomBtn.onClick.AddListener(() => { CreateRoom(); EmitSoundClick.Emit(); });
            refreshListRoom.onClick.AddListener(() => { RefreshListRoom(); EmitSoundClick.Emit(); });
            confirmSearchRoom.onClick.AddListener(() => { SearchRoomByCode(); EmitSoundClick.Emit(); });
            displaySearchRoomCanvas.onClick.AddListener(() => { OnDisplaySearchRoomCanvas(); EmitSoundClick.Emit(); });
            closeSearchRoomCanvasBtn.onClick.AddListener(() => { OnDisplaySearchRoomCanvas(); EmitSoundClick.Emit(); });
            switchBackToMainMenuButton.onClick.AddListener(() => { SwitchBackToMainMenu(); EmitSoundClick.Emit(); });
        }

        private void SwitchBackToMainMenu()
        {
            gameLauncher.UpdatePageState(GameLauncher.PageState.MenuGame);
        }

        public void ShowHideSearchRoomCanvasUI(bool show)
        {
            searchRoomCanvas.SetActive(show);
        }

        public async void ShowLobby(Lobby lobby)
        {
            if (lobby.HostId != gameLauncher.GetPlayerDetail().unityID) return;
            if (lobby.IsLocked == false) return;
            
            UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions()
            {
                IsPrivate = false,
                IsLocked = false,
            };

            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateLobbyOptions);
        }

        public async void HideLobby(Lobby lobby)
        {
            if (lobby.HostId != gameLauncher.GetPlayerDetail().unityID) return;
            if (lobby.IsLocked == true) return;


            UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions()
            {
                IsPrivate = true,
                IsLocked = true,
            };

            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateLobbyOptions);
        }
    }
}
