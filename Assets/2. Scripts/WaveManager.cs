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
            GameManger.instance.UiManager.WaveClear(currentWave+1);
            StartCoroutine("StartWave");
    }
    IEnumerator StartWave()
    {
        int currentACount = waves[currentWave].enemyACount, currentBCount = waves[currentWave].enemyBCount;
        yield return new WaitForSeconds(5);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if ((currentWave + 1) % 10 == 0 && (currentWave + 1) >= 10)
            {
                BossSpawn((currentWave + 1) / 10);
                break;
            }
            //if (currentWave == 0)
            //{
            //    BossSpawn(2);
            //    break;
            //}
            //else if(currentWave == 4)
            //{
            //    BossSpawn(1);
            //    break;
            //}
            string ranType = Random.Range(0, 2) == 0 ? "EnemyA" : "EnemyB";

            if (currentACount == 0 && currentBCount == 0)
                break;
            if(currentACount == 0)
                ranType = "EnemyB";
            else if(currentBCount == 0)
                ranType = "EnemyA";

            if (ranType == "EnemyA")
                currentACount--;
            else if(ranType == "EnemyB")
                currentBCount--;

            EnemySpawn(ranType);
        }
    }
    public void EnemySpawn(string type)
    {
        GameObject enemy = GameManger.instance.poolManager.GetPool(type);
        if (type == "EnemyA")
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
        Boss boss = Instantiate(this.boss[bossType]).GetComponent<Boss>();
        boss.transform.position = spawnPoint[0].position;
        boss.BossLookPlayer();
        boss.playerController = GameManger.instance.player;
        boss.waveManager = this;
    }
}
