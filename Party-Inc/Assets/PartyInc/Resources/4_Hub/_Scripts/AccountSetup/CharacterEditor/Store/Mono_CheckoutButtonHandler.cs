using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_CheckoutButtonHandler : MonoBehaviour
        {
            [SerializeField] private Text _assetNameText;
            [SerializeField] private Text _rarityText;
            [SerializeField] private Image _assetImage;
            [SerializeField] private Text _assetBasePriceText;

            private AssetsStoreData _thisAssetsData;

            // GET IMAGE
            public void ButtonInitialize(AssetsStoreData assetData)
            {
                _thisAssetsData = assetData;

                _assetNameText.text = assetData.storename;
                _assetBasePriceText.text = assetData.baseprice.ToString();

                switch (assetData.rarity)
                {
                    case Enum_AssetRarities.COMMON:
                        _rarityText.text = "Common";
                        _rarityText.color = Color.grey;
                        break;
                    case Enum_AssetRarities.UNCOMMON:
                        _rarityText.text = "Uncommon";
                        _rarityText.color = Color.blue;
                        break;
                    case Enum_AssetRarities.RARE:
                        _rarityText.text = "Rare";
                        _rarityText.color = Color.yellow;
                        break;
                    case Enum_AssetRarities.UNIQUE:
                        _rarityText.text = "Unique";
                        _rarityText.color = Color.red;
                        break;
                }
            }
        }
    }
}


