using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private string enemyType;
    [SerializeField]
    private float enemySpeed;
    [SerializeField]
    private float enemyDamage;
    [SerializeField]
    private float maxEnemyHealth;
    [SerializeField]
    private float enemyHealth;
    [SerializeField]
    private float enemyGold;

    public PlayerController playerController;
    public WaveManager waveManager;
    bool isEnemyLive;

    Vector2 playerPos = new Vector2(0.0f, 1.0f);
    
    private void Start()
    {
        EnemyLookPlayer();
    }
    public void SetEnemyState(float enemyHealth, float enemyDamage)
    {
        this.enemyHealth = enemyHealth;
        this.enemyDamage = enemyDamage;

    }
    public void EnemyLookPlayer()
    {
        float targetAngle = Vector2.Angle(transform.up, playerPos - (Vector2)transform.position);
        targetAngle = transform.position.x >= 0 ? targetAngle : -targetAngle;
        transform.Rotate(new Vector3(0, 0, targetAngle));
    }
    private void MoveEnemy()
    {
        transform.position += transform.up * Time.deltaTime * enemySpeed;
    }
    void FixedUpdate()
    {
        MoveEnemy();
    }

    private void OnEnable()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
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
        else if (other.gameObject.tag == "Protocol")
        {
            enemySpeed *= other.gameObject.GetComponent<PartsContorller>().partsValue;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Protocol")
        {
            enemySpeed *= 1 / other.gameObject.GetComponent<PartsContorller>().partsValue;
        }
    }

    public void GetDamage(float damage)
    {
        if (enemyHealth - damage <= 0)
        {
            EnemyDie();
        }
        else
            enemyHealth -= damage;
    }

    public float GetEnemyDamage()
    {
        return enemyDamage;
    }
    public void EnemyDie()
    {
        transform.localEulerAngles = new Vector3(0, 0, 0);
        enemyHealth = 0;
        this.gameObject.SetActive(false);

        GameManger.instance.player.playerGold += 3;
        if (enemyType == "EnemyA")
            waveManager.waves[waveManager.currentWave].enemyACount--;
        else if(enemyType == "EnemyB")
            waveManager.waves[waveManager.currentWave].enemyBCount--;

    }
}
