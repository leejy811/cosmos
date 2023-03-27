using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// 적 생성하고 무적 시간 두기 (0.2초 정도)
//
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private string enemyType;
    [SerializeField]
    public float enemySpeed;
    [SerializeField] 
    private float knockBackSpeed;
    [SerializeField] 
    private float knockBackDistance;

    private float enemyDamage;
    private float enemyHealth;
    private int enemyPrice;
    private int enemyJem;

    [SerializeField]
    private Transform[] transforms;

    public PlayerController playerController;
    public WaveManager waveManager;
    public bool isEnemyLive;


    Vector2 playerPos = new Vector2(0.0f, 1.0f);
    public Vector2 targetPos;
    public bool moveLerp;


    [SerializeField]
    private GameObject enemyDieEffect;

    private void Start()
    {

    }
    public void SetEnemyState(float enemyHealth, float enemyDamage, int enemyPrice, int enemyJem)
    {
        this.enemyHealth = enemyHealth;
        this.enemyDamage = enemyDamage;
        this.enemyPrice = enemyPrice;
        this.enemyJem = enemyJem;
        isEnemyLive = true;
    }
    public void EnemyLookPlayer()
    {
        if(moveLerp)
        {
            float targetAngle = Vector2.Angle(transform.up, playerPos - targetPos);
            targetAngle = targetPos.x >= 0 ? targetAngle : -targetAngle;
            transform.Rotate(new Vector3(0, 0, targetAngle));
        }
        else
        {
            if(isEnemyLive)
            {
                float targetAngle = Vector2.Angle(transform.up, playerPos - (Vector2)transform.position);
                targetAngle = transform.position.x >= 0 ? targetAngle : -targetAngle;
                transform.Rotate(new Vector3(0, 0, targetAngle));
            }
        }
    }
    private void MoveEnemy()
    {
        if (!isEnemyLive)
            return;
        if(moveLerp)
        {
            //Debug.Log("DOTween Move Start");
            this.gameObject.transform.DOLocalMove(new Vector3(targetPos.x, targetPos.y, 0), 0.4f).SetEase(Ease.OutSine);
            if (targetPos == (Vector2)transform.position)
            {
                    moveLerp = false;
            }
        }
        else
        {
            transform.position += transform.up * Time.deltaTime * enemySpeed;
        }
    }
    void FixedUpdate()
    {
        MoveEnemy();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEnemyLive)
            return;

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
        else if (other.gameObject.tag == "Barrier")
        {
            enemySpeed *= other.gameObject.GetComponent<PartsContorller>().partsValue;
        }
        else if (other.gameObject.tag == "Laser")
        {
            float damage = other.gameObject.GetComponentInParent<PartsContorller>().GetPartsDamage();
            GetDamage(damage);
        }
        else if (other.gameObject.tag == "Emp")
        {
            float damage = other.gameObject.GetComponentInParent<PartsContorller>().GetPartsDamage();
            GetDamage(damage);

            if (other.gameObject.GetComponentInParent<PartsContorller>().partsValue == 0)
                return;

            GetEmpAttack();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Barrier")
        {
            enemySpeed *= 1 / other.gameObject.GetComponent<PartsContorller>().partsValue;
        }
    }

    public void GetDamage(float damage)
    {
        //if (isEnemyGetDamage)
        //    return;
        DamageEffect(damage);

        if (enemyHealth - damage <= 0)
            EnemyDie(false) ;
        else
            enemyHealth -= damage;
    }
    public void DamageEffect(float damage)
    {
        GameObject hudText = GameManger.instance.poolManager.GetPool("DamageText");
        hudText.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, 0);
        hudText.GetComponent<DamageText>().damage = damage;

    }
    public float GetEnemyDamage()
    {
        return enemyDamage;
    }

    private void GetEmpAttack()
    {
        if (gameObject.activeSelf == false)
            return;
        StartCoroutine("KnockBackEffect");
    }

    IEnumerator KnockBackEffect()
    {
        float speed = knockBackSpeed;
        while (speed > 0)
        {
            transform.position -= transform.up * Time.deltaTime * speed;
            speed -= Time.deltaTime * speed / knockBackDistance;
            yield return null;
        }
    }


    public void EnemyDie(bool EnterPlayer)
    {
        SoundManager.instance.PlaySFX("MonsterKillSound");
        if (enemyType == "EnemyA")
        {
            if(waveManager.waves[waveManager.currentWave].enemyACount > 0)
                waveManager.waves[waveManager.currentWave].enemyACount--;
        }
        else if(enemyType == "EnemyB")
        {
            if (waveManager.waves[waveManager.currentWave].enemyBCount > 0)
                waveManager.waves[waveManager.currentWave].enemyBCount--;
        }
        else if(enemyType == "EnemyC")
        {
            if (waveManager.waves[waveManager.currentWave].enemyCCount > 0)
                waveManager.waves[waveManager.currentWave].enemyCCount--;
        }
        else if(enemyType == "EnemyD")
        {
            if (waveManager.waves[waveManager.currentWave].enemyDCount > 0)
                waveManager.waves[waveManager.currentWave].enemyDCount--;
            for (int i=0; i<2; i++)
            {
                GameObject enemy = GameManger.instance.poolManager.GetPool("EnemyA");
                enemy.GetComponent<Enemy>().SetEnemyState(waveManager.waves[waveManager.currentWave].enemyAHp, waveManager.waves[waveManager.currentWave].enemyADamage, waveManager.waves[waveManager.currentWave].enemyAPrice, waveManager.waves[waveManager.currentWave].enemyAJem);
                enemy.transform.position = transform.position;
                enemy.GetComponent<Enemy>().moveLerp = true;
                enemy.GetComponent<Enemy>().EnemyLookPlayer();
                enemy.GetComponent<Enemy>().targetPos = transforms[i].position ;
                enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
                enemy.GetComponent<Enemy>().waveManager = this.waveManager;
            }
        }
        if(!EnterPlayer)
        {
            LocalDatabaseManager.instance.JemCount += enemyJem;
            GameManger.instance.player.playerGold += enemyPrice;
        }
        if(this.moveLerp)
            moveLerp = false;

        isEnemyLive = false;
        enemyDieEffect.SetActive(true);
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Invoke("EnemyDieEffect", 0.5f);
    }
    private void EnemyDieEffect()
    {
        enemyHealth = 0;
        enemyDieEffect.SetActive(false);
        gameObject.SetActive(false);
        gameObject.tag = "Enemy";
        gameObject.layer = 6;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}
