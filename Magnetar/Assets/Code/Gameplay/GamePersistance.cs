using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Magnetar
{
    public class GamePersistance : MonoBehaviour
    {
        public const float LOADING_FADE_TIME = 0.3f;

        public static GamePersistance Instance { get; private set; }

        [field: SerializeField] public CanvasGroup LoadingCanvas { get; private set; }

        private string previouslyLoadedScene = null;

        void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
#if UNITY_EDITOR
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene == SceneManager.GetSceneByName("_GamePersistance"))
            {
                GoToMainMenu();
            }
            else // We're likely debugging
            {
                LoadingCanvas.alpha = 0.0f;
                if(activeScene.name == "GameScene")
                {
                    PlayableZoneController.Instance.Initialize();
                }
            }
#else
            GoToMainMenu();
#endif
        }

        public void GoToMainMenu()
        {
            LoadScene("MainMenu");
        }

        public void GoToGameScene(bool isMultiShipScene)
        {
            LoadScene("GameScene", isMultiShipScene ? GameSceneInitMulti : GameSceneInitSingle);
        }
        private void LoadScene(string sceneName, Func<Scene, IEnumerator> onCompleted = null)
        {
            StartCoroutine(DoLoadScene(sceneName, onCompleted));
        }

        private IEnumerator DoLoadScene(string sceneName, Func<Scene, IEnumerator> onCompleted)
        {
            if (previouslyLoadedScene != null)
            {
                for (float i = 0.0f; i < 1.0f; i += Time.deltaTime / LOADING_FADE_TIME)
                {
                    LoadingCanvas.alpha = i;
                    yield return null;
                }
            }
            LoadingCanvas.alpha = 1.0f;

            Debug.Log($"Loading scene {sceneName}");
            var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return asyncOp;

            Debug.Log($"Loaded scene {sceneName}");

            if (previouslyLoadedScene != null)
            {
                Debug.Log($"Unloading scene {sceneName}");
                yield return SceneManager.UnloadSceneAsync(previouslyLoadedScene);
                Debug.Log($"Unloaded scene {sceneName}");
            }

            previouslyLoadedScene = sceneName;
            var scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);

            if (onCompleted != null)
            {
                yield return onCompleted(scene);
            }

            for (float i = 1.0f; i  > 0.0f; i -= Time.deltaTime / LOADING_FADE_TIME)
            {
                LoadingCanvas.alpha = i;
                yield return null;
            }
            LoadingCanvas.alpha = 0.0f;
        }

        private IEnumerator GameSceneInitSingle(Scene scene)
        {
            while (PlayableZoneController.Instance == null)
            {
                yield return null;
            }

            PlayableZoneController.Instance.IsTwoShipMode = false;
            PlayableZoneController.Instance.Initialize();
        }

        private IEnumerator GameSceneInitMulti(Scene scene)
        {
            while (PlayableZoneController.Instance == null)
            {
                yield return null;
            }

            PlayableZoneController.Instance.IsTwoShipMode = true;
            PlayableZoneController.Instance.Initialize();
        }
    }
}
