using System;
using UnityEngine;
using UnityEngine.UI;

namespace Process
{
    public class AccountManager : MonoBehaviour
    {
        public enum  AccountState
        {
            Login,
            Register
        }
        
        private AccountState accountState = AccountState.Login;
        [SerializeField] private GameObject registerPage;
        [SerializeField] private GameObject loginPage;
        
        [SerializeField] private Button confirmLoginBtn;
        [SerializeField] private Button confirmRegisterBtn;
        [SerializeField] private Button quitBtn;
        
        [SerializeField] private Button switchToRegisterBtn;
        [SerializeField] private Button switchToLoginBtn;
        
        public Action OnUserLogin;
        public Action OnUserRegister;


        private void Awake()
        {
            SetButtonEvents();
        }

        private void OnClickLogin()
        {
            OnUserLogin.Invoke();
        }

        private void OnClickRegister()
        {
           OnUserRegister.Invoke();
        }
        
        public void UpdateAccountState(AccountState newAccountState)
        {
            registerPage.SetActive(false);
            loginPage.SetActive(false);
            switch (newAccountState)
            {
                case AccountState.Login:
                    loginPage.SetActive(true);
                    break;
                case AccountState.Register:
                    registerPage.SetActive(true);
                    break;
            }

            accountState = newAccountState;
        }

        private void SetButtonEvents()
        {
            confirmLoginBtn.onClick.AddListener(() => { OnClickLogin(); EmitSoundClick.Emit(); });

            confirmRegisterBtn.onClick.AddListener(() => { OnClickRegister(); EmitSoundClick.Emit(); });

            switchToRegisterBtn.onClick.AddListener(() => { UpdateAccountState(AccountState.Register); EmitSoundClick.Emit(); });
              
            switchToLoginBtn.onClick.AddListener(() => { UpdateAccountState(AccountState.Login); EmitSoundClick.Emit(); });

            quitBtn.onClick.AddListener(() => { Quit(); EmitSoundClick.Emit(); });
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}
