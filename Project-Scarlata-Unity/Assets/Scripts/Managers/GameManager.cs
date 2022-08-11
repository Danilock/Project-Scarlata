using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace Rewriters
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [SerializeField] private int _frameRate;
        [SerializeField] private float _timeScale = 1f;

        [SerializeField, FoldoutGroup("Scene Loading")] private float _screenFadeTimeOnSceneLoading = 1f;
        [SerializeField, ReadOnly, FoldoutGroup("Scene Loading")] private string _sceneToLoad;

        private void Update()
        {
            Application.targetFrameRate = _frameRate;

            Time.timeScale = _timeScale;

            if (Input.GetKeyDown(KeyCode.R))
                LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadScene(string sceneName)
        {
            _sceneToLoad = sceneName;

            UIManager.Instance.DoScreenFade(Color.black, 1f, _screenFadeTimeOnSceneLoading, 0f, LoadSceneProcess);
        }

        private void LoadSceneProcess() => StartCoroutine(LoadScene_CO());

        private IEnumerator LoadScene_CO()
        {
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(_sceneToLoad);

            while (!loadSceneAsync.isDone)
            {
                yield return null;
            }

            UIManager.Instance.DoScreenFade(Color.black, 0f, _screenFadeTimeOnSceneLoading);
        }
    }
}