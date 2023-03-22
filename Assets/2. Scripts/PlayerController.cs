using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

/* PlayerController Class
 * 이 클래스는 Player의 단발성 변수를 관리하고 기본 공격을 구현하는 클래스다.
 */
public class PlayerController : MonoBehaviour
{
    
    //Player 스탯
    public float playerDamage { get; private set; } = 1;
    public float playerAttackSpeed { get; private set; } = 2;
    public float maxPlayerHealth { get; private set; } = 5;
    public float playerHealth { get; private set; } = 5;
    public int playerShield { get; set; } = 0;
    //public float playerHealth { get; private set; } = 10000;

    public float playerHealthRecorvery { get; private set; } = 0;

    //Player 스탯 업그레이드 Cost
    public int playerDamageCost { get; private set; } = 3;
    public int playerAtkSpeedCost { get; private set; } = 3;
    public int playerMaxHealthCost { get; private set; } = 5;
    public int playerHealthRecorveryCost { get; private set; } = 5;

    //플레이어 현재 골드
    public int playerGold { get; set; } = 0;

    //타겟 관련 변수
    private Collider2D[] targets;
    public float attackRange { get; private set; } = 4;
    public Transform nearestTarget { get; private set; }

    //Player 스탯 LevelUp 관련 함수들
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

    //PayGold는 playerGold를 지불하는 함수이다.
    private void PayGold(int gold)
    {
        playerGold -= gold;
    }

    //Start 함수는 Bullet을 발사하는 코루틴과 체력을 초기화 해준다.
    void Start()
    {
        StartCoroutine(Shoot());
        playerHealth = maxPlayerHealth;
    }

    //FixedUpdate는 체력재생과 타겟을 최신화 해준다.
    void FixedUpdate()
    {
        Recovery();
        nearestTarget = GetTarget(true);
    }

    //Shoot 코루틴은 총알을 공격속도마다 발사해주는 코루틴이다.
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

    //GetDamage는 PlayerHealth를 감소시키는 함수로 데미지를 입는 함수이다.
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

    //PlayerDie는 플레이어 사망 로직을 작성하는 함수이다.
    private void PlayerDie()
    {
        //플레이어 사망 로직
        GameManger.instance.cameraResolution.Shake();
            this.gameObject.SetActive(false);
            GameManger.instance.GameOver();
    }

    //Recovery는 체력을 재생하는 함수이다.
    private void Recovery()
    {
        if (playerHealth + playerHealthRecorvery * Time.deltaTime >= maxPlayerHealth)
            playerHealth = maxPlayerHealth;
        else
            playerHealth += playerHealthRecorvery * Time.deltaTime;
    }

    //GetTarget은 가장 가까운 타겟을 타겟팅할지를 받아오는 매개변수로 판단하여 가장 가깝거나 가장 먼 타겟을 가져온다.
    //그리고 결과로 가져온 타겟의 Transform을 반환한다.
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