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

                Fb_FirestorePlayers.Current.AddNewPlayer(userId, newPlayer.ToDictionary());
            }

            public void BackToSignIn()
            {
                SceneManager.LoadScene(Stt_SceneIndexes.LAUNCHER_SIGNIN);
            }
        }
    }
}


