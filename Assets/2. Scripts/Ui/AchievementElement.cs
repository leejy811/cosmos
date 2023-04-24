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
    [SerializeField] private Image rewardButton;
    [SerializeField] private Transform particleBase;
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private Sprite[] iconImages;
    [SerializeField] private Sprite[] btnImages;
    [SerializeField] private bool isHiddenMission;

    private IAchieve achieve;
    private bool isTweening=false;

    void Start()
    {
        achieve = AchievementManager.instance.achieves[achievementName.text];
        SetAchievementValue();
    }

    public void SetAchievementValue()
    {
        if (achieve.achieveLevel >= achieve.maxAchieveLevel)     // Set clear panel if max level
        {
            clearPanel.SetActive(true);
            return;
        }

        if(!isHiddenMission)                                                      // Only show level icon if not hidden mission
            achievementIcon.sprite = iconImages[achieve.achieveLevel];

        if (isHiddenMission)         // Set Mission description
            achievementDescription.text = achieve.achieveDescription;
        else
            achievementDescription.text =  achieve.achieveDescription+ " " +  achieve.maxCondition[achieve.achieveLevel];
        achievementRateBar.fillAmount = (float)achieve.curValue / achieve.maxCondition[achieve.achieveLevel];
        achievementReward.text = achieve.reward[achieve.achieveLevel].ToString();

        if (achieve.isSuccess())
            rewardButton.sprite = btnImages[1];
        else
            rewardButton.sprite = btnImages[0];
    }

    public void OnRewardButton()
    {
        // return if condition not achieved
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

        // Add button&sound effect
        if (!isTweening)    // Prevent multi-clicking
        {
            isTweening = true;
            if (!isHiddenMission)
                achievementIcon.transform.DOPunchScale(achievementIcon.transform.localScale*0.2f, 0.2f);
            particleBase.DOPunchScale(particleBase.localScale * 0.2f, 0.2f, 0, 1f).OnComplete(() =>
            {
                isTweening = false;
            });
        }
        SoundManager.instance.PlaySFX("PartsUpgradeSound");

        // Add Jems and save to local
        LocalDatabaseManager.instance.JemCount += achieve.reward[achieve.achieveLevel];
        LocalDatabaseManager.instance.SaveGameData();
        GameManger.instance.lobbyUiManager.SetJem(particleBase);

        achieve.achieveLevel++;
        SetAchievementValue();
        AchievementManager.instance.SaveAchieve();

        // Reset Clear Info in Lobby UI
        GameManger.instance.lobbyUiManager.SetAchievementFlag();
        if (achieve.isSuccess())
            rewardButton.sprite = btnImages[1];
        else
            rewardButton.sprite = btnImages[0];
    }
}
