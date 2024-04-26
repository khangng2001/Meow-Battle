using System;
using Managers;
using Process;
using TMPro;
using UnityEngine;

namespace Services
{
    public class ModifiedName : MonoBehaviour
    {
        [Header("PROPS")]
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TextMeshProUGUI greetingText;

        [Header("CLASS REFERENCE")]
        
        [SerializeField] private MainMenuManager mainMenuManager;
        private GameLauncher gameLauncher;

        [Header("EVENT TO SUBSCRIBE")]
        [SerializeField] private SignIn signIn;

        public Action<string> OnMessageModifieldName;

        private void Awake()
        {
            gameLauncher = GetComponentInParent<GameLauncher>();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void UnsubscribeFromEvents()
        {
            mainMenuManager.OnConfirmModifiedName -= SubmitModifiedName;
            signIn.OnPlayerDetailsSignIn -= ActionGreetingName;
        }

        private void SubscribeToEvents()
        {
            signIn.OnPlayerDetailsSignIn += ActionGreetingName;
            mainMenuManager.OnConfirmModifiedName += SubmitModifiedName;
        }

        private async void SubmitModifiedName()
        {
            //PLAYER DETAILS: PUT WITH NEW USERNAME
            try
            {
                gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);

                if (nameInput.text.Trim().Length <= 3)
                {
                    throw new Exception("Name is NOT valid !!!");
                }

                if (gameLauncher.GetPlayerDetail() == null)
                {
                    throw new Exception("The system have some problem: Get Player Detail FAILURE!!!");
                }

                bool check = await SingletonAPI.Instance.PutUpdateName(gameLauncher.GetPlayerDetail(), nameInput.text);

                if (!check)
                {
                    throw new Exception("The system have some problem: Update Name NOT success!!!");
                }

                SetGreetingName(username: nameInput.text);
                gameLauncher.UpdatePageState(GameLauncher.PageState.MenuGame);
                mainMenuManager.DisplayAskNameMenu();
            }
            catch (Exception ex)
            {
                OnMessageModifieldName?.Invoke(ex.Message);
                gameLauncher.UpdatePageState(GameLauncher.PageState.MenuGame);
            }
        }

        private void ActionGreetingName(PlayerDetails playerDetails)
        {
            SetGreetingName(playerDetails.name);
        }

        private void SetGreetingName(string username)
        {
            greetingText.text = $"Hello {username}";
            nameInput.text = "";
        }
    }
}
