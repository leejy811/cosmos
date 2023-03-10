using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDatabaseManager : MonoBehaviour
{
    public static LocalDatabaseManager instance;

    public int JemCount { get; set; } = 0;
    private int highScore = 0;
    public int HighScore {
        get { return highScore; }
        set { highScore = Mathf.Max(value, highScore); }
     }
    public int Parts1 { get; set; } = 0;
    public int Parts2 { get; set; } = 0;
    public int Parts3 { get; set; } = 0;
    public int Parts4 { get; set; } = 0;

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

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("JemCount"))
            JemCount = PlayerPrefs.GetInt("JemCount");
        if (PlayerPrefs.HasKey("HighScore"))
            HighScore = PlayerPrefs.GetInt("HighScore");
        if (PlayerPrefs.HasKey("Parts1"))
            Parts1 = PlayerPrefs.GetInt("Parts1");
        if (PlayerPrefs.HasKey("Parts2"))
            Parts2 = PlayerPrefs.GetInt("Parts2");
        if (PlayerPrefs.HasKey("Parts3"))
            Parts3 = PlayerPrefs.GetInt("Parts3");
        if (PlayerPrefs.HasKey("Parts4"))
            Parts4 = PlayerPrefs.GetInt("Parts4");
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("JemCount", JemCount);
        PlayerPrefs.SetInt("HighScore", HighScore);
        PlayerPrefs.SetInt("Parts1", Parts1);
        PlayerPrefs.SetInt("Parts2", Parts2);
        PlayerPrefs.SetInt("Parts3", Parts3);
        PlayerPrefs.SetInt("Parts4", Parts4);
    }
}
