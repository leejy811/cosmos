using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D player;
    [SerializeField]
    private float enemySpeed;
    [SerializeField]
    private float enemyDamage;
    [SerializeField]
    private float enemyHealth;

    Rigidbody2D rigid;
    bool isEnemyLive;

    Vector2 playerPos = new Vector2(0.0f, 1.3f);
    

    private void Start()
    {
        float targetAngle = Vector2.Angle(transform.up, playerPos - (Vector2)transform.position);
        targetAngle = transform.position.x >= 0 ? targetAngle : -targetAngle;
        transform.Rotate(new Vector3(0, 0, targetAngle));
    }

    public void EnemyLookPlayer()
    {
        float targetAngle = Vector2.Angle(transform.up, playerPos - (Vector2)transform.position);
        targetAngle = transform.position.x >= 0 ? targetAngle : -targetAngle;
        transform.Rotate(new Vector3(0, 0, targetAngle));
    }
    void FixedUpdate()
    {
        transform.position += transform.up * Time.deltaTime * enemySpeed;
    }

 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            GetDamage(3f);
            other.gameObject.SetActive(false);
        }
    }
    public void GetDamage(float damage)
    {
        if (enemyHealth - damage <= 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
            enemyHealth = 0;
            this.gameObject.SetActive(false);

        }
        else
            enemyHealth -= damage;
    }
}
