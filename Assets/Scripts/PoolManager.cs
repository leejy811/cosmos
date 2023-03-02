using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 프리팹 보관할 변수
    [SerializeField]
    private GameObject[] enemyPrefabs;

    // 풀 담당하는 리스트
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

        // 선택한 풀의 내부에서 비활성화 된 게임 오브젝트 확인해서 selectEnemy에 할당 초과된다면 새로 생성
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
