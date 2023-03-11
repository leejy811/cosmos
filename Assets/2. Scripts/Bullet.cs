using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    protected float bulletSpeed;
    protected Transform target;
    protected Vector3 attckDirection;

    protected void FixedUpdate()
    {
        if (target == null)
        {
            transform.position += attckDirection * bulletSpeed * Time.fixedDeltaTime;
            return;
        }

        if (!target.gameObject.activeSelf && gameObject.tag != "Missile")
        {
            gameObject.SetActive(false);
            return;
        }

        if (attckDirection == null)
            return;

        transform.position += attckDirection * bulletSpeed * Time.fixedDeltaTime;
    }

    public void Init(Transform nearTarget)
    {
        target = nearTarget;
        attckDirection = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, attckDirection);
    }
}