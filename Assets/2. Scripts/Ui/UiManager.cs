using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;

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
    [SerializeField] private Text resultHighScore;
    [SerializeField] private Text resultTicket;
    [SerializeField] private Text resultTime;
    [SerializeField] private Text currentRecovery;
    [SerializeField] private Text recoveryUpGold;
    [SerializeField] private Text waveCleaeAnimText;
    [SerializeField] private Animation bottomUiAnim;
    [SerializeField] private Animator waveClearAnim;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject waveClearAnimBase;
    [SerializeField] private GameObject attackRange;
    [SerializeField] private GameObject safeArea;
    [SerializeField] private Image panel;
    [SerializeField] private Image bloodEffect;
    [SerializeField] private GameObject backgroundBase;
    [SerializeField] private float backgroundMoveSpeed;
    [SerializeField] private Text timeScaleText;
    [SerializeField] private Image[] statButtons;
    [SerializeField] private float punchScale = 0.2f;
    [SerializeField] private float punchPosition = 20f;

    [SerializeField] private GameObject pauseUi;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private PostProcessVolume postProcessVolume;
    private Bloom bloom;
    #endregion

    #region Member Variables
    private bool isTweening = false;
    private float currentTime = 0f;
    private float fadeInTime = 2f;
    private Vector2 moveDir;
    private float moveX=1;
    private float moveY=1;
    private float xScreenHalfSize;
    private float yScreenHalfSize;
    float[] times = new float[] {1.0f,1.5f, 2.0f };
    private int timeScaleIdx = 0;
    private int curAtk = 0;
    #endregion


   
    private void Start()
    {
        SetPlayerState();
        StartCoroutine("FadeIn");
        yScreenHalfSize = Screen.height / 2;
        xScreenHalfSize = Screen.width  / 2;
        moveDir=new Vector2(moveX, moveY);

        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out bloom);

        if (!GameManger.instance.onBloomEffect)
            SetBloomIntensity(0);

        bgmSlider.onValueChanged.AddListener(SetBgmSlider);
        sfxSlider.onValueChanged.AddListener(SetSfxSlider);
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
        if(GameManger.instance.onHitEffect)
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
        Time.timeScale = 0;
        GameManger.instance.isPlaying = false;
        pauseUi.SetActive(true);
        bgmSlider.value = SoundManager.instance.GetBgmVolume();
        sfxSlider.value = SoundManager.instance.GetSfxVolume();
    }
    public void SkipButton()
    {
        GameManger.instance.waveManager.WaveSkipButton();
    }
    public void ResumeGame()
    {
        pauseUi.SetActive(false);
        Time.timeScale = times[timeScaleIdx % times.Length];
        GameManger.instance.isPlaying = true;
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
        curAtk = (int)((Mathf.Round(GameManger.instance.player.playerDamage * 10) * 0.1f) * 100);
        currentAtk.text = curAtk.ToString();
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
        if(GameManger.instance.player.PlayerDamageLevelUp())
            ButtonAccepted(0);
        else
            ButtonDenied(0);
        curAtk = (int)((Mathf.Round(GameManger.instance.player.playerDamage * 10) * 0.1f) * 100);
        //curAtk = (int)(GameManger.instance.player.playerDamage * 100);
        currentAtk.text = curAtk.ToString();
        atkUpGold.text = GameManger.instance.player.playerDamageCost.ToString() + " G";
    }

    public void OnSpeedUpButton()
    {
        if (GameManger.instance.player.PlayerAttackSpeedLevelUp())
            ButtonAccepted(1);
        else
            ButtonDenied(1);
        currentSpeed.text = GameManger.instance.player.playerAttackSpeed.ToString("F2");
        speedUpGold.text = GameManger.instance.player.playerAtkSpeedCost.ToString() + " G";
    }

    public void OnHpUpButton()
    {
        if (GameManger.instance.player.PlayerMaxHealthLevelUp())
            ButtonAccepted(2);
        else
            ButtonDenied(2);
        currentHp.text = GameManger.instance.player.maxPlayerHealth.ToString();
        hpUpGold.text = GameManger.instance.player.playerMaxHealthCost.ToString() + " G";
    }

    public void OnRecoveryUpButton()
    {
        if (GameManger.instance.player.PlayerHealthRecorveryLevelUp())
            ButtonAccepted(3);
        else
            ButtonDenied(3);
        currentRecovery.text = GameManger.instance.player.playerHealthRecorvery.ToString("F2") + " /sec";
        recoveryUpGold.text = GameManger.instance.player.playerHealthRecorveryCost.ToString() + " G";
    }

   
    private void ButtonAccepted(int index)
    {
        SoundManager.instance.PlaySFX("PartsUpgradeSound");
        Vector3 originalScale = statButtons[index].transform.localScale;
        if (!isTweening)    // Prevent multi-clicking
        {
            isTweening = true;
            statButtons[index].transform.DOPunchScale(originalScale * punchScale, 0.2f, 0, 1f).OnComplete(() =>
            {
                isTweening = false;
            });
        }
    }

    private void ButtonDenied(int index)
    {
        SoundManager.instance.PlaySFX("ButtonDenied");
        if (!isTweening)    // Prevent multi-clicking
        {
            isTweening = true;
            statButtons[index].transform.DOPunchPosition(new Vector3(punchPosition, 0, 0), 0.5f, 10, 1f).OnComplete(() =>
            {
                isTweening = false;
            });
        }
    }
    public void ActiveGameOverUI()
    {
        gameOverUI.SetActive(true);
        int time = (int)GameManger.instance.playTime;
        string timeString = time / 60 == 0 ? time + "s" : time / 60 + "m " + time % 60 + "s";

        Sequence resultSequence = DOTween.Sequence();
        resultSequence.
            Append(resultScore.DOText("Wave : " + waveLevel.text, 1.5f)).
            Append(resultHighScore.DOText("High Score : " + LocalDatabaseManager.instance.HighScore, 1.5f)).
            Append(resultJem.DOText("Jem : " + GameManger.instance.player.playerJem, 1.5f)).
            Append(resultTicket.DOText("Ticket : " + GameManger.instance.playTicket, 1.5f)).
            Append(resultTime.DOText("Play Time : "+ timeString, 1.5f));
    }

    public void SaveGameResult()
    {
        int currentGameJem = GameManger.instance.player.playerJem;
        GameManger.instance.player.playerJem = LocalDatabaseManager.instance.isTicketMode ? (int)(currentGameJem * 1.5f) : currentGameJem;
        LocalDatabaseManager.instance.JemCount += GameManger.instance.player.playerJem;
        LocalDatabaseManager.instance.HighScore = int.Parse(waveLevel.text);
        LocalDatabaseManager.instance.Ticket += GameManger.instance.playTicket;
        LocalDatabaseManager.instance.SaveGameData();
    }

    public void CloseCanvas()
    {
        attackRange.gameObject.SetActive(false);
        safeArea.gameObject.SetActive(false);
    }

    public void AddBloomIntensity(float value)
    {
        if(GameManger.instance.onBloomEffect)
            bloom.intensity.value += value;
    }

    public void SetBloomIntensity(float value)
    {
        bloom.intensity.value = value;
    }

    public float GetBloomIntensity()
    {
        return bloom.intensity.value;
    }

    public void PushLobbyButton()
    {
        GameManger.instance.GoLobby();
    }

    public void WaveClear(int num)
    {
        if (num == 41)
        {
            waveLevel.text = "Bonus";
        }
        else
        {
            waveLevel.text = Convert.ToString(num);
        }
        StartCoroutine("WaveClearAnim");
    }

    IEnumerator WaveClearAnim()
    {
        string[] texts =new string[] { "Wave Clear !", "3", "2", "1", "" };
        texts[texts.Length - 1] = "Wave " + waveLevel.text;

        yield return new WaitForSeconds(0.5f);
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
        float time = times[++timeScaleIdx % times.Length];
        timeScaleText.text = String.Format("{0:0.0}", time);
        Time.timeScale= time;
    }

    private void SetBgmSlider(float value)
    {
        SoundManager.instance.SetBgmVolume(value);
    }
    private void SetSfxSlider(float value)
    {
        SoundManager.instance.SetSfxVolume(value);
    }

    public void NextStage()
    {
        GameManger.instance.waveManager.WaveSkipButton();
    }
}
