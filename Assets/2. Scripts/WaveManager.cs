using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct IWave
{
    public int enemyACount;
    public float enemyADamage;
    public float enemyAHp;
    public int enemyBCount;
    public float enemyBHp;
    public float enemyBDamage;
}
public class WaveManager: MonoBehaviour
{

    [SerializeField]
    private Transform[] spawnPoint;
    public IWave[] waves;

    float spawntimer;
    public int currentWave = -1;
    public bool isWaveEnd;

    void Start()
    {
        waves = new IWave[40];
        for(int i=0; i<40;i++)
        {
            waves[i].enemyACount = (i * 5) + 5;
            waves[i].enemyAHp = (i * 5) + 3;
            waves[i].enemyADamage = (i * 2) + 2;
            waves[i].enemyBCount = (i * 7) + 5;
            waves[i].enemyBHp = (i * 4) + 5;
            waves[i].enemyBDamage = (i * 6) + 6;
        }
        Debug.Log("Stage : " + (currentWave + 1));
        StartCoroutine("StartWave");
    }
    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();

    }
    void Update()
    {
        CheckWaveEnd();

    }
    private void CheckWaveEnd()
    {
        if (waves[currentWave].enemyACount == 0 && waves[currentWave].enemyBCount == 0)
        {
            currentWave++;
            Debug.Log("Stage : " + (currentWave + 1));
            GameManger.instance.UiManager.IncreaseWave(currentWave + 1);
            StartCoroutine("StartWave");
        }
    }
    IEnumerator StartWave()
    {
        int currentACount = waves[currentWave].enemyACount, currentBCount = waves[currentWave].enemyBCount;
        yield return new WaitForSeconds(5);
        Debug.Log("Wave : " + (currentWave + 1) + " Enemy A Cout : " + waves[currentWave].enemyACount + " Enemy B count : " + waves[currentWave].enemyBCount);
        Debug.Log("Wave : " + (currentWave + 1) + " Enemy A Cout : " + currentACount + " Enemy B count : " + currentBCount);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            string ranType = Random.Range(0, 2) == 0 ? "EnemyTriangle" : "EnemyCircle";

            if (currentACount == 0 && currentBCount == 0)
                break;
            if(currentACount == 0)
                ranType = "EnemyCircle";
            else if(currentBCount == 0)
                ranType = "EnemyTriangle";

            if (ranType == "EnemyTriangle")
                currentACount--;
            else if(ranType == "EnemyCircle")
                currentBCount--;

            Debug.Log("1.Enemy A count : " + currentACount + " Enemy B count : " + currentBCount);
            Debug.Log("2.Enemy A Cout : " + waves[currentWave].enemyACount + " Enemy B count : " + waves[currentWave].enemyBCount);

            EnemySpawn(ranType);
        }
        Debug.Log("Wave Spawn End");
    }
    void EnemySpawn(string type)
    {
        GameObject enemy = GameManger.instance.poolManager.GetPool(type);
        if (type == "EnemyTriangle")
            enemy.GetComponent<Enemy>().SetEnemyState(waves[currentWave].enemyAHp, waves[currentWave].enemyADamage);
        else
            enemy.GetComponent<Enemy>().SetEnemyState(waves[currentWave].enemyBHp, waves[currentWave].enemyBDamage);

        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().EnemyLookPlayer();
        enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
        enemy.GetComponent<Enemy>().waveManager = this;

    }
}
