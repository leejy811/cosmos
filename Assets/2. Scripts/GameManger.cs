using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger instance;
    public PoolManager poolManager;
    public PlayerController player;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }
}
