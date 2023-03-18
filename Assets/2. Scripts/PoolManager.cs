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
}
