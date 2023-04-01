using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocalDatabaseManager : MonoBehaviour
{
    // Save and Manage all user data( high score, jem, parts info)

    // used as Singleton pattern
    public static LocalDatabaseManager instance;

    #region User Data
    public int JemCount { get; set; } = 0;
    private int highScore = 0;
    public int HighScore {
        get { return highScore; }
        set { highScore = Mathf.Max(value, highScore); }
     }
    public string CurrentParts { get; set; } = "Missile";

    //Parts Upgrade info, each index indicates how many times it had been upgraded
    public int[] MaxUpgradeInfo { get; } = { 6, 6, 1 };
    public int[] PartsMissile { get; set; } = { 0, 0, 0};
    public int[] PartsBarrier { get; set; } = { 0, 0, 0};
    public int[] PartsLaser { get; set; } = { 0, 0, 0};
    public int[] PartsEmp { get; set; } = { 0, 0, 0};
    public int[] PartsValue { get; set; }

    public int Ticket { get; set; } = 3;



    /// <summary>
    /// 사용법(기본적으로 3차원 배열 형태, 근데 딕셔너리를 사용해서 첫번째 원소는 파츠 이름으로 쉽게 알아볼수 있도록..)
    /// 미사일 파츠의 첫번째 강화 정도를 알고 싶다면?
    /// LocalDatabaseManager.instance.PartsStatInfo["Missile"][0, LocalDatabaseManager.instance.PartsMissile[0]];
    /// 두번째 능력의 강화 정도를 알고싶다면?
    /// LocalDatabaseManager.instance.PartsStatInfo["Missile"][1, LocalDatabaseManager.instance.PartsMissile[1]];
    /// PartsStatInfo 자체는 데이터 테이블 느낌으로 readonly, PartsMissile에서 저장하는 강화 '정도'를 이용해서 현재 강화 정도에 해당하는 값 '참조'
    /// </summary>
    public Dictionary<string, float[,]> PartsStatInfo { get; } =new Dictionary<string, float[,]>
    {
        // 각 파츠별로 세 가지 속성(공격력, 공격 속도 - 공통, 특수 능력 해방 여부)- 순서대로
        //                                                 공격력                                                      공격속도                                     특수능력 해방(1이면 해방)
        {"Missile",new float[3,7]{ { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f }, { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f }, { 0, 1, -1, -1, -1, -1, -1 } } },
        {"Laser",new float[3,7]{ { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f }, { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f }, { 0, 1, -1, -1, -1, -1, -1 } } },
        {"Barrier",new float[3,7]{ { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f }, { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f }, { 0, 1, -1, -1, -1, -1, -1 } } },
        {"Emp",new float[3,7]{ { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f }, { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f }, { 0, 1, -1, -1, -1, -1, -1 } } },
    };

    // readonly data table of amount of the jem comsumtion for each parts' upgrade
    public int[,,] PartsUpgradeJem { get; } = {
        {{500,600,700,800,900,1000 },{500,600,700,800,900,1000 },{2000,-1,-1,-1,-1,-1 } },
        {{500,600,700,800,900,1000 },{500,600,700,800,900,1000 },{2000,-1,-1,-1,-1,-1 } },
        {{500,600,700,800,900,1000 },{500,600,700,800,900,1000 },{2000,-1,-1,-1,-1,-1 } },
        {{500,600,700,800,900,1000 },{500,600,700,800,900,1000 },{2000,-1,-1,-1,-1,-1 } },
    };
    
    #endregion

    void Awake()
    {
        if (instance == null)   
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);     
        }
        else
            Destroy(this.gameObject);

        LoadData();
        switch (CurrentParts)
        {
            case "Missile":
                PartsValue = PartsMissile;
                break;
            case "Laser":
                PartsValue = PartsLaser;
                break;
            case "Barrier":
                PartsValue = PartsBarrier;
                break;
            case "Emp":
                PartsValue = PartsEmp;
                break;
        }
    }

    /// <summary>
    /// Load all data at the start of the Game, in Lobby Scene
    /// </summary>
    public void LoadData()
    {
        if (PlayerPrefs.HasKey("JemCount"))
            JemCount = PlayerPrefs.GetInt("JemCount");
        if (PlayerPrefs.HasKey("HighScore"))
            HighScore = PlayerPrefs.GetInt("HighScore");
        if (PlayerPrefs.HasKey("Ticket"))
            HighScore = PlayerPrefs.GetInt("Ticket");
        if (PlayerPrefs.HasKey("CurrentParts"))
            CurrentParts = PlayerPrefs.GetString("CurrentParts");

        // string format : "val1, val2, val3"
        string[] temp;
        if (PlayerPrefs.HasKey("PartsMissile"))
        {
            temp = PlayerPrefs.GetString("PartsMissile").Split(',');
            for(int i = 0; i < PartsMissile.Length; i++)
                PartsMissile[i] = int.Parse(temp[i]);
        }
        if (PlayerPrefs.HasKey("PartsBarrier"))
        {
            temp = PlayerPrefs.GetString("PartsBarrier").Split(',');
            for (int i = 0; i < PartsBarrier.Length; i++)
                PartsBarrier[i] = int.Parse(temp[i]);
        }
        if (PlayerPrefs.HasKey("PartsLaser"))
        {
            temp = PlayerPrefs.GetString("PartsLaser").Split(',');
            for (int i = 0; i < PartsLaser.Length; i++)
                PartsLaser[i] = int.Parse(temp[i]);
        }
        if (PlayerPrefs.HasKey("PartsEmp"))
        {
            temp = PlayerPrefs.GetString("PartsEmp").Split(',');
            for (int i = 0; i < PartsEmp.Length; i++)
                PartsEmp[i] = int.Parse(temp[i]);
        }
    }

    /// <summary>
    /// Save Ingame Data, jem+high score 
    /// </summary>
    public void SaveGameData()
    {
        PlayerPrefs.SetInt("JemCount", JemCount);
        PlayerPrefs.SetInt("HighScore", HighScore);
        PlayerPrefs.SetInt("Ticket", Ticket);
    }

    /// <summary>
    /// Save Parts Data, currnet equiped parts+parts upgrade info
    /// </summary>
    public void SavePartsData()
    {
        PlayerPrefs.SetString("CurrentParts", CurrentParts);
        string temp = "";
        foreach(int part in PartsMissile)
            temp += (part.ToString()+",");
        PlayerPrefs.SetString("PartsMissile", temp);
        temp = "";
        foreach (int part in PartsBarrier)
            temp += (part.ToString() + ",");
        PlayerPrefs.SetString("PartsBarrier", temp);
        temp = "";
        foreach (int part in PartsLaser)
            temp += (part.ToString() + ",");
        PlayerPrefs.SetString("PartsLaser", temp);
        temp = "";
        foreach (int part in PartsEmp)
            temp += (part.ToString() + ",");
        PlayerPrefs.SetString("PartsEmp", temp);
    }
}
