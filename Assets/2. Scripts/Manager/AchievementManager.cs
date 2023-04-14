using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IAchieve
{
    public string achieveName;
    public string achieveDescription;
    public int[] maxCondition;
    public int[] reward;
    public int curValue;
    public int achieveLevel;
    public bool isSuccess()
    {
        if (maxCondition[achieveLevel] <= curValue)
            return true;
        else
            return false;
    }
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    public Dictionary<string, IAchieve> achieves = new Dictionary<string,IAchieve>();
    private string[] achieveNames;

    public DataBase achieveDB;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        achieveDB = GameManger.instance.database;

        LoadAchieveStringIndex();
        LoadAchieveInfo();
    }

    private void Start()
    {
        LoadAchieve();
    }

    public void LoadAchieve()
    {
        LocalDatabaseManager.instance.LoadAchieveData();

        for(int i = 0; i < achieves.Count; i++)
        {
            achieves[achieveNames[i]].curValue = LocalDatabaseManager.instance.AchieveCurValue[i];
            achieves[achieveNames[i]].achieveLevel = LocalDatabaseManager.instance.AchieveCurLevel[i];
        }
    }

    public void SaveAchieve()
    {
        for (int i = 0; i < achieves.Count; i++)
        {
            LocalDatabaseManager.instance.AchieveCurValue[i] = achieves[achieveNames[i]].curValue;
            LocalDatabaseManager.instance.AchieveCurLevel[i] = achieves[achieveNames[i]].achieveLevel;
        }

        LocalDatabaseManager.instance.SaveAchieveData();
    }

    public void LoadAchieveStringIndex()
    {
        achieveNames = new string[achieveDB.Achieves.Count];
        for (int achieveCount = 0; achieveCount < achieveDB.Achieves.Count; achieveCount++)
        {
            achieveNames[achieveCount] = achieveDB.Achieves[achieveCount].achieveName;
            achieves[achieveNames[achieveCount]] = new IAchieve();
        }
    }

    public void LoadAchieveInfo()
    {
        string[] tempCondition;
        string[] tempReward;

        for (int achieveCount = 0; achieveCount < achieves.Count; achieveCount++)
        {
            achieves[achieveNames[achieveCount]].achieveName = achieveDB.Achieves[achieveCount].achieveName;
            achieves[achieveNames[achieveCount]].achieveDescription = achieveDB.Achieves[achieveCount].achieveDescription;

            achieves[achieveNames[achieveCount]].maxCondition = new int[achieveDB.Achieves[achieveCount].maxAchieveLevel];
            achieves[achieveNames[achieveCount]].reward = new int[achieveDB.Achieves[achieveCount].maxAchieveLevel];

            tempCondition = achieveDB.Achieves[achieveCount].maxCondition.Split(',');
            tempReward = achieveDB.Achieves[achieveCount].reward.Split(',');
            for (int achieveLevel = 0; achieveLevel < achieveDB.Achieves[achieveCount].maxAchieveLevel; achieveLevel++)
            {
                achieves[achieveNames[achieveCount]].maxCondition[achieveLevel] = int.Parse(tempCondition[achieveLevel]);
                achieves[achieveNames[achieveCount]].reward[achieveLevel] = int.Parse(tempReward[achieveLevel]);
            }
        }
    }

    public void SavePartsAchieve()
    {
        if (LocalDatabaseManager.instance.PartsMissile[2] == 1)
            if (LocalDatabaseManager.instance.PartsBarrier[2] == 1)
                if (LocalDatabaseManager.instance.PartsEmp[2] == 1)
                    if (LocalDatabaseManager.instance.PartsLaser[2] == 1)
                        achieves["World Class Engineer"].curValue = 4;
    }
}