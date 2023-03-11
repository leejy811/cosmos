using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private bool isHaveParts = true;
    private Transform nearestTarget;

    private void Start()
    {
        if (isHaveParts)
            StartCoroutine(ShootMissile());
    }

    private void Update()
    {
        nearestTarget = GameManger.instance.player.nearestTarget;
    }

    IEnumerator ShootMissile()
    {
        while (true)
        {
            if (nearestTarget == null)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            Vector3 directionPosition = nearestTarget.position - transform.position;

            GameObject missile = GameManger.instance.poolManager.GetPool("Missile");
            missile.transform.position = transform.position;
            missile.GetComponent<PartsContorller>().Init(nearestTarget);
            missile.GetComponent<PartsContorller>().player = GameManger.instance.player;

            yield return new WaitForSeconds(missile.GetComponent<PartsContorller>().partsAttackSpeed);
        }
    }
}
