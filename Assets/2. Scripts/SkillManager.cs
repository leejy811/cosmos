using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 * SkillManager Class
 * �� Ŭ������ Parts�� �ߵ��κ��� ����ϴ� Class�� Player���� �Ҵ�Ǿ� �ִ�.
 * Missile, Laser, EMP�� ������ �����ϰ� ���� Parts�� ������ �ִ� ���ε� �����Ѵ�.
 */
public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Barrier;

    private Transform Target;
    private string currentParts;

    //Start �Լ����� �� �߻� �ڷ�ƾ�� ������ ����ϴµ� ������� ������ �ֳĿ� ���� ����ó�� �����̴�.
    private void Start()
    {
        currentParts = PlayerPrefs.GetString("CurrentParts");

        if (currentParts == "Barrier")
            Barrier.SetActive(true);
        else
            StartCoroutine(ShootParts(currentParts));
    }

    //Update �Լ����� �Լ��� �ֽ����� ������Ʈ���ش�.
    private void Update()
    {
        Target = gameObject.GetComponent<PlayerController>().GetTarget(false);
    }

    //ShootParts�� Parts�� �����ϴ� �ڷ�ƾ���� ���� Ÿ���� �Է����� �޴´�.
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