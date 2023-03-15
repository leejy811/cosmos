using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스 A : 사거리에서 멈춘 뒤 보스 A의 패턴을 시작
// 보스 B : 플레이어의 사거리 전에는 빠른 속도로 접근하다가 사거리에 도착하면 느린 속도로 접근
// 보스 C : Player를 중심으로 일정한 속도로 공전하면서 적 C 생성

public class Boss : MonoBehaviour
{
    [SerializeField]
    private int bossType;
    [SerializeField]
    private float bossHealth;
    [SerializeField]
    private float bossSpeed;
    [SerializeField]
    private Transform[] spawnPoints;
    public PlayerController playerController;
    public WaveManager waveManager;

    private float bossPatternTime;
    private bool enterBossPlayerRange;
    List<GameObject> enemyList = new List<GameObject>();

    Vector2 playerPos = new Vector2(0.0f, 1.0f);
    bool bossCSpawnEnemy;
    public void Start()
    {
        BossLookPlayer();
        enterBossPlayerRange = false;
        bossAPattern = true;
        bossCSpawnEnemy = true;
    }

    public void BossLookPlayer()
    {
        float targetAngle = Vector2.Angle(transform.up, playerPos - (Vector2)transform.position);
        targetAngle = transform.position.x >= 0 ? targetAngle : -targetAngle;
        transform.Rotate(new Vector3(0, 0, targetAngle));
    }

    public void MoveBoss()
    {
        CheckPlayer();
        if (enterBossPlayerRange && bossType == 2)
        {
            this.gameObject.transform.RotateAround(playerPos, Vector3.forward, bossSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += transform.up * Time.deltaTime * bossSpeed;

        }
    }


    // 보스의 타입에 따라서 플레이어의 사거리까지 간 뒤 속도 조절
    public void CheckPlayer()
    {
        float changeSpeed = 0;

        if (transform.position.y - playerPos.y <= playerController.attackRange)
        {
            switch (bossType)
            {
                case 0:
                    changeSpeed = 0;
                    break;
                case 1:
                    changeSpeed = 0.3f;
                    break;
                case 2:
                    changeSpeed = 15f;
                    break;
                case 3:
                    changeSpeed = 0;
                    break;
            }

            bossSpeed = changeSpeed;
            enterBossPlayerRange = true;
        }
    }
    bool bossAPattern = true;
    public void FixedUpdate()
    {
        MoveBoss();
        if(enterBossPlayerRange && bossType == 0 && bossAPattern)
        {
            StartCoroutine("BossAPattern");
            bossAPattern = false;
        }

        if(enterBossPlayerRange && bossType == 2 && bossCSpawnEnemy)
        {
            StartCoroutine("BossCPattern");
            bossCSpawnEnemy = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            float damage = playerController.GetPlayerDamage();
            GetDamage(damage);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.tag == "Missile")
        {
            other.gameObject.GetComponent<PartsContorller>().MissileAttack();
        }
    }

    public void GetDamage(float damage)
    {
        if (bossHealth - damage <= 0)
        {
            BossDie();
        }
        else
            bossHealth -= damage;
    }

    public void BossDie()
    {
        bossHealth = 0;
        waveManager.CheckWaveEnd();
        Destroy(gameObject);
    }

    public void BossPattern(int type)
    {
        switch (type)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;

        }

    }


    IEnumerator BossAPattern()
    {
        yield return new WaitForSeconds(5f);
        //enemyList.Clear();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject enemy = GameManger.instance.poolManager.GetPool("EnemyA");
            enemy.GetComponent<Enemy>().SetEnemyState(waveManager.waves[waveManager.currentWave].enemyAHp, waveManager.waves[waveManager.currentWave].enemyADamage);
            enemy.transform.position = spawnPoints[i].position;
            enemy.GetComponent<Enemy>().EnemyLookPlayer();
            enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
            enemy.GetComponent<Enemy>().waveManager = this.waveManager;
            enemy.GetComponent<Enemy>().enemySpeed = 0;
            enemyList.Add(enemy);
            yield return new WaitForSeconds(0.5f);
        }
        for(int i=0; i<enemyList.Count; i++)
        {
            enemyList[i].GetComponent<Enemy>().enemySpeed = 1;
        }
        enemyList.Clear();
        yield return new WaitForSeconds(5f);
        bossAPattern = true;
    }
    IEnumerator BossCPattern()
    {
        yield return new WaitForSeconds(1f);
        GameObject enemyC = GameManger.instance.poolManager.GetPool("EnemyC");
        enemyC.GetComponent<Enemy>().SetEnemyState(5, 3);
        enemyC.transform.position = this.transform.position;
        enemyC.GetComponent<Enemy>().EnemyLookPlayer();
        enemyC.GetComponent<Enemy>().playerController = GameManger.instance.player;
        enemyC.GetComponent<Enemy>().waveManager = this.waveManager;
        enemyC.GetComponent<Enemy>().enemySpeed = 2;

        yield return new WaitForSeconds(2f);
        bossCSpawnEnemy = true;
    }
    IEnumerator StopBossC()
    {
        yield return new WaitForSeconds(10f);
    }
}
