using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private int bossType;
    [SerializeField]
    private float bossHealth;
    [SerializeField]
    private float bossSpeed;

    public PlayerController playerController;
    public WaveManager waveManager;

    Vector2 playerPos = new Vector2(0.0f, 1.0f);

    public void Start()
    {
        BossLookPlayer();
    }

    public void BossLookPlayer()
    {
        float targetAngle = Vector2.Angle(transform.up, playerPos - (Vector2)transform.position);
        targetAngle = transform.position.x >= 0 ? targetAngle : -targetAngle;
        transform.Rotate(new Vector3(0, 0, targetAngle));
    }

    public void MoveBoss()
    {
        CheckPlayer();
        transform.position += transform.up * Time.deltaTime * bossSpeed;
    }

    public void CheckPlayer()
    {
        float changeSpeed = 0;

        if (transform.position.y - playerPos.y <= playerController.attackRange)
        {
            switch (bossType)
            {
                case 0:
                    changeSpeed = 0.3f;
                break;
                case 1:
                    changeSpeed = 0;
                    break;
                case 2:
                    changeSpeed = 0;
                    break;
                case 3:
                    changeSpeed = 0;
                    break;
            }

            bossSpeed = changeSpeed;
        }
    }

    public void FixedUpdate()
    {
        MoveBoss();
    }

    public void OnTriggerEnter2D(Collider2D other)
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

    public void GetDamage(float damage)
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
        waveManager.CheckWaveEnd();
        Destroy(gameObject);
    }

    public void BossPattern()
    { }

}
