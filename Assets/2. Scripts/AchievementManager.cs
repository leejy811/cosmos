using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IAchieve
{
    public string achieveName;
    public string achieveDescription;
    public int[] maxCondition;
    public int[] reward;
    public int curValue;
    public bool[] isSuccess;
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    public IAchieve[] achieves = new IAchieve[12];

    public int[] achieveLevel { get; set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        achieveLevel = new int[achieves.Length];
    }

    public void LoadAchieve()
    {

        for(int i = 0; i < achieves.Length; i++)
        {
            achieves[i].curValue = LocalDatabaseManager.instance.AchieveCurValue[i];
            achieveLevel[i] = LocalDatabaseManager.instance.AchieveCurLevel[i];
        }
    }

    public void SaveAchieve()
    {
        for (int i = 0; i < achieves.Length; i++)
        {
            LocalDatabaseManager.instance.AchieveCurValue[i] = achieves[i].curValue;
            LocalDatabaseManager.instance.AchieveCurLevel[i] = achieveLevel[i];
        }
    }
}