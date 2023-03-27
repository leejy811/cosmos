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
    public IWave[] waves;

    public int currentWave = -1;

    [SerializeField]
    private GameObject[] boss;

    private bool isBossWave;
    public bool isBossLive;
    private int currenBossType;
    void Start()
    {
        isBossWave = false;
        Debug.Log("Stage : " + (currentWave + 1));
        StartCoroutine("StartWave");
    }

    void Update()
    {
        if (waves[currentWave].enemyACount == 0 && waves[currentWave].enemyBCount == 0 
            && waves[currentWave].enemyCCount == 0 && waves[currentWave].enemyDCount == 0 && !isBossWave)
        {
            GoNextWave();
        }
        if(isBossWave && !isBossLive)
        {
            if (CheckBossWaveEnd())
                GoNextWave();
        }
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

        StopCoroutine("StartWave");
        GoNextWave();
    }
    bool CheckBossWaveEnd()
    {
        Debug.Log("Check Boss Wave End Start ");
        if (currenBossType == 0)
        {
            if (GameManger.instance.poolManager.CheckPool("EnemyA"))
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
        GameManger.instance.UiManager.WaveClear(currentWave+1);
        StartCoroutine("StartWave");
    }
    IEnumerator StartWave()
    {
        isBossWave = false;
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
            //    BossSpawn(0);
            //    break;
            //}
            //if (currentWave == 1)
            //{
            //    BossSpawn(1);
            //    break;
            //}
            //if (currentWave == 2)
            //{
            //    BossSpawn(2);
            //    break;
            //}
            //if (currentWave == 3)
            //{
            //    BossSpawn(3);
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
            
            // ��� �� ��ȯ�ϸ� �ڷ�ƾ ����
            if (currentACount == 0 && currentBCount == 0 && currentCCount == 0 && currentDCount == 0)
                break;

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
            yield return new WaitForSeconds(waves[currentWave].spawnCoolTime);
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
}
