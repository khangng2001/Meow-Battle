using System;
using TMPro;
using UnityEngine;

namespace Process
{
    public class UserUtilities : MonoBehaviour
    {
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject); // Ensures only one instance exists
            }
        }

        public static UserUtilities Instance { get; private set; }
    
        public bool InputValidation(string username, string password)
        {
            return username.Trim().Length > 0 || password.Trim().Length > 0;
        }

        public void ResetInput(TMP_InputField usernameInputField, TMP_InputField passwordInputField)
        {
            usernameInputField.text = "";
            passwordInputField.text = "";
        }
        
        
        
    }
}
