using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DataBaseEntity;

[System.Serializable]
public struct IWave
{
    public int enemyACount;
    public float enemyAHp;
    public float enemyADamage;
    public int enemyAPrice;
    public int enemyAJem;

    public int enemyBCount;
    public float enemyBHp;
    public float enemyBDamage;
    public int enemyBPrice;
    public int enemyBJem;

    public int enemyCCount;
    public float enemyCHp;
    public float enemyCDamage;
    public int enemyCPrice;
    public int enemyCJem;

    public int enemyDCount;
    public float enemyDHp;
    public float enemyDDamage;
    public int enemyDPrice;
    public int enemyDJem;

    public float spawnCoolTime;
}

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

    private DataBase waveDB;

    public int waveACount;
    public int waveBCount;
    public int waveCCount;
    public int waveDCount;

    void Start()
    {
        LoadWaveData();
        isBossWave = false;
        bonusWaveTime = 0;
        StartCoroutine("StartWave");
    }

    void Update()
    {
        if (GameManger.instance.player.isPlayerDie)
            return;

        if (waveACount == 0 && waveBCount == 0 
            && waveCCount == 0 && waveDCount == 0 && !CheckBossWave())
        {
            GoNextWave();
        }
        if(isBossWave && !isBossLive)
        {
            if (CheckBossWaveEnd())
            {
                isBossWave = false;
                SoundManager.instance.PlayBGM("InGameBGM");
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
        GameManger.instance.uiManager.WaveClear(currentWave+1);
        StartCoroutine("StartWave");
    }
    IEnumerator StartWave()
    {

        waveACount = waves[currentWave].enemyACount;
        waveBCount = waves[currentWave].enemyBCount;
        waveCCount = waves[currentWave].enemyCCount;
        waveDCount = waves[currentWave].enemyDCount;

        int currentACount = waves[currentWave].enemyACount, currentBCount = waves[currentWave].enemyBCount;
        int currentCCount = waves[currentWave].enemyCCount, currentDCount = waves[currentWave].enemyDCount;

        if ((currentWave + 1) % 10 == 0 && (currentWave + 1) >= 10)
        {
            SoundManager.instance.PlaySFX("BossAppear");
            GameManger.instance.uiManager.BossWaveEffectOn();
            yield return new WaitForSeconds(5);
            SoundManager.instance.StopSfx("BossAppear");
            GameManger.instance.uiManager.BossWaveEffectOff();
            
            BossSpawn(currentWave / 10);
            yield break;
        }
        yield return new WaitForSeconds(5);

        while (true)
        {
            if (GameManger.instance.player.isPlayerDie)
                break;

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
            waveACount += 2;
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

        SoundManager.instance.PlayBGM("Boss" + (char)(bossType + 65));
        boss.transform.position = spawnPoint[0].position;
        boss.BossLookPlayer();
        boss.playerController = GameManger.instance.player;
        boss.waveManager = this;
    }

    private void LoadWaveData()
    {
        waveDB = GameManger.instance.database;
        waves = waveDB.Waves.ToList();

        /*
        waves = new IWave[waveDB.Waves.Count];
        for(int i = 0; i < waveDB.Waves.Count; i++)
        {
            waves[i].enemyACount = waveDB.Waves[i].enemyACount;
            waves[i].enemyAHp = waveDB.Waves[i].enemyAHp;
            waves[i].enemyADamage = waveDB.Waves[i].enemyADamage;
            waves[i].enemyAPrice = waveDB.Waves[i].enemyAPrice;
            waves[i].enemyAJem = waveDB.Waves[i].enemyAJem;

            waves[i].enemyBCount = waveDB.Waves[i].enemyBCount;
            waves[i].enemyBHp = waveDB.Waves[i].enemyBHp;
            waves[i].enemyBDamage = waveDB.Waves[i].enemyBDamage;
            waves[i].enemyBPrice = waveDB.Waves[i].enemyBPrice;
            waves[i].enemyBJem = waveDB.Waves[i].enemyBJem;

            waves[i].enemyCCount = waveDB.Waves[i].enemyCCount;
            waves[i].enemyCHp = waveDB.Waves[i].enemyCHp;
            waves[i].enemyCDamage = waveDB.Waves[i].enemyCDamage;
            waves[i].enemyCPrice = waveDB.Waves[i].enemyCPrice;
            waves[i].enemyCJem = waveDB.Waves[i].enemyCJem;

            waves[i].enemyDCount = waveDB.Waves[i].enemyDCount;
            waves[i].enemyDHp = waveDB.Waves[i].enemyDHp;
            waves[i].enemyDDamage = waveDB.Waves[i].enemyDDamage;
            waves[i].enemyDPrice = waveDB.Waves[i].enemyDPrice;
            waves[i].enemyDJem = waveDB.Waves[i].enemyDJem;

            waves[i].spawnCoolTime = waveDB.Waves[i].spawnCoolTime;
        }
         */
    }
}
