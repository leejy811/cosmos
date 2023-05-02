using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    public static GameManger instance;

    public PoolManager poolManager;
    public PlayerController player;
    public UiManager uiManager;
    public CameraResolution cameraResolution;
    public LobbyUiManager lobbyUiManager;
    public WaveManager waveManager;
    public DataBase database;

    public bool onBloomEffect { get; set; } = true;
    public bool onHitEffect { get; set; } = true;

    // play info in 'each' game
    public bool isPlaying { get; set; } = false;
    public int playTicket { get; set; } = 0;
    public float playTime { get; set; } = 0f;
    public bool isNewRecord { get; set; } = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SoundManager.instance.PlayBGM("LobbyBGM");
        lobbyUiManager = GameObject.Find("Canvas").GetComponent<LobbyUiManager>();
    }

    private void Update()
    {
        if (isPlaying)
            playTime += Time.deltaTime/Time.timeScale;
        DoExit();
    }

    private void SetIngameVar()
    {
        poolManager = GameObject.Find("PoolManager").GetComponent<PoolManager>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        uiManager = GameObject.Find("MainCanvas").GetComponent<UiManager>();
        waveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
        cameraResolution = Camera.main.GetComponent<CameraResolution>();
    }

    public IEnumerator GameOver()
    {
        uiManager.DoGameOverWorks();
        uiManager.SaveGameResult();
        isPlaying = false;

        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.08f);

        uiManager.CloseCanvas();
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.5f);

        Time.timeScale = 0.8f;
        float weight = 0.7f;
        while (Time.timeScale > 0.4f)
        {
            try
            {
                Time.timeScale -= 0.2f * Time.fixedDeltaTime * weight;
            }
            catch { Time.timeScale = 0.1f; }
            uiManager.AddBloomIntensity(weight);
            weight += (Time.fixedDeltaTime / 5) * 10;
            yield return new WaitForFixedUpdate();
        }
        
        Time.timeScale = 1;
        float intensity = 100;
        while (uiManager.GetBloomIntensity()>20f)
        {
            intensity = Mathf.Lerp(intensity, 1.5f, Time.deltaTime*2);
            uiManager.SetBloomIntensity(intensity);
            yield return null;
        }
        uiManager.SetBloomIntensity(1.5f);
        if (waveManager.currentWave == 40)
            StartCoroutine(uiManager.ShowEndingCredit());
        else
            uiManager.ActiveGameOverUI();
        SaveAchieveResult();
        AchievementManager.instance.SaveAchieve();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        StartCoroutine("LoadIngameScene");
        SoundManager.instance.PlayBGM("InGameBGM");
    }

    IEnumerator LoadIngameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("InGameScene");
        yield return asyncLoad;

        SetIngameVar();
        isPlaying = true;
        ResetGameData();
    }

    public void ResetGameData()
    {
        playTime = 0;
        playTicket = 0;
        isNewRecord = false;
    }

    public void GoLobby()
    {
        AchievementManager.instance.LoadAchieve();
        StartCoroutine("IGoLobby");
    }

    IEnumerator IGoLobby()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LobbyScene");
        yield return asyncLoad;

        lobbyUiManager = GameObject.Find("Canvas").GetComponent<LobbyUiManager>();
        SoundManager.instance.PlayBGM("LobbyBGM");
        Time.timeScale = 1;
        isPlaying = false;
        playTime = 0;
        playTicket = 0;
    }

    private void DoExit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPlaying)
                uiManager.PauseGame();
            else if (playTime > 0)
                uiManager.ResumeGame();
            else if (lobbyUiManager.isPopupOpen)
                lobbyUiManager.ClosePopupUi();
            else
                lobbyUiManager.OpenExitPopup();
        }
    }

    public void SaveAchieveResult()
    {
        AchievementManager.instance.achieves["Start from the basic"].curValue += waveManager.totalKillEnemyACount;
        AchievementManager.instance.achieves["Not so fast?"].curValue += waveManager.totalKillEnemyBCount;
        AchievementManager.instance.achieves["Quite Fragile"].curValue += waveManager.totalKillEnemyCCount;
        AchievementManager.instance.achieves["Gross. Go Away"].curValue += waveManager.totalKillEnemyDCount;
        AchievementManager.instance.achieves["MotherShip Down"].curValue += waveManager.currentWave >= 10 ? 1 : 0;
        AchievementManager.instance.achieves["It was close"].curValue += waveManager.currentWave >= 20 ? 1 : 0;
        AchievementManager.instance.achieves["I¡¯m Not a Sun"].curValue += waveManager.currentWave >= 30 ? 1 : 0;
        AchievementManager.instance.achieves["SpaceKing"].curValue += waveManager.currentWave >= 40 ? 1 : 0;
        AchievementManager.instance.achieves["What a good balance"].curValue = Mathf.Min(player.playerDamageLevel, player.playerAttackSpeedLevel, player.playerHealthLevel, player.playerHealthRecorveryLevel);
        AchievementManager.instance.achieves["Alien? Zombie?"].curValue = Mathf.Min(player.playerHealthLevel, player.playerHealthRecorveryLevel);
        AchievementManager.instance.achieves["StarWars"].curValue = waveManager.bonusWaveTime;

        if(int.Parse(LocalDatabaseManager.instance.HighScore) > 30)
        {
            AchievementManager.instance.achieves["World Class Engineer"].curValue = 4;
        }
    }
}
