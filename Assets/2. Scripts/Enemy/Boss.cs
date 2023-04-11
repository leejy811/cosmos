using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    [SerializeField]
    private Transform[] targetPoints;

    public PlayerController playerController;
    public WaveManager waveManager;

    private float bossPatternTime;
    private bool enterBossPlayerRange;
    List<GameObject> enemyList = new List<GameObject>();

    Vector2 playerPos = new Vector2(0.0f, 1.0f);

    bool bossAPattern;
    bool bossCPattern;
    bool bossDPattern;

    private bool colorEffectOn;

    Color targetColor = new Color32(255, 0, 0, 255);
    Color bossColor;

    [SerializeField]
    private GameObject bossDLaser;
    [SerializeField]
    private GameObject bossDieEffect;
    public void Start()
    {
        BossLookPlayer();
        enterBossPlayerRange = false;

        bossAPattern = true;
        bossCPattern = true;
        bossDPattern = true;

        waveManager.isBossLive = true;
        bossColor = this.gameObject.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().material.color;

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
        if (enterBossPlayerRange)
            return;

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
    public void FixedUpdate()
    {
        if (GameManger.instance.player.isPlayerDie)
            return;
        MoveBoss();
        if(enterBossPlayerRange && bossType == 0 && bossAPattern)
        {
            StartCoroutine("BossAPattern");
            bossAPattern = false;
        }
        if(enterBossPlayerRange && bossType == 1)
        {
            this.transform.GetChild(0).transform.Rotate(Vector3.forward, 8f);
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
        else if (other.gameObject.tag == "Laser")
        {
            float damage = other.gameObject.GetComponentInParent<PartsContorller>().GetPartsDamage();
            GetDamage(damage);
        }
    }

    public void GetDamage(float damage)
    {
        FloatingDamageEffect(damage);
        if (bossHealth - damage <= 0)
        {
            BossDie();
        }
        else
            bossHealth -= damage;
    }
    private void FloatingDamageEffect(float damage)
    {
        this.gameObject.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().material.DOColor(targetColor, 0.5f);

        this.gameObject.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().material.DOColor(bossColor, 0.5f);
        GameObject hudText = GameManger.instance.poolManager.GetPool("DamageText");
        hudText.transform.position = new Vector3(this.transform.position.x +Random.Range(-0.5f, 0.5f), this.transform.position.y + 1f, 0);
        hudText.GetComponent<DamageText>().damage = (int)((Mathf.Round(GameManger.instance.player.playerDamage * 10) * 0.1f) * 100);
        //hudText.GetComponent<DamageText>().damage = (int)(GameManger.instance.player.playerDamage * 100);
    }
    //IEnumerator ColorEffect()
    //{
    //    // ���� A�� ��� �ǰ� �� ũ�Ⱑ Ŀ���� �۾���(����)
    //    if (bossType == 0 && bossType == 3)
    //        this.gameObject.transform.DOShakeScale(0.2f, 0.1f, 2, 90, false, ShakeRandomnessMode.Full);
    //    // ������ ���� ���� ������ ����
    //    Color targetColor = new Color(255, 0, 0, 255);
    //    Color bossColor;
    //    bossColor = this.gameObject.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().material.color;
    //    this.gameObject.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().material.DOColor(targetColor, 0.1f);

    //    yield return new WaitForSeconds(0.3f);

    //    // ������ �� ��ȯ
    //    this.gameObject.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().material.DOColor(bossColor, 0.1f);
    //    colorEffectOn = false;
    //}
    public void BossDie()
    {
        Debug.Log("Boss Die");
        bossDieEffect.SetActive(true);

        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.tag = "Untagged";
        gameObject.layer = 0;


        Invoke("BossDieEffectEnd", 1.5f);
    }
    public void GameOverDie()
    {
        bossDieEffect.SetActive(true);
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        Invoke("BossDieEffectEnd", 1.5f);
    }

    private void BossDieEffectEnd()
    {
        bossHealth = 0;
        waveManager.isBossLive = false;
        bossDieEffect.SetActive(false);
        gameObject.tag = "Boss";
        gameObject.layer = 6;
        Destroy(gameObject);
    }

IEnumerator BossAPattern()
    {
        yield return new WaitForSeconds(2f);

        //enemyList.Clear();

        for (int i = 0; i < targetPoints.Length; i++)
        {


            GameObject enemy = GameManger.instance.poolManager.GetPool("BossASpawnEnemy");
            // ������ ��ȯ�ϴ� �� ���� ���� (ü��, ������, ���, ��)
            enemy.GetComponent<Enemy>().SetEnemyState(5, 3, 0, 0);
            enemy.transform.position = spawnPoints[i/3].position;
            enemy.GetComponent<Enemy>().targetPos = targetPoints[i].position;
            enemy.GetComponent<Enemy>().moveLerp = true;
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
        // ������ ��ȯ�ϴ� �� ���� ���� (ü��, ������, ���, ��)
        bossCSpawnEnemy.GetComponent<Enemy>().SetEnemyState(80, 15, 0, 0);
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
                StartCoroutine("BossDSpawnEnemy");
                break;
            case 3:
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
            // ������ ��ȯ�ϴ� �� ���� ���� (ü��, ������, ���, ��)
            bossDSpawnEnemy.GetComponent<Enemy>().SetEnemyState(40, 30, 0, 0);
            //bossDSpawnEnemy.transform.position = spawnPoints[i].position;
            bossDSpawnEnemy.GetComponent<Enemy>().targetPos = spawnPoints[i].position;
            bossDSpawnEnemy.GetComponent<Enemy>().moveLerp = true;
            bossDSpawnEnemy.GetComponent<Enemy>().EnemyLookPlayer();

            bossDSpawnEnemy.transform.position = this.transform.position;
            bossDSpawnEnemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
            bossDSpawnEnemy.GetComponent<Enemy>().waveManager = this.waveManager;
        }
        yield return new WaitForSeconds(5f);

    }
     IEnumerator BossDShootLaser()
    {

        bossDLaser.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        SoundManager.instance.PlaySFX("BossDLaserSound");
        yield return new WaitForSeconds(1.0f);
        bossDLaser.SetActive(false);
    }
}