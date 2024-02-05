using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += PlaySceneLoaded;
    }

    private void PlaySceneLoaded(Scene scene, LoadSceneMode _)
    {
        if (scene.buildIndex != 1)
            return;
        else
            MainManager.Instance.AgentManager.Setup();
    }

    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }
}
