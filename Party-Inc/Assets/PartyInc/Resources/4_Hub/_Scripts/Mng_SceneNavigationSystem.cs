using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PartyInc
{
    /// <summary>
    /// TODO
    /// Tentative idea. Not convinced yet. Idea on Notes.
    /// </summary>
    public class Mng_SceneNavigationSystem : MonoSingleton<Mng_SceneNavigationSystem>
    {
        public float Progress { get; private set; } = 0f;
        public bool LoadingScenesAsync { get; private set; } = false;

        private Stack<int> _sceneStack = new Stack<int>();

        public bool ActivateLoadedSceneIgnoreStack(int index)
        {
            Scene sceneToActivate = SceneManager.GetSceneByBuildIndex(index);

            if (sceneToActivate.IsValid() && sceneToActivate.isLoaded)
            {
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


