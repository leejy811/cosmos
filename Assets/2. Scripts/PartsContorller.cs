using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsContorller : Bullet
{
    [SerializeField]
    private string partsType;

    [SerializeField]
    private float partsRange;
    [SerializeField]
    private float partsDamage;
    public float partsAttackSpeed;
    public float partsValue;

    [SerializeField]
    private GameObject missileParticle;

    private bool isBomb;

    public PlayerController player;

    [SerializeField]
    private LineRenderer LaserStart;
    [SerializeField]
    private LineRenderer LaserEnd;

    private void Start()
    {
        if (partsType == "Protocol")
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            partsRange = player.attackRange;
            StartCoroutine(ProtocolAttack());
        }
    }

    private new void FixedUpdate()
    {
        if (partsType == "Missile" && !isBomb)
        {
            CheckRange();
            base.FixedUpdate();
        }
    }

    public new void Init(Transform nearTarget)
    {
        base.Init(nearTarget);

        if (partsType == "Laser")
            LaserInit();
    }

    private void CheckRange()
    {
        if (Vector3.Distance(player.transform.position, transform.position) > player.attackRange)
            MissileAttack();
    }

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

    private void BombEnd()
    {
        isBomb = false;
        missileParticle.SetActive(false);
        gameObject.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    IEnumerator ProtocolAttack()
    {
        while (true)
        {
            CircleRangeAttack();
            yield return new WaitForSeconds(1 / partsAttackSpeed);
        }
    }

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

    public void LaserInit()
    {
        Vector3[] Vec3points = new Vector3[] { transform.position, target.transform.position * 10 - Vector3.up * 10 };

        LaserStart.SetPositions(Vec3points);
        LaserEnd.SetPositions(Vec3points);
    }

    public float GetPartsDamage()
    {
        return partsDamage;
    }

    public void EndLaser()
    {
        gameObject.SetActive(false);
    }
}