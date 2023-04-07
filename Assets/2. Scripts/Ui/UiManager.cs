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
    private PostProcessVolume postProcessVolume;
    private Bloom bloom;
    #endregion

    #region Member Variables
    private bool isFold = false;
    private bool isTweening = false;
    private float currentTime = 0f;
    private float fadeInTime = 2f;
    private Vector2 moveDir;
    private float moveX=1;
    private float moveY=1;
    private float xScreenHalfSize;
    private float yScreenHalfSize;
    private int timeScaleIdx = 0;
    private float curAtk = 0;
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
        jemCount.text = LocalDatabaseManager.instance.JemCount.ToString();
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
        curAtk = (float.Parse)(GameManger.instance.player.playerDamage.ToString("F1")) * 100;
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
        curAtk = (float.Parse)(GameManger.instance.player.playerDamage.ToString("F1")) * 100;
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

    public void SetBottomUi()
    {
        if (isFold)
            bottomUiAnim.Play("BottomUiUpAnim");
        else
            bottomUiAnim.Play("BottomUiCloseAnim");
        isFold = !isFold;
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
        resultScore.text = "High Score : "+waveLevel.text;
        resultJem.text = "Jem : "+jemCount.text;
    }

    public void SaveGameResult()
    {
        LocalDatabaseManager.instance.JemCount += int.Parse(jemCount.text);
        LocalDatabaseManager.instance.HighScore = int.Parse(waveLevel.text);
        LocalDatabaseManager.instance.SaveGameData();
    }

    public void CloseCanvas()
    {
        attackRange.gameObject.SetActive(false);
        safeArea.gameObject.SetActive(false);
    }

    public void AddBloomIntensity(float value)
    {
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
