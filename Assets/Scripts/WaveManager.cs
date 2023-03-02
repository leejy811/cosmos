using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager: MonoBehaviour
{

    [SerializeField]
    private Transform[] spawnPoint;

    float spawntimer;

    void Start()
    {

    }
    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();

    }
    void Update()
    {
        spawntimer += Time.deltaTime;

        if (spawntimer > 0.7f)
        {
            EnemySpawn();
            spawntimer = 0;
        }

    }
    void EnemySpawn()
    {
        string ranType = Random.Range(0, 2) == 0 ? "EnemyTriangle" : "EnemyCircle";
        GameObject enemy = GameManger.instance.poolManager.GetPool(ranType);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
    }
   
}
