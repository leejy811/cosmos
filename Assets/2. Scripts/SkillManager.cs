using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 * SkillManager Class
 * 이 클래스는 Parts의 발동부분을 담당하는 Class로 Player에게 할당되어 있다.
 * Missile, Laser, EMP의 생성을 관리하고 무슨 Parts를 가지고 있는 여부도 관리한다.
 */
public class SkillManager : MonoBehaviour
{
    private Transform Target;

    //Start 함수에선 각 발사 코루틴의 실행을 담당하는데 어떤파츠를 가지고 있냐에 따른 예외처리 예정이다.
    private void Start()
    {
        StartCoroutine(ShootParts("Missile"));
        StartCoroutine(ShootParts("Laser"));
    }

    //Update 함수에선 함수를 최신으로 업데이트해준다.
    private void Update()
    {
        Target = gameObject.GetComponent<PlayerController>().GetTarget(false);
    }

    //ShootParts는 Parts를 생성하는 코루틴으로 파츠 타입을 입력으로 받는다.
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