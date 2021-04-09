using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        [RequireComponent(typeof(Toggle), typeof(Image))]
        public class Mono_ToggleSpriteChange : MonoBehaviour
        {
            private Image _theToggleImage;
            private Toggle _theToggle;

            [SerializeField] private Sprite _isOffSprite;
            [SerializeField] private Sprite _isOnSprite;

            private void Start()
            {
                _theToggleImage = GetComponent<Image>();
                _theToggle = GetComponent<Toggle>();
            }

            public void ToggleChanged(bool isOn)
            {
                print(gameObject.name);
                print(_theToggle.isOn);
                if (_theToggle.isOn)
                {
                    _theToggleImage.sprite = _isOnSprite;
                }
                else
                {
                    _theToggleImage.sprite = _isOffSprite;
                }
            }
        }
    }
}


