using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PartyInc
{
    /// <summary>
    /// TODO
    /// Tentative idea. Not convinced yet. Idea on Notes.
    /// </summary>
    public class Mng_SceneNavigationSystem : MonoSingleton<Mng_SceneNavigationSystem>
    {
        [SerializeField] private Image _blackForeground;
        [SerializeField] private GameObject _blackForegroundGO;

        public float Progress { get; private set; } = 0f;
        public bool LoadingScenesAsync { get; private set; } = false;

        private Stack<int> _sceneStack = new Stack<int>();

        public int[] EssentialHubScenes { get; private set; }

        private void Start()
        {
            int[] essScns = { (int)Stt_SceneIndexes.GAME_LIST, (int)Stt_SceneIndexes.LAUNCHER_SIGNIN, (int)Stt_SceneIndexes.LAUNCHER_SIGNUP, (int)Stt_SceneIndexes.PLAYER_FORK };
            EssentialHubScenes = essScns;
        }

        /////// SHOULD BE ON ANOTHER CLASS
        /// Didnt pass it yet since Im not sure this will be a big thing.
        public IEnumerator DramaticSceneTransitionStartCo(float time)
        {
            _blackForegroundGO.SetActive(true);
            yield return StartCoroutine(UIHelper.AlphaGrowthCo(_blackForeground, 1f, time, true));
        }

        public IEnumerator DramaticSceneTransitionEndCo(float time )
        {
            yield return StartCoroutine(UIHelper.AlphaGrowthCo(_blackForeground, 0f, time, false));
            _blackForegroundGO.SetActive(false);
        }

        public void DramaticSceneTransitionStart(float time)
        {
            StartCoroutine(DramaticSceneTransitionStartCo(time));
        }

        public void DramaticSceneTransitionEnd(float time)
        {
            StartCoroutine(DramaticSceneTransitionEndCo(time));
        }

        ///////

        public bool ActivateLoadedSceneIgnoreStack(int index)
        {
            Scene sceneToActivate = SceneManager.GetSceneByBuildIndex(index);

            if (sceneToActivate.IsValid() && sceneToActivate.isLoaded)
            {
                SceneManager.SetActiveScene(sceneToActivate);

                foreach (GameObject go in sceneToActivate.GetRootGameObjects())
                {
                    go.SetActive(true);
                }

                return true;
            }

            return false;
        }

        public bool DeactivateLoadedSceneIgnoreStack(int index)
        {
            Scene sceneToActivate = SceneManager.GetSceneByBuildIndex(index);

            if (sceneToActivate.IsValid() && sceneToActivate.isLoaded)
            {
                foreach (GameObject go in sceneToActivate.GetRootGameObjects())
                {
                    go.SetActive(false);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Activates the scene of the index provided within the loaded scenes list.
        /// If its not found, it returns false.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool ActivateLoadedScene(int index)
        {
            Scene sceneToActivate = SceneManager.GetSceneByBuildIndex(index);

            if (sceneToActivate.IsValid() && sceneToActivate.isLoaded)
            {
                SceneManager.SetActiveScene(sceneToActivate);

                foreach (GameObject go in sceneToActivate.GetRootGameObjects())
                {
                    go.SetActive(true);
                }

                _sceneStack.Push(index);

                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Deactivates the scene of the index provided within the loaded scenes list.
        /// If its not found, it returns false.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool DeactivateLoadedScene(int index)
        {
            Scene sceneToActivate = SceneManager.GetSceneByBuildIndex(index);

            if (sceneToActivate.IsValid() && sceneToActivate.isLoaded)
            {
                foreach (GameObject go in sceneToActivate.GetRootGameObjects())
                {
                    go.SetActive(false);
                }

                if(_sceneStack.Count != 0 && _sceneStack.Peek() == index)
                {
                    _sceneStack.Pop();
                }
                else
                {
                    Debug.Log("SCENE THAT WAS DEACTIVATED WAS NOT ON THE TOP OF THE STACK.");
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads additively a bunch of scenes.
        /// This class provides the progress of the latest additive operationg and whether its operating or not.
        /// This class will also save all loaded scenes in the background (unactivated).
        /// </summary>
        /// <param name="scenesToLoad"></param>
        /// <returns></returns>
        public IEnumerator LoadScenesAsyncAdditive(int[] scenesToLoad)
        {
            List<AsyncOperation> loadingScenes = new List<AsyncOperation>();
            LoadingScenesAsync = true;

            foreach (int sceneIndex in scenesToLoad)
            {
                AsyncOperation load = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                loadingScenes.Add(load);
            }

            StartCoroutine(UpdateProgress(scenesToLoad, loadingScenes));

            for(int i = 0; i < scenesToLoad.Length; i++)
            {
                yield return new WaitUntil(() => loadingScenes[i].isDone);
                //yield return null;
                DeactivateLoadedSceneIgnoreStack(scenesToLoad[i]);
            }

            Progress = 1f;

            yield return new WaitForSeconds(1f);

            Progress = 0f;
            LoadingScenesAsync = false;
        }

        private IEnumerator UpdateProgress(int[] scenesToLoad, List<AsyncOperation> loadingScenes)
        {
            for (int i = 0; i < scenesToLoad.Length; i++)
            {
                while (!loadingScenes[i].isDone)
                {
                    foreach (AsyncOperation loadingScene in loadingScenes)
                    {
                        Progress += loadingScene.progress;
                    }

                    Progress = Progress / scenesToLoad.Length;

                    yield return new WaitForEndOfFrame();
                }
            }
        }

        public void DeactivateActiveScene()
        {
            DeactivateLoadedScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Loads singularly a single scene.
        /// </summary>
        /// <param name="sceneIndex"></param>
        public void LoadSceneSingular(int sceneIndex)
        {
            _sceneStack.Push(sceneIndex);

            SceneManager.LoadScene(sceneIndex);
        }

        /// <summary>
        /// Gets the last scene we went to.
        /// </summary>
        /// <returns></returns>
        public int GetLastScene()
        {
            return _sceneStack.Peek();
        }

        public void GoToLastScene()
        {
            int lastSceneIndex = _sceneStack.Pop();

            if (SceneManager.GetSceneByBuildIndex(lastSceneIndex).IsValid())
            {
                ActivateLoadedSceneIgnoreStack(lastSceneIndex);
            }
            else
            {
                SceneManager.LoadScene(lastSceneIndex);
            }
        }
    }

    public struct SceneLoading
    {
        public int index;
        public AsyncOperation loadOperation;

        public SceneLoading(int index, AsyncOperation operation)
        {
            this.index = index;
            loadOperation = operation;
        }
    }
}


