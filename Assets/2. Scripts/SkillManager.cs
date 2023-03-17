using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private Transform Target;

    private void Start()
    {
        StartCoroutine(ShootParts("Missile"));
        StartCoroutine(ShootParts("Laser"));
    }

    private void Update()
    {
        Target = gameObject.GetComponent<PlayerController>().GetTarget(false);
    }

    IEnumerator ShootParts(string partsType)
    {
        while (true)
        {
            if (Target == null)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            PartsContorller parts = GameManger.instance.poolManager.GetPool(partsType).GetComponent<PartsContorller>();
            parts.transform.position = transform.position;
            parts.Init(Target);
            parts.player = GameManger.instance.player;

            yield return new WaitForSeconds(1 / parts.partsAttackSpeed);
        }
    }
}