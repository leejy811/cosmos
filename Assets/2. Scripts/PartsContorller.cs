using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * PartsController Class
 * �� Ŭ������ �� ���� ������Ʈ�� ���� Ŭ������ ������ �������� ������ ����ϰ� �ִ�.
 *  Missile�� Bullet�� �Ȱ��� �����ϰ� Laser���� Target�� ������ �ϹǷ� Bullet�� ����Ͽ�
 *  BulletSpeed�� ������ �ӵ��� �׸��� Ÿ�ٰ� Ÿ������ �߻��ϴ� ������ ������̴�.
*/

public class PartsContorller : Bullet
{
    //���� Status ���� ������
    [SerializeField]
    private string partsType;
    [SerializeField]
    private float partsRange;
    [SerializeField]
    private float partsDamage;
    public float partsValue;

    public PlayerController player;

    //Missile Parts ����
    [SerializeField]
    private GameObject missileBombParticle;
    [SerializeField]
    private GameObject missileBoostParticle;
    private bool isBomb;

    //Laser Parts ����
    [SerializeField]
    private LineRenderer[] lasers;

    private Animator barrierAnimatior;

    //Protocol�� Ǯ���� �����ϱ� ������ Protocol�� �ʱ�ȭ�� ���ش�.
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

    //Missile�� �߻��ؾ� �Ǳ� ������ Bullet�� FixedUpdate�� �������̵��ؼ� ����Ѵ�
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

    //Init�� Ǯ���� ����ϴ� �������� �ʱ�ȭ ���ٶ� ����ϴ� �Լ��̴�.
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

    //CheckRange�� ���� Missile�� �ƹ��� Ÿ�ٵ� ������ ���ϰ� ��Ÿ��� �����ϸ� ������ �ϴ� �Լ��̴�.
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

    //MissileAttack�� Missile�� ���������� �����ϴ� �Լ��̴�.
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

    //BombEnd�� Missile�� ������ ���� ����Ʈ�� ������ �ٽ� �ʱ�ȭ�� �ϴ� �Լ��̴�.
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

    //ProtocolAttack�� Protocol�� �������� ������ ���� partsAttackSpeed���� �ݺ����ִ� �ڷ�ƾ�̴�.
    IEnumerator BarrierAttack()
    {
        while (true)
        {
            CircleRangeAttack();
            yield return new WaitForSeconds(1 / bulletSpeed);
        }
    }

    //CircleRangeAttack�� circle����� Range ���� ������ �����ϴ� �Լ��̴�.
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

    //LaserInit�� Laser�� LineRenderer�� Position�� �ʱ�ȭ���ִ� �Լ��̴�.
    private void LaserInit()
    {
        Vector3[] points = new Vector3[] { transform.position, target.transform.position * 15 - Vector3.up * 15 };

        foreach(LineRenderer laser in lasers)
            laser.SetPositions(points);
    }

    //GetPartsDamage�� partsDamage�� get �Լ��̴�.
    public float GetPartsDamage()
    {
        return partsDamage;
    }

    public void LaserSound()
    {
        SoundManager.instance.PlaySFX("PlayerLaserSound");
    }

    //EndLaser�� Laser �ִϸ��̼��� ���� �� ȣ��Ǵ� �̺�Ʈ �Լ��̴�.
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