using System;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField] private Button battleBtn;
        [SerializeField] private Button shopBtn;
        [SerializeField] private Button tutorialBtn;
        [SerializeField] private Button exitGameBtn;
        [SerializeField] private Button quitGameBtn;
        
        [Header("Modified Name")] 
        [SerializeField] private Button modifiedNameBtn;
        [SerializeField] private GameObject askNamePanel;
        [SerializeField] private Button confirmModifiedNameBtn;
        [SerializeField] private Button closeAskNamePanel;
        [SerializeField] private Button listFriendsBtn;

        private GameLauncher gameLauncher;
        private FriendsListManager friendsListManager;

        public Action OnConfirmModifiedName;
        public Action OnBattleGame;
        public Action OnExitAcount;

        private void Awake()
        {
            friendsListManager = GetComponentInChildren<FriendsListManager>();
            gameLauncher = GetComponentInParent<GameLauncher>();
            modifiedNameBtn.onClick.AddListener(() => { DisplayAskNameMenu(); EmitSoundClick.Emit(); });
            confirmModifiedNameBtn.onClick.AddListener(() => { ModifiedName(); EmitSoundClick.Emit(); });

            battleBtn.onClick.AddListener(() => { ProceedToListRoomPage(); EmitSoundClick.Emit(); });

            tutorialBtn.onClick.AddListener(() => { ShowTutorialPage(); EmitSoundClick.Emit(); });

            exitGameBtn.onClick.AddListener(() => { ExitAcount(); EmitSoundClick.Emit(); });

            closeAskNamePanel.onClick.AddListener(() => { DisplayAskNameMenu(); EmitSoundClick.Emit(); });

            quitGameBtn.onClick.AddListener(() => { QuitGame(); EmitSoundClick.Emit(); });

            listFriendsBtn.onClick.AddListener(() => { gameLauncher.UpdatePageState(GameLauncher.PageState.FriendsList); EmitSoundClick.Emit(); });
        }

        private void ExitAcount()
        {
            gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
            AuthenticationService.Instance.SignOut();
            // status = 0
            friendsListManager.EmitUpdateStatus(0, gameLauncher.GetPlayerDetail().unityID);

            gameLauncher.UpdatePageState(GameLauncher.PageState.Account);
        }

        private void QuitGame()
        {
            AuthenticationService.Instance.SignOut();
            //status = 0
            friendsListManager.EmitUpdateStatus(0, gameLauncher.GetPlayerDetail().unityID);

            Application.Quit();
        }

        private void ProceedToListRoomPage()
        {
            gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
            //reset list room here
            //await gameLauncher.FetchListRoom();
            OnBattleGame?.Invoke();
            gameLauncher.UpdatePageState(GameLauncher.PageState.ListRoom);
        }

        private void ModifiedName()
        {
            OnConfirmModifiedName?.Invoke();
        }
        
        public void DisplayAskNameMenu()
        {
            askNamePanel.SetActive(!askNamePanel.activeSelf);
        }

        private void ShowTutorialPage()
        {
            gameLauncher.UpdatePageState(GameLauncher.PageState.Tutorial);
        }

        private void OnApplicationQuit()
        {
            //status = 0
            friendsListManager.EmitUpdateStatus(0, gameLauncher.GetPlayerDetail().unityID);
        }
    }
}
