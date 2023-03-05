using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct IWave
{
    public int enemyACount;
    public int enemyBCount;
}
public class WaveManager: MonoBehaviour
{

    [SerializeField]
    private Transform[] spawnPoint;

    private IWave[] waves;

    float spawntimer;
    int currentWave = 0;
    bool isWaveEnd = false;

    void Start()
    {
        waves = new IWave[40];
        for(int i=0; i<40;i++)
        {
            waves[i].enemyACount = (i * 10) + 10;
            waves[i].enemyBCount = (i * 12) * 10;
        }
    }
    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();

    }
    void Update()
    {
        //spawntimer += Time.deltaTime;

        //if (spawntimer > 0.7f)
        //{
        //    EnemySpawn();
        //    spawntimer = 0;
        //}
        //만약 적이 하나도 남아있지 않다면 ~~deltatime만큼 보낸 뒤
        //currentwave 증가시키고 코루틴 실행
        if(isWaveEnd)
        {
            Debug.Log("Stage : " + currentWave + 1); 
            currentWave++;
            isWaveEnd = false;
            StartCoroutine("StartWave");
        }

    }

    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(10);
        int currentACount = waves[currentWave].enemyACount, currentBCount = waves[currentWave].enemyACount;
        bool isSpawnEnd = false;
        while (currentACount != 0 && currentBCount != 0)
        {
            string ranType = Random.Range(0, 2) == 0 ? "EnemyTriangle" : "EnemyCircle";
            GameObject enemy = GameManger.instance.poolManager.GetPool(ranType);
            enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
            enemy.GetComponent<Enemy>().EnemyLookPlayer();
            enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
            
            if(ranType == "EnemyTriangle")
            {
                currentACount--;
            }
            else
            {
                currentBCount--;
            }
            yield return new WaitForSeconds(0.7f);
        }
    }
    void EnemySpawn()
    {
        string ranType = Random.Range(0, 2) == 0 ? "EnemyTriangle" : "EnemyCircle";
        GameObject enemy = GameManger.instance.poolManager.GetPool(ranType);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().EnemyLookPlayer();
        enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
    }
}
