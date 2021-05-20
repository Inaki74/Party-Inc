using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        public class Mono_CheckoutHandler : MonoBehaviour
        {
            [SerializeField] private GameObject _scrollContent;
            [SerializeField] private GameObject _checkoutButtonPrefab;

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
                AssetsStoreData[] checkoutAssets = Mng_CharacterEditorChoicesCache.Current.GetCart();

                for(int i = 0; i < checkoutAssets.Length; i++)
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


