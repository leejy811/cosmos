using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DataBaseEntity;

public class WaveManager: MonoBehaviour
{

    [SerializeField]
    private Transform[] spawnPoint;
    public List<WaveDBEntity> waves;

    public int currentWave = -1;

    public int totalKillEnemyACount;
    public int totalKillEnemyBCount;
    public int totalKillEnemyCCount;
    public int totalKillEnemyDCount;
    public int bonusWaveTime;

    [SerializeField]
    private GameObject[] boss;

    private bool isBossWave;
    public bool isBossLive;
    private int currenBossType;

    public DataBase waveDB;
    
    void Start()
    {
        isBossWave = false;
        bonusWaveTime = 0;
        //Debug.Log("Stage : " + (currentWave + 1));
        //GameManger.instance.uiManager.WaveClear(currentWave + 1);
        StartCoroutine("StartWave");
        LoadWaveData();
    }

    void Update()
    {
        if (waves[currentWave].enemyACount == 0 && waves[currentWave].enemyBCount == 0 
            && waves[currentWave].enemyCCount == 0 && waves[currentWave].enemyDCount == 0 && !CheckBossWave())
        {
            GoNextWave();
        }
        if(isBossWave && !isBossLive)
        {
            if (CheckBossWaveEnd())
            {
                isBossWave = false;
                GoNextWave();
            }
        }
        if(currentWave == 40)
        {
            bonusWaveTime += (int)Time.deltaTime;
        }
    }

    private bool CheckBossWave()
    {
        if ((currentWave + 1) % 10 == 0)
            return true;
        else return false;
    }

    public void WaveSkipButton()
    {
        if(isBossWave)
        {
            GameObject bossA = GameObject.Find("Boss A(Clone)");
            GameObject bossB = GameObject.Find("Boss B(Clone)");
            GameObject bossC = GameObject.Find("Boss C(Clone)");
            GameObject bossD = GameObject.Find("Boss D(Clone)");
            if (bossA != null)
                Destroy(bossA);
            else if (bossB != null)
                Destroy(bossB);
            else if (bossC != null)
                Destroy(bossC);
            else if (bossD != null)
                Destroy(bossD);
        }
        GameManger.instance.poolManager.DelPoolObject();
        GameManger.instance.player.playerGold += waves[currentWave].enemyACount * waves[currentWave].enemyAPrice;
        GameManger.instance.player.playerGold += waves[currentWave].enemyBCount * waves[currentWave].enemyBPrice;
        GameManger.instance.player.playerGold += waves[currentWave].enemyCCount * waves[currentWave].enemyCPrice;
        GameManger.instance.player.playerGold += waves[currentWave].enemyDCount * waves[currentWave].enemyDPrice;
        GameManger.instance.player.playerJem += waves[currentWave].enemyBCount * waves[currentWave].enemyBJem;
        GameManger.instance.player.playerJem += waves[currentWave].enemyCCount * waves[currentWave].enemyCJem;
        GameManger.instance.player.playerJem += waves[currentWave].enemyDCount * waves[currentWave].enemyDJem;

        StopCoroutine("StartWave");
        GoNextWave();
    }
    bool CheckBossWaveEnd()
    {
        Debug.Log("Check Boss Wave End Start ");
        if (currenBossType == 0)
        {
            if (GameManger.instance.poolManager.CheckPool("BossASpawnEnemy"))
                return false;
        }
        else if (currenBossType == 2)
        {
            if (GameManger.instance.poolManager.CheckPool("BossCSpawnEnemy"))
                return false;
        }
        else if (currenBossType == 3)
        {
            if (GameManger.instance.poolManager.CheckPool("BossDSpawnEnemy"))
                return false;
        }
        return true;
    }
    public void GoNextWave()
    {
        currentWave++;
        Debug.Log("Stage : " + (currentWave + 1));
        GameManger.instance.uiManager.WaveClear(currentWave+1);
        StartCoroutine("StartWave");
    }
    IEnumerator StartWave()
    {
        int currentACount = waves[currentWave].enemyACount, currentBCount = waves[currentWave].enemyBCount;
        int currentCCount = waves[currentWave].enemyCCount, currentDCount = waves[currentWave].enemyDCount;      
        yield return new WaitForSeconds(5);

        while (true)
        {
            if (GameManger.instance.player.isPlayerDie)
                break;
            if ((currentWave + 1) % 10 == 0 && (currentWave + 1) >= 10)
            {
                BossSpawn(currentWave / 10);
                break;
            }

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

            if (currentACount == 0 && ranType == "EnemyA")
                continue;
            else if (currentBCount == 0 && ranType == "EnemyB")
                continue;
            else if (currentCCount == 0 && ranType == "EnemyC")
                continue;
            else if (currentDCount == 0 && ranType == "EnemyD")
                continue;

            if (ranType == "EnemyA" && currentWave != 40)
                currentACount--;
            else if (ranType == "EnemyB" && currentWave != 40)
                currentBCount--;
            else if (ranType == "EnemyC" && currentWave != 40)
                currentCCount--;
            else if (ranType == "EnemyD" && currentWave != 40)
                currentDCount--;

            EnemySpawn(ranType);
            yield return new WaitForSeconds(waves[currentWave].spawnCoolTime);

            // 모든 적 소환하면 코루틴 종료
            if (currentACount == 0 && currentBCount == 0 && currentCCount == 0 && currentDCount == 0)
                break;
        }
    }
    public void EnemySpawn(string type)
    {
        GameObject enemy = GameManger.instance.poolManager.GetPool(type);
        if (type == "EnemyA")
            enemy.GetComponent<Enemy>().SetEnemyState(waves[currentWave].enemyAHp, waves[currentWave].enemyADamage, waves[currentWave].enemyAPrice, waves[currentWave].enemyAJem);
        else if(type == "EnemyB")
            enemy.GetComponent<Enemy>().SetEnemyState(waves[currentWave].enemyBHp, waves[currentWave].enemyBDamage, waves[currentWave].enemyBPrice, waves[currentWave].enemyBJem);
        else if(type == "EnemyC")
            enemy.GetComponent<Enemy>().SetEnemyState(waves[currentWave].enemyCHp, waves[currentWave].enemyCDamage, waves[currentWave].enemyCPrice, waves[currentWave].enemyCJem);
        else if (type == "EnemyD")
        {
            enemy.GetComponent<Enemy>().SetEnemyState(waves[currentWave].enemyDHp, waves[currentWave].enemyDDamage, waves[currentWave].enemyDPrice, waves[currentWave].enemyDJem);
            waves[currentWave].enemyACount += 2;
        }

        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().EnemyLookPlayer();
        enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
        enemy.GetComponent<Enemy>().waveManager = this;

    }

    void BossSpawn(int bossType)
    {
        isBossWave = true;
        currenBossType = bossType;
        Boss boss = Instantiate(this.boss[bossType]).GetComponent<Boss>();
        boss.transform.position = spawnPoint[0].position;
        boss.BossLookPlayer();
        boss.playerController = GameManger.instance.player;
        boss.waveManager = this;
    }

    private void LoadWaveData()
    {
        waves = waveDB.Waves.ToList(); ;
    }
}
