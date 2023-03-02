using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 프리팹 보관할 변수
    [SerializeField]
    private GameObject[] enemyPrefabs;
    [SerializeField]
    private GameObject bulletPrefab;

    // 풀 담당하는 리스트
    List<GameObject>[] enemyPools;
    List<GameObject> bulletPool;

    // 풀을 담을 위치
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

        // 선택한 풀의 내부에서 비활성화 된 게임 오브젝트 확인해서 selectEnemy에 할당 초과된다면 새로 생성
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
