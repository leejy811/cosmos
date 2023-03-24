using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/*
 * SkillManager Class
 * �� Ŭ������ Parts�� �ߵ��κ��� ����ϴ� Class�� Player���� �Ҵ�Ǿ� �ִ�.
 * Missile, Laser, EMP�� ������ �����ϰ� ���� Parts�� ������ �ִ� ���ε� �����Ѵ�.
 */
public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private PartsContorller barrier;
    [SerializeField]
    private Transform[] randomLaserPoints;

    [SerializeField]
    private Transform Target;
    private string currentParts;
    private float[] partsValue;

    //Start �Լ����� �� �߻� �ڷ�ƾ�� ������ ����ϴµ� ������� ������ �ֳĿ� ���� ����ó�� �����̴�.
    private void Start()
    {
        currentParts = PlayerPrefs.GetString("CurrentParts");

        if (currentParts == "Barrier")
        {
            partsValue = LocalDatabaseManager.instance.PartsProtocol;
            barrier.Init(null, partsValue);
            partsValue = new float[] { 1, 0.5f, 0 };
            if (partsValue[2] == 1)
                gameObject.GetComponent<PlayerController>().playerShield = 5;
            barrier.gameObject.SetActive(true);
            return;
        }
        else if (currentParts == "Missile")
        {
            partsValue = LocalDatabaseManager.instance.PartsMissile;
            partsValue = new float[] { 1, 0.5f, 1 };
        }
        else if (currentParts == "Laser")
        {
            partsValue = LocalDatabaseManager.instance.PartsLaser;
            partsValue = new float[] { 1, 0.5f, 1 };
            if (partsValue[2] == 1)
                randomLaserPoints = gameObject.GetComponentsInChildren<Transform>();
        }
        else if (currentParts == "Emp")
        {
            partsValue = LocalDatabaseManager.instance.PartsEmp;
            partsValue = new float[] { 0.3f, 0.2f, 0 };
        }

        StartCoroutine(ShootParts(currentParts));
    }

    //Update �Լ����� �Լ��� �ֽ����� ������Ʈ���ش�.
    private void Update()
    {
        Target = gameObject.GetComponent<PlayerController>().GetTarget(false);
        if (currentParts == "Barrier")
            barrier.Init(null, partsValue);
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
            parts.player = GameManger.instance.player;
            parts.Init(Target, partsValue);
            if (partsValue[2] == 1 && partsType == "Laser")
            {
                ShootRandomLaser();
            }

            yield return new WaitForSeconds(1 / partsValue[1]);
        }
    }

    private void ShootRandomLaser()
    {
        int ranIndex = Random.Range(1, randomLaserPoints.Length);
        Transform ranTarget = randomLaserPoints[ranIndex];
        PartsContorller parts = GameManger.instance.poolManager.GetPool("Laser").GetComponent<PartsContorller>();
        parts.transform.position = transform.position;
        parts.player = GameManger.instance.player;
        parts.Init(ranTarget, partsValue);
    }
}