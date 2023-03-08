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
    [SerializeField] private Text currentRecovery;
    [SerializeField] private Text recoveryUpGold;
    [SerializeField] private Animation bottomUiAnim;
    #endregion

    #region Member Variables
    private bool isFold = false;
    #endregion

    private void Awake()
    {
        SetPlayerState();
    }
    private void Update()
    {
        SetHpUI();
        SetGold();
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

    public void IncreaseWave(int num)
    {
        waveLevel.text = (Convert.ToString(num));
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

}
