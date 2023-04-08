using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

/* PlayerController Class
 * �� Ŭ������ Player�� �ܹ߼� ������ �����ϰ� �⺻ ������ �����ϴ� Ŭ������.
 */
public class PlayerController : MonoBehaviour
{
    
    //Player ����
    public float playerDamage { get; private set; } = 1;
    public float playerAttackSpeed { get; private set; } = 2;
    public float maxPlayerHealth { get; private set; } = 5;
    public float playerHealth { get; private set; } = 5;
    public int playerShield { get; set; } = 0;
    //public float playerHealth { get; private set; } = 10000;

    public float playerHealthRecorvery { get; private set; } = 0;

    //Player ���� ���׷��̵� Cost
    public int playerDamageCost { get; private set; } = 3;
    public int playerAtkSpeedCost { get; private set; } = 3;
    public int playerMaxHealthCost { get; private set; } = 5;
    public int playerHealthRecorveryCost { get; private set; } = 5;

    //�÷��̾� ���� ��ȭ
    public int playerGold { get; set; } = 0;
    public int playerJem { get; set; } = 0;

    //Ÿ�� ���� ����
    private Collider2D[] targets;
    public float attackRange { get; private set; } = 5;
    public Transform nearestTarget { get; private set; }

    private Transform playerSprite;

    private float rotateSpeed=-50;
    private float targetSpeed;
    private float standardTime = 2f;
    private float currentTime;

    [SerializeField]
    private GameObject PlayerDieParticle;
    public bool isPlayerDie;

    //Player ���� LevelUp ���� �Լ���
    public bool PlayerDamageLevelUp()
    {
        if (playerGold < playerDamageCost)
            return false;
        PayGold(playerDamageCost);
        playerDamage += 0.1f;
        playerDamageCost += 2;
        playerDamage = Mathf.Round(playerDamage * 10) * 0.1f;
        return true;
    }
    public bool PlayerAttackSpeedLevelUp()
    {
        if (playerGold < playerAtkSpeedCost)
            return false;
        PayGold(playerAtkSpeedCost);
        playerAttackSpeed += 0.05f;
        playerAtkSpeedCost += 2;
        return true;
    }
    public bool PlayerMaxHealthLevelUp()
    {
        if (playerGold < playerMaxHealthCost)
            return false;
        PayGold(playerMaxHealthCost);
        maxPlayerHealth += 3f;
        playerHealth += 3f;
        playerMaxHealthCost += 5;
        return true;
    }
    public bool PlayerHealthRecorveryLevelUp()
    {
        if (playerGold < playerHealthRecorveryCost)
            return false;
        PayGold(playerHealthRecorveryCost);
        playerHealthRecorvery += 0.1f;
        playerHealthRecorveryCost += 5;
        return true;
    }

    //PayGold�� playerGold�� �����ϴ� �Լ��̴�.
    private void PayGold(int gold)
    {
        playerGold -= gold;
    }

    //Start �Լ��� Bullet�� �߻��ϴ� �ڷ�ƾ�� ü���� �ʱ�ȭ ���ش�.
    void Start()
    {
        StartCoroutine(Shoot());
        playerHealth = maxPlayerHealth;
        currentTime = standardTime;
        playerSprite = transform.GetChild(0);
    }

    //FixedUpdate�� ü������� Ÿ���� �ֽ�ȭ ���ش�.
    void FixedUpdate()
    {
        if (isPlayerDie)
        {
            GetTarget(true);
            return;
        }
        Recovery();
        ElapseTime();
        nearestTarget = GetTarget(true);
        playerSprite.Rotate( Time.deltaTime * rotateSpeed * Vector3.forward );
    }

