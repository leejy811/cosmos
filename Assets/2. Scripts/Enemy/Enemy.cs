using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DamageNumbersPro;
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

    private bool deathDelay;
    [SerializeField]
    private Transform[] transforms;

    public PlayerController playerController;
    public WaveManager waveManager;
    public bool isEnemyLive;
    [SerializeField]
    private DamageNumber numberPrefab;

    Vector2 playerPos = new Vector2(0.0f, 1.0f);
    public Vector2 targetPos;
    public bool moveLerp;
    Color targetColor = new Color(255, 0, 0, 255);
    Color enemyColor;
    [SerializeField]
    private GameObject enemyDieEffect;

    private void Start()
    {
        enemyColor = this.gameObject.transform.GetComponent<Renderer>().material.color;

    }
    public void SetEnemyState(float enemyHealth, float enemyDamage, int enemyPrice, int enemyJem)
    {
        this.enemyHealth = enemyHealth;
        this.enemyDamage = enemyDamage;
        this.enemyPrice = enemyPrice;
        this.enemyJem = enemyJem;
        isEnemyLive = true;
        deathDelay = true;
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
            this.gameObject.transform.DOLocalMove(new Vector3(targetPos.x, targetPos.y, 0), 0.5f).SetEase(Ease.OutBounce);
            if (Mathf.Abs(targetPos.x - transform.position.x) <= 0.2f && Mathf.Abs(targetPos.y - transform.position.y) <= 0.2f)
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
        if(deathDelay)
        {
            Invoke("DeathDelayEnd", 1f);
        }
        MoveEnemy();
    }
    private void DeathDelayEnd()
    {
        deathDelay = false;
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
            if (enemyType == "EnemyC")
                return;
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
            if (enemyType == "EnemyC")
                return;
            enemySpeed *= 1 / other.gameObject.GetComponent<PartsContorller>().partsValue;
        }
    }

    public void GetDamage(float damage)
    {
        if (deathDelay)
            return;
        //if (isEnemyGetDamage)
        //    return;
        DamageEffect(damage);

        if (enemyHealth - damage <= 0)
            EnemyDie(false) ;
        else
            enemyHealth -= damage;
    }
    private void DamageEffect(float damage)
    {
        this.gameObject.transform.GetComponent<Renderer>().material.DOColor(targetColor, 0.1f);
        this.gameObject.transform.GetComponent<Renderer>().material.DOColor(enemyColor, 0.1f);
        //GameObject hudText = GameManger.instance.poolManager.GetPool("DamageText");
        //hudText.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, 0);
        //hudText.GetComponent<DamageText>().damage = (int)((Mathf.Round(GameManger.instance.player.playerDamage * 10) * 0.1f) * 100);
        DamageNumber damageNumber = numberPrefab.Spawn(new Vector3(transform.position.x, transform.position.y + 0.5f, 0), (int)((Mathf.Round(GameManger.instance.player.playerDamage * 10) * 0.1f) * 100));
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

    public void GameOverDie()
    {
        enemyDieEffect.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Invoke("EnemyDieEffect", 1.5f);
    }

    // 게임 EndDIe 함수 하나 더 만들기
    public void EnemyDie(bool EnterPlayer)
    {
        SoundManager.instance.PlaySFX("MonsterKillSound");
        if (enemyType == "EnemyA")
        {
            if(waveManager.waves[waveManager.currentWave].enemyACount > 0 && waveManager.currentWave != 40)
                waveManager.waves[waveManager.currentWave].enemyACount--;
            waveManager.totalKillEnemyACount++;
        }
        else if(enemyType == "EnemyB")
        {
            if (waveManager.waves[waveManager.currentWave].enemyBCount > 0 && waveManager.currentWave != 40)
                waveManager.waves[waveManager.currentWave].enemyBCount--;
            waveManager.totalKillEnemyBCount++;

        }
        else if(enemyType == "EnemyC")
        {
            if (waveManager.waves[waveManager.currentWave].enemyCCount > 0 && waveManager.currentWave != 40)
                waveManager.waves[waveManager.currentWave].enemyCCount--;
            waveManager.totalKillEnemyCCount++;

        }
        else if(enemyType == "EnemyD")
        {
            if (waveManager.waves[waveManager.currentWave].enemyDCount > 0 && waveManager.currentWave != 40)
                waveManager.waves[waveManager.currentWave].enemyDCount--;
            waveManager.totalKillEnemyDCount++;
            for (int i=0; i<2; i++)
            {
                GameObject enemy = GameManger.instance.poolManager.GetPool("EnemyA");
                enemy.GetComponent<Enemy>().SetEnemyState(waveManager.waves[waveManager.currentWave].enemyAHp, waveManager.waves[waveManager.currentWave].enemyADamage, waveManager.waves[waveManager.currentWave].enemyAPrice, waveManager.waves[waveManager.currentWave].enemyAJem);
                enemy.transform.position = transform.position;
                enemy.GetComponent<Enemy>().moveLerp = true;
                enemy.GetComponent<Enemy>().targetPos = transforms[i].position;
                enemy.GetComponent<Enemy>().EnemyLookPlayer();
                enemy.GetComponent<Enemy>().playerController = GameManger.instance.player;
                enemy.GetComponent<Enemy>().waveManager = this.waveManager;
            }
        }
        // 플레이어 닿았을 때 골드나 잼 안올라가게 하기
        if(!EnterPlayer)
        {
            GameManger.instance.player.playerJem += enemyJem;
            GameManger.instance.player.playerGold += enemyPrice;

            int ranTicket = Random.Range(0, 100);
            if(ranTicket == 1)
            {
                GameManger.instance.playTicket += 1;
                GameObject ticket = GameManger.instance.poolManager.GetPool("Ticket");
                ticket.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, 0);
                ticket.GetComponent<FloatingTicket>().FloatingEffet();
            }
        }
        if(this.moveLerp)
            moveLerp = false;

        isEnemyLive = false;
        enemyDieEffect.SetActive(true);
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Invoke("EnemyDieEffect", 1.5f);
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
        this.gameObject.transform.GetComponent<Renderer>().material.color = enemyColor;
    }
}
