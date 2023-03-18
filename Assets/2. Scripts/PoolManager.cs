using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* IPool Interface
 * IPool은 Pool 을 구성하는 Interface로 name, container, prefab을 받아온다.
 */
[System.Serializable]
public struct IPool
{
    public string name;
    public Transform container;
    public GameObject prefab;
}

/* PoolManager Class
 * 이 클래스는 오브젝트 풀링을 담당하는 클래스이다.
 * IPool형태로 프리펩을 보관하고 poolLists에 실제 풀을 저장한다.
 */
public class PoolManager : MonoBehaviour
{
    // 프리팹 보관할 변수
    public IPool[] pools; 

    // 풀 담당하는 리스트
    private List<GameObject>[] poolLists;

    //Awake 함수에서 풀링을 할 List 를 초기화 해준다.
    void Awake()
    {
        poolLists = new List<GameObject>[pools.Length];

        for (int index = 0; index < pools.Length; index++)
            poolLists[index] = new List<GameObject>();
    }

    /* GetPool을 이용하면 Pool에서 오브젝트를 가져온다. 매개변수로 IPool에 등록한 이름을 사용하고
     * 오브젝트를 반환한다. 그리고 Pool을 처음부터 생성하는 것이 아닌 동적으로 생성한다.
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
