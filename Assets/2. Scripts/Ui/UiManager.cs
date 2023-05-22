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
    [Header("Top UI========================================")]
    [SerializeField] private Text jemCount;
    [SerializeField] private Text goldCount;
    [Header("Ingame Stats====================================")]
    [SerializeField] private Image[] statButtons;
    [SerializeField] private float punchScale = 0.2f;
    [SerializeField] private float punchPosition = 20f;

    [SerializeField] private Image[] hpBars;
    [SerializeField] private Text waveLevel;
    [SerializeField] private Text currentAtk;
    [SerializeField] private Text atkUpGold;
    [SerializeField] private Text currentSpeed;
    [SerializeField] private Text speedUpGold;
    [SerializeField] private Text currentHp;
    [SerializeField] private Text hpUpGold;
    [SerializeField] private Text currentRecovery;
    [SerializeField] private Text recoveryUpGold;

    [Header("Background=====================================")]
    [SerializeField] private GameObject backgroundBase;
    [SerializeField] private float backgroundMoveSpeed;

    [Header("Animation Text===================================")]
    [SerializeField] private GameObject bossAppearEffect;
    [SerializeField] private GameObject waveClearEffect;
    [SerializeField] private GameObject bonusWaveEffect;

    [Header("Ingame Effect====================================")]
    private PostProcessVolume postProcessVolume;
    private Bloom bloom;
    [SerializeField] private Image panel;
    [SerializeField] private Image bloodEffect;

    [Header("Pause UI=======================================")]
    [SerializeField] private GameObject pauseUi;
    [SerializeField] private Scrollbar bgmSlider;
    [SerializeField] private Scrollbar sfxSlider;

    [Header("GameOver UI====================================")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Text resultJem;
    [SerializeField] private Text resultScore;
    [SerializeField] private Text resultHighScore;
    [SerializeField] private GameObject newRecordText;
    [SerializeField] private Text resultTicket;
    [SerializeField] private GameObject jemDoubleText;
    [SerializeField] private Text resultTime;

    [Header("System Settings==================================")]
    [SerializeField] private GameObject attackRange;
    [SerializeField] private GameObject safeArea;
    [SerializeField] private Text timeScaleText;

    [Header("Ending Credit====================================")]
    [SerializeField] private GameObject endingCredit;
    [SerializeField] private GameObject endingCreditText;
    [SerializeField] private Text thanksText;
    [SerializeField] private Image endingImage;

    [Header("For Debugging===================================")]
    [SerializeField] private GameObject waveSkipButton;
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

    private int currentHpIndex;
    private int targetHpIndex;
    private bool isHpCalc = false;
    private int prevHp;

    private int curAtk = 0;
    private int curWave;
    #endregion
   
    private void Start()
    {
        StartCoroutine("FadeIn");
        SetPlayerState();
        SetBgMove();
        SetHpBar();
        SetBloomEffect();
        SetPauseMenu();
        CheckPlatform();
    }

    private void Update()
    {
        SetHpUI();
        SetGold();
        SetJem();
        MoveBackground();
    }

    #region Initiallization
    private void SetPlayerState()
    {
        curAtk = (int)((Mathf.Round(GameManger.instance.player.playerDamage * 10) * 0.1f) * 1000);
        currentAtk.text = curAtk.ToString();
        atkUpGold.text = GameManger.instance.player.playerDamageCost.ToString() + " G";
        currentSpeed.text = GameManger.instance.player.playerAttackSpeed.ToString("F2");
        speedUpGold.text = GameManger.instance.player.playerAtkSpeedCost.ToString() + " G";
        currentHp.text = GameManger.instance.player.playerHealth.ToString();
        hpUpGold.text = GameManger.instance.player.playerMaxHealthCost.ToString() + " G";
        currentRecovery.text = GameManger.instance.player.playerHealthRecorvery.ToString("F2") + " /sec";
        recoveryUpGold.text = GameManger.instance.player.playerHealthRecorveryCost.ToString() + " G";
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

    private void SetBgMove()
    {
        yScreenHalfSize = Screen.height / 2;
        xScreenHalfSize = Screen.width / 2;
        moveDir = new Vector2(moveX, moveY);
    }

    private void SetBloomEffect()
    {
        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out bloom);

        if (!GameManger.instance.onBloomEffect)
            SetBloomIntensity(0);
    }

    private void SetHpBar()
    {
        currentHpIndex = hpBars.Length - 1;
        prevHp = hpBars.Length;
        StartCoroutine(GageAnim());
    }

    IEnumerator GageAnim()      // progressbar anim, show 1 cycle at the start
    {
        yield return new WaitForSeconds(1f);

        for (int i = hpBars.Length - 1; i > 0 - 1; i--)
        {
            yield return new WaitForSeconds(0.05f);
            hpBars[i].DOKill();
            hpBars[i].DOFade(0f, 0f).SetEase(Ease.InCubic);
        }

        for (int i = 0; i < hpBars.Length; i++)
        {
            yield return new WaitForSeconds(0.05f);
            hpBars[i].DOKill();
            hpBars[i].DOFade(1f, 0f);
        }
        yield return new WaitForSeconds(0.3f);
    }

    private void SetPauseMenu()
    {
        bgmSlider.onValueChanged.AddListener(x=> SoundManager.instance.SetBgmVolume(x));
        sfxSlider.onValueChanged.AddListener(x => SoundManager.instance.SetSfxVolume(x));
    }

    private void CheckPlatform()
    {
        if (Application.isEditor)
            waveSkipButton.SetActive(true);
    }
    #endregion

    #region GameLogic per Frame
    private void MoveBackground()
    {
        float x = backgroundBase.GetComponent<RectTransform>().anchoredPosition.x;
        float y = backgroundBase.GetComponent<RectTransform>().anchoredPosition.y;

        // Set Bg move direction randomly
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
    void SetGold()
    {
        goldCount.text = GameManger.instance.player.playerGold.ToString() + " G";
    }
    void SetJem()
    {
        jemCount.text = GameManger.instance.player.playerJem.ToString();
    }
    void SetHpUI()
    {
        // Calc current hp value Converting to the range of (0~20, the # of hp bars)
        int temp = (int)Math.Round((GameManger.instance.player.playerHealth / GameManger.instance.player.maxPlayerHealth) * 20);
        temp = Math.Min(temp, 20);
        temp = Math.Max(temp, 0);

        // Make progressbar animation if hp changed
        if (Math.Abs(temp-prevHp)>0)
        {
            int gap = temp - prevHp;
            prevHp = temp;

            // Just add gap amount if effect is alreary in applying
            if (isHpCalc)
                targetHpIndex += gap;
            else // if not, start new progress bar animation
            {
                targetHpIndex = gap;
                StartCoroutine(IHpAnimation());
            }
        }
    }
    IEnumerator IHpAnimation()
    {
        isHpCalc = true; 
        while (targetHpIndex != 0)  // make effect if target amount remains
        {
            if (targetHpIndex > 0)
            {
                if(currentHpIndex< hpBars.Length - 1)
                {
                    // make Recovery anim, fill in progressbar
                    var tween = hpBars[currentHpIndex + 1].DOFade(1f, 0.1f);   
                    yield return tween.WaitForCompletion();
                    currentHpIndex += 1;
                }
                targetHpIndex -= 1;
            }
            else
            {
                if (currentHpIndex >= 0)
                {
                    // make Get Damage anim, erase progressbar
                    var tween = hpBars[currentHpIndex ].DOFade(0f, 0.1f);   
                    yield return tween.WaitForCompletion();
                    currentHpIndex -= 1;
                }
                targetHpIndex += 1;
            }
        }
        isHpCalc = false;
    }
    #endregion

    #region Ingame Animation
    public void StartDamageEffect()
    {
        if (GameManger.instance.onHitEffect)
            StartCoroutine("DamageEffect");
    }
    IEnumerator DamageEffect()
    {
        bloodEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        bloodEffect.gameObject.SetActive(false);
    }
    public void SetBossWaveEffect()
    {
        bossAppearEffect.SetActive(!bossAppearEffect.activeSelf);
    }

    public void WaveClear(int num)
    {
        curWave = num;
        if (num == 41)
        {
            waveLevel.text = "Bonus";
            StartCoroutine("BonusWaveEffect");
            return;
        }
        else if (num % 10 == 0)
        {
            waveLevel.text = "Boss";
            return;
        }
        else
        {
            waveLevel.text = "Wave " + Convert.ToString(num);
        }
        StartCoroutine("WaveClearEffect");
    }
    IEnumerator BonusWaveEffect()
    {
        bonusWaveEffect.SetActive(true);
        yield return new WaitForSeconds(4f);
        bonusWaveEffect.SetActive(false);
    }
    IEnumerator WaveClearEffect()
    {
        waveClearEffect.SetActive(true);
        yield return new WaitForSeconds(4f);
        waveClearEffect.SetActive(false);
    }
    #endregion

    #region Ingame Settings
    public void PauseGame()
    {
        SoundManager.instance.PlaySFX("BasicButtonSound");
        Time.timeScale = 0;
        GameManger.instance.isPlaying = false;
        pauseUi.SetActive(true);
        bgmSlider.value = SoundManager.instance.GetBgmVolume();
        sfxSlider.value = SoundManager.instance.GetSfxVolume();
    }
    public void ResumeGame()
    {
        pauseUi.SetActive(false);
        Time.timeScale = times[timeScaleIdx % times.Length];
        GameManger.instance.isPlaying = true;
    }
    public void SetTimeScale()
    {
        float time = times[++timeScaleIdx % times.Length];
        timeScaleText.text = String.Format("{0:0.0}", time);
        Time.timeScale = time;
    }
    #endregion

    #region Stat Upgrade Button
    public void OnAttackUpButton()
    {
        if(GameManger.instance.player.PlayerDamageLevelUp())
            ButtonAccepted(0);
        else
            ButtonDenied(0);
        curAtk = (int)((Mathf.Round(GameManger.instance.player.playerDamage * 10) * 0.1f) * 1000);
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
            statButtons[index].transform.DOPunchScale(originalScale * punchScale, 0.2f, 0, 1f).SetUpdate(true).OnComplete(() =>
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
            statButtons[index].transform.DOPunchPosition(new Vector3(punchPosition, 0, 0), 0.5f, 10, 1f).SetUpdate(true).OnComplete(() =>
            {
                isTweening = false;
            });
        }
    }
    #endregion

    #region Game Result
    public IEnumerator ShowEndingCredit()
    {
        endingCredit.SetActive(true);
        var textTween=endingCreditText.GetComponent<RectTransform>().DOAnchorPosY(6336, 70).SetEase(Ease.Linear);
        yield return textTween.WaitForCompletion();

        var endTween = thanksText.DOFade(1, 2);
        yield return endTween.WaitForCompletion();

        endTween = thanksText.DOFade(0, 5.5f);
        endingImage.DOFade(0, 5.5f);
        yield return endTween.WaitForCompletion();
        endingCredit.SetActive(false);

        ActiveGameOverUI();
        SoundManager.instance.PlayBGM("InGameBGM");
    }

    public void SkipEndingCredit()
    {
        DOTween.KillAll();
        endingCredit.SetActive(false);

        ActiveGameOverUI();
    }

    public void ActiveGameOverUI()
    {
        gameOverUI.SetActive(true);
        if (GameManger.instance.isTicketMode)
            jemDoubleText.SetActive(true);
        if (GameManger.instance.isNewRecord)
            newRecordText.SetActive(true);
        StartCoroutine(ResultCountingEffect());
    }

    // Set GameOver Ui Contents
    // Counting effect, the number is flipped by x-axis while increasing
    IEnumerator ResultCountingEffect()
    {
        int value= curWave;
        int loops = Math.Min(10, value);
        int count = Math.Max(1, (int)Math.Round((float)value / 10));

        for (int i = 0; i < loops; i++)
        {
            var tween=resultScore.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.1f).SetRelative();
            yield return tween.WaitForCompletion();
            resultScore.text = (i * count).ToString();
        }
        resultScore.text = curWave.ToString();

        value = int.Parse(LocalDatabaseManager.instance.HighScore);
        loops = Math.Min(10, value);
        count = Math.Max(1, (int)Math.Round((float)value / 10));
        for (int i = 0; i < loops; i++)
        {
            var tween = resultHighScore.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.1f).SetRelative();
            yield return tween.WaitForCompletion();
            resultHighScore.text = (i *count).ToString();
        }
        resultHighScore.text = LocalDatabaseManager.instance.HighScore;

        int time = (int)GameManger.instance.playTime;
        string timeString = time < 60 ? time + "s" : time / 60 + "m " + time % 60 + "s";
        var result=resultTime.DOText(timeString, 1.5f);
        yield return result.WaitForCompletion();

        value = GameManger.instance.player.playerJem;
        loops = Math.Min(10, value);
        count = Math.Max(1, (int)Math.Round((float)value / 10));
        for (int i = 0; i < loops; i++)
        {
            var tween = resultJem.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.1f).SetRelative();
            yield return tween.WaitForCompletion();
            resultJem.text = (i *count).ToString();
        }
        resultJem.text = value.ToString();

        value = GameManger.instance.playTicket;
        loops = Math.Min(10, value);
        count = Math.Max(1, (int)Math.Round((float)value / 10));
        for (int i = 0; i < loops; i++)
        {
            var tween = resultTicket.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.1f).SetRelative();
            yield return tween.WaitForCompletion();
            resultTicket.text = (i *count).ToString();
        }
        resultTicket.text = value.ToString();
    }

    public void PushLobbyButton()
    {
        GameManger.instance.GoLobby();
    }

    public void SaveGameResult()
    {
        int currentGameJem = GameManger.instance.player.playerJem;
        if (curWave > 40)
            curWave = 40;
        if (curWave > int.Parse(LocalDatabaseManager.instance.HighScore))
            GameManger.instance.isNewRecord = true;

        GameManger.instance.player.playerJem = GameManger.instance.isTicketMode ? (int)(currentGameJem * 1.5f) : currentGameJem;
        LocalDatabaseManager.instance.JemCount += GameManger.instance.player.playerJem;
        LocalDatabaseManager.instance.HighScore = curWave.ToString();
        LocalDatabaseManager.instance.Ticket += GameManger.instance.playTicket;
        LocalDatabaseManager.instance.SaveGameData();
    }
    #endregion

    #region GameOver Effect
    public IEnumerator IResetHpBars()
    {
        // Clear all remaining Hp bars when the Game ends
        for (int i = currentHpIndex; i >= 0; i--)
        {
            hpBars[i].DOKill();
            var tween = hpBars[i].DOFade(0f, 0.05f).SetEase(Ease.InCubic).SetUpdate(true);
            yield return tween.WaitForCompletion();
        }
    }

    public void CloseCanvas()
    {
        attackRange.SetActive(false);
        safeArea.SetActive(false);
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
    #endregion

    #region For Debugging
    public void SkipButton()
    {
        GameManger.instance.waveManager.WaveSkipButton();
    }
    #endregion
}