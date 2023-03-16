using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private Transform nearestTarget;

    private void Start()
    {
        StartCoroutine(ShootParts("Missile"));
        StartCoroutine(ShootParts("Laser"));
    }

    private void Update()
    {
        nearestTarget = GameManger.instance.player.nearestTarget;
    }

    IEnumerator ShootParts(string partsType)
    {
        while (true)
        {
            if (nearestTarget == null)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            PartsContorller parts = GameManger.instance.poolManager.GetPool(partsType).GetComponent<PartsContorller>();
            parts.transform.position = transform.position;
            parts.Init(nearestTarget);
            parts.player = GameManger.instance.player;

            yield return new WaitForSeconds(1 / parts.partsAttackSpeed);
        }
    }
}