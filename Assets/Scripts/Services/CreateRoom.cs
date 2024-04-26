using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Managers;
using Process;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    public class CreateRoom : MonoBehaviour
    {
        private GameLauncher gameLauncher;
        private ListRoomManager listRoomManager;
        private InRoomManager inRoomManager;
        
        [SerializeField] private ToggleGroup gameModeToggleGroup;
        [SerializeField] private ToggleGroup mapToggleGroup;
        
        [Header("PLAYER SIZE")]
        [SerializeField] private TextMeshProUGUI playerSizeTextMeshProUGUI;

        [SerializeField] private Button increasePlayerSizeBtn;
        [SerializeField] private Button decreasePlayerSizeBtn;
        private string mapName = "";
        private string gameMode = "";
        private int roomMaxPlayers = 2;
        

        public Action<string> OnCreateRoomMessage;
        public Action<string> OnFetchPlayer;
        public static Func<Lobby> OnSendLobbyData;
        public Action OnNotifyRoomCreated;

        private void Awake()
        {
            gameLauncher = GetComponentInParent<GameLauncher>();
            listRoomManager = GetComponent<ListRoomManager>();
            inRoomManager = gameLauncher.GetComponentInChildren<InRoomManager>();
            increasePlayerSizeBtn.onClick.AddListener(OnIncreasePlayerSize);
            decreasePlayerSizeBtn.onClick.AddListener(OnDecreasePlayerSize);
            SubscribeEvents();
            SubscribeToggleGroup();
            
        }

        

        private void Update()
        {
            playerSizeTextMeshProUGUI.text = roomMaxPlayers.ToString();
        }

        private void OnIncreasePlayerSize()
        {
            if (roomMaxPlayers == 4) return;
            
            if (Regex.IsMatch(gameMode, "(?i)ZONE"))
            {
                roomMaxPlayers = 4;

            }
            else if (Regex.IsMatch(gameMode, "(?i)CLASSIC"))
            {
                roomMaxPlayers++;
            }
            
        }


        private void OnDecreasePlayerSize()
        {
            if (roomMaxPlayers == 2) return;

            if (Regex.IsMatch(gameMode, "(?i)ZONE"))
            {
                roomMaxPlayers = 2;

            }
            else if (Regex.IsMatch(gameMode, "(?i)CLASSIC"))
            {
                roomMaxPlayers--;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            listRoomManager.OnCreateRoom += OnCreateRoom;

        }

        private void UnsubscribeEvents()
        {
            listRoomManager.OnCreateRoom -= OnCreateRoom;
        }

        private async void OnCreateRoom()
        {
            mapName = GetValueFromToggle(mapToggleGroup, ref mapName).ToLower();
            gameMode = GetValueFromToggle(gameModeToggleGroup, ref gameMode).ToLower();
            gameLauncher.GetPlayerDetail();
            try
            {
                if (string.IsNullOrEmpty(mapName))
                {
                    return;
                }
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
                lobbyOptions.IsPrivate = false;
                lobbyOptions.Data = new Dictionary<string, DataObject>()
                {
                    {
                        "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)
                    },
                    {
                        "Map", new DataObject(DataObject.VisibilityOptions.Public, mapName)
                    },
                    {
                        "Ip", new DataObject(DataObject.VisibilityOptions.Member, value: "")
                    },
                    {
                        "Port", new DataObject(DataObject.VisibilityOptions.Member, value: "")
                    }
                };

                lobbyOptions.Player = new Player(
                    data: new Dictionary<string, PlayerDataObject>()
                    {
                        {
                            "PlayerID" , new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, gameLauncher.GetPlayerDetail().unityID)
                        },
                        {
                            "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, gameLauncher.GetPlayerDetail().name)
                        },
                        {
                            "PlayerSkin", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Skin_00")
                        },
                        {
                            "PlayerExpression", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Expression_00")
                        },
                        {
                            "ReadyState", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "Ready")
                        }
                    });
                
                
                listRoomManager.OnDisplayCreateRoomCanvas();
                gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
                
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(gameLauncher.GetPlayerDetail().name + "'s Room", roomMaxPlayers, lobbyOptions);
                listRoomManager.SetCurrentHostLobby(lobbyHost: lobby, lobbyCurrent: lobby);
                await listRoomManager.SubscribeLobbyEvents(true);
                OnFetchPlayer?.Invoke(lobby.Id);
                inRoomManager.SetCodeRoom(lobby.LobbyCode);
                OnSendLobbyData = listRoomManager.GetLobbyCurrent;
                OnNotifyRoomCreated?.Invoke();
                gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
            }
            catch (Exception e)
            {
                OnCreateRoomMessage?.Invoke(e.Message);
                inRoomManager.SetCodeRoom("");
                await listRoomManager.SubscribeLobbyEvents(false);
                listRoomManager.SetCurrentHostLobby(lobbyHost: null, lobbyCurrent: null);
                gameLauncher.UpdatePageState(GameLauncher.PageState.ListRoom);
            }
        }
        
        private string GetValueFromToggle(ToggleGroup toggleGroup, ref string targetValue)
        {
            var toggle = toggleGroup.ActiveToggles().FirstOrDefault();
            
            if (toggle != null)
            {
                targetValue = toggle.GetComponentInChildren<TextMeshProUGUI>().text;
                
            }
            return targetValue;
        }
        
        private void SubscribeToggleGroup()
        {
            foreach (Toggle toggle in gameModeToggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        UpdateValueColor();
                        
                    }
                });
            }
        }

        private void UpdateValueColor()
        {
            Toggle selectedToggle = null;

            // Find the selected toggle within the toggle group
            foreach (Toggle toggle in gameModeToggleGroup.GetComponentsInChildren<Toggle>())
            {
                if (toggle.isOn)
                {
                    selectedToggle = toggle;
                    break;
                }
            }

            if (selectedToggle != null)
            {
                //targetValue = selectedToggle.GetComponentInChildren<TextMeshProUGUI>().text;

                // Set color to red when the toggle is selected
                selectedToggle.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                gameMode = GetValueFromToggle(gameModeToggleGroup, ref gameMode);
                //Debug.Log("MODE: " + gameMode);
                if(gameMode == "ZONE")
                {
                    if(roomMaxPlayers % 2 == 0) return;
                    roomMaxPlayers = 2;
                    playerSizeTextMeshProUGUI.text =  roomMaxPlayers.ToString();
                }

                // Unselect other toggles
                foreach (Toggle otherToggle in gameModeToggleGroup.GetComponentsInChildren<Toggle>())
                {
                    if (otherToggle != selectedToggle)
                    {
                        otherToggle.isOn = false;
                        // Reset color to white for deselected toggles
                        otherToggle.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                    }
                }
            }
            else
            {
                // No toggle is selected, reset color to white for all toggles
                foreach (Toggle otherToggle in gameModeToggleGroup.GetComponentsInChildren<Toggle>())
                {
                    otherToggle.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                }
            }
        }
    }
}
