using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolerManager : MonoBehaviour
{
    //It can be useless maybe; but I dont use yet for this project
    public Dictionary<string, Queue<GameObject>> poolerDictionary;
    public List<Pool> pools;


    #region singleton
    public static PoolerManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        poolerDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> pO = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject o = Instantiate(pool.prefab);
                o.SetActive(false);
                pO.Enqueue(o);
            }

            poolerDictionary.Add(pool.tag, pO);
        }
    }

    public void SetDeActiveAll()
    {
        foreach (Pool pool in pools)
        {
            for (int i = 0; i < pool.size; i++)
            {
                if (poolerDictionary.ContainsKey(pool.tag))
                {
                    GameObject oS = poolerDictionary[pool.tag].Dequeue();
                    Destroy(oS);
                }
            }
        }
        Init();
    }

    /// <summary>
    /// spawn according to tag from pool
    /// </summary>
    /// <param name="tag">pool object tag</param>
    /// <param name="position">pool object position</param>
    /// <returns></returns>
    public GameObject SpawnPoolTag(string tag,Vector3 position)
    {
        if (!poolerDictionary.ContainsKey(tag))
        {
            return null;
        }
        GameObject oS = poolerDictionary[tag].Dequeue();
        oS.SetActive(true);
        oS.transform.position = position;

        poolerDictionary[tag].Enqueue(oS);
        return oS;
    }
}
