using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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
    #endregion

    #region Member Variables
    private bool isFold = false;
    private float currentTime = 0f;
    private float fadeInTime = 2f;
    #endregion

    private void Awake()
    {
        SetPlayerState();
    }
    private void Start()
    {
        StartCoroutine("FadeIn"); 
    }
    private void Update()
    {
        SetHpUI();
        SetGold();
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
        goldCount.text = GameManger.instance.player.playerGold.ToString();
    }
    void SetHpUI()
    {
        hpScrollbar.size = GameManger.instance.player.PlayerMaxHealthPerCurHealth();
    }
    public void PauseGame()
    {

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
        currentAtk.text = GameManger.instance.player.GetPlayerDamage().ToString();
        atkUpGold.text = GameManger.instance.player.playerDamageCost.ToString() + " G";
        currentSpeed.text = GameManger.instance.player.GetPlayerAtkSpeed().ToString();
        speedUpGold.text = GameManger.instance.player.playerAtkSpeedCost.ToString() + " G";
        currentHp.text = GameManger.instance.player.GetPlayerHealth().ToString();
        hpUpGold.text = GameManger.instance.player.playerMaxHealthCost.ToString() + " G";
        currentRecovery.text = GameManger.instance.player.GetPlayerHealthRecorvery().ToString() + " /sec";
        recoveryUpGold.text = GameManger.instance.player.playerHealthRecorveryCost.ToString() + " G";
    }
    public void OnAttackUpButton()
    {
        GameManger.instance.player.PlayerDamageLevelUp();
        currentAtk.text = GameManger.instance.player.GetPlayerDamage().ToString();
        atkUpGold.text = GameManger.instance.player.playerDamageCost.ToString() + " G";
    }

    public void OnSpeedUpButton()
    {
        GameManger.instance.player.PlayerAttackSpeedLevelUp();
        currentSpeed.text = GameManger.instance.player.GetPlayerAtkSpeed().ToString();
        speedUpGold.text = GameManger.instance.player.playerAtkSpeedCost.ToString() + " G";
    }

    public void OnHpUpButton()
    {
        GameManger.instance.player.PlayerMaxHealthLevelUp();
        currentHp.text = GameManger.instance.player.GetPlayerHealth().ToString();
        hpUpGold.text = GameManger.instance.player.playerMaxHealthCost.ToString() + " G";
    }

    public void OnRecoveryUpButton()
    {
        GameManger.instance.player.PlayerHealthRecorveryLevelUp();
        currentRecovery.text = GameManger.instance.player.GetPlayerHealthRecorvery().ToString() + " /sec";
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
        LocalDatabaseManager.instance.JemCount += int.Parse(jemCount.text);
        LocalDatabaseManager.instance.HighScore = int.Parse(waveLevel.text);
        LocalDatabaseManager.instance.SaveData();
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
        string[] texts =new string[] { "Wave Clear !", "5", "4", "3", "2", "1", "" };
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
}
