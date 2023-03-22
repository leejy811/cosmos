using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/* Bullet Class
 * 이 클래스는 Bullet의 이동을 담당하는 클래스이다.
 * Bullet 오브젝트에 할당되어 있고 발사되는 다양한 파츠에 상속하여 사용된다.
 */
public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    protected Transform target;
    protected Vector3 attckDirection;

    //FixedUpdate에선 Bullet을 이동시켜주고 Bullet이 Target이 없어지면 사라지도록 예외처리가 되어있다.
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

    //Init 함수는 Bullet을 소환할때 초기화 해주는 함수로 target을 매개변수로 받아서 방향을 초기화해준다.
    public void Init(Transform nearTarget)
    {
        target = nearTarget;
        attckDirection = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, attckDirection);
    }
}