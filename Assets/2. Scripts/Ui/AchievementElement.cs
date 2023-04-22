using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementElement : MonoBehaviour
{
    [SerializeField] private Text achievementName;
    [SerializeField] private Text achievementDescription;
    [SerializeField] private Image achievementRateBar;
    [SerializeField] private Text achievementReward;
    [SerializeField] private Image achievementIcon;

    private IAchieve achieve;

    void Start()
    {
        achieve = AchievementManager.instance.achieves[achievementName.text];
        SetAchievementValue();
    }

    public void SetAchievementValue()
    {
        if (achieve.achieveDescription == "?????????????")
            achievementDescription.text = achieve.achieveDescription;
        else
            achievementDescription.text =  achieve.achieveDescription+ " " +  achieve.maxCondition[achieve.achieveLevel];
        achievementRateBar.fillAmount = (float)achieve.curValue / achieve.maxCondition[achieve.achieveLevel];
        achievementReward.text = achieve.reward[achieve.achieveLevel].ToString();
    }

    public void OnRewardButton()
    {
        if (!achieve.isSuccess() || achieve.maxAchieveLevel==achieve.achieveLevel)
            return;
        
        LocalDatabaseManager.instance.JemCount += achieve.reward[achieve.achieveLevel];
        LocalDatabaseManager.instance.SaveGameData();
        GameManger.instance.lobbyUiManager.SetJem();

        achieve.achieveLevel++;
        SetAchievementValue();
        AchievementManager.instance.SaveAchieve();
    }
}
