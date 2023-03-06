using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    // Manages all UI component
    // Contains functions for each Button

    #region UI Components
    [SerializeField] private TMP_Text jemCount;
    [SerializeField] private TMP_Text goldCount;
    [SerializeField] private Scrollbar hpScrollbar;
    [SerializeField] private TMP_Text waveLevel;
    [SerializeField] private TMP_Text currentAtk;
    [SerializeField] private TMP_Text atkUpGold;
    [SerializeField] private TMP_Text currentSpeed;
    [SerializeField] private TMP_Text speedUpGold;
    [SerializeField] private TMP_Text currentHp;
    [SerializeField] private TMP_Text hpUpGold;
    [SerializeField] private TMP_Text currentRecovery;
    [SerializeField] private TMP_Text recoveryUpGold;
    [SerializeField] private Animation bottomUiAnim;
    #endregion

    #region Member Variables
    private bool isFold = false;
    #endregion

    public void PauseGame()
    {

    }

    public void AddJem(int num=0)
    {
        jemCount.SetText(Convert.ToString(num));
    }

    public void SetRock(int num=0)
    {
        goldCount.SetText(Convert.ToString(num));
    }

    public void SetHp(float value=0.0f)
    {
        hpScrollbar.value = value;
    }

    public void IncreaseWave(int num)
    {
        waveLevel.SetText(Convert.ToString(num));
    }

    public void OnAttackUpButton()
    {
        GameManger.instance.player.PlayerDamageLevelUp();
        Debug.Log(this.name);
    }

    public void OnSpeedUpButton()
    {
        GameManger.instance.player.PlayerAttackSpeedLevelUp();
        Debug.Log(this.transform.name);
    }

    public void OnHpUpButton()
    {
        GameManger.instance.player.PlayerMaxHealthLevelUp();
        Debug.Log(this.transform.name);
    }

    public void OnRecoveryUpButton()
    {
        GameManger.instance.player.PlayerHealthRecorveryLevelUp();
        Debug.Log(this.transform.name);
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
