using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

namespace PartyInc
{
    using PartyFirebase.Firestore;
    namespace PartyFirebase.Auth
    {
        public struct FireauthCallResult
        {
            public bool success;
            public string uid;
            public string username;
            public List<Firebase.FirebaseException> exceptions;
        }

        public class Fb_FirebaseAuthenticateManager : MonoSingleton<Fb_FirebaseAuthenticateManager>
        {
            private FirebaseAuth _auth;
            public FirebaseAuth Auth
            {
                get
                {
                    return _auth;
                }
                private set
                {
                    _auth = value;
                }
            }

            [SerializeField] private Text _errorText;
            private bool _runStartOnce;

            private void Start()
            {
                if (_runStartOnce) return;

                _runStartOnce = true;
                Fb_FirebaseManager.Current.InitializeService(AuthInit);
            }

            // Update is called once per frame
            void Update()
            {

            }

            private void AuthInit()
            {
                _auth = FirebaseAuth.DefaultInstance;
            }

            public void SignInEmailPassword(string email, string password, Action<FireauthCallResult> Callback = null)
            {
                FireauthCallResult result = new FireauthCallResult();
                result.exceptions = new List<Firebase.FirebaseException>();

                Debug.Log("Attempting to sign in with: " + email + " " + password);

                _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log("Faulted");

                        AuthError error = (AuthError)(task.Exception.Flatten().InnerException as Firebase.FirebaseException).ErrorCode;
                        ManageErrorCodes(error);

                        result.exceptions.Add(task.Exception.Flatten().InnerException as Firebase.FirebaseException);
                        result.success = false;
                        result.uid = null;
                        result.username = null;

                        Callback(result);
                        return;
                    }

                    if (task.IsCompleted)
                    {
                        Debug.Log("User with email: " + _auth.CurrentUser.Email + " logged in!");

                        result.success = true;
                        result.uid = task.Result.UserId;
                        result.username = task.Result.DisplayName;

                        Callback(result);
                        return;
                    }
                });
            }

            public void SignUpEmailPassword(string email, string password, string verification, string username, Action<FireauthCallResult> Callback = null)
            {
                FireauthCallResult result = new FireauthCallResult();
                result.exceptions = new List<Firebase.FirebaseException>();
                Debug.Log("Attempting to sign up with: " + email + " " + password);

                if (password != verification)
                {
                    SetErrorMessage("Password and verification dont match!", Color.red);

                    result.exceptions.Add(new Firebase.FirebaseException((int)AuthError.Cancelled, "Password and verification dont match!"));
                    result.success = false;
                    result.uid = null;
                    result.username = null;

                    Callback(result);
                    return;
                }

                _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        AuthError error = (AuthError)(task.Exception.Flatten().InnerException as Firebase.FirebaseException).ErrorCode;
                        ManageErrorCodes(error);

                        result.exceptions.Add(task.Exception.Flatten().InnerException as Firebase.FirebaseException);
                        result.success = false;
                        result.uid = null;
                        result.username = null;
                        
                        Callback(result);
                        return;
                    }

                    if (task.IsCompleted)
                    {
                        Debug.Log("User with email: " + email + " added!");

                        UserProfile profile = new UserProfile
                        {
                            DisplayName = username,
                        };

                        _auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task2 =>
                        {
                            if (task2.IsFaulted || task2.IsCanceled)
                            {
                                Debug.Log("Faulted");

                                AuthError error = (AuthError)(task2.Exception.Flatten().InnerException as Firebase.FirebaseException).ErrorCode;
                                ManageErrorCodes(error);

                                result.exceptions.Add(task.Exception.Flatten().InnerException as Firebase.FirebaseException);
                                result.success = false;
                                result.uid = null;
                                result.username = null;
                                Debug.Log("Setting of username failed: " + error.ToString());

                                Callback(result);
                                return;
                            }

                            if (task2.IsCompleted)
                            {
                                Debug.Log("Username set!");

                                result.success = true;
                                result.uid = task.Result.UserId;
                                result.username = username;

                                Callback(result);
                                return;
                            }
                        });
                    }
                });
            }

            private void SetErrorMessage(string message, Color state)
            {
                _errorText.color = state;
                _errorText.text = message;

                StopCoroutine(ResetErrorMessage());
                StartCoroutine(ResetErrorMessage());
            }

            private IEnumerator ResetErrorMessage()
            {
                yield return new WaitForSeconds(5f);

                _errorText.text = "";
            }

            public void ManageErrorCodes(AuthError errorCode)
            {
                switch (errorCode)
                {
                    case AuthError.EmailAlreadyInUse: // 
                        SetErrorMessage("That email is already in use!", Color.yellow);
                        break;
                    case AuthError.InvalidEmail: // 
                        SetErrorMessage("That email is invalid!", Color.red);
                        break;
                    case AuthError.MissingEmail: // 
                        SetErrorMessage("Please provide an email!", Color.yellow);
                        break;
                    case AuthError.MissingPassword: // 
                        SetErrorMessage("Please provide a Password!", Color.yellow);
                        break;
                    case AuthError.WrongPassword: // 
                        SetErrorMessage("That password isn't correct!", Color.red);
                        break;
                    case AuthError.AccountExistsWithDifferentCredentials: // 
                        SetErrorMessage("That account exists already!", Color.red);
                        break;
                    case AuthError.UserNotFound: // 
                        SetErrorMessage("That account doesn't exist!", Color.red);
                        break;
                }
            }
        }
    }
        
}