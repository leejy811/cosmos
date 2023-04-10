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
        Debug.Log(waveManager.bonusWaveTime);
        Time.timeScale = 0.05f;
        yield return new WaitForSeconds(0.08f);

        UiManager.CloseCanvas();
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.5f);

        Time.timeScale = 0.8f;
        float weight = 0.7f;
        while (Time.timeScale > 0.3f)
        {
            try
            {
                Time.timeScale -= 0.2f * Time.fixedDeltaTime * weight;
            }
            catch { Time.timeScale = 0.1f; }
            UiManager.AddBloomIntensity(weight);
            weight += (Time.fixedDeltaTime / 3) * 2;
            yield return new WaitForFixedUpdate();
        }
        
        Time.timeScale = 1;
        float intensity = 100;
        while (UiManager.GetBloomIntensity()>20f)
        {
            intensity = Mathf.Lerp(intensity, 1.5f, Time.deltaTime);
            UiManager.SetBloomIntensity(intensity);
            yield return null;
        }
        UiManager.SetBloomIntensity(1.5f);
        UiManager.ActiveGameOverUI();
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
