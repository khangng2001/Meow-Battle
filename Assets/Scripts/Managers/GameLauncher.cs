using Process;
using System;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameLauncher : MonoBehaviour
    {
        public enum PageState
        {
            Loading,
            Account,
            MenuGame,
            ListRoom,
            InRoom,
            InGame,
            Shop,
            Tutorial,
            FriendsList,
        }

        [SerializeField] private PageState currentPageState = PageState.Loading;
        [SerializeField] private GameObject loadingPage;
        [SerializeField] private GameObject accountPage;
        [SerializeField] private GameObject mainMenuPage;
        [SerializeField] private GameObject listRoomPage;
        [SerializeField] private GameObject inRoomPage;
        [SerializeField] private GameObject ribbon;
        [SerializeField] private GameObject shopPage;
        [SerializeField] private GameObject tutorialPage;
        [SerializeField] private GameObject inGamePage;
        [SerializeField] private GameObject backgroundMain;
        [SerializeField] private GameObject backgroundShop;
        [SerializeField] private GameObject backgroundRoomList;
        [SerializeField] private GameObject friendsListPage;
        


        [Header("Camera")]
        [SerializeField] private GameObject cameraMain;

        private FriendsListManager friendsListManager;

        private AccountManager accountManager;

        private PlayerDetails playerDetails;
        private void Awake()
        {
            Application.runInBackground = true;

            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(loadingPage.GetComponentInParent<Canvas>().gameObject);
            DontDestroyOnLoad(cameraMain);
            
            playerDetails = new PlayerDetails();
            accountManager = GetComponentInChildren<AccountManager>();
            friendsListManager = GetComponentInChildren<FriendsListManager>();
        }

        private async void Start()
        {
            SceneManager.LoadSceneAsync("Main Game", LoadSceneMode.Single);
            

            await UnityServices.InitializeAsync();
            UpdatePageState(PageState.Account);
        }

        public void UpdatePageState(PageState newPageState)
        {
            loadingPage.SetActive(false);
            accountPage.SetActive(false);
            mainMenuPage.SetActive(false);
            listRoomPage.SetActive(false);
            inRoomPage.SetActive(false);
            ribbon.SetActive(false);
            shopPage.SetActive(false);
            tutorialPage.SetActive(false);
            inGamePage.SetActive(false);
            backgroundMain.SetActive(false);
            backgroundShop.SetActive(false);
            backgroundRoomList.SetActive(false);
            inGamePage.SetActive(false);
            friendsListPage.SetActive(false);
            cameraMain.GetComponent<AudioListener>().enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            switch (newPageState)
            {
                case PageState.Loading:
                    loadingPage.SetActive(true);
                    break;
                case PageState.Account:
                    accountPage.SetActive(true);
                    backgroundMain.SetActive(true);

                    // status = 0
                    //friendsListManager.EmitUpdateStatus(0, GetPlayerDetail().unityID);
                    break;
                case PageState.MenuGame:
                    mainMenuPage.SetActive(true);
                    backgroundMain.SetActive(true);

                    // status = 1
                    friendsListManager.EmitUpdateStatus(1, GetPlayerDetail().unityID);
                    break;
                case PageState.ListRoom:
                    listRoomPage.SetActive(true);
                    backgroundMain.SetActive(true);
                    backgroundRoomList.SetActive(true);

                    // status = 1
                    friendsListManager.EmitUpdateStatus(1, GetPlayerDetail().unityID);
                    break;
                case PageState.InRoom:
                    inRoomPage.SetActive(true);
                    ribbon.SetActive(true);
                    // loadingPage.GetComponentInParent<Canvas>().worldCamera = cameraMain.GetComponent<Camera>();

                    //status = 2;
                    friendsListManager.EmitUpdateStatus(2, GetPlayerDetail().unityID);
                    Debug.Log("InRoom");

                    // loadingPage.GetComponentInParent<Canvas>().worldCamera = cameraMain.GetComponent<Camera>();
                    break;
                case PageState.InGame:
                    cameraMain.GetComponent<AudioListener>().enabled = false;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;

                    //status = 2;
                    friendsListManager.EmitUpdateStatus(2, GetPlayerDetail().unityID);
                    break;
                case PageState.Shop:
                    shopPage.SetActive(true);
                    backgroundShop.SetActive(true);

                    // status = 1
                    friendsListManager.EmitUpdateStatus(1, GetPlayerDetail().unityID);
                    break;
                case PageState.Tutorial:
                    tutorialPage.SetActive(true);

                    // status = 1
                    friendsListManager.EmitUpdateStatus(1, GetPlayerDetail().unityID);
                    break;
                case PageState.FriendsList:
                    friendsListPage.SetActive(true);
                    backgroundMain.SetActive(true);
                    // status = 1
                    friendsListManager.EmitUpdateStatus(1, GetPlayerDetail().unityID);
                    break;
            }

            currentPageState = newPageState;
        }

        public void SetPlayerDetail(PlayerDetails player)
        {
            playerDetails = player;
        }

        public PlayerDetails GetPlayerDetail()
        {
            return playerDetails;
        } 
    }
}
