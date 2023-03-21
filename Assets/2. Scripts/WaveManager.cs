using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct IWave
{
    public int enemyACount;
    public float enemyAHp;
    public float enemyADamage;
    public int enemyAPrice;

    public int enemyBCount;
    public float enemyBHp;
    public float enemyBDamage;
    public int enemyBPrice;

    public int enemyCCount;
    public float enemyCHp;
    public float enemyCDamage;
    public int enemyCPrice;

    public int enemyDCount;
    public float enemyDHp;
    public float enemyDDamage;
    public int enemyDPrice;

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
        //waves = new IWave[40];
        //for(int i=0; i<40;i++)
        //{
        //    waves[i].enemyACount = (i * 3) + 5;
        //    waves[i].enemyAHp = (i * 2) + 1;
        //    waves[i].enemyADamage = i;
        //    waves[i].enemyBCount = (i * 4) + 5;
        //    waves[i].enemyBHp = (i * 3) + 5;
        //    waves[i].enemyBDamage = i;
        //    waves[i].enemyCCount = (i * 4) + 10;
        //    waves[i].enemyCHp = (i * 3) + 5;
        //    waves[i].enemyCDamage = i;
        //    waves[i].enemyDCount = (i * 4) + 10;
        //    waves[i].enemyDHp = (i * 3) + 5;
        //    waves[i].enemyDDamage = i;
        //}

        Debug.Log("Stage : " + (currentWave + 1));
        StartCoroutine("StartWave");
    }
    private void Awake()
    {

    }
    void Update()
    {
        if (waves[currentWave].enemyACount == 0 && waves[currentWave].enemyBCount == 0 && waves[currentWave].enemyCCount == 0 && waves[currentWave].enemyDCount == 0)
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
        int currentCCount = waves[currentWave].enemyCCount, currentDCount = waves[currentWave].enemyDCount;      
        yield return new WaitForSeconds(5);

        while (true)
        {
            if ((currentWave + 1) % 10 == 0 && (currentWave + 1) >= 10)
            {
                BossSpawn(currentWave / 10);
                break;
            }
            //if (currentWave == 0)
            //{
            //    BossSpawn(4);
            //    break;
            //}
            //else if (currentWave == 1)
            //{
            //    BossSpawn(1);
            //    break;
            //}
            //else if(currentWave == 2)
            //{
            //    BossSpawn(2);
            //    break;
            //}
            string ranType;

            int enemyT = Random.Range(0, 4);
            if(enemyT == 0)
            {
                ranType = "EnemyA";
            }
            else if(enemyT == 1)
            {
                ranType = "EnemyB";
            }
            else if(enemyT == 2)
            {
                ranType = "EnemyC";
            }
            else
            {
                ranType = "EnemyD";
            }

            if (currentACount == 0 && currentBCount == 0 && currentCCount == 0 && currentDCount == 0)
                break;
            //if(currentACount == 0 && currentDCount == 0)
            //    ranType = "EnemyB";
            //else if(currentBCount == 0 && currentDCount == 0)
            //    ranType = "EnemyA";
            //else if(currentACount == 0 && currentBCount == 0)
            //    ranType = "EnemyD";

            if (currentACount == 0 && ranType == "EnemyA")
                continue;
            else if (currentBCount == 0 && ranType == "EnemyB")
                continue;
            else if (currentCCount == 0 && ranType == "EnemyC")
                continue;
            else if (currentDCount == 0 && ranType == "EnemyD")
                continue;

            if (ranType == "EnemyA")
                currentACount--;
            else if (ranType == "EnemyB")
                currentBCount--;
            else if (ranType == "EnemyC")
                currentCCount--;
            else if (ranType == "EnemyD")
                currentDCount--;

            EnemySpawn(ranType);
            yield return new WaitForSeconds(1f);

        }
    }
    public void EnemySpawn(string type)
    {
        GameObject enemy = GameManger.instance.poolManager.GetPool(type);
        if (type == "EnemyA")
            enemy.GetComponent<Enemy>().SetEnemyState(waves[currentWave].enemyAHp, waves[currentWave].enemyADamage, waves[currentWave].enemyAPrice);
        else if(type == "EnemyB")
            enemy.GetComponent<Enemy>().SetEnemyState(waves[currentWave].enemyBHp, waves[currentWave].enemyBDamage, waves[currentWave].enemyBPrice);
        else if(type == "EnemyC")
            enemy.GetComponent<Enemy>().SetEnemyState(waves[currentWave].enemyCHp, waves[currentWave].enemyCDamage, waves[currentWave].enemyCPrice);
        else if (type == "EnemyD")
        {
            enemy.GetComponent<Enemy>().SetEnemyState(waves[currentWave].enemyDHp, waves[currentWave].enemyDDamage, waves[currentWave].enemyDPrice);
            waves[currentWave].enemyACount += 2;
        }

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
