using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IAchieve 클래스는 업적 정보의 인터페이스를 제공하는 클래스이다.
/// </summary>
[System.Serializable]
public class IAchieve
{
    public string achieveName;
    public string achieveDescription;
    public int[] maxCondition;
    public int[] reward;
    public int curValue;
    public int achieveLevel;
    public int maxAchieveLevel;
    public bool isSuccess()
    {
        if (maxCondition[achieveLevel] <= curValue)
            return true;
        else
            return false;
    }
}

/// <summary>
/// AchievementManager는 업적들의 정보를 저장하고 관리하는 변수이다. achieves에는 업적의 정보가 저장되어 있고 
/// Key 값은 업적의 이름으로 되어있다. achieves의 정보를 접근할땐 예를들어 achieves["Starting from the basic"].achieveName이렇게 호출하면 된다.
/// 업적의 성공여부는 isSuccess함수의 반환 값을 사용하면 된다.
/// </summary>
public class AchievementManager : MonoBehaviour
{
    //Singleton Pattern
    public static AchievementManager instance;

    //업적의 정보가 모두 들어있는 Dictionary
    public Dictionary<string, IAchieve> achieves = new Dictionary<string, IAchieve>();
    private string[] achieveNames;

    //Execl안의 정보를 가져온 DataBase
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
        LoadAchieve();
    }


    //LocalDataBase에서 업적 정보를 가져오는 함수
    public void LoadAchieve()
    {
        LocalDatabaseManager.instance.LoadAchieveData();

        for (int i = 0; i < achieves.Count; i++)
        {
            achieves[achieveNames[i]].curValue = LocalDatabaseManager.instance.AchieveCurValue[i];
            achieves[achieveNames[i]].achieveLevel = LocalDatabaseManager.instance.AchieveCurLevel[i];
        }
    }

    //LocalDataBase에 업적 정보를 저장하는 함수
    public void SaveAchieve()
    {
        for (int i = 0; i < achieves.Count; i++)
        {
            LocalDatabaseManager.instance.AchieveCurValue[i] = achieves[achieveNames[i]].curValue;
            LocalDatabaseManager.instance.AchieveCurLevel[i] = achieves[achieveNames[i]].achieveLevel;
        }

        LocalDatabaseManager.instance.SaveAchieveData();
    }

    //achieves의 Key값을 초기화 해주는 함수
    public void LoadAchieveStringIndex()
    {
        achieveNames = new string[achieveDB.Achieves.Count];
        for (int achieveCount = 0; achieveCount < achieveDB.Achieves.Count; achieveCount++)
        {
            achieveNames[achieveCount] = achieveDB.Achieves[achieveCount].achieveName;
            achieves[achieveNames[achieveCount]] = new IAchieve();
        }
    }

    //Execl에서 업적 정보를 가져올 때 변수를 할당해주는 함수다.
    public void LoadAchieveInfo()
    {
        string[] tempCondition;
        string[] tempReward;

        for (int achieveCount = 0; achieveCount < achieves.Count; achieveCount++)
        {
            achieves[achieveNames[achieveCount]].achieveName = achieveDB.Achieves[achieveCount].achieveName;
            achieves[achieveNames[achieveCount]].achieveDescription = achieveDB.Achieves[achieveCount].achieveDescription;
            achieves[achieveNames[achieveCount]].maxAchieveLevel = achieveDB.Achieves[achieveCount].maxAchieveLevel;

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

    //파츠 업적 정보를 저장하는 함수다.
    public void SavePartsAchieve()
    {
        if (LocalDatabaseManager.instance.PartsMissile[2] == 1)
            if (LocalDatabaseManager.instance.PartsBarrier[2] == 1)
                if (LocalDatabaseManager.instance.PartsEmp[2] == 1)
                    if (LocalDatabaseManager.instance.PartsLaser[2] == 1)
                        achieves["World Class Engineer"].curValue = 4;
    }

    //return if any achievement cleared
    public bool IsNewAchievementCleared()
    {
        foreach(IAchieve achieve in achieves.Values)
            if (achieve.isSuccess())
                return true;
        return false;
    }
}