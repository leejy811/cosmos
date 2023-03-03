using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed;
    private Vector3 attckDirection;
    Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (attckDirection == null)
            return;

        transform.position += attckDirection * bulletSpeed * Time.fixedDeltaTime;
    }

    public void Init(Vector3 Direction)
    {
        attckDirection = Direction;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, Direction);
    }
}