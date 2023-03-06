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
    public int currentWave;
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
        //만약 적이 하나도 남아있지 않다면 ~~deltatime만큼 보낸 뒤
        //currentwave 증가시키고 코루틴 실행
        if(isWaveEnd)
        {
            Debug.Log("Stage : " + currentWave); 
            isWaveEnd = false;
            StartCoroutine("StartWave");
        }
    }

    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(5);
        int currentACount = waves[currentWave].enemyACount, currentBCount = waves[currentWave].enemyACount;
        //Debug.Log("Current Wave A Count : " + currentACount);
        //Debug.Log("Current Wave B Count : " + currentBCount);

        while (currentACount != 0 && currentBCount != 0)
        {
            string ranType = EnemySpawn();

            if (ranType == "EnemyTriangle")
            {
                currentACount--;
            }
            else
            {
                currentBCount--;
            }
            //Debug.Log("Current Wave A Count : " + currentACount + "  Current Wave B Count : " + currentBCount);

            yield return new WaitForSeconds(0.7f);
        }
        currentWave++;
    }
    string EnemySpawn()
    {
        string ranType = Random.Range(0, 2) == 0 ? "EnemyTriangle" : "EnemyCircle";
        GameObject enemy = GameManger.instance.poolManager.GetPool(ranType);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().EnemyLookPlayer();
        enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
        enemy.GetComponent<Enemy>().waveManager = this;
        return ranType;
    }
}
