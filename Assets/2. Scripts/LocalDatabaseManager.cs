using UnityEngine;

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
    public int[] PartsMissile { get; set; } = { 0, 0, 0, 0 };
    public int[] PartsProtocol { get; set; } = { 0, 0, 0, 0 };
    public int[] PartsLaser { get; set; } = { 0, 0, 0, 0 };
    public int[] PartsEmp { get; set; } = { 0, 0, 0, 0 };
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
        if (PlayerPrefs.HasKey("CurrentParts"))
            CurrentParts = PlayerPrefs.GetString("CurrentParts");

        // string format : "val1, val2, val3, val4"
        string[] temp;
        if (PlayerPrefs.HasKey("PartsMissile"))
        {
            temp = PlayerPrefs.GetString("PartsMissile").Split(',');
            for(int i = 0; i < PartsMissile.Length; i++)
                PartsMissile[i] = int.Parse(temp[i]);
        }
        if (PlayerPrefs.HasKey("PartsProtocol"))
        {
            temp = PlayerPrefs.GetString("PartsProtocol").Split(',');
            for (int i = 0; i < PartsProtocol.Length; i++)
                PartsProtocol[i] = int.Parse(temp[i]);
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
        foreach (int part in PartsProtocol)
            temp += (part.ToString() + ",");
        PlayerPrefs.SetString("PartsProtocol", temp);
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
