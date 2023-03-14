using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float bossAPatterntime;
    private bool enterBossPlayerRange;

    Vector2 playerPos = new Vector2(0.0f, 1.0f);

    public void Start()
    {
        BossLookPlayer();
        enterBossPlayerRange = false;
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
        transform.position += transform.up * Time.deltaTime * bossSpeed;
    }

    // 보스의 타입에 따라서 플레이어의 사거리까지 간 뒤 속도 조절
    // 보스 A : 사거리에서 멈춘 뒤 보스 A의 패턴을 시작
    // 보스 B : 플레이어의 사거리 전에는 빠른 속도로 접근하다가 사거리에 도착하면 느린 속도로 접근

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
                    changeSpeed = 0;
                    break;
                case 3:
                    changeSpeed = 0;
                    break;
            }

            bossSpeed = changeSpeed;
            enterBossPlayerRange = true;
        }
    }

    public void FixedUpdate()
    {
        MoveBoss();
        if(enterBossPlayerRange && bossAPatterntime > 10)
        {
            BossAPattern();
            bossAPatterntime = 0;
        }
        bossAPatterntime += Time.deltaTime;
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
                BossAPattern();
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;

        }

    }
    void BossAPattern()
    {
        for(int i=0; i<10;i++)
        {
            GameObject enemy = GameManger.instance.poolManager.GetPool("EnemyTriangle");
            enemy.GetComponent<Enemy>().SetEnemyState(waveManager.waves[waveManager.currentWave].enemyAHp, waveManager.waves[waveManager.currentWave].enemyADamage);
            enemy.transform.position = spawnPoints[i].position;
            enemy.GetComponent<Enemy>().EnemyLookPlayer();
            enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
            enemy.GetComponent<Enemy>().waveManager = this.waveManager;
        }
    }
}
