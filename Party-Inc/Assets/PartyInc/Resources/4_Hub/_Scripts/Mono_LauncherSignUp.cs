using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_LauncherSignUp : MonoBehaviour
        {
            [SerializeField] private InputField _nicknameField;
            [SerializeField] private InputField _emailField;
            [SerializeField] private InputField _passwordField;
            [SerializeField] private InputField _passwordVerificationField;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void SignUp()
            {
                Fb_FirebaseAuthenticateManager.Current.SignUpEmailPassword(_emailField.text, _passwordField.text, _passwordVerificationField.text, _nicknameField.text);
            }

            public void BackToSignIn()
            {
                SceneManager.LoadScene(Stt_SceneIndexes.LAUNCHER_SIGNIN);
            }
        }
    }
}


