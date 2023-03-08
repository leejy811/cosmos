using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger instance;
    public PoolManager poolManager;
    public PlayerController player;
    public UiManager UiManager;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }
    public void GameOver()
    {

    }
}
