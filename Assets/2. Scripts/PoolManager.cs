using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IPool
{
    public string name;
    public Transform container;
    public GameObject prefab;
}

public class PoolManager : MonoBehaviour
{
    // 프리팹 보관할 변수
    public IPool[] pools; 

    // 풀 담당하는 리스트
    private List<GameObject>[] poolLists;

    void Awake()
    {
        poolLists = new List<GameObject>[pools.Length];

        for (int index = 0; index < pools.Length; index++)
            poolLists[index] = new List<GameObject>();
    }

    public GameObject GetPool(string poolName)
    {
        GameObject selectObject = null;
        int poolIndex = -1;

        for (int index = 0; index < pools.Length; index++)
        {
            if (pools[index].name.Equals(poolName))
            {
                poolIndex = index;
                break;
            }
        }

        if (poolIndex == -1)
            return null;

        for (int index = 0; index < poolLists[poolIndex].Count; index++)
        {
            GameObject getObject = poolLists[poolIndex][index];
            if (!getObject.activeSelf)
            {
                selectObject = getObject;
                selectObject.SetActive(true);
                break;
            }
        }
        if (selectObject == null)
        {
            selectObject = Instantiate(pools[poolIndex].prefab, pools[poolIndex].container);
            poolLists[poolIndex].Add(selectObject);
        }

        return selectObject;
    }
}
