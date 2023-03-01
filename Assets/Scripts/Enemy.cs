using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float enemySpeed;
    private float enemyDamage;
    private float enemyHealth;

    Rigidbody2D rigid;
    bool isEnemyLive;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        LookPlayer();
    }
    void FixedUpdate()
    {
        Move();
    }
    
    private void Move()
    {
        Vector2 dirVector = Vector2.zero - rigid.position;
        Vector2 nextVector = dirVector.normalized * enemySpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVector);
        rigid.velocity = Vector2.zero;
    }
    
    private void LookPlayer()
    {
        Vector2 dirVector = Vector2.zero - rigid.position;
        float lookAngle = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(lookAngle - 90f, Vector3.forward);
        transform.rotation = angleAxis;
    }
}
