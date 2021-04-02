using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_PasswordResetController : MonoBehaviour
        {
            [SerializeField] private GameObject _signInForm;
            [SerializeField] private GameObject _resetPasswordForm;

            [SerializeField] private Button _backButton;

            [SerializeField] private InputField _emailInput;

            // Start is called before the first frame update
            void Start()
            {
                
            }

            // Update is called once per frame
            void Update()
            {

            }

            private void OnEnable()
            {
                //print(_backButton.onClick.GetPersistentMethodName(0));
            }

            public void BtnRequestPasswordChange()
            {
                string email = _emailInput.text;

                PartyFirebase.Auth.Fb_FirebaseAuthenticateManager.Current.PasswordChangeRequest(email, res =>
                {
                    if (res.success)
                    {

                    }
                    else
                    {

                    }
                });
            }

            public void BtnBackToSignIn()
            {
                if (_signInForm.activeInHierarchy) return;

                _signInForm.SetActive(true);
                _resetPasswordForm.SetActive(false);
            }
        }
    }
}


