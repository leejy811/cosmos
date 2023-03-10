using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    public static GameManger instance;
    public PoolManager poolManager;
    public PlayerController player;
    public UiManager UiManager;
    public CameraResolution cameraResolution;
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }
    public void GameOver()
    {
        UiManager.ActiveGameOverUI();
        UiManager.SaveGameResult();
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("InGameScene");
        Time.timeScale = 1;
    }

    public void GoLobby()
    {
        SceneManager.LoadScene("LobbyScene");
        Time.timeScale = 1;
    }
}
