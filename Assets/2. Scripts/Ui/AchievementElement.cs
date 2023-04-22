using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AchievementElement : MonoBehaviour
{
    [SerializeField] private Text achievementName;
    [SerializeField] private Text achievementDescription;
    [SerializeField] private Image achievementRateBar;
    [SerializeField] private Text achievementReward;
    [SerializeField] private Image achievementIcon;
    [SerializeField] private Transform particleBase;

    private IAchieve achieve;
    private bool isTweening=false;

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
        if (!achieve.isSuccess() || achieve.maxAchieveLevel == achieve.achieveLevel)
        {
            SoundManager.instance.PlaySFX("ButtonDenied");
            if (!isTweening)    // Prevent multi-clicking
            {
                isTweening = true;
                particleBase.transform.DOPunchPosition(new Vector3(20, 0, 0), 0.5f, 10, 1f).OnComplete(() =>
                {
                    isTweening = false;
                });
            }
            return;
        }

        if (!isTweening)    // Prevent multi-clicking
        {
            isTweening = true;
            particleBase.DOPunchScale(particleBase.localScale * 0.2f, 0.2f, 0, 1f).OnComplete(() =>
            {
                isTweening = false;
            });
        }
        SoundManager.instance.PlaySFX("PartsUpgradeSound");

        LocalDatabaseManager.instance.JemCount += achieve.reward[achieve.achieveLevel];
        LocalDatabaseManager.instance.SaveGameData();
        GameManger.instance.lobbyUiManager.SetJem(particleBase);

        achieve.achieveLevel++;
        SetAchievementValue();
        AchievementManager.instance.SaveAchieve();
    }
}
