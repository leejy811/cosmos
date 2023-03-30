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

    // ��� ���̺� ��ŵ�Ϸ��� WaveManager �߰��ҰԿ� ���߿� �뷱�� ������ ���� �� ����
    public WaveManager waveManager;
    void Awake()
    {
        instance = this;
        
    }

    private void Start()
    {
        SoundManager.instance.PlayBGM("InGameBGM");
    }

     public IEnumerator GameOver()
    {
        UiManager.SaveGameResult();
        Time.timeScale = 0.8f;
        while (Time.timeScale > 0.2f)
        {
            Time.timeScale -= 0.3f * Time.fixedDeltaTime;
            //Bloom Intensity ���� ���� ���ּ���
            yield return new WaitForFixedUpdate();
        }

        SceneManager.LoadScene("GameOverScene");
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
