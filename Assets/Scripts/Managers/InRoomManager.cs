using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Process;
using Services;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public class InRoomManager : MonoBehaviour
    {
        [SerializeField] private Button startGameBtn;
        [SerializeField] private Button unstartGameBtn;
        [SerializeField] private Button readyGameBtn;
        [SerializeField] private Button unreadyGameBtn;
        [SerializeField] private Button logOutBtn;
        [SerializeField] private TextMeshProUGUI codeRoom;
        
        [Header("SELECT SKIN PROPS")]
        [SerializeField] private Button displaySkinCanvasBtn;
        [SerializeField] private Button chooseSkinBtn;
        [SerializeField] private Button closeSkinCanvasBtn;
        [SerializeField] private GameObject skinCanvas;
        
        
        [Header("SELECT EXPRESSION PROPS")]
        [SerializeField] private Button displayExpressionCanvasBtn;
        [SerializeField] private Button chooseExpressionBtn;
        [SerializeField] private Button closeExpressionCanvasBtn;
        [SerializeField] private GameObject expressionCanvas;

        [Header("SELECT BUDDY PROPS")]
        [SerializeField] private Button displayBuddyCanvasBtn;
        [SerializeField] private Button closeBuddyCanvasBtn;
        [SerializeField] private GameObject buddyCanvas;
        [SerializeField] private ToggleGroup toggleBuddy;
        [SerializeField] private GameObject inviteFriendsCanvas;
        [SerializeField] private GameObject memberCanvas;


        [Header("ROOM RECONFIGURE")] 
        [SerializeField] private Button displayRoomConfigCanvasBtn;
        [SerializeField] private Button closeRoomConfigCanvasBtn;
        [SerializeField] private GameObject roomConfigCanvas;
        
        
        
        [Header("SKIN UI")]
        [SerializeField] private SkinSlotUI prefabSkinSlotUI;
        [SerializeField] private SkinSlotUI prefabExpressionSlotUI;
        [SerializeField] private Transform contentSkinTransform;
        [SerializeField] private Transform contentExpressionTransform;
        [SerializeField] private SkinListSO skinListSO;

        [Header("TOGGLE SKIN")]
        [SerializeField] private ToggleGroup skinToggleGroup;
        [SerializeField] private ToggleGroup expressionToggleGroup;
        
        [Header("EVENTS TO PUBLISH")]
        public Action OnLogOutRoomAction;
        public Action<DataType, SkinSlotUI, Transform, SkinListSO> OnFetchPlayerData;
        public Action OnStartGameAction;
        public Action<StartGameButtonState> OnReadyClick;

        private string valueFromToggle = "";

        private GameLauncher gameLauncher;
        private ListRoomManager listRoomManager;
        private HandlerSkinListSO handlerSkinListSO;
        
        private void Awake()
        {
            gameLauncher = GetComponentInParent<GameLauncher>();
            listRoomManager = GetComponentInParent<ListRoomManager>();
            handlerSkinListSO = gameLauncher.GetComponentInChildren<HandlerSkinListSO>();
            SubscribeEventButtons();

            SubscribeBuddyToggleGroup(toggleBuddy, inviteFriendsCanvas, memberCanvas);
        }
        private void OnLogOutRoom()
        {
            OnLogOutRoomAction?.Invoke();
        }

        private void OnStartGame()
        {
            OnStartGameAction?.Invoke();
        }

        private void SubscribeEventButtons()
        {
            logOutBtn.onClick.AddListener(() => { OnLogOutRoom(); EmitSoundClick.Emit(); });
            displaySkinCanvasBtn.onClick.AddListener(() => { OnDisplaySkinCanvas(); EmitSoundClick.Emit(); });
            displayExpressionCanvasBtn.onClick.AddListener(() => { OnDisplayExpressionCanvas(); EmitSoundClick.Emit(); });
            displayRoomConfigCanvasBtn.onClick.AddListener(() => { OnDisplayRoomConfigCanvas(); EmitSoundClick.Emit(); });
            chooseSkinBtn.onClick.AddListener(() => { OnConfirmChooseSkin(); EmitSoundClick.Emit(); });
            chooseExpressionBtn.onClick.AddListener(() => { OnConfirmChooseExpression(); EmitSoundClick.Emit(); });
            closeBuddyCanvasBtn.onClick.AddListener(OnDisplayBuddyCanvas);
            displayBuddyCanvasBtn.onClick.AddListener(() => { OnDisplayBuddyCanvas(); EmitSoundClick.Emit(); });
            //displayBuddyCanvasBtn.onClick.AddListener();

            closeExpressionCanvasBtn.onClick.AddListener(() =>
            {
                expressionCanvas.SetActive(!expressionCanvas.activeSelf);
                EmitSoundClick.Emit();
            });
            
            closeSkinCanvasBtn.onClick.AddListener((() =>
            {
                skinCanvas.SetActive(!skinCanvas.activeSelf);
                EmitSoundClick.Emit();
            }));
            
            readyGameBtn.onClick.AddListener(() =>
            {
                SwitchButtonState(StartGameButtonState.None);
                OnReadyClick?.Invoke(StartGameButtonState.Ready);
                EmitSoundClick.Emit();
            });
            unreadyGameBtn.onClick.AddListener(() =>
            {
                SwitchButtonState(StartGameButtonState.None);
                OnReadyClick?.Invoke(StartGameButtonState.UnReady);
                EmitSoundClick.Emit();
            });
            startGameBtn.onClick.AddListener(() =>
            {
                SwitchButtonState(StartGameButtonState.None);
                OnStartGame();
                EmitSoundClick.Emit();
            });
            unstartGameBtn.onClick.AddListener(() =>
            {
                // SwitchButtonState(StartGameButtonState.None);
                EmitSoundClick.Emit();
            });

        }

        public void OnDisplayRoomConfigCanvas()
        {
            roomConfigCanvas.SetActive(!roomConfigCanvas.activeSelf);
        }

        private async void OnConfirmChooseExpression()
        {
            string nameExpression = GetValueFromToggle(expressionToggleGroup, ref this.valueFromToggle);
            
            // Get Expression
            List<Material> expressions = new List<Material>();
            foreach (SkinSO item in handlerSkinListSO.SkinListFullSO.expressionSOList)
            {
                expressions.Add(item.GetSkinMaterial());
            }
            Material expressionFinded = expressions.Find((Material material) => { if (material.name == nameExpression) return true;return false; });
            if (expressionFinded != null) handlerSkinListSO.ExpressionCurrent.CopyPropertiesFromMaterial(expressionFinded);
            
            await UpdatePlayerExpressionData(listRoomManager.GetLobbyCurrent().Id, nameExpression);
            OnDisplayExpressionCanvas();
        }

        private async void OnConfirmChooseSkin()
        {
            string nameSkin = GetValueFromToggle(skinToggleGroup, ref this.valueFromToggle);

            // Get Skins
            List<Material> skins = new List<Material>();
            foreach (SkinSO item in handlerSkinListSO.SkinListFullSO.skinSOList)
            {
                skins.Add(item.GetSkinMaterial());
            }
            Material skinFinded = skins.Find((Material material) => { if (material.name == nameSkin) return true;return false; });
            if (skinFinded != null) handlerSkinListSO.SkinCurrent.CopyPropertiesFromMaterial(skinFinded);
            
            await UpdatePlayerSkinData(listRoomManager.GetLobbyCurrent().Id, nameSkin);
            OnDisplaySkinCanvas();
        }

        private void SubscribeBuddyToggleGroup(ToggleGroup toggleGroup, GameObject invite, GameObject member)
        {
            foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
            {
                toggle.onValueChanged.AddListener((ison) =>
                {
                    if (ison)
                    {
                        Debug.Log(toggle.name);

                        if (toggle.name == "Invite Friends")
                        {
                            //Debug.Log(toggle.name);
                            invite.SetActive(true);
                            member.SetActive(false);
                        }
                        else
                        {
                            invite.SetActive(false);
                            member.SetActive(true);
                        }
                    }
                });
            }
        }

        private void OnDisplayBuddyCanvas()
        {
            if (skinCanvas.activeSelf || expressionCanvas.activeSelf)
            {
                skinCanvas.SetActive(false);
                expressionCanvas.SetActive(false);
            }
            buddyCanvas.SetActive(!buddyCanvas.activeSelf);
        }

        private void OnDisplayExpressionCanvas()
        {
            if (skinCanvas.activeSelf)
            {
                skinCanvas.SetActive(!skinCanvas.activeSelf);
            }
            expressionCanvas.SetActive(!expressionCanvas.activeSelf);   
            
            if (contentExpressionTransform.childCount >= skinListSO.expressionSOList.Count) return;

            foreach (Transform child in contentExpressionTransform)
            {
                Destroy(child);
            }
            OnFetchPlayerData?.Invoke(DataType.Expression, prefabExpressionSlotUI, contentExpressionTransform, skinListSO);
        }

        private void OnDisplaySkinCanvas()
        {
            if (expressionCanvas.activeSelf)
            {
                expressionCanvas.SetActive(!expressionCanvas.activeSelf);  
            }
            skinCanvas.SetActive(!skinCanvas.activeSelf);

            if (contentSkinTransform.childCount >= skinListSO.skinSOList.Count) return;

            foreach (Transform child in contentSkinTransform)
            {
                Destroy(child);
            }
            OnFetchPlayerData?.Invoke(DataType.Skin, prefabSkinSlotUI, contentSkinTransform, skinListSO);
        }

        public void SetCodeRoom(string code)
        {
            codeRoom.text = "Code: " + code;
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
        
        private async Task UpdatePlayerSkinData(string lobbyId, string nameSkin)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "PlayerSkin", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: nameSkin)
                    }
                };

                PlayerDetails playerDetails = gameLauncher.GetPlayerDetail();

                await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerDetails.unityID, options);

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        
        private async Task UpdatePlayerExpressionData(string lobbyId, string nameExpression)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "PlayerExpression", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: nameExpression)
                    }
                };

                PlayerDetails playerDetails = gameLauncher.GetPlayerDetail();

                await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerDetails.unityID, options);

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        
        public async Task UpdateSkinOtherPlayer(int indexPlayer)
        {
            try
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId: listRoomManager.GetLobbyCurrent().Id);
                Player player = lobby.Players[indexPlayer];
                if (player.Id == gameLauncher.GetPlayerDetail().unityID) return;

                // Get Skins
                List<Material> skins = new List<Material>();
                foreach (SkinSO item in handlerSkinListSO.SkinListFullSO.skinSOList)
                {
                    skins.Add(item.GetSkinMaterial());
                }
                // Get Expression
                List<Material> expressions = new List<Material>();
                foreach (SkinSO item in handlerSkinListSO.SkinListFullSO.expressionSOList)
                {
                    expressions.Add(item.GetSkinMaterial());
                }

                // Find Skin And Express In Asset
                Material skinFinded = skins.Find((Material material) => { if (material.name == player.Data["PlayerSkin"].Value) return true; return false; });
                Material expressionFinded = expressions.Find((Material material) => { if (material.name == player.Data["PlayerExpression"].Value) return true; return false; });

                // Ready Check
                bool isReady;
                if (player.Data["ReadyState"].Value == "Ready") isReady = true;
                else isReady = false;
                
                // Apply
                Dictionary<string, GameObject> playerList = GetComponent<FetchPlayer>().PLayerList;
                playerList.TryGetValue(player.Data["PlayerID"].Value, out GameObject playerGameObject);
                if (playerGameObject != null)
                {
                    // Skin / Expression
                    playerGameObject.GetComponentInChildren<SkinnedMeshRenderer>().SetMaterials(new List<Material>()
                    {
                        skinFinded,
                        expressionFinded
                    });
                    
                    // Ready Check
                    playerGameObject.transform.GetChild(2).gameObject.SetActive(isReady);
                }

                listRoomManager.SetCurrentHostLobby(listRoomManager.GetLobbyHost(), lobby);
            }
            catch
            {
                await UpdateSkinOtherPlayer(indexPlayer);
                //gameLauncher.UpdatePageState(GameLauncher.PageState.InRoom);
            }
        }

        public void SwitchButtonState(StartGameButtonState startGameButtonState)
        {
            startGameBtn.gameObject.SetActive(false);
            unstartGameBtn.gameObject.SetActive(false);
            readyGameBtn.gameObject.SetActive(false);
            unreadyGameBtn.gameObject.SetActive(false);
            logOutBtn.gameObject.SetActive(false);
            displaySkinCanvasBtn.gameObject.SetActive(false);
            displayExpressionCanvasBtn.gameObject.SetActive(false);
            displayBuddyCanvasBtn.gameObject.SetActive(false);
            displayRoomConfigCanvasBtn.gameObject.SetActive(false);

            switch (startGameButtonState)
            {
                case StartGameButtonState.Play:
                    startGameBtn.gameObject.SetActive(true);
                    logOutBtn.gameObject.SetActive(true);
                    displaySkinCanvasBtn.gameObject.SetActive(true);
                    displayExpressionCanvasBtn.gameObject.SetActive(true);
                    displayBuddyCanvasBtn.gameObject.SetActive(true);
                    //displayRoomConfigCanvasBtn.gameObject.SetActive(true);
                    break;
                case StartGameButtonState.UnPlay:
                    unstartGameBtn.gameObject.SetActive(true);
                    logOutBtn.gameObject.SetActive(true);
                    displaySkinCanvasBtn.gameObject.SetActive(true);
                    displayExpressionCanvasBtn.gameObject.SetActive(true);
                    displayBuddyCanvasBtn.gameObject.SetActive(true);
                    //displayRoomConfigCanvasBtn.gameObject.SetActive(true);
                    break;
                case StartGameButtonState.Ready:
                    readyGameBtn.gameObject.SetActive(true);
                    logOutBtn.gameObject.SetActive(true);
                    displaySkinCanvasBtn.gameObject.SetActive(true);
                    displayExpressionCanvasBtn.gameObject.SetActive(true);
                    displayBuddyCanvasBtn.gameObject.SetActive(true);
                    break;
                case StartGameButtonState.UnReady:
                    unreadyGameBtn.gameObject.SetActive(true);
                    break;
                case StartGameButtonState.None:
                    break;
            }
        }
    }
}
