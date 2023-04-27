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
    private DamageNumber[] floatingTextPrefab;


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
            // Calc angle between enemy and player
            float targetAngle = Vector2.Angle(transform.up, playerPos - targetPos);
            // Convert targetAngle to Signed value
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
            GetDamage(damage, false);
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
            GetDamage(damage, true);
        }
        else if (other.gameObject.tag == "Emp")
        {

            float damage = other.gameObject.GetComponentInParent<PartsContorller>().GetPartsDamage();
            GetDamage(damage, true);

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

    public void GetDamage(float damage, bool isPartsDamage)
    {
        if (deathDelay)
            return;
        //if (isEnemyGetDamage)
        //    return;
        DamageEffect(damage, isPartsDamage);

        if (enemyHealth - damage <= 0)
            EnemyDie(false) ;
        else
            enemyHealth -= damage;
    }
    private void DamageEffect(float damage, bool isPartsDamage)
    {
        this.gameObject.transform.GetComponent<Renderer>().material.DOColor(targetColor, 0.2f);
        this.gameObject.transform.GetComponent<Renderer>().material.DOColor(enemyColor, 0.2f);
        DamageNumber damageNumber;
        if (isPartsDamage)
        {
            damageNumber = floatingTextPrefab[0].Spawn(new Vector3(transform.position.x, transform.position.y + 0.5f, 0), (int)((Mathf.Round(damage * 1000) * 0.001f) * 1000));
        }
        else
             damageNumber = floatingTextPrefab[1].Spawn(new Vector3(transform.position.x, transform.position.y + 0.5f, 0), (int)((Mathf.Round( damage * 10) * 0.1f) * 1000));
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
            transform.position -= Time.deltaTime * speed *transform.up;
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

    public void EnemyDie(bool EnterPlayer)
    {
        SoundManager.instance.PlaySFX("MonsterKillSound");
        if (enemyType == "EnemyA")
        {
            if(waveManager.waveACount > 0 && waveManager.currentWave != 40)
                waveManager.waveACount--;
            waveManager.totalKillEnemyACount++;
        }
        else if(enemyType == "EnemyB")
        {
            if (waveManager.waveBCount > 0 && waveManager.currentWave != 40)
                waveManager.waveBCount--;
            waveManager.totalKillEnemyBCount++;

        }
        else if(enemyType == "EnemyC")
        {
            if (waveManager.waveCCount > 0 && waveManager.currentWave != 40)
                waveManager.waveCCount--;
            waveManager.totalKillEnemyCCount++;

        }
        else if(enemyType == "EnemyD")
        {
            if (waveManager.waveDCount > 0 && waveManager.currentWave != 40)
                waveManager.waveDCount--;
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
            DamageNumber floatingText;
            string jemtext = "+" + enemyJem.ToString() + "Jem";
            string pricetext = "+" + enemyPrice.ToString() + "Gold";

            if (enemyJem != 0)
                floatingText = floatingTextPrefab[2].Spawn(new Vector3(transform.position.x + 0.5f, transform.position.y + 2.1f, 0), jemtext);
            if (enemyPrice != 0)
                floatingText = floatingTextPrefab[3].Spawn(new Vector3(transform.position.x, transform.position.y + 1.5f, 0), pricetext);

            GameManger.instance.player.playerJem += enemyJem;
            GameManger.instance.player.playerGold += enemyPrice;

            int ranTicket = Random.Range(0, 2);
            if(ranTicket == 1)
            {
                GameManger.instance.playTicket += 1;
                floatingText = floatingTextPrefab[4].Spawn(new Vector3(transform.position.x, transform.position.y, 0), "+1 Ticket!");
                //GameObject ticket = GameManger.instance.poolManager.GetPool("Ticket");
                //ticket.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, 0);
                //ticket.GetComponent<FloatingTicket>().FloatingEffet();
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
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.SetActive(false);
        gameObject.tag = "Enemy";
        gameObject.layer = 6;
        transform.localEulerAngles = new Vector3(0, 0, 0);
        this.gameObject.transform.GetComponent<Renderer>().material.color = enemyColor;
    }
}
