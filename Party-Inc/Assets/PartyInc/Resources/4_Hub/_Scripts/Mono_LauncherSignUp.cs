using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace PartyInc
{
    using PartyFirebase.Firestore;
    using PartyFirebase.Auth;
    namespace Hub
    {
        public class Mono_LauncherSignUp : MonoBehaviour
        {
            [SerializeField] private InputField _nicknameField;
            [SerializeField] private InputField _emailField;
            [SerializeField] private InputField _passwordField;
            [SerializeField] private InputField _passwordVerificationField;
            [SerializeField] private InputField _cityField;
            [SerializeField] private InputField _countryField;
            [SerializeField] private InputField _languageField;

            [SerializeField] private Text _statusText;
            private bool _signingUp;
            private float _triggerTimer = 1.0f;

            // Update is called once per frame
            void Update()
            {
                if (_signingUp)
                {
                    if(_triggerTimer < 0f)
                    {
                        _triggerTimer = 1.0f;
                    }

                    string signingUp = "";

                    if(_triggerTimer <= 1.0f && _triggerTimer > 0.67f)
                    {
                        signingUp = "Signing up .";
                    }else if (_triggerTimer <= 0.67f && _triggerTimer > 0.34f)
                    {
                        signingUp = "Signing up . .";
                    }
                    else if (_triggerTimer <= 0.34f && _triggerTimer > 0.0f)
                    {
                        signingUp = "Signing up . . .";
                    }

                    _statusText.text = signingUp;

                    _triggerTimer -= Time.deltaTime;
                }
                else if(_statusText.text != "")
                {
                    _statusText.text = "";
                }
            }

            public void SignUp()
            {
                _signingUp = true;
                Fb_FirebaseAuthenticateManager.Current.SignUpEmailPassword(_emailField.text, _passwordField.text, _passwordVerificationField.text, _nicknameField.text, SignUpOnFirestore);
            }

            public void SignUpOnFirestore(string userId)
            {
                Fb_FirestoreStructures.FSPlayer newPlayer = new Fb_FirestoreStructures.FSPlayer();
                Fb_FirestoreStructures.FSPlayer.FSData newData = new Fb_FirestoreStructures.FSPlayer.FSData();

                newData.nickname = _nicknameField.text;
                newData.city = _cityField.text;
                newData.country = _countryField.text;
                newData.language = _languageField.text;

                newPlayer.data = newData.ToDictionary();

                // ADD TO PLAYERS
                Fb_FirestoreManager.Current.Add(Fb_FirestoreManager.Current.Players, newPlayer.ToDictionary(), userId, res =>
                {
                    _signingUp = false;
                    if (res.success)
                    {
                        Debug.Log("PLAYER ADDED");

                        SceneManager.LoadScene(Stt_SceneIndexes.HUB);
                    }
                    else
                    {
                        Debug.Log("COULDNT ADD PLAYER");
                        Debug.Log(res.exceptions[0].Message);
                    }
                });

                Fb_FirestoreStructures.FSPlayerSocial newPlayerSoc = new Fb_FirestoreStructures.FSPlayerSocial();


                // ADD TO PLAYERSOCIAL
                Fb_FirestoreManager.Current.Add(Fb_FirestoreManager.Current.Players, newPlayer.ToDictionary(), userId, res =>
                {
                    if (res.success)
                    {
                        Debug.Log("PLAYER SOCIAL ADDED");
                    }
                    else
                    {
                        Debug.Log("COULDNT ADD PLAYER");
                        Debug.Log(res.exceptions[0].Message);
                    }
                });
            }

            public void BackToSignIn()
            {
                SceneManager.LoadScene(Stt_SceneIndexes.LAUNCHER_SIGNIN);
            }
        }
    }
}