    private void ElapseTime()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
            ResetAction();
    }

    private void ResetAction()
    {
        int action = UnityEngine.Random.Range(0, 10);

        if (action < 8)
            targetSpeed = -50;
        else if (action == 8)
            targetSpeed = UnityEngine.Random.Range(250f, 500f);
        else
            targetSpeed = UnityEngine.Random.Range(-500f, -250f);

        StartCoroutine("RotateSpeedLerp");
        currentTime = standardTime;
    }

    IEnumerator RotateSpeedLerp()
    {
        float elapseTime = 0;
        while (elapseTime <= standardTime)
        {
            elapseTime += Time.deltaTime;
            rotateSpeed = Mathf.Lerp(rotateSpeed, targetSpeed, Time.deltaTime);
            yield return null;
        }
        rotateSpeed = targetSpeed;
    }

    //Shoot �ڷ�ƾ�� �Ѿ��� ���ݼӵ����� �߻����ִ� �ڷ�ƾ�̴�.
    IEnumerator Shoot()
    {
        while (true)
        {
            if (isPlayerDie)
                break;

            if (nearestTarget == null)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            Bullet bullet = GameManger.instance.poolManager.GetPool("Bullet").GetComponent<Bullet>();
            SoundManager.instance.PlaySFX("PlayerBulletSound");
            //bullet.bulletSpeed = playerAttackSpeed * 3;
            bullet.transform.position = transform.position;
            bullet.Init(nearestTarget);

            yield return new WaitForSeconds((float)1 / playerAttackSpeed);
        }
    }

    //GetDamage�� PlayerHealth�� ���ҽ�Ű�� �Լ��� �������� �Դ� �Լ��̴�.
    public void GetDamage(float damage)
    {
        if (isPlayerDie)
            return;

        if(playerShield > 0 )
        {
            GameManger.instance.cameraResolution.Shake();
            GameManger.instance.UiManager.StartDamageEffect();
            SoundManager.instance.PlaySFX("PlayerHitSound");
            playerShield -= 1;
            if (playerShield == 0)
            {
                gameObject.GetComponent<SkillManager>().SetShieldActive(false);
                return;
            }
            gameObject.GetComponent<SkillManager>().SetShieldText(playerShield);
            return;
        }

        if (playerHealth - damage <= 0)
        {
            playerHealth = 0;
            PlayerDie();
            return;
        }
        else
            playerHealth -= damage;

        GameManger.instance.cameraResolution.Shake();
        GameManger.instance.UiManager.StartDamageEffect();
        SoundManager.instance.PlaySFX("PlayerHitSound");
    }

    //PlayerDie�� �÷��̾� ��� ������ �ۼ��ϴ� �Լ��̴�.
    private void PlayerDie()
    {
        isPlayerDie = true;
        playerSprite.gameObject.SetActive(false);
        PlayerDieParticle.SetActive(true);
        StartCoroutine(GameManger.instance.GameOver());
    }

    //Recovery�� ü���� ����ϴ� �Լ��̴�.
    private void Recovery()
    {
        if (playerHealth + playerHealthRecorvery * Time.deltaTime >= maxPlayerHealth)
            playerHealth = maxPlayerHealth;
        else
            playerHealth += playerHealthRecorvery * Time.deltaTime;
    }

    //GetTarget�� ���� ����� Ÿ���� Ÿ���������� �޾ƿ��� �Ű������� �Ǵ��Ͽ� ���� �����ų� ���� �� Ÿ���� �����´�.
    //�׸��� ����� ������ Ÿ���� Transform�� ��ȯ�Ѵ�.
    public Transform GetTarget(bool nearest)
    {
        if (isPlayerDie)
            attackRange = 100;

        targets = Physics2D.OverlapCircleAll(new Vector2(0, 1), attackRange, LayerMask.GetMask("Enemy"));
        Transform resultTransform = null;
        float distance = nearest ? attackRange : 0;

        foreach (Collider2D target in targets)
        {
            if (isPlayerDie)
            {
                Enemy enemy = target.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.GameOverDie();
                if(enemy == null)
                {
                    target.GetComponent<Boss>().GameOverDie();
                }
                continue;
            }
            Vector3 playerPosition = transform.position;
            Vector3 targetPosition = target.transform.position;
            float curDistance = Vector3.Distance(playerPosition, targetPosition);

            bool checkDistance = nearest ? curDistance < distance : distance < curDistance;

            if (checkDistance)
            {
                if (curDistance > attackRange)
                    continue;
                distance = curDistance;
                resultTransform = target.transform;
            }
        }

        return resultTransform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayerDie)
            return;
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            float damage = enemy.GetEnemyDamage();

            GetDamage(damage);
            enemy.EnemyDie(true) ;
        }
        if (collision.gameObject.tag == "Boss")
        {
            PlayerDie();
        }
        // ���� D ������ ������ ����
        if (collision.gameObject.tag == "BossLaser")
        {
            GetDamage(0.5f);
        }
    }
}