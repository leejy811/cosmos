using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/*
 * SkillManager Class
 * 이 클래스는 Parts의 발동부분을 담당하는 Class로 Player에게 할당되어 있다.
 * Missile, Laser, EMP의 생성을 관리하고 무슨 Parts를 가지고 있는 여부도 관리한다.
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

    //Start 함수에선 각 발사 코루틴의 실행을 담당하는데 어떤파츠를 가지고 있냐에 따른 예외처리 예정이다.
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

    //Update 함수에선 함수를 최신으로 업데이트해준다.
    private void Update()
    {
        Target = gameObject.GetComponent<PlayerController>().GetTarget(false);
        if (currentParts == "Barrier")
            barrier.Init(null, partsValue);
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