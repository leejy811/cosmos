using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerDamage;
    [SerializeField]
    public int playerDamageCost;
    [SerializeField]
    private float playerAttackSpeed;
    [SerializeField]
    public int playerAtkSpeedCost;
    [SerializeField]
    private float maxPlayerHealth;
    [SerializeField]
    public int playerMaxHealthCost;
    [SerializeField]
    private float playerHealth;
    [SerializeField]
    private float playerHealthRecorvery;
    [SerializeField]
    public int playerHealthRecorveryCost;

    public int playerGold;

    [SerializeField]
    private LayerMask targetLayer;
    private Collider2D[] targets;
    public float attackRange;
    public Transform nearestTarget;
    
    public void PlayerDamageLevelUp()
    {
        if (playerGold < playerDamageCost)
            return;
        PayGold(playerDamageCost);
        playerDamage += 3f;
        playerDamageCost += 4;
    }

    public void PlayerAttackSpeedLevelUp()
    {
        if (playerGold < playerAtkSpeedCost)
            return;
        PayGold(playerAtkSpeedCost);
        playerAttackSpeed += 0.2f;
        playerAtkSpeedCost += 8;

    }
    public void PlayerMaxHealthLevelUp()
    {
        if (playerGold < playerMaxHealthCost)
            return;
        PayGold(playerMaxHealthCost);
        playerHealth += 5f;
        maxPlayerHealth += 5f;
        playerMaxHealthCost += 10;
    }
    public void PlayerHealthRecorveryLevelUp()
    {
        if (playerGold < playerHealthRecorveryCost)
            return;
        PayGold(playerHealthRecorveryCost);
        playerHealthRecorvery += 0.1f;
        playerHealthRecorveryCost += 8; 
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

            GameObject bullet = GameManger.instance.poolManager.GetPool("Bullet");
            bullet.transform.position = transform.position;
            bullet.GetComponent<Bullet>().Init(nearestTarget);

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
        targets = Physics2D.OverlapCircleAll((Vector2)transform.position, attackRange, targetLayer.value);
        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform resultTransform = null;
        float distance = 100;

        foreach (Collider2D target in targets)
        {
            Vector3 playerPosition = transform.position;
            Vector3 targetPosition = target.transform.position;
            float curDistance = Vector3.Distance(playerPosition, targetPosition);

            if (curDistance < distance)
            {
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

    public float GetPlayerDamage()
    {
        return playerDamage;
    }
    public float GetPlayerAtkSpeed()
    {
        return playerAttackSpeed;
    }
    public float GetPlayerHealth()
    {
        return maxPlayerHealth;
    }
    public float GetPlayerHealthRecorvery()
    {
        return playerHealthRecorvery;
    }
    public float PlayerMaxHealthPerCurHealth()
    {
        return (float)playerHealth / maxPlayerHealth;
    }
}