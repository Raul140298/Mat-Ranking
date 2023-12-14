using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THIS SCRIPT ASSUMES THAT THE GAMEOBJECT THAT HAS THIS COMPONENT ATTACHED 
// IS THE CONTAINER FOR ALL THE POOLABLE OBJECTS OF THE SPECIFIED TYPE

public class Pooler : MonoBehaviour
{
    [SerializeField]
    private List<Poolable> list;

    [SerializeField]
    private GameObject prefab;
    
    [SerializeField]
    private ePoolableObjectType poolableObjectType;

    public void Awake()
    {
        GetReferences();
    }

    public GameObject GetObject()
    {
        foreach (Poolable p in list)
        {
            if (p.IsActive() == false)
            {
                return p.gameObject;
            }
        }

        return CreateNewInstance();
    }

    public GameObject GetRandomObject()
    {
        int inactiveCount = GetInactiveObjectsCount();

        if (inactiveCount > 0)
        {
            int randomIndex = Random.Range(0, list.Count);

            while (list[randomIndex].IsActive() == true)
            { 
                randomIndex = Random.Range(0, list.Count);
            }

            return list[randomIndex].gameObject;
        }

        return CreateNewInstance();
    }

    private GameObject CreateNewInstance()
    {
        GameObject go = GameObject.Instantiate(prefab);

        go.name = prefab.name;
        go.transform.parent = gameObject.transform;
        go.SetLocalPositionXY(0, 0);

        Poolable p = go.GetComponent<Poolable>();

        p.Deactivate();

        list.Add(p);

        return go;
    }

    public void DeactivateObject(GameObject go)
    {
        go.GetComponent<Poolable>().Deactivate();
    }

    public void ActivateAll()
    {
        foreach (Poolable p in list)
        {
            p.Activate();
        }
    }

    public void DeactivateAll()
    {
        foreach (Poolable p in list)
        {
            p.Deactivate();
        }
    }

    public List<GameObject> GetActiveGameObjects()
    {
        List<GameObject> activeGOs = new List<GameObject>();

        foreach (Poolable p in list)
        {
            if (p.IsActive() == true)
            {
                activeGOs.Add(p.gameObject);
            }
        }

        return activeGOs;
    }

    private void GetReferences()
    {
        Poolable[] comps = gameObject.GetComponentsInChildren<Poolable>(true);

        list = new List<Poolable>();

        foreach (Poolable p in comps)
        {
            if (p.AskForType() == poolableObjectType)
            {
                p.InitializeActiveStatus();
                list.Add(p);
            }
        }
    }

    private int GetInactiveObjectsCount()
    {
        int inactiveCount = 0;

        foreach (Poolable p in list)
        {
            if (p.IsActive() == false)
            {
                inactiveCount += 1;
            }
        }

        return inactiveCount;
    }
}
