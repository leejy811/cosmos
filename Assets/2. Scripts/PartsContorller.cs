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

    [SerializeField]
    private GameObject particle;

    private bool isBomb;

    public PlayerController player;
    private  new void FixedUpdate()
    {
        if (partsType != "Missile" || isBomb)
            return;

        CheckRange();

        base.FixedUpdate();
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
        LayerMask targetLayer = LayerMask.GetMask("Enemy");
        RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, partsRange, Vector2.zero, 0, targetLayer); 

        foreach(RaycastHit2D target in targets)
        {
            if (Vector3.Distance(target.transform.position, transform.position) > partsRange)
                continue;

            Enemy enemy = target.transform.gameObject.GetComponent<Enemy>();

            if (enemy == null)
            {
                Boss boss = target.transform.gameObject.GetComponent<Boss>();
                boss.GetDamage(partsDamage);
            }
            else
                enemy.GetDamage(partsDamage);
        }

        isBomb = true;
        particle.SetActive(true);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Invoke("BombEnd", 0.5f);
    }

    private void BombEnd()
    {
        isBomb = false;
        particle.SetActive(false);
        gameObject.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
}
