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

    public int currentWave = -1;
    [SerializeField]
    private GameObject[] boss;

    void Start()
    {
        waves = new IWave[40];
        for(int i=0; i<40;i++)
        {
            waves[i].enemyACount = (i * 3) + 5;
            waves[i].enemyAHp = (i * 2) + 3;
            waves[i].enemyADamage = i;
            waves[i].enemyBCount = (i * 4) + 5;
            waves[i].enemyBHp = (i * 3) + 5;
            waves[i].enemyBDamage = i;
        }
        Debug.Log("Stage : " + (currentWave + 1));
        StartCoroutine("StartWave");
    }
    private void Awake()
    {
        //spawnPoint = GetComponentsInChildren<Transform>();

    }
    void Update()
    {
        if (waves[currentWave].enemyACount == 0 && waves[currentWave].enemyBCount == 0)
        {
            CheckWaveEnd();
        }
    }
    public void CheckWaveEnd()
    {
            currentWave++;
            Debug.Log("Stage : " + (currentWave + 1));
            GameManger.instance.UiManager.IncreaseWave(currentWave + 1);
            StartCoroutine("StartWave");
    }
    IEnumerator StartWave()
    {
        int currentACount = waves[currentWave].enemyACount, currentBCount = waves[currentWave].enemyBCount;
        yield return new WaitForSeconds(5);
        //Debug.Log("Wave : " + (currentWave + 1) + " Enemy A Cout : " + waves[currentWave].enemyACount + " Enemy B count : " + waves[currentWave].enemyBCount);
        //Debug.Log("Wave : " + (currentWave + 1) + " Enemy A Cout : " + currentACount + " Enemy B count : " + currentBCount);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            //if ((currentWave +1)% 10 == 1 && (currentWave + 1) >= 10)
            //{
            //    BossSpawn(1);
            //    break;
            //}
            if(currentWave == 3)
            {
                BossSpawn(1);
                break;
            }
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

            //Debug.Log("1.Enemy A count : " + currentACount + " Enemy B count : " + currentBCount);
            //Debug.Log("2.Enemy A Cout : " + waves[currentWave].enemyACount + " Enemy B count : " + waves[currentWave].enemyBCount);

            EnemySpawn(ranType);
        }
       // Debug.Log("Wave Spawn End");
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

    void BossSpawn(int bossType)
    {
        Boss boss = Instantiate(this.boss[bossType - 1]).GetComponent<Boss>();
        boss.transform.position = spawnPoint[0].position;
        boss.BossLookPlayer();
        boss.playerController = GameManger.instance.player;
        boss.waveManager = this;
    }
}
