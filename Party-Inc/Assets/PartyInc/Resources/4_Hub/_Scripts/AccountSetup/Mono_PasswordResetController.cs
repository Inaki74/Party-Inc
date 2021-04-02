using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        using PartyFirebase.Auth;
        public class Mono_PasswordResetController : MonoBehaviour
        {
            [SerializeField] private GameObject _signInForm;
            [SerializeField] private GameObject _resetPasswordForm;
            [SerializeField] private GameObject _requestUI;
            [SerializeField] private GameObject _completedUI;


            [SerializeField] private InputField _emailInput;

            [SerializeField] private Text _statusText;
            private bool _processing;
            private float _triggerTimer = 1.0f;

            // Update is called once per frame
            void Update()
            {
                if (_processing)
                {
                    if (_triggerTimer < 0f)
                    {
                        _triggerTimer = 1.0f;
                    }

                    string signingIn = "";

                    if (_triggerTimer <= 1.0f && _triggerTimer > 0.67f)
                    {
                        signingIn = ".";
                    }
                    else if (_triggerTimer <= 0.67f && _triggerTimer > 0.34f)
                    {
                        signingIn = ". .";
                    }
                    else if (_triggerTimer <= 0.34f && _triggerTimer > 0.0f)
                    {
                        signingIn = ". . .";
                    }

                    _statusText.text = signingIn;

                    _triggerTimer -= Time.deltaTime;
                }
                else if (_statusText.text != "")
                {
                    _statusText.text = "";
                }
            }

            public void BtnRequestPasswordChange()
            {
                _processing = true;
                string email = _emailInput.text;

                Fb_FirebaseAuthenticateManager.Current.PasswordChangeRequest(email, res =>
                {
                    _processing = false;
                    if (res.success)
                    {
                        _requestUI.SetActive(false);
                        _completedUI.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Password reset failed: " + res.exceptions[0].Message);
                    }
                });
            }

            public void BtnBackToSignIn()
            {
                if (_signInForm.activeInHierarchy) return;

                _requestUI.SetActive(true);
                _completedUI.SetActive(false);

                _signInForm.SetActive(true);
                _resetPasswordForm.SetActive(false);
            }
        }
    }
}


