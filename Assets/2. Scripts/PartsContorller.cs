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

    private  new void FixedUpdate()
    {
        if (partsType != "Missile")
            return;

        base.FixedUpdate();
    }

    public void MissileAttack()
    {
        LayerMask targetLayer = LayerMask.GetMask("Enemy");
        RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, partsRange, Vector2.zero, 0, targetLayer); 

        foreach(RaycastHit2D target in targets)
        {
            Enemy enemy = target.transform.gameObject.GetComponent<Enemy>();
            enemy.GetDamage(partsDamage);
            gameObject.SetActive(false);
        }
    }
}
