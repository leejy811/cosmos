using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/* Bullet Class
 * �� Ŭ������ Bullet�� �̵��� ����ϴ� Ŭ�����̴�.
 * Bullet ������Ʈ�� �Ҵ�Ǿ� �ְ� �߻�Ǵ� �پ��� ������ ����Ͽ� ���ȴ�.
 */
public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    protected Transform target;
    protected Vector3 attckDirection;

    //FixedUpdate���� Bullet�� �̵������ְ� Bullet�� Target�� �������� ��������� ����ó���� �Ǿ��ִ�.
    protected void FixedUpdate()
    {
        if ((!target.gameObject.GetComponent<Enemy>().isEnemyLive && gameObject.tag == "Bullet") || (target == null && gameObject.tag == "Missile"))
        {
            gameObject.SetActive(false);
            return;
        }

        if (attckDirection == null)
            return;

        transform.position += attckDirection * bulletSpeed * Time.fixedDeltaTime;
    }

    //Init �Լ��� Bullet�� ��ȯ�Ҷ� �ʱ�ȭ ���ִ� �Լ��� target�� �Ű������� �޾Ƽ� ������ �ʱ�ȭ���ش�.
    public void Init(Transform nearTarget)
    {
        target = nearTarget;
        attckDirection = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, attckDirection);
    }
}