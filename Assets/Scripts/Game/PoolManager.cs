using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    List<GameObject> pools = new();

    public GameObject SpawnByIndex(int inIndex, out int outIndex, bool startOverIfEnd)
    {
        outIndex = -1;

        if ((inIndex >= pools.Count || inIndex < 0) && startOverIfEnd)
        {
            inIndex %= pools.Count;
        }

        for (int i = inIndex; i < pools.Count; i++)
        {
            if (!pools[i].activeInHierarchy)
            {
                pools[i].SetActive(true);
                outIndex = i;
                return pools[i];
            }
        }

        return null;
    }
    public GameObject SpawnYoungest()
    {
        for (int i = 0; i < pools.Count; i++)
        {
            if (!pools[i].activeInHierarchy)
            {
                pools[i].SetActive(true);
                return pools[i];
            }
        }
        return null;
    }

    public bool DespawnOldest()
    {
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].activeInHierarchy)
            {
                pools[i].SetActive(false);
                return true;
            }
        }
        return false;
    }

    public bool DespawnByIndex(int inIndex, out int outIndex, bool startOverIfEnd)
    {
        outIndex = -1;


        if ((inIndex >= pools.Count || inIndex < 0) && startOverIfEnd)
        {
            inIndex %= pools.Count;
        }

        if (!pools[inIndex].activeInHierarchy)
        {
            pools[inIndex].SetActive(true);
            outIndex = inIndex;
            return true;
        }

        Debug.LogError("!!! Ёлемент не был деспавнен!");
        return false;
    }

    public void Add(GameObject Gobject)
    {
        pools.Add(Gobject);
    }

    public PoolManager(List<GameObject> Gobjects = null)
    {
        if (Gobjects == null)
            throw new System.Exception("ќтсутствуют объекты в PoolManager");
        else
            pools = Gobjects;
    }

    public PoolManager(int count, GameObject Gobject)
    {
        if (count <= 0)
            throw new System.Exception("ќтсутствуют объекты в PoolManager");

        pools.Capacity = count;
        for (int i = 0; i < count; i++)
            pools.Add(Gobject);
    }
}
