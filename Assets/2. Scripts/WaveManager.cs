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

    public IWave[] waves;

    float spawntimer;
    public int currentWave = -1;
    public bool isWaveEnd;

    void Start()
    {
        waves = new IWave[40];
        for(int i=0; i<40;i++)
        {
            waves[i].enemyACount = (i * 10) + 10;
            waves[i].enemyBCount = (i * 12) + 10;
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
            currentWave++;
            isWaveEnd = false;
            Debug.Log("Stage : " + currentWave);

            StartCoroutine("StartWave");
        }

        if (waves[currentWave].enemyACount == 0 && waves[currentWave].enemyBCount == 0)
            isWaveEnd = true;
    }

    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(5);
        int currentACount = waves[currentWave].enemyACount, currentBCount = waves[currentWave].enemyACount;

        while (true)
        {
            string ranType = Random.Range(0, 2) == 0 ? "EnemyTriangle" : "EnemyCircle";

            if (currentACount == 0 && currentBCount == 0)
                break;
            if(currentACount == 0)
            {
                ranType = "EnemyCircle";
            }
            else if(currentBCount == 0)
            {
                ranType = "EnemyTriangle";
            }

            EnemySpawn(ranType);

            if (ranType == "EnemyTriangle")
            {
                currentACount--;
            }
            else
            {
                currentBCount--;
            }
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Wave Spawn End");
    }
    void EnemySpawn(string type)
    {
        GameObject enemy = GameManger.instance.poolManager.GetPool(type);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().EnemyLookPlayer();
        enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
        enemy.GetComponent<Enemy>().waveManager = this;

    }
}
