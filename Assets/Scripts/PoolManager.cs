using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // ������ ������ ����
    [SerializeField]
    private GameObject[] enemyPrefabs;

    // Ǯ ����ϴ� ����Ʈ
    List<GameObject>[] enemyPools;

    void Awake()
    {
        enemyPools = new List<GameObject>[enemyPrefabs.Length];
        
        for(int idx =0; idx<enemyPools.Length; idx++)
        {
            enemyPools[idx] = new List<GameObject>();
        }
    }

    public GameObject GetEnemy(int idx)
    {
        GameObject selectEnemy = null;

        // ������ Ǯ�� ���ο��� ��Ȱ��ȭ �� ���� ������Ʈ Ȯ���ؼ� selectEnemy�� �Ҵ� �ʰ��ȴٸ� ���� ����
        foreach(GameObject go in enemyPools[idx])
        {
            if(!go.activeSelf)
            {
                selectEnemy = go;
                selectEnemy.SetActive(true);
                break;
            }
        }
        if(selectEnemy == null)
        {
            selectEnemy = Instantiate(enemyPrefabs[idx], transform);
            enemyPools[idx].Add(selectEnemy);
        }

        return selectEnemy;
    }
}
