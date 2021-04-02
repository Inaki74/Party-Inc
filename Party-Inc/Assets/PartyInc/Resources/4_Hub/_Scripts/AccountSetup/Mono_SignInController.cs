using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PartyInc.PartyFirebase.Firestore;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_SignInController : MonoBehaviour
        {
            [SerializeField] private GameObject _passwordResetForm;
            [SerializeField] private GameObject _signInForm;

            [SerializeField] private InputField _emailField;
            [SerializeField] private InputField _passwordField;

            [SerializeField] private Button _signInButton;
            [SerializeField] private Button _signUpButton;
            [SerializeField] private Button _passwordResetButton;

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

            // Start is called before the first frame update
            void Start()
            {
                _signInButton.interactable = false;
                _signUpButton.interactable = false;
                _passwordResetButton.interactable = false;

                PartyFirebase.Fb_FirebaseManager.Current.InitializeService(InitButtons);
            }

            private void InitButtons()
            {
                _signInButton.interactable = true;
                _signUpButton.interactable = true;
                _passwordResetButton.interactable = true;
            }

            private IEnumerator GoToHubCo()
            {
                Fb_FirestoreSession.Current.Setup();

                AsyncOperation scene = SceneManager.LoadSceneAsync(Stt_SceneIndexes.HUB, LoadSceneMode.Additive);
                scene.allowSceneActivation = false;

                yield return new WaitUntil(() => Fb_FirestoreSession.Current.SetupCompleted);

                yield return new WaitUntil(() => Mng_NetworkManager.Current.PhotonAuthComplete);

                scene.allowSceneActivation = true;
                yield return new WaitUntil(() => scene.isDone);

                _signingIn = false;
            }

            public void BtnSignIn()
            {
                _signingIn = true;
                PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.SignInEmailPassword(_emailField.text, _passwordField.text, (result) =>
                {
                    if (result.success)
                    {
                        StartCoroutine(GoToHubCo());
                    }
                    else
                    {
                        _signingIn = false;
                        Debug.Log("Authentication failed: " + result.exceptions[0].Message);
                    }
                });
            }

            public void BtnSignUp()
            {
                SceneManager.LoadScene(Stt_SceneIndexes.LAUNCHER_SIGNUP);
            }

            public void BtnResetPassword()
            {
                _passwordResetForm.SetActive(true);
                _signInForm.SetActive(false);
            }

            public void BtnBack()
            {
                if (_passwordResetForm.activeInHierarchy) return;

                SceneManager.LoadScene(Stt_SceneIndexes.PLAYER_FORK);
            }
        }
    }
}