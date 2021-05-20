using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_CheckoutHandler : MonoBehaviour
        {
            [SerializeField] private GameObject _scrollContent;
            [SerializeField] private GameObject _checkoutButtonPrefab;

            [SerializeField] private Text _priceText;

            private List<AssetsStoreData> _checkoutItems = new List<AssetsStoreData>();

            private int _totalPrice;

            private void OnEnable()
            {
                InitializeCheckoutScreen();
            }

            private void OnDisable()
            {
                CleanupScroll();
            }

            private void InitializeCheckoutScreen()
            {
                List<AssetsStoreData> checkoutAssets = Mng_CharacterEditorChoicesCache.Current.GetCart();

                InitializeItems(checkoutAssets);

                _totalPrice = GetTotalPrice(checkoutAssets);

                _priceText.text = _totalPrice.ToString();

                _checkoutItems = checkoutAssets;
            }

            private int GetTotalPrice(List<AssetsStoreData> checkoutPrice)
            {
                int sum = 0;

                foreach(AssetsStoreData asset in checkoutPrice)
                {
                    sum += asset.baseprice;
                }

                return sum;
            }

            private void InitializeItems(List<AssetsStoreData> checkoutAssets)
            {
                for (int i = 0; i < checkoutAssets.Count; i++)
                {
                    AssetsStoreData assetData = checkoutAssets[i];

                    GameObject newButton = Instantiate(_checkoutButtonPrefab);
                    newButton.GetComponent<Mono_CheckoutButtonHandler>().ButtonInitialize(assetData);

                    newButton.transform.SetParent(_scrollContent.transform, false);
                }
            }

            private void CleanupScroll()
            {
                for(int i = 0; i < _scrollContent.transform.childCount; i++)
                {
                    Destroy(_scrollContent.transform.GetChild(i).gameObject);
                }
            }

            public void BtnOnCheckout()
            {
                // Set processing animations.
                // Tally price
                // Apply price to account balance if its smaller that what you have
                // There should be a second check later. (Or bring account balance directly from the server and check with that)
                // Afterwards, add transaction to transactions list.
                // Finally add assets to the player's account
                // Once thats done, send the player to the closet.
            }
        }
    }
}


