using System;
using Managers;
using Process;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Services
{
    public class SignIn : MonoBehaviour
    {
        [Header("LOGIN")]
        [SerializeField] private TMP_InputField usernameLoginInputField;
        [SerializeField] private TMP_InputField passwordLoginInputField;
        
        private GameLauncher gameLauncher;
        private AccountManager accountManager;

        public Action<string> OnMessageSignIn;
        public Action<PlayerDetails> OnPlayerDetailsSignIn;
        
        private async void Awake()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            gameLauncher = GetComponentInParent<GameLauncher>();
            accountManager = GetComponent<AccountManager>();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private async void UserLogin()
        {
            try
            {
                if (!UserUtilities.Instance.InputValidation(usernameLoginInputField.text, passwordLoginInputField.text))
                {
                    throw new Exception("Please Input all fields!!!");
                }

                gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(usernameLoginInputField.text,
                    passwordLoginInputField.text);

                try
                {
                    //PlayerDetails
                    PlayerDetails player = await SingletonAPI.Instance.GetOnePlayer(AuthenticationService.Instance.PlayerId);
                    if (player == null)
                    {
                        throw new Exception("The system have some problem: Login FAILED!!!");
                    }
                    gameLauncher.SetPlayerDetail(player);
                    OnPlayerDetailsSignIn?.Invoke(player);

                    UserUtilities.Instance.ResetInput(usernameLoginInputField, passwordLoginInputField);
                    gameLauncher.UpdatePageState(GameLauncher.PageState.MenuGame);
                }
                catch (Exception ex)
                {
                    OnMessageSignIn?.Invoke(ex.Message);
                    AuthenticationService.Instance.SignOut(true);
                    gameLauncher.UpdatePageState(GameLauncher.PageState.Account);
                    accountManager.UpdateAccountState(AccountManager.AccountState.Login);
                }
            }
            catch (AuthenticationException ex)
            {
                OnMessageSignIn?.Invoke(ex.Message);
                gameLauncher.UpdatePageState(GameLauncher.PageState.Account);
                accountManager.UpdateAccountState(AccountManager.AccountState.Login);
            }
            catch (RequestFailedException ex)
            {
                OnMessageSignIn?.Invoke(ex.Message);
                gameLauncher.UpdatePageState(GameLauncher.PageState.Account);
                accountManager.UpdateAccountState(AccountManager.AccountState.Login);
            }
            catch (Exception e)
            {
                OnMessageSignIn?.Invoke(e.Message);
                gameLauncher.UpdatePageState(GameLauncher.PageState.Account);
                accountManager.UpdateAccountState(AccountManager.AccountState.Login);
                //UserUtilities.Instance.ResetInput(usernameLoginInputField, passwordLoginInputField);
            }
        }

        private void SubscribeToEvents()
        {
            accountManager.OnUserLogin += UserLogin;
        }
        
        private void UnsubscribeFromEvents()
        {
            accountManager.OnUserLogin -= UserLogin;
        }
    }
}
