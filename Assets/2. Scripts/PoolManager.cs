using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* IPool Interface
 * IPool�� Pool �� �����ϴ� Interface�� name, container, prefab�� �޾ƿ´�.
 */
[System.Serializable]
public struct IPool
{
    public string name;
    public Transform container;
    public GameObject prefab;
}

/* PoolManager Class
 * �� Ŭ������ ������Ʈ Ǯ���� ����ϴ� Ŭ�����̴�.
 * IPool���·� �������� �����ϰ� poolLists�� ���� Ǯ�� �����Ѵ�.
 */
public class PoolManager : MonoBehaviour
{
    // ������ ������ ����
    public IPool[] pools; 

    // Ǯ ����ϴ� ����Ʈ
    private List<GameObject>[] poolLists;

    //Awake �Լ����� Ǯ���� �� List �� �ʱ�ȭ ���ش�.
    void Awake()
    {
        poolLists = new List<GameObject>[pools.Length];

        for (int index = 0; index < pools.Length; index++)
            poolLists[index] = new List<GameObject>();
    }

    /* GetPool�� �̿��ϸ� Pool���� ������Ʈ�� �����´�. �Ű������� IPool�� ����� �̸��� ����ϰ�
     * ������Ʈ�� ��ȯ�Ѵ�. �׸��� Pool�� ó������ �����ϴ� ���� �ƴ� �������� �����Ѵ�.
     */
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
    public bool CheckPool(string poolName)
    {
        int poolIndex = -1;
        for(int idx = 0; idx<pools.Length;idx++)
        {
            if(pools[idx].name.Equals(poolName))
            {
                poolIndex = idx;
                break;
            }
        }
        for(int index = 0; index <poolLists[poolIndex].Count; index++)
        {
            GameObject ckObject = poolLists[poolIndex][index];

            if(ckObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
    
    public void DelPoolObject()
    {
        Debug.Log("Del Pool Object Start !");
        for(int poolIdx = 0; poolIdx <pools.Length; poolIdx++)
        {
            Debug.Log(poolLists[poolIdx].Count);
            for(int idx = 0; idx <poolLists[poolIdx].Count; idx++)
            {
                GameObject gameObject = poolLists[poolIdx][idx];
                Debug.Log(poolLists[poolIdx][idx].name);

                if (gameObject.tag != "Enemy")
                    return;

                if (gameObject.activeSelf)
                    gameObject.SetActive(false);
            }
        }
    }
}
