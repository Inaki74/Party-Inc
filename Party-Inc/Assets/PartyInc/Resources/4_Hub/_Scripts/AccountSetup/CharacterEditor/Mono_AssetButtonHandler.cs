using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        [RequireComponent(typeof(Toggle))]
        public class Mono_AssetButtonHandler : MonoBehaviour
        {
            public Data_CharacterAssetMetadata AssetData
            {
                get
                {
                    return _assetData;
                }
                protected set
                {
                    _assetData = value;
                }
            }

            protected Data_CharacterAssetMetadata _assetData;
            protected Toggle _theToggle;

            //DATA

            private void Awake()
            {
                AwakeOverride();
            }

            private void OnDestroy()
            {
                DestroyOverride();
            }

            protected virtual void DestroyOverride()
            {
                
            }

            protected virtual void AwakeOverride()
            {
                _theToggle = GetComponent<Toggle>();
            }

            public void ToggleButton()
            {
                if (_theToggle.isOn)
                {
                    _theToggle.SetIsOnWithoutNotify(false);
                }
                else
                {
                    _theToggle.isOn = true;
                }
            }

            public virtual void InitializeButton(Data_CharacterAssetMetadata assetData, ToggleGroup theTG, Action<Data_CharacterAssetMetadata> onToggle)
            {
                _assetData = assetData;

                _theToggle.group = theTG;

                _theToggle.onValueChanged.AddListener(delegate
                {
                    onToggle(_assetData);
                });
            }
        }
    }
}


