using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    protected float bulletSpeed;
    protected Vector3 attckDirection;

    protected void FixedUpdate()
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