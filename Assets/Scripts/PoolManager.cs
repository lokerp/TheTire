using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    List<GameObject> pools = new List<GameObject>();

    public GameObject SpawnUnused(int inIndex, out int outIndex, bool startOverIfEnd)
    {
        outIndex = -1;

        if (inIndex + 1 >= pools.Count && startOverIfEnd)
        {
            pools.Add(pools[0]);
            pools.RemoveAt(0);
            inIndex--;
        }

        for (int i = inIndex + 1; i < pools.Count; i++)
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
    public GameObject SpawnYoungest(out int outIndex)
    {
        outIndex = -1;
        for (int i = 0; i < pools.Count; i++)
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

    public bool DespawnOldest(out int outIndex)
    {
        outIndex = -1;
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].activeInHierarchy)
            {
                pools[i].SetActive(false);
                outIndex = i;
                return true;
            }
        }
        return false;
    }

    public void Add(GameObject Gobject)
    {
        pools.Add(Gobject);
    }

    public PoolManager(List<GameObject> Gobjects = null)
    {
        if (Gobjects == null)
            throw new System.Exception("Отсутствуют объекты в PoolManager");
        else
            pools = Gobjects;
    }

    public PoolManager(int count, GameObject Gobject)
    {
        if (count <= 0)
            throw new System.Exception("Отсутствуют объекты в PoolManager");

        pools.Capacity = count;
        for (int i = 0; i < count; i++)
            pools.Add(Gobject);
    }
}
