using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // ������ ������ ����
    [SerializeField]
    private GameObject[] enemyPrefabs;
    [SerializeField]
    private GameObject bulletPrefab;

    // Ǯ ����ϴ� ����Ʈ
    List<GameObject>[] enemyPools;
    List<GameObject> bulletPool;

    // Ǯ�� ���� ��ġ
    [SerializeField] 
    Transform enemyTrianglePoolParent;
    [SerializeField] 
    Transform enemyCirclePoolParent;
    [SerializeField] 
    Transform bulletPoolParent;

    void Awake()
    {
        enemyPools = new List<GameObject>[enemyPrefabs.Length];
        
        for(int idx =0; idx<enemyPools.Length; idx++)
        {
            enemyPools[idx] = new List<GameObject>();
        }

        bulletPool = new List<GameObject>();
    }

    public GameObject GetPool(string name)
    {
        GameObject selectObject = null;
        GameObject prefab = null;
        Transform poolParent = null;
        List<GameObject> pool = new List<GameObject>();

        switch (name)
        {
            case "EnemyTriangle":
                pool = enemyPools[0];
                prefab = enemyPrefabs[0];
                poolParent = enemyTrianglePoolParent;
                break;
            case "EnemyCircle":
                pool = enemyPools[1];
                prefab = enemyPrefabs[1];
                poolParent = enemyCirclePoolParent;
                break;
            case "Bullet":
                pool = bulletPool;
                prefab = bulletPrefab;
                poolParent = bulletPoolParent;
                break;
        }

        // ������ Ǯ�� ���ο��� ��Ȱ��ȭ �� ���� ������Ʈ Ȯ���ؼ� selectEnemy�� �Ҵ� �ʰ��ȴٸ� ���� ����
        foreach (GameObject go in pool)
        {
            if(!go.activeSelf)
            {
                selectObject = go;
                selectObject.SetActive(true);
                break;
            }
        }
        if(selectObject == null)
        {
            selectObject = Instantiate(prefab, poolParent);
            pool.Add(selectObject);
        }

        return selectObject;
    }
}
