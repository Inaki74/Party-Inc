using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PartyInc
{
    using PartyFirebase;
    using PartyFirebase.Firestore;
    using PartyFirebase.Auth;
    namespace Hub
    {
        public class Mono_LoadingScreen : MonoBehaviour
        {
            [SerializeField] private GameObject _loadingCanvas;
            [SerializeField] private GameObject _forescreenCanvas;
            [SerializeField] private Text _loadingStatusText;
            [SerializeField] private Image _blackForeground;

            // Start is called before the first frame update
            void Start()
            {
                StartCoroutine(LoadingCo());
            }

            // Update is called once per frame
            void Update()
            {

            }

            private IEnumerator LoadingCo()
            {
                float progress = 0f;

                _loadingStatusText.text = "   Connecting...";

                yield return new WaitUntil(() => Fb_FirebaseManager.Current.ConnectedToFirebaseServices && Fb_FirebaseAuthenticateManager.Current.AuthInitialized);

                int sceneToActivate = 0;

                _loadingStatusText.text = "   Getting account...";

                if (Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser != null)
                {
                    Fb_FirestoreSession.Current.Setup();
                    sceneToActivate = (int)Stt_SceneIndexes.HUB;

                    yield return new WaitUntil(() => Fb_FirestoreSession.Current.SetupCompleted);

                    _loadingStatusText.text = "   Connecting to game servers...";

                    yield return new WaitUntil(() => Mng_PhotonManager.Current.PhotonAuthComplete);
                }
                else
                {
                    sceneToActivate = (int)Stt_SceneIndexes.PLAYER_FORK;
                }

                _loadingStatusText.text = "   Loading some resources...";

                yield return StartCoroutine(Mng_SceneNavigationSystem.Current.LoadScenesAsyncAdditive(Mng_SceneNavigationSystem.Current.EssentialHubScenes));

                _loadingStatusText.text = "   Finishing up...";

                yield return new WaitForSeconds(3f);

                // Fade out black
                yield return StartCoroutine(Mng_SceneNavigationSystem.Current.DramaticSceneTransitionStartCo(1.0f));

                Mng_SceneNavigationSystem.Current.DeactivateActiveScene();
                Mng_SceneNavigationSystem.Current.ActivateLoadedScene(sceneToActivate);

                Mng_SceneNavigationSystem.Current.DramaticSceneTransitionEnd(0.7f);
            }
        }
    }
}


