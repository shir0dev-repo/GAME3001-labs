using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    public static Action OnGameWin, OnGameLose;

    public SoundManager SoundManager { get; private set; }
    public UIManager UIManager { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        UIManager = GetComponentInChildren<UIManager>();
        SoundManager = GetComponentInChildren<SoundManager>();
    }

    private void OnEnable()
    {
        OnGameWin += WinGame;
        OnGameLose += LoseGame;
    }

    private void OnDisable()
    {
        OnGameWin -= WinGame;
        OnGameLose -= LoseGame;
    }

    private void LoseGame()
    {
        SoundManager.PlaySound("Aww");
        Debug.Log("You lose!");
        LoadScene(2);
    }

    private void WinGame()
    {
        SoundManager.PlaySound("Woo");
        
        Debug.Log("You win!");
        LoadScene(2);
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        UIManager.SetUI(sceneIndex);

        if (sceneIndex == 1)
        {
            SoundManager.PlayTrack("InGame");
            Cursor.lockState = CursorLockMode.Locked;
        }
        else Cursor.lockState = CursorLockMode.None;
        
    }
}
