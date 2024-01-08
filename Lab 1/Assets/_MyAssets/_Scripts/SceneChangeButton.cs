using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeButton : MonoBehaviour
{
    public void ChangeSceneTo(int sceneIndexToLoad)
    {
        // Call the static method from the SceneManagerHelper class
        SceneLoader.LoadSceneByIndex(sceneIndexToLoad);
    }
}
