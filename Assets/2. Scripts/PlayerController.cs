using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerDamage { get; private set; } = 1;
    public int playerDamageCost { get; private set; } = 3;
    public float playerAttackSpeed { get; private set; } = 2;
    public int playerAtkSpeedCost { get; private set; } = 3;
    public float maxPlayerHealth { get; private set; } = 5;
    public int playerMaxHealthCost { get; private set; } = 5;
    public float playerHealth { get; private set; } = 5;
    public float playerHealthRecorvery { get; private set; } = 0;
    public int playerHealthRecorveryCost { get; private set; } = 5;

    public int playerGold { get; set; } = 0;

    private Collider2D[] targets;
    public float attackRange { get; private set; } = 4;
    public Transform nearestTarget { get; private set; }
    
    public void PlayerDamageLevelUp()
    {
        if (playerGold < playerDamageCost)
            return;
        PayGold(playerDamageCost);
        playerDamage += 0.5f;
        playerDamageCost += 2;
    }

    public void PlayerAttackSpeedLevelUp()
    {
        if (playerGold < playerAtkSpeedCost)
            return;
        PayGold(playerAtkSpeedCost);
        playerAttackSpeed += 0.05f;
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

    private void PayGold(int gold)
    {
        playerGold -= gold;
    }

    void Start()
    {
        StartCoroutine(Shoot());
        playerHealth = maxPlayerHealth;
    }

    void FixedUpdate()
    {
        Recovery();
        CheckEnemy();
    }

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

    public void GetDamage(float damage)
    {
        GameManger.instance.cameraResolution.Shake();
        if (playerHealth - damage <= 0)
        {
            playerHealth = 0;
            PlayerDie();
        }
        else
            playerHealth -= damage;
    }
    private void PlayerDie()
    {
        //플레이어 사망 로직
        GameManger.instance.cameraResolution.Shake();
            this.gameObject.SetActive(false);
            GameManger.instance.GameOver();
    }

    private void Recovery()
    {
        if (playerHealth + playerHealthRecorvery * Time.deltaTime >= maxPlayerHealth)
            playerHealth = maxPlayerHealth;
        else
            playerHealth += playerHealthRecorvery * Time.deltaTime;
    }

    private void CheckEnemy()
    {
        nearestTarget = GetTarget(true);
    }

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
            enemy.EnemyDie();
        }
        if (collision.gameObject.tag == "Boss")
        {
            Boss bossB = collision.GetComponent<Boss>();
            bossB.BossDie();
           
            PlayerDie();
        }
    }
}