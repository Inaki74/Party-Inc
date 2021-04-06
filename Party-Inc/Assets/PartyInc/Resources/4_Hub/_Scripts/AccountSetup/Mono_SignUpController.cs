using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase;

namespace PartyInc
{
    using PartyFirebase.Firestore;
    using PartyFirebase.Auth;
    namespace Hub
    {
        public class Mono_SignUpController : MonoBehaviour
        {
            [SerializeField] private InputField _nicknameField;
            [SerializeField] private InputField _emailField;
            [SerializeField] private InputField _passwordField;
            [SerializeField] private InputField _passwordVerificationField;
            [SerializeField] private InputField _DOBField;
            [SerializeField] private InputField _cityField;
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
                        signingUp = ".";
                    }else if (_triggerTimer <= 0.67f && _triggerTimer > 0.34f)
                    {
                        signingUp = ". .";
                    }
                    else if (_triggerTimer <= 0.34f && _triggerTimer > 0.0f)
                    {
                        signingUp = ". . .";
                    }

                    _statusText.text = signingUp;

                    _triggerTimer -= Time.deltaTime;
                }
                else if(_statusText.text != "")
                {
                    _statusText.text = "";
                }
            }

            private Timestamp DMYDateToFirebaseTimestamp(string date)
            {
                string[] decomposedDate = date.Split('/');

                string day = decomposedDate[1];
                string month = decomposedDate[0];
                string year = decomposedDate[2];

                DateTime thisDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                return Timestamp.FromDateTime(thisDate);
            }

            private bool ValidateBirthdate(string dob)
            {
                if (string.IsNullOrEmpty(dob))
                {
                    Fb_FirebaseAuthenticateManager.Current.SetErrorMessage("Please provide a birthdate!", Color.yellow);

                    return false;
                }

                string[] decomposedDate = dob.Split('/');

                int day = int.Parse(decomposedDate[1]);
                int month = int.Parse(decomposedDate[0]);
                int year = int.Parse(decomposedDate[2]);

                if(year < 1900 || year > DateTime.Now.Year)
                {
                    // Unacceptable year
                    Fb_FirebaseAuthenticateManager.Current.SetErrorMessage("That year is not valid!", Color.yellow);

                    return false;
                }

                if(year == DateTime.Now.Year)
                {
                    if(month > DateTime.Now.Month || (month == DateTime.Now.Month && day > DateTime.Now.Day))
                    {
                        Fb_FirebaseAuthenticateManager.Current.SetErrorMessage("That's the future, Terminator.", Color.yellow);

                        return false;
                    }
                }

                if(month <= 0 || month > 12)
                {
                    // Unacceptable month
                    Fb_FirebaseAuthenticateManager.Current.SetErrorMessage("That month doesn't exist!", Color.yellow);

                    return false;
                }

                bool isLeapYear = year % 4 == 0 && !(year % 100 == 0 && !(year % 400 == 0)); // Its a leap year if its divisible by 4, except for years that are divisible by 100 and not divisible by 400 at the same time.
                bool isPairMonth = month % 2 == 0; // If its a pair month (except february) it has 30 days

                int upperBoundDay = 0;
                int lowerBoundDay = 0;

                if(month == 2)
                {
                    //Its february
                    if (isLeapYear)
                    {
                        // If its a leap year, then the range is 0 - 29
                        upperBoundDay = 29;
                    }
                    else
                    {
                        // If its not a leap year, then the range is 0 - 28
                        upperBoundDay = 28;
                    }
                }
                else
                {
                    // Its not february
                    if (isPairMonth)
                    {
                        // If its pair, then its a range of 0 - 30
                        upperBoundDay = 30;
                    }
                    else
                    {
                        // If its not pair, then its a range of 0 - 31
                        upperBoundDay = 31;
                    }
                }

                if(day < lowerBoundDay || day > upperBoundDay)
                {
                    // Unacceptable day
                    Fb_FirebaseAuthenticateManager.Current.SetErrorMessage("That day doesn't exist!", Color.yellow);

                    return false;
                }

                return true;
            }

            private bool ValidateLanguage(string lang)
            {
                if (string.IsNullOrEmpty(lang))
                {
                    Fb_FirebaseAuthenticateManager.Current.SetErrorMessage("Please provide a language!", Color.yellow);

                    return false;
                }

                return true;
            }

            private bool ValidateCity(string cit)
            {
                if (string.IsNullOrEmpty(cit))
                {
                    Fb_FirebaseAuthenticateManager.Current.SetErrorMessage("Please provide a location!", Color.yellow);

                    return false;
                }

                return true;
            }

            private IEnumerator GoToHubCo()
            {
                Fb_FirestoreSession.Current.Setup();

                yield return new WaitUntil(() => Fb_FirestoreSession.Current.SetupCompleted);

                yield return new WaitUntil(() => Mng_PhotonManager.Current.PhotonAuthComplete);

                Mng_SceneNavigationSystem.Current.DeactivateActiveScene();
                Mng_SceneNavigationSystem.Current.ActivateLoadedScene((int)Stt_SceneIndexes.HUB);

                _signingUp = false;
            }

            public void SignUp()
            {
                _signingUp = true;

                if (!ValidateBirthdate(_DOBField.text) || !ValidateLanguage(_languageField.text) || !ValidateCity(_cityField.text))
                {
                    _signingUp = false;
                    return;
                }

                Fb_FirebaseAuthenticateManager.Current.SignUpEmailPassword(_emailField.text, _passwordField.text, _passwordVerificationField.text, _nicknameField.text, SignUpOnFirestore);
            }

            public void SignUpOnFirestore(FireauthCallResult result)
            {
                if (result.success)
                {
                    Fb_FirestoreStructures.FSPlayer newPlayer = new Fb_FirestoreStructures.FSPlayer();
                    Fb_FirestoreStructures.FSPlayer.FSData newData = new Fb_FirestoreStructures.FSPlayer.FSData();

                    newData.nickname = _nicknameField.text;
                    newData.city = _cityField.text;
                    newData.birthdate = DMYDateToFirebaseTimestamp(_DOBField.text);
                    newData.language = _languageField.text;

                    newPlayer.data = newData.ToDictionary();

                    // ADD TO PLAYERS
                    Fb_FirestoreManager.Current.Add(Fb_FirestoreManager.Current.Players, newPlayer.ToDictionary(), result.uid, res =>
                    {
                        if (res.success)
                        {
                            Debug.Log("PLAYER ADDED");

                            StartCoroutine(GoToHubCo());
                        }
                        else
                        {
                            _signingUp = false;
                            Debug.Log("COULDNT ADD PLAYER");
                            Debug.Log(res.exceptions[0].Message);
                        }
                    });

                    Fb_FirestoreStructures.FSPlayerSocial newPlayerSoc = new Fb_FirestoreStructures.FSPlayerSocial();

                    // ADD TO PLAYERSOCIAL
                    Fb_FirestoreManager.Current.Add(Fb_FirestoreManager.Current.PlayerSocial, newPlayerSoc.ToDictionary(), result.uid, res =>
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
                else
                {
                    _signingUp = false;
                    Debug.Log("Authentication failed: " + result.exceptions[0].Message);
                }
            }

            public void BtnBack()
            {
                Mng_SceneNavigationSystem.Current.DeactivateActiveScene();
                Mng_SceneNavigationSystem.Current.ActivateLoadedScene((int)Stt_SceneIndexes.PLAYER_FORK);
            }
        }
    }
}


