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

    Vector2 playerPos = new Vector2(0.0f, 1.3f);
    
    private void Start()
    {
        EnemyLookPlayer();
        SetEnemyState();
    }
    private void SetEnemyState()
    {
        enemyHealth = maxEnemyHealth;

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
        SetEnemyState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            float damage = playerController.GetPlayerDamage();
            GetDamage(damage);
            other.gameObject.SetActive(false);
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

        if (enemyType == "EnemyA")
            waveManager.waves[waveManager.currentWave].enemyACount--;
        else
            waveManager.waves[waveManager.currentWave].enemyBCount--;

        if(waveManager.waves[waveManager.currentWave].enemyACount == 0 && waveManager.waves[waveManager.currentWave].enemyBCount == 0)
            waveManager.isWaveEnd = true;
    }
}
