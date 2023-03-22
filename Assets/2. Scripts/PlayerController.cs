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

    //�÷��̾� ���� ���
    public int playerGold { get; set; } = 0;

    //Ÿ�� ���� ����
    private Collider2D[] targets;
    public float attackRange { get; private set; } = 4;
    public Transform nearestTarget { get; private set; }

    //Player ���� LevelUp ���� �Լ���
    public void PlayerDamageLevelUp()
    {
        if (playerGold < playerDamageCost)
            return;
        PayGold(playerDamageCost);
        playerDamage += 0.1f;
        playerDamageCost += 2;
    }
    public void PlayerAttackSpeedLevelUp()
    {
        if (playerGold < playerAtkSpeedCost)
            return;
        PayGold(playerAtkSpeedCost);
        playerAttackSpeed += 0.1f;
        playerAtkSpeedCost += 2;
    }
    public void PlayerMaxHealthLevelUp()
    {
        if (playerGold < playerMaxHealthCost)
            return;
        PayGold(playerMaxHealthCost);
        maxPlayerHealth += 5f;
        playerHealth += 5f;
        playerMaxHealthCost += 5;
    }
    public void PlayerHealthRecorveryLevelUp()
    {
        if (playerGold < playerHealthRecorveryCost)
            return;
        PayGold(playerHealthRecorveryCost);
        playerHealthRecorvery += 0.1f;
        playerHealthRecorveryCost += 5; 
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
    }

    //FixedUpdate�� ü������� Ÿ���� �ֽ�ȭ ���ش�.
    void FixedUpdate()
    {
        Recovery();
        nearestTarget = GetTarget(true);
    }

    //Shoot �ڷ�ƾ�� �Ѿ��� ���ݼӵ����� �߻����ִ� �ڷ�ƾ�̴�.
    IEnumerator Shoot()
    {
        while (true)
        {
            if (nearestTarget == null)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            Bullet bullet = GameManger.instance.poolManager.GetPool("Bullet").GetComponent<Bullet>();
            bullet.bulletSpeed = playerAttackSpeed * 3;
            bullet.transform.position = transform.position;
            bullet.Init(nearestTarget);

            yield return new WaitForSeconds((float)1 / playerAttackSpeed);
        }
    }

    //GetDamage�� PlayerHealth�� ���ҽ�Ű�� �Լ��� �������� �Դ� �Լ��̴�.
    public void GetDamage(float damage)
    {
        GameManger.instance.cameraResolution.Shake();
        GameManger.instance.UiManager.StartDamageEffect();

        if(playerShield > 0 )
        {
            playerShield -= 1;
            return;
        }

        if (playerHealth - damage <= 0)
        {
            playerHealth = 0;
            PlayerDie();
        }
        else
            playerHealth -= damage;
    }

    //PlayerDie�� �÷��̾� ��� ������ �ۼ��ϴ� �Լ��̴�.
    private void PlayerDie()
    {
        //�÷��̾� ��� ����
        GameManger.instance.cameraResolution.Shake();
            this.gameObject.SetActive(false);
            GameManger.instance.GameOver();
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
        targets = Physics2D.OverlapCircleAll(new Vector2(0, 1), attackRange, LayerMask.GetMask("Enemy"));
        Transform resultTransform = null;
        float distance = nearest ? attackRange : 0;

        foreach (Collider2D target in targets)
        {
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
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            float damage = enemy.GetEnemyDamage();

            GetDamage(damage);
            enemy.EnemyDie(true) ;
        }
        if (collision.gameObject.tag == "Boss")
        {
            Boss bossB = collision.GetComponent<Boss>();
            bossB.BossDie();
           
            PlayerDie();
        }
        if (collision.gameObject.tag == "BossLaser")
        {
            GetDamage(10f);
        }
    }
}