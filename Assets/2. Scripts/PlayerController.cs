using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerDamage;
    [SerializeField]
    private float playerAttackSpeed;
    [SerializeField]
    private float maxPlayerHealth;
    private float playerHealth;
    private float playerHealthRecorvery;

    [SerializeField]
    private float attackRange;
    public LayerMask targetLayer;
    private RaycastHit2D[] targets;
    private Transform nearestTarget;
    
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
             
            Vector3 directionPosition = nearestTarget.position - transform.position;

            GameObject bullet = GameManger.instance.poolManager.GetPool("Bullet");
            bullet.transform.position = transform.position;
            bullet.GetComponent<Bullet>().Init(directionPosition.normalized);

            yield return new WaitForSeconds(playerAttackSpeed);
        }
    }

    public void GetDamage(float damage)
    {
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
        targets = Physics2D.CircleCastAll(transform.position, attackRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform resultTransform = null;
        float distance = 100;

        foreach (RaycastHit2D target in targets)
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
            GetDamage(10f);
            collision.gameObject.SetActive(false);
        }
    }
}