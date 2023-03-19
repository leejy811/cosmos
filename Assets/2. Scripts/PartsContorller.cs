using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * PartsController Class
 * 이 클래스는 각 파츠 오브젝트에 들어가는 클래스로 파츠의 실질적인 동작을 담당하고 있다.
 *  Missile이 Bullet과 똑같이 동작하고 Laser또한 Target을 가져야 하므로 Bullet을 상속하여
 *  BulletSpeed는 파츠의 속도로 그리고 타겟과 타겟으로 발사하는 로직을 사용중이다.
*/

public class PartsContorller : Bullet
{
    //파츠 Status 관련 변수들
    [SerializeField]
    private string partsType;
    [SerializeField]
    private float partsRange;
    [SerializeField]
    private float partsDamage;
    public float partsAttackSpeed;
    public float partsValue;

    public PlayerController player;

    //Missile Parts 관련
    [SerializeField]
    private GameObject missileParticle;
    private bool isBomb;

    //Laser Parts 관련
    [SerializeField]
    private LineRenderer[] lasers;

    //Protocol만 풀링을 사용안하기 때문에 Protocol만 초기화를 해준다.
    private void Start()
    {
        if (partsType == "Protocol")
        {
            gameObject.GetComponentInParent<SpriteRenderer>().color = Color.red;
            partsRange = player.attackRange;
            StartCoroutine(ProtocolAttack());
        }
    }

    //Missile만 발사해야 되기 때문에 Bullet의 FixedUpdate를 오버라이딩해서 사용한다
    private new void FixedUpdate()
    {
        if (partsType == "Missile" && !isBomb)
        {
            CheckRange();
            base.FixedUpdate();
        }
    }

    //Init은 풀링을 사용하는 파츠들을 초기화 해줄때 사용하는 함수이다.
    public new void Init(Transform nearTarget)
    {
        base.Init(nearTarget);

        if (partsType == "Laser")
            LaserInit();
        if (partsType == "Emp")
            StartCoroutine("EmpAttack");
    }

    //CheckRange는 만약 Missile이 아무런 타겟도 맞추지 못하고 사거리에 도달하면 터지게 하는 함수이다.
    private void CheckRange()
    {
        if (Vector3.Distance(player.transform.position, transform.position) > player.attackRange)
            MissileAttack();
    }

    //MissileAttack은 Missile의 광역폭발을 실행하는 함수이다.
    public void MissileAttack()
    {
        if (isBomb)
            return;

        CircleRangeAttack();

        isBomb = true;
        missileParticle.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Invoke("BombEnd", 0.5f);
    }

    //BombEnd는 Missile이 터지고 난뒤 이펙트가 끝나면 다시 초기화를 하는 함수이다.
    private void BombEnd()
    {
        isBomb = false;
        missileParticle.SetActive(false);
        gameObject.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    IEnumerator EmpAttack()
    {
        transform.localScale = new Vector3(1, 1, 1);
        float explosionSpeed = (partsRange - transform.localScale.x) * bulletSpeed * Time.deltaTime;
        while (transform.localScale.x< partsRange)
        {
            transform.localScale += new Vector3(explosionSpeed, explosionSpeed, 0);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    //ProtocolAttack은 Protocol의 지속적인 공격을 위해 partsAttackSpeed마다 반복해주는 코루틴이다.
    IEnumerator ProtocolAttack()
    {
        while (true)
        {
            CircleRangeAttack();
            yield return new WaitForSeconds(1 / partsAttackSpeed);
        }
    }

    //CircleRangeAttack은 circle모양의 Range 내의 적에게 공격하는 함수이다.
    private void CircleRangeAttack()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, partsRange, LayerMask.GetMask("Enemy"));

        foreach (Collider2D target in targets)
        {
            if (Vector3.Distance(target.transform.position, transform.position) > partsRange)
                continue;

            Enemy enemy = target.gameObject.GetComponent<Enemy>();

            if (enemy == null)
            {
                Boss boss = target.gameObject.GetComponent<Boss>();
                boss.GetDamage(partsDamage);
            }
            else
                enemy.GetDamage(partsDamage);
        }
    }

    //LaserInit은 Laser의 LineRenderer의 Position을 초기화해주는 함수이다.
    private void LaserInit()
    {
        Vector3[] points = new Vector3[] { transform.position, target.transform.position * 10 - Vector3.up * 10 };

        foreach(LineRenderer laser in lasers)
            laser.SetPositions(points);
    }

    //GetPartsDamage은 partsDamage의 get 함수이다.
    public float GetPartsDamage()
    {
        return partsDamage;
    }

    //EndLaser는 Laser 애니메이션이 끝난 후 호출되는 이벤트 함수이다.
    public void EndLaser()
    {
        gameObject.SetActive(false);
    }
}