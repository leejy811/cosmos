using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private string enemyType;
    [SerializeField]
    public float enemySpeed;
    [SerializeField]
    private float enemyDamage;
    [SerializeField]
    private float enemyHealth;
    [SerializeField]
    public int enemyPrice;
    [SerializeField]
    private Transform[] transforms;

    public PlayerController playerController;
    public WaveManager waveManager;
    bool isEnemyLive;


    Vector2 playerPos = new Vector2(0.0f, 1.0f);
    
    private void Start()
    {
        EnemyLookPlayer();
    }
    public void SetEnemyState(float enemyHealth, float enemyDamage, int enemyPrice)
    {
        this.enemyHealth = enemyHealth;
        this.enemyDamage = enemyDamage;
        this.enemyPrice = enemyPrice;

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
        else if (other.gameObject.tag == "Laser")
        {
            float damage = other.gameObject.GetComponentInParent<PartsContorller>().GetPartsDamage();
            GetDamage(damage);
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
        if (enemyType == "EnemyA")
        {
            if(waveManager.waves[waveManager.currentWave].enemyACount > 0)
                waveManager.waves[waveManager.currentWave].enemyACount--;
            GameManger.instance.player.playerGold +=  enemyPrice;
        }
        else if(enemyType == "EnemyB")
        {
            if (waveManager.waves[waveManager.currentWave].enemyBCount > 0)
                waveManager.waves[waveManager.currentWave].enemyBCount--;
            GameManger.instance.player.playerGold += enemyPrice;
            //LocalDatabaseManager.instance.JemCount += enemyPrice;
        }
        else if(enemyType == "EnemyD")
        {
            if (waveManager.waves[waveManager.currentWave].enemyDCount > 0)
                waveManager.waves[waveManager.currentWave].enemyDCount--;
            GameManger.instance.player.playerGold += enemyPrice;

            for (int i=0; i<2; i++)
            {
                GameObject enemy = GameManger.instance.poolManager.GetPool("EnemyA");
                enemy.GetComponent<Enemy>().SetEnemyState(waveManager.waves[waveManager.currentWave].enemyAHp, waveManager.waves[waveManager.currentWave].enemyADamage, waveManager.waves[waveManager.currentWave].enemyAPrice);
                enemy.transform.position = transforms[i].position;
                enemy.GetComponent<Enemy>().EnemyLookPlayer();
                enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
                enemy.GetComponent<Enemy>().waveManager = this.waveManager;
            }
        }

        transform.localEulerAngles = new Vector3(0, 0, 0);
        enemyHealth = 0;
        this.gameObject.SetActive(false);

    }
}
