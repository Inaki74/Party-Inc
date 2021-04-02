using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        /// <summary>
        /// Class that handles the Date of Birth input.
        /// Makes sure the format is dd/mm/yyyy
        /// </summary>
        public class Mono_DOBHandler : MonoBehaviour
        {
            [SerializeField] private InputField _DOBInputField;
            [SerializeField] private Text _DOBText;
            private bool _deletedSlash = false;
            private string _lastInput = "";

            public void SolveDateInput()
            {
                string input = _DOBInputField.text;

                print(input);
                print(_lastInput);
                // xx/xx/xxxx

                if(input.Length < _lastInput.Length && _lastInput.EndsWith("/"))
                {
                    input = input.Remove(input.Length - 1);

                    _DOBInputField.SetTextWithoutNotify(input);
                    _DOBText.text = input;

                    _DOBInputField.caretPosition = _DOBInputField.text.Length;

                    _lastInput = _DOBInputField.text;

                    //_deletedSlash = true;

                    return;
                }
                
                if (_deletedSlash && input.Length > _lastInput.Length)
                {
                    string newChar = input.Substring(input.Length - 1);

                    _DOBInputField.SetTextWithoutNotify(_lastInput + "/" + newChar);
                    _DOBText.text = _lastInput + "/" + newChar;

                    _DOBInputField.caretPosition = _DOBInputField.text.Length;

                    _lastInput = _DOBInputField.text;

                    _deletedSlash = false;

                    return;
                }

                _deletedSlash = false;

                if (input.Length == 2 || input.Length == 5)
                {
                    _DOBInputField.SetTextWithoutNotify(input + "/");
                    _DOBText.text = input + "/";

                    _DOBInputField.caretPosition = _DOBInputField.text.Length;

                    _lastInput = _DOBInputField.text;

                    return;
                }

                if (input.EndsWith("/"))
                {
                    print("penis");
                    input = input.Remove(input.Length - 1);

                    _DOBInputField.SetTextWithoutNotify(input);
                    _DOBText.text = input;

                    _DOBInputField.caretPosition = _DOBInputField.text.Length;

                    _lastInput = _DOBInputField.text;

                    _deletedSlash = true;
                    return;
                }
            }
        }
    }
}


