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
    [SerializeField] private TMP_Text rocksCount;
    [SerializeField] private Scrollbar hpScrollbar;
    [SerializeField] private TMP_Text waveLevel;
    [SerializeField] private TMP_Text currentAtk;
    [SerializeField] private TMP_Text atkUpRocks;
    [SerializeField] private TMP_Text currentSpeed;
    [SerializeField] private TMP_Text speedUpRocks;
    [SerializeField] private TMP_Text currentHp;
    [SerializeField] private TMP_Text hpUpRocks;
    [SerializeField] private TMP_Text currentRecovery;
    [SerializeField] private TMP_Text recoveryUpRocks;
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
        rocksCount.SetText(Convert.ToString(num));
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
        //this.transform.GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshPro>().text = GameManger.instance.player.GPD();
        Debug.Log(this.transform.GetChild(2).GetChild(1).GetChild(0).GetChild(1).GetChild(0).name);
    }

    public void OnSpeedUpButton()
    {
        GameManger.instance.player.PlayerAttackSpeedLevelUp();
        Debug.Log(this.transform.GetChild(2).GetChild(2).GetChild(1).name);
    }

    public void OnHpUpButton()
    {
        GameManger.instance.player.PlayerMaxHealthLevelUp();
        Debug.Log(this.transform.GetChild(2).GetChild(3).GetChild(2).name);
    }

    public void OnRecoveryUpButton()
    {
        GameManger.instance.player.PlayerHealthRecorveryLevelUp();
        Debug.Log(this.transform.GetChild(2).GetChild(2).GetChild(2).name);
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
