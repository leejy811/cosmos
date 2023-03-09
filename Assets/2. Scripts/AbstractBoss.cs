using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBoss : MonoBehaviour
{
    [SerializeField]
    protected string bossType;
    [SerializeField]
    protected float bossHealth;
    [SerializeField]
    protected float bossSpeed;

    public PlayerController playerController;

    Vector2 playerPos = new Vector2(0.0f, 1.0f);

    protected void Start()
    {
        BossLookPlayer();
    }

    protected void BossLookPlayer()
    {
        float targetAngle = Vector2.Angle(transform.up, playerPos - (Vector2)transform.position);
        targetAngle = transform.position.x >= 0 ? targetAngle : -targetAngle;
        transform.Rotate(new Vector3(0, 0, targetAngle));
    }

    protected void MoveBoss()
    {
        transform.position += transform.up * Time.deltaTime * bossSpeed;
    }

    protected void CheckPlayer(float changeSpeed)
    {
        if (transform.position.y - playerPos.y <= playerController.attackRange)
            bossSpeed = changeSpeed;
    }

    protected void FixedUpdate()
    {
        MoveBoss();
    }

    protected void OnTriggerEnter2D(Collider2D other)
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
    }

    protected void GetDamage(float damage)
    {
        if (bossHealth - damage <= 0)
        {
            BossDie();
        }
        else
            bossHealth -= damage;
    }

    public void BossDie()
    {
        bossHealth = 0;
        Destroy(gameObject);
    }

    protected abstract void BossPattern();
}
