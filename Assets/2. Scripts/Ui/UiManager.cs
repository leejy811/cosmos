using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class UiManager : MonoBehaviour
{
    // Manages all UI component
    // Contains functions for each Button
    #region UI Components
    [SerializeField] private Text jemCount;
    [SerializeField] private Text goldCount;
    [SerializeField] private Scrollbar hpScrollbar;
    [SerializeField] private Text waveLevel;
    [SerializeField] private Text currentAtk;
    [SerializeField] private Text atkUpGold;
    [SerializeField] private Text currentSpeed;
    [SerializeField] private Text speedUpGold;
    [SerializeField] private Text currentHp;
    [SerializeField] private Text hpUpGold;
    [SerializeField] private Text resultJem;
    [SerializeField] private Text resultScore;
    [SerializeField] private Text currentRecovery;
    [SerializeField] private Text recoveryUpGold;
    [SerializeField] private Text waveCleaeAnimText;
    [SerializeField] private Animation bottomUiAnim;
    [SerializeField] private Animator waveClearAnim;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject waveClearAnimBase;
    [SerializeField] private Image panel;
    [SerializeField] private Image bloodEffect;
    [SerializeField] private GameObject backgroundBase;
    [SerializeField] private float backgroundMoveSpeed;
    [SerializeField] private Text timeScaleText;
    private PostProcessVolume postProcessVolume;
    private Bloom bloom;
    #endregion

    #region Member Variables
    private bool isFold = false;
    private float currentTime = 0f;
    private float fadeInTime = 2f;
    private Vector2 moveDir;
    private float moveX=1;
    private float moveY=1;
    private float xScreenHalfSize;
    private float yScreenHalfSize;
    private int timeScaleIdx = 0;
    #endregion

    private void Awake()
    {
        SetPlayerState();
    }
    private void Start()
    {
        StartCoroutine("FadeIn");
        yScreenHalfSize = Screen.height / 2;
        xScreenHalfSize = Screen.width  / 2;
        moveDir=new Vector2(moveX, moveY);

        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out bloom);
    }
    private void Update()
    {
        SetHpUI();
        SetGold();
        SetJem();
        MoveBackground();
    }

    private void MoveBackground()
    {
        float x = backgroundBase.GetComponent<RectTransform>().anchoredPosition.x;
        float y = backgroundBase.GetComponent<RectTransform>().anchoredPosition.y;

        if (x < -xScreenHalfSize)
            moveX = UnityEngine.Random.Range(0, 1f);
        else if (x > xScreenHalfSize)
            moveX = UnityEngine.Random.Range(-1f, 0);
        if (y < -yScreenHalfSize)
            moveY = UnityEngine.Random.Range(0, 1f);
        else if (y > yScreenHalfSize)
            moveY = UnityEngine.Random.Range(-1f, 0);

        moveDir.x = moveX;
        moveDir.y = moveY;
        backgroundBase.GetComponent<RectTransform>().anchoredPosition += backgroundMoveSpeed * Time.deltaTime * moveDir.normalized;
    }
    public void StartDamageEffect()
    {
        StartCoroutine("DamageEffect");
    }
    IEnumerator DamageEffect()
    {
        bloodEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        bloodEffect.gameObject.SetActive(false);
    }
    IEnumerator FadeIn()
    {
        Color alpha = panel.color;
        while (alpha.a > 0)
        {
            currentTime += Time.deltaTime / fadeInTime;
            alpha.a = Mathf.Lerp(1, 0, currentTime);
            panel.color = alpha;
            yield return null;
        }
        panel.gameObject.SetActive(false);
    }
    void SetGold()
    {
        goldCount.text = GameManger.instance.player.playerGold.ToString() + " G";
    }
    void SetHpUI()
    {
        hpScrollbar.size = GameManger.instance.player.playerHealth / GameManger.instance.player.maxPlayerHealth;
    }
    public void PauseGame()
    {
        GameManger.instance.waveManager.WaveSkipButton();
    }
    void SetJem()
    {
        jemCount.text = GameManger.instance.player.playerJem.ToString();
    }
    public void AddJem(int num=0)
    {
        jemCount.text=(Convert.ToString(num));
    }

    public void SetRock(int num=0)
    {
        goldCount.text = (Convert.ToString(num));
    }

    public void SetHp(float value=0.0f)
    {
        hpScrollbar.value = value;
    }

    private void SetPlayerState()
    {
        currentAtk.text = GameManger.instance.player.playerDamage.ToString("F1");
        atkUpGold.text = GameManger.instance.player.playerDamageCost.ToString() + " G";
        currentSpeed.text = GameManger.instance.player.playerAttackSpeed.ToString("F2");
        speedUpGold.text = GameManger.instance.player.playerAtkSpeedCost.ToString() + " G";
        currentHp.text = GameManger.instance.player.playerHealth.ToString();
        hpUpGold.text = GameManger.instance.player.playerMaxHealthCost.ToString() + " G";
        currentRecovery.text = GameManger.instance.player.playerHealthRecorvery.ToString("F2") + " /sec";
        recoveryUpGold.text = GameManger.instance.player.playerHealthRecorveryCost.ToString() + " G";
    }
    public void OnAttackUpButton()
    {
        GameManger.instance.player.PlayerDamageLevelUp();
        currentAtk.text = GameManger.instance.player.playerDamage.ToString("F1");
        atkUpGold.text = GameManger.instance.player.playerDamageCost.ToString() + " G";
    }

    public void OnSpeedUpButton()
    {
        GameManger.instance.player.PlayerAttackSpeedLevelUp();
        currentSpeed.text = GameManger.instance.player.playerAttackSpeed.ToString("F2");
        speedUpGold.text = GameManger.instance.player.playerAtkSpeedCost.ToString() + " G";
    }

    public void OnHpUpButton()
    {
        GameManger.instance.player.PlayerMaxHealthLevelUp();
        currentHp.text = GameManger.instance.player.maxPlayerHealth.ToString();
        hpUpGold.text = GameManger.instance.player.playerMaxHealthCost.ToString() + " G";
    }

    public void OnRecoveryUpButton()
    {
        GameManger.instance.player.PlayerHealthRecorveryLevelUp();
        currentRecovery.text = GameManger.instance.player.playerHealthRecorvery.ToString("F2") + " /sec";
        recoveryUpGold.text = GameManger.instance.player.playerHealthRecorveryCost.ToString() + " G";
    }

    public void SetBottomUi()
    {
        if (isFold)
            bottomUiAnim.Play("BottomUiUpAnim");
        else
            bottomUiAnim.Play("BottomUiCloseAnim");
        isFold = !isFold;
    }

    public void ActiveGameOverUI()
    {
        gameOverUI.SetActive(true);
        resultScore.text = "High Score : "+waveLevel.text;
        resultJem.text = "Jem : "+jemCount.text;
    }

    public void SaveGameResult()
    {
        int currentGameJem = GameManger.instance.player.playerJem;
        LocalDatabaseManager.instance.JemCount += LocalDatabaseManager.instance.isTicketMode ? (int)(currentGameJem * 1.5f) : currentGameJem;
        LocalDatabaseManager.instance.HighScore = int.Parse(waveLevel.text);
        LocalDatabaseManager.instance.SaveGameData();
    }

    public void CloseCanvas()
    {
        this.gameObject.SetActive(false);
    }

    public void SetBloomIntensity(float value)
    {
        bloom.intensity.value += value;
    }

    public void PushRetryButton()
    {
        GameManger.instance.RestartGame();
    }

    public void PushLobbyButton()
    {
        GameManger.instance.GoLobby();
    }

    public void WaveClear(int num)
    {
        waveLevel.text = Convert.ToString(num);
        StartCoroutine("WaveClearAnim");
    }

    IEnumerator WaveClearAnim()
    {
        string[] texts =new string[] { "Wave Clear !", "3", "2", "1", "" };
        texts[texts.Length - 1] = "Wave " + waveLevel.text;

        waveClearAnimBase.SetActive(true);
        foreach (string s in texts)
        {
            waveCleaeAnimText.text = s;
            waveClearAnim.SetTrigger("ChangeString");
            yield return new WaitForSeconds(1f);
        }
        waveClearAnimBase.SetActive(false);
    }

    public void SetTimeScale()
    {
        float[] times = new float[] { 1.5f, 2.0f, 1.0f };
        float time = times[timeScaleIdx++ % times.Length];
        timeScaleText.text = String.Format("{0:0.0}", time);
        Time.timeScale= time;
    }
}
