using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        using PartyFirebase.Firestore;


        public class Mng_CharacterEditorCache : MonoSingleton<Mng_CharacterEditorCache>
        {
            // An array of strings which each array position maps to a list of assets owned by the player of a certain type:
            // Outfit assets populate numbers 0 - 7
            // Wallpaper assets populate the number 8
            // Emote assets populate numbers 9 - 14
            // Tune assets populate the number 15
            // Face assets populate numbers 16 - 23 (last since they are not buyable)
            private List<string>[] _classifiedOwnedAssets = new List<string>[24];

            private void Awake()
            {
                for (int i = 0; i < _classifiedOwnedAssets.Length; i++)
                {
                    _classifiedOwnedAssets[i] = new List<string>();
                }
            }

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public List<string> GetTypeList(int type)
            {
                return _classifiedOwnedAssets[type];
            }

            private IEnumerator StartCache()
            {
                // Get the owned assets list (already in memory)
                // Async load the resources
                // Build up lists for each type of asset (24)
                // Save them in an Array of List of strings

                List<Dictionary<string, object>> ownedAssets = (List<Dictionary<string, object>>)Fb_FirestoreSession.Current.LocalPlayerData[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS];

                yield return null;

                foreach (Dictionary<string, object> asset in ownedAssets)
                {
                    string assetId = (string)asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_ID];

                    LoadAssetImages(assetId);
                    LoadAssetModels(assetId);
                }

                ClassifyAssets(ownedAssets);
            }

            private void ClassifyAssets(List<Dictionary<string, object>> assets)
            {

                foreach (Dictionary<string, object> asset in assets)
                {
                    string id = (string)asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_ID];
                    int type = (int)asset[Fb_Constants.FIRESTORE_KEY_PLAYER_ASSETS_TYPE];

                    _classifiedOwnedAssets[type].Add(id);
                }
            }

            //TODO
            private void LoadAssetModels(string assetId)
            {

            }

            //TODO
            private void LoadAssetImages(string assetId)
            {

            }
        }
    }
}
