using DG.Tweening;
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
    private float[] partsValue = new float[3];

    [SerializeField] 
    private GameObject shield;

    //Start �Լ����� �� �߻� �ڷ�ƾ�� ������ ����ϴµ� ������� ������ �ֳĿ� ���� ����ó�� �����̴�.
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