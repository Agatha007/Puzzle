using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayState : Singleton<PlayState>
{
    public enum STATES
    {
        None,
        Login,
        WorldMap
    }

    private string currentScene;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        System.GC.Collect();
    }

    public void ChangePlayState(STATES state)
    {
        currentScene = state.ToString();

        StartCoroutine(AsyncChangeScene());
    }

    IEnumerator AsyncChangeScene()
    {
        var async = SceneManager.LoadSceneAsync(currentScene);

        yield return async;
    }
}
