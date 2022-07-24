using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rewriters
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [SerializeField] private int _frameRate;
        [SerializeField] private float _timeScale = 1f;

        private void Update()
        {
            Application.targetFrameRate = _frameRate;

            Time.timeScale = _timeScale;

            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}