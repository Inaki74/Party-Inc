using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_LauncherSignIn : MonoBehaviour
        {
            [SerializeField] private InputField _emailField;
            [SerializeField] private InputField _passwordField;

            [SerializeField] private Button _signInButton;
            [SerializeField] private Button _signUpButton;

            // Start is called before the first frame update
            void Start()
            {
                _signInButton.interactable = false;
                _signUpButton.interactable = false;

                PartyFirebase.Fb_FirebaseManager.Current.InitializeService(InitButtons);
            }


            private void InitButtons()
            {
                _signInButton.interactable = true;
                _signUpButton.interactable = true;
            }

            public void SignIn()
            {
                PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.SignInEmailPassword(_emailField.text, _passwordField.text);
            }

            public void SignUp()
            {
                SceneManager.LoadScene(Stt_SceneIndexes.LAUNCHER_SIGNUP);
            }
        }
    }
}