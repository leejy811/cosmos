using DG.Tweening;
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
    private float[] partsValue = new float[3];

    [SerializeField] 
    private GameObject shield;

    //Start 함수에선 각 발사 코루틴의 실행을 담당하는데 어떤파츠를 가지고 있냐에 따른 예외처리 예정이다.
    private void Start()
    {
        currentParts = LocalDatabaseManager.instance.CurrentParts;

        for (int i = 0; i < 3; i++)
            partsValue[i] = LocalDatabaseManager.instance.PartsStatInfo[currentParts][i, LocalDatabaseManager.instance.PartsValue[i]];

        if (currentParts == "Barrier")
        {
            barrier.Init(null, partsValue);
            gameObject.GetComponent<PlayerController>().playerShield = (int)partsValue[2];
            SetShieldText((int)partsValue[2]);
            barrier.gameObject.SetActive(true);
            if ((int)partsValue[2] != 0)
                SetShieldActive(true);
            return;
        }
        else if (currentParts == "Laser")
        {
            if (partsValue[2] == 1)
                randomLaserPoints = gameObject.GetComponentsInChildren<Transform>();
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
            if (GetComponent<PlayerController>().isPlayerDie)
                break;

            if (Target == null)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            SpawnParts(partsType);

            if (partsValue[2] == 1 && partsType == "Laser")
                ShootRandomLaser();

            yield return new WaitForSeconds(1 / partsValue[1]);
        }
    }

    private void ShootRandomLaser()
    {
        int ranIndex = Random.Range(1, randomLaserPoints.Length);
        Target = randomLaserPoints[ranIndex];
        SpawnParts("Laser");
    }

    private void SpawnParts(string partsType)
    {
        PartsContorller parts = GameManger.instance.poolManager.GetPool(partsType).GetComponent<PartsContorller>();
        parts.transform.position = transform.position;
        parts.player = GameManger.instance.player;
        parts.Init(Target, partsValue);
    }

    public void SetShieldActive(bool active)
    {
        shield.GetComponentInChildren<MeshRenderer>().sortingLayerName = "Player";
        shield.SetActive(active);
    }

    public void SetShieldText(int shieldValue)
    {
        shield.GetComponentInChildren<TextMesh>().text = "X " + shieldValue.ToString();
    }
}