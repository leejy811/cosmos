using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger instance;
    public PoolManager poolManager;
    public GameObject player;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }
}
