using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Managers;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace Services
{
    public class UpdateLobbyConfig : MonoBehaviour
    {
        private GameLauncher gameLauncher;
        private CreateRoom createRoomEvents;
        private InRoomManager roomManager;

       [SerializeField] private TextMeshProUGUI roomSize;
       [SerializeField] private ToggleGroup gameModeToggleGroup;
       [SerializeField] private ToggleGroup mapNameToggleGroup;
       [SerializeField] private Button confirmUpdateLobbyData;
       [SerializeField] private Button upRoomSize;
       [SerializeField] private Button downRoomSize;

       private string currentGameMode;
       private int currentRoomSize;
       private string currentMapName;
       
        private Lobby currentLobby;

        

        private void Awake()
        {
            gameLauncher = GetComponentInParent<GameLauncher>();
            createRoomEvents = gameLauncher != null ? gameLauncher.GetComponentInChildren<CreateRoom>() : null;
            roomManager = gameLauncher != null ? gameLauncher.GetComponentInChildren<InRoomManager>() : null;
            confirmUpdateLobbyData.onClick.AddListener(OnConfirmUpdateLobbyData);
            upRoomSize.onClick.AddListener(OnIncreasePlayerSize);
            downRoomSize.onClick.AddListener(OnDecreasePlayerSize);
            SubscribeRoomCreatedEvents();
            SubscribeGameModeToggleGroup();
        }

        private void OnDecreasePlayerSize()
        {
            currentGameMode = GetValueFromToggle(gameModeToggleGroup, ref currentGameMode);
            if (Regex.IsMatch(currentGameMode, "(?i)ZONE"))
            {
                currentRoomSize = 2;
                roomSize.text = currentRoomSize.ToString();
            }
            else if (Regex.IsMatch(currentGameMode, "(?i)CLASSIC"))
            {
                if(currentRoomSize <= 2) return;
                currentRoomSize--;
                roomSize.text = currentRoomSize.ToString();
            }
            
        }

        private void OnIncreasePlayerSize()
        {
            currentGameMode = GetValueFromToggle(gameModeToggleGroup, ref currentGameMode);
            if (Regex.IsMatch(currentGameMode, "(?i)ZONE"))
            {
                currentRoomSize = 4;
                roomSize.text = currentRoomSize.ToString();
            }
            else if (Regex.IsMatch(currentGameMode, "(?i)CLASSIC"))
            {
                if(currentRoomSize >= 4) return;
                currentRoomSize++;
                roomSize.text = currentRoomSize.ToString();
            }
        }

        private async void OnConfirmUpdateLobbyData()
        {
            try
            {
                currentGameMode = GetValueFromToggle(mapNameToggleGroup, ref currentGameMode);
                currentMapName = GetValueFromToggle(mapNameToggleGroup, ref currentMapName);
                UpdateLobbyOptions options = new UpdateLobbyOptions();
                
                options.Data = new Dictionary<string, DataObject>()
                {
                    {
                        "GameMode", new DataObject(DataObject.VisibilityOptions.Public, currentGameMode)
                    },
                    {
                        "Map", new DataObject(DataObject.VisibilityOptions.Public, currentMapName)
                    },
                };
                options.MaxPlayers = currentRoomSize;
                var newLobby = await LobbyService.Instance.UpdateLobbyAsync( currentLobby.Id , options);
                roomManager.OnDisplayRoomConfigCanvas();

            }
            catch
            {
                gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
            }
        }

        private void SubscribeRoomCreatedEvents()
        {
            createRoomEvents.OnNotifyRoomCreated += OnNotifyRoomCreated;
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

        private void OnNotifyRoomCreated()
        {
            //Fetch lobby data
            currentLobby = CreateRoom.OnSendLobbyData?.Invoke();
            Debug.Log(currentLobby.Data["GameMode"].Value);
            Debug.Log(currentLobby.Data["Map"].Value.ToUpper());
            
            currentRoomSize = currentLobby.MaxPlayers;
            roomSize.text = currentRoomSize.ToString();
            Toggle[] gameModeToggles = gameModeToggleGroup.GetComponentsInChildren<Toggle>();
            Toggle[] mapToggles = mapNameToggleGroup.GetComponentsInChildren<Toggle>();
            
            foreach (var toggle in gameModeToggles)
            {
                if (toggle.gameObject.name == currentLobby.Data["GameMode"].Value.ToUpper())
                {
                    toggle.isOn = true;
                    break;
                }
            }
            foreach (var toggle in mapToggles)
            {
                if (toggle.gameObject.name == currentLobby.Data["Map"].Value.ToUpper())
                {
                    toggle.isOn = true;
                    break;
                }
            }
            
        }
        
        private void SubscribeGameModeToggleGroup()
        {
            foreach (Toggle toggle in gameModeToggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        UpdateColorToggle();
                    }
                });
            }
        }
        private void UpdateColorToggle()
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
                currentGameMode = GetValueFromToggle(gameModeToggleGroup, ref currentGameMode);
                if (currentGameMode == "ZONE")
                {
                    if(currentRoomSize % 2 == 0) return;
                    currentRoomSize = 2;
                    roomSize.text = currentRoomSize.ToString();
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
