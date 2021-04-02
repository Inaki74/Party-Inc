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
                _loadingStatusText.text = "   Connecting...";

                yield return new WaitUntil(() => Fb_FirebaseManager.Current.ConnectedToFirebaseServices && Fb_FirebaseAuthenticateManager.Current.AuthInitialized);

                int sceneToLoad = 0;

                _loadingStatusText.text = "   Getting account...";

                if (Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser != null)
                {
                    Fb_FirestoreSession.Current.Setup();
                    sceneToLoad = Stt_SceneIndexes.HUB;

                    yield return new WaitUntil(() => Fb_FirestoreSession.Current.SetupCompleted);

                    _loadingStatusText.text = "   Connecting to game servers...";

                    yield return new WaitUntil(() => Mng_NetworkManager.Current.PhotonAuthComplete);
                }
                else
                {
                    sceneToLoad = Stt_SceneIndexes.PLAYER_FORK;
                }

                _loadingStatusText.text = "   Loading some resources...";

                AsyncOperation scene = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
                scene.allowSceneActivation = false;

                yield return new WaitUntil(() => scene.progress == 0.9f);

                _loadingStatusText.text = "   Finishing up...";

                yield return new WaitForSeconds(3f);

                // Fade out black
                yield return StartCoroutine(UIHelper.AlphaGrowthCo(_blackForeground, 1f, 1f, true));

                // Load scene
                _loadingCanvas.SetActive(false);
                scene.allowSceneActivation = true;

                // Fade in
                yield return StartCoroutine(UIHelper.AlphaGrowthCo(_blackForeground, 0f, 0.7f, false));

                _forescreenCanvas.SetActive(false);
            }
        }
    }
}


