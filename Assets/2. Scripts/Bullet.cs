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

        transform.position += attckDirection * bulletSpeed * Time.fixedDeltaTime;

        if (target == null)
            return;
        Enemy targetEnemy = target.gameObject.GetComponent<Enemy>();
        if (targetEnemy != null)
        {
            if (!target.gameObject.GetComponent<Enemy>().isEnemyLive && gameObject.tag != "Missile")
            {
                gameObject.SetActive(false);
                return;
            }
        }
        else if (target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (attckDirection == null)
            return;

        
    }
    //Init �Լ��� Bullet�� ��ȯ�Ҷ� �ʱ�ȭ ���ִ� �Լ��� target�� �Ű������� �޾Ƽ� ������ �ʱ�ȭ���ش�.
    public void Init(Transform nearTarget)
    {
        target = nearTarget;
        attckDirection = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, attckDirection);
    }
}