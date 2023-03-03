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


    void Awake()
    {
        player = GameManger.instance.player.GetComponent<Rigidbody2D>();
        rigid = GetComponent<Rigidbody2D>();
        LookPlayer();

    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 dirVector = player.position - rigid.position;

        //Vector2 dirVector = new Vector2(player.transform.position.x - rigid.position.x, player.transform.position.y - rigid.position.y);
        Vector2 nextVector = dirVector.normalized * enemySpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVector);
        rigid.velocity = Vector2.zero;
    }
    
    public void LookPlayer()
    {
        //Debug.Log("target Position : " + player.position);
        //Debug.Log("this Position : " + rigid.position);
        //Debug.Log("this transform position " + this.gameObject.name);
        Vector2 dirVector = player.position - rigid.position;
        float lookAngle = Mathf.Atan2(dirVector.y, dirVector.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(lookAngle - 90f, Vector3.forward);
        transform.rotation = angleAxis;
        //Debug.Log("This Rotation  : " + transform.rotation);
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
            enemyHealth = 0;
            this.gameObject.SetActive(false);
        }
        else
            enemyHealth -= damage;
    }
}
