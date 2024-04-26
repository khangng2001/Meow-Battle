using System;
using Managers;
using Process;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Services
{
    public class Register : MonoBehaviour
    {
        [Header("REGISTER")]
        
        [SerializeField] private TMP_InputField usernameRegisterInputField;
        [SerializeField] private TMP_InputField passwordRegisterInputField;
        
        private GameLauncher gameLauncher;
        private AccountManager accountManager;

        public Action<string> OnMessageRegister;

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
        private void SubscribeToEvents()
        {
            if (accountManager != null)
            {
                accountManager.OnUserRegister += UserRegister;
            }
        }
        private void UnsubscribeFromEvents()
        {
            if (accountManager != null)
            {
                accountManager.OnUserRegister -= UserRegister;
            }
        }
        
        
        private async void UserRegister()
        {
            try
            {
                if (!UserUtilities.Instance.InputValidation(usernameRegisterInputField.text,
                    passwordRegisterInputField.text))
                {
                    throw new Exception("Please Input all fields!!!");
                }

                gameLauncher.UpdatePageState(GameLauncher.PageState.Loading);
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(
                usernameRegisterInputField.text, passwordRegisterInputField.text);

                // If Register success in Unity but API Post error it will delete this account in Unity Management
                try
                {
                    //PlayerDetails
                    PlayerDetails playerDetails = new PlayerDetails()
                    {
                        name = usernameRegisterInputField.text,
                        unityID = AuthenticationService.Instance.PlayerId,
                        skins = new string[] { "skin_00", "skin_01", "skin_02", "skin_03" },
                        expression = new string[] { "eps_00", "eps_01", "eps_02", "eps_03" },
                        status = 0,
                        coin = 2000,
                        listfriends = new string[] { }
                    };
                    bool check = await SingletonAPI.Instance.PostAddPlayer(playerDetails);
                    if (!check)
                    {
                        throw new Exception("The system have some problem: Server connection problem!!!");
                    }

                    // Register and Post API success
                    UserUtilities.Instance.ResetInput(usernameRegisterInputField, passwordRegisterInputField);
                    AuthenticationService.Instance.SignOut(true);
                    gameLauncher.UpdatePageState(GameLauncher.PageState.Account);
                    accountManager.UpdateAccountState(AccountManager.AccountState.Login);
                }
                catch (Exception e)
                {
                    OnMessageRegister?.Invoke(e.Message);
                    await AuthenticationService.Instance.DeleteAccountAsync();
                    gameLauncher.UpdatePageState(GameLauncher.PageState.Account);
                    accountManager.UpdateAccountState(AccountManager.AccountState.Register);
                }
            }
            catch (AuthenticationException ex)
            {
                OnMessageRegister?.Invoke(ex.Message);
                gameLauncher.UpdatePageState(GameLauncher.PageState.Account);
                accountManager.UpdateAccountState(AccountManager.AccountState.Register);
            }
            catch (RequestFailedException ex)
            {
                OnMessageRegister?.Invoke(ex.Message);
                gameLauncher.UpdatePageState(GameLauncher.PageState.Account);
                accountManager.UpdateAccountState(AccountManager.AccountState.Register);
            }
            catch (Exception e)
            {
                OnMessageRegister?.Invoke(e.Message);
                //Debug.LogException(e);
                gameLauncher.UpdatePageState(GameLauncher.PageState.Account);
                accountManager.UpdateAccountState(AccountManager.AccountState.Register);
                //UserUtilities.Instance.ResetInput(usernameRegisterInputField, passwordRegisterInputField);
            }
        }
    }
}
