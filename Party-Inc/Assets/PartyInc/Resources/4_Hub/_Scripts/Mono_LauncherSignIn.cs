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

            [SerializeField] private Text _statusText;
            private bool _signingIn;
            private float _triggerTimer = 1.0f;

            // Update is called once per frame
            void Update()
            {
                if (_signingIn)
                {
                    if (_triggerTimer < 0f)
                    {
                        _triggerTimer = 1.0f;
                    }

                    string signingIn = "";

                    if (_triggerTimer <= 1.0f && _triggerTimer > 0.67f)
                    {
                        signingIn = "Signing in .";
                    }
                    else if (_triggerTimer <= 0.67f && _triggerTimer > 0.34f)
                    {
                        signingIn = "Signing in . .";
                    }
                    else if (_triggerTimer <= 0.34f && _triggerTimer > 0.0f)
                    {
                        signingIn = "Signing in . . .";
                    }

                    _statusText.text = signingIn;

                    _triggerTimer -= Time.deltaTime;
                }
                else if (_statusText.text != "")
                {
                    _statusText.text = "";
                }
            }

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
                _signingIn = true;
                PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.SignInEmailPassword(_emailField.text, _passwordField.text, (result) =>
                {
                    _signingIn = false;
                    if (result.success)
                    {
                        SceneManager.LoadScene(Stt_SceneIndexes.HUB);
                    }
                    else
                    {
                        Debug.Log("Authentication failed: " + result.exceptions[0].Message);
                    }
                });
            }

            public void SignUp()
            {
                SceneManager.LoadScene(Stt_SceneIndexes.LAUNCHER_SIGNUP);
            }
        }
    }
}