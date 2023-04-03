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
    public float partsValue;

    public PlayerController player;

    //Missile Parts 관련
    [SerializeField]
    private GameObject missileBombParticle;
    [SerializeField]
    private GameObject missileBoostParticle;
    private bool isBomb;

    //Laser Parts 관련
    [SerializeField]
    private LineRenderer[] lasers;

    private Animator barrierAnimatior;

    //Protocol만 풀링을 사용안하기 때문에 Protocol만 초기화를 해준다.
    private void Start()
    {
        if (partsType == "Barrier")
        {
            //gameObject.GetComponentInParent<SpriteRenderer>().color = Color.red;
            barrierAnimatior = gameObject.GetComponent<Animator>();
            partsRange = player.attackRange;
            StartCoroutine(BarrierAttack());
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
        if (partsType == "Barrier")
            CheckBarrier();
    }

    //Init은 풀링을 사용하는 파츠들을 초기화 해줄때 사용하는 함수이다.
    public void Init(Transform nearTarget, float[] value)
    {
        partsDamage = player.playerDamage * value[0];

        if (partsType == "Barrier")
        {
            this.partsValue = value[1];
            return;
        }
        else if (partsType == "Emp")
        {
            this.partsValue = value[2];
            StartCoroutine("EmpAttack");
            return;
        }

        Init(nearTarget);

        if (partsType == "Missile" && value[2] != 0)
            partsRange = value[2];
        else if (partsType == "Laser")
            LaserInit();
    }

    //CheckRange는 만약 Missile이 아무런 타겟도 맞추지 못하고 사거리에 도달하면 터지게 하는 함수이다.
    private void CheckRange()
    {
        if (Vector3.Distance(player.transform.position, transform.position) > player.attackRange)
            MissileAttack();
    }

    private void CheckBarrier()
    {
        bool isTarget = player.nearestTarget != null;
        if(barrierAnimatior.GetBool("IsTarget") != isTarget && barrierAnimatior.GetBool("IsAttack") == false)
        {
            barrierAnimatior.SetBool("IsTarget", isTarget);
            if (isTarget)
            {
                barrierAnimatior.SetBool("IsAttack", true);
                barrierAnimatior.SetTrigger("DoAttack");
            }
        }
    }

    //MissileAttack은 Missile의 광역폭발을 실행하는 함수이다.
    public void MissileAttack()
    {
        if (isBomb)
            return;

        CircleRangeAttack();
        SoundManager.instance.PlaySFX("PlayerMissileSound");
        isBomb = true;
        missileBoostParticle.SetActive(false);
        missileBombParticle.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Invoke("BombEnd", 0.5f);
    }

    //BombEnd는 Missile이 터지고 난뒤 이펙트가 끝나면 다시 초기화를 하는 함수이다.
    private void BombEnd()
    {
        isBomb = false;
        missileBombParticle.SetActive(false);
        gameObject.SetActive(false);
        missileBoostParticle.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void EmpAttack()
    {
        SoundManager.instance.PlaySFX("PlayerEmpSound");
        transform.localScale = new Vector3(0.3f, 0.3f, 1);
        gameObject.GetComponent<Animator>().SetFloat("EmpSpeed", bulletSpeed);
        //float explosionSpeed = (partsRange - transform.localScale.x) * bulletSpeed * Time.fixedDeltaTime;
        //while (transform.localScale.x< partsRange)
        //{
        //    transform.localScale += new Vector3(explosionSpeed, explosionSpeed, 0);
        //    yield return new WaitForFixedUpdate();
        //}
    }

    //ProtocolAttack은 Protocol의 지속적인 공격을 위해 partsAttackSpeed마다 반복해주는 코루틴이다.
    IEnumerator BarrierAttack()
    {
        while (true)
        {
            CircleRangeAttack();
            yield return new WaitForSeconds(1 / bulletSpeed);
        }
    }

    //CircleRangeAttack은 circle모양의 Range 내의 적에게 공격하는 함수이다.
    private bool CircleRangeAttack()
    {
        bool isTarget = false;
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

            isTarget = true;
        }

        return isTarget;
    }

    //LaserInit은 Laser의 LineRenderer의 Position을 초기화해주는 함수이다.
    private void LaserInit()
    {
        Vector3[] points = new Vector3[] { transform.position, target.transform.position * 15 - Vector3.up * 15 };

        foreach(LineRenderer laser in lasers)
            laser.SetPositions(points);
    }

    //GetPartsDamage은 partsDamage의 get 함수이다.
    public float GetPartsDamage()
    {
        return partsDamage;
    }

    public void LaserSound()
    {
        SoundManager.instance.PlaySFX("PlayerLaserSound");
    }

    //EndLaser는 Laser 애니메이션이 끝난 후 호출되는 이벤트 함수이다.
    public void EndLaser()
    {
        gameObject.SetActive(false);
    }

    public void EndBarrierAttack()
    {
        barrierAnimatior.SetBool("IsAttack", false);
        Debug.Log(barrierAnimatior.GetBool("IsAttack"));
    }

    public void EndEmp()
    {
        gameObject.SetActive(false);
    }
}