using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� A : ��Ÿ����� ���� �� ���� A�� ������ ����
// ���� B : �÷��̾��� ��Ÿ� ������ ���� �ӵ��� �����ϴٰ� ��Ÿ��� �����ϸ� ���� �ӵ��� ����
// ���� C : Player�� �߽����� ������ �ӵ��� �����ϸ鼭 �� C ����

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

    bool bossAPattern;
    bool bossCPattern;
    bool bossDPattern;

    [SerializeField]
    private GameObject bossDLaser;

    public void Start()
    {
        BossLookPlayer();
        enterBossPlayerRange = false;

        bossAPattern = true;
        bossCPattern = true;
        bossDPattern = true;
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


    // ������ Ÿ�Կ� ���� �÷��̾��� ��Ÿ����� �� �� �ӵ� ����
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
                    changeSpeed = 10f;
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
        if(enterBossPlayerRange && bossType == 0 && bossAPattern)
        {
            StartCoroutine("BossAPattern");
            bossAPattern = false;
        }

        if(enterBossPlayerRange && bossType == 2 && bossCPattern)
        {
            StartCoroutine("BossCPattern");
            bossCPattern = false;
        }

        if(enterBossPlayerRange && bossType == 3 && bossDPattern)
        {
            StartCoroutine("BossDPattern");
            bossDPattern = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            float damage = playerController.playerDamage;
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


    IEnumerator BossAPattern()
    {
        yield return new WaitForSeconds(2f);
        //enemyList.Clear();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject enemy = GameManger.instance.poolManager.GetPool("EnemyA");
            enemy.GetComponent<Enemy>().SetEnemyState(waveManager.waves[waveManager.currentWave].enemyAHp, waveManager.waves[waveManager.currentWave].enemyADamage, 
                                                      waveManager.waves[waveManager.currentWave].enemyAPrice, waveManager.waves[waveManager.currentWave].enemyAJem);
            //enemy.transform.position = spawnPoints[i].position;
            enemy.GetComponent<Enemy>().targetPos = spawnPoints[i].position;
            enemy.GetComponent<Enemy>().moveLerp = true;
            enemy.GetComponent<Enemy>().bossLerp = true;
            enemy.transform.position = this.transform.position;
            enemy.GetComponent<Enemy>().EnemyLookPlayer();
            enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
            enemy.GetComponent<Enemy>().waveManager = this.waveManager;
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
        bossAPattern = true;
    }
  
    IEnumerator BossCPattern()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject bossCSpawnEnemy = GameManger.instance.poolManager.GetPool("BossCSpawnEnemy");
        bossCSpawnEnemy.GetComponent<Enemy>().SetEnemyState(1, 1, 1, 1);
        bossCSpawnEnemy.transform.position = this.transform.position;
        bossCSpawnEnemy.GetComponent<Enemy>().EnemyLookPlayer();
        bossCSpawnEnemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
        bossCSpawnEnemy.GetComponent<Enemy>().waveManager = this.waveManager;
        bossCSpawnEnemy.GetComponent<Enemy>().enemySpeed = 2;

        yield return new WaitForSeconds(2f);
        bossCPattern = true;
    }
    IEnumerator BossDPattern()
    {
        yield return new WaitForSeconds(0.1f);
        int ranBossActive = Random.Range(0, 5);
        switch(ranBossActive)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                StartCoroutine("BossDSpawnEnemy");
                break;
            case 4:
                StartCoroutine("BossDShootLaser");
                break;
        }
        yield return new WaitForSeconds(3f);
        bossDPattern = true;
    }
    IEnumerator BossDSpawnEnemy()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject bossDSpawnEnemy = GameManger.instance.poolManager.GetPool("BossDSpawnEnemy");
            bossDSpawnEnemy.GetComponent<Enemy>().SetEnemyState(0, 1, 2, 1);
            //bossDSpawnEnemy.transform.position = spawnPoints[i].position;
            bossDSpawnEnemy.GetComponent<Enemy>().targetPos = spawnPoints[i].position;
            bossDSpawnEnemy.GetComponent<Enemy>().moveLerp = true;
            //bossDSpawnEnemy.GetComponent<Enemy>().bossLerp = true;
            bossDSpawnEnemy.GetComponent<Enemy>().EnemyLookPlayer();

            bossDSpawnEnemy.transform.position = this.transform.position;
            bossDSpawnEnemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
            bossDSpawnEnemy.GetComponent<Enemy>().waveManager = this.waveManager;
        }
        yield return new WaitForSeconds(5f);

    }
    // Boss D �� ������ ��� �ڷ�ƾ ������ּ���
     IEnumerator BossDShootLaser()
    {

        bossDLaser.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        bossDLaser.SetActive(false);
    }
}
