using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    /// <summary>
    /// 玩家子弹数组
    /// </summary>
    [Header("玩家子弹数组")]
    [SerializeField] Pool[] playerProjectilePools;
    
    /// <summary>
    /// 敌人子弹数组
    /// </summary>
    [Header("敌人子弹数组")]
    [SerializeField] Pool[] enemyProjectilePools;

    /// <summary>
    /// 特效池
    /// </summary>
    [Header("特效池")]
    [SerializeField] Pool[] vFXPools;

    /// <summary>
    /// 敌人池
    /// </summary>
    [Header("敌人池")]
    [SerializeField] Pool[] enemyPools;



    static Dictionary<GameObject, Pool> dictionary;
    private void Awake()
    {
        dictionary = new Dictionary<GameObject, Pool>();

        Initialize(enemyPools);
        Initialize(playerProjectilePools);
        Initialize(enemyProjectilePools);
        Initialize(vFXPools);
    }

    private void OnDestroy()
    {
        CheckPoolSize(enemyPools);
        CheckPoolSize(playerProjectilePools);
        CheckPoolSize(enemyProjectilePools);
        CheckPoolSize(vFXPools);
    }

    /// <summary>
    /// 尺寸检测
    /// </summary>
    /// <param name="pools"></param>
    void CheckPoolSize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if (pool.RuntimeSize>pool.Size)
            {
                Debug.LogWarning(string.Format("Pool:{0}运行时的大小大于{1}的初始大小{2}",pool.Prefab.name,pool.RuntimeSize,pool.Size));

            }
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="pools"></param>
    void Initialize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if (dictionary.ContainsKey(pool.Prefab))
            {
                Debug.Log("已经有相同的键");
                continue;
            }
            dictionary.Add(pool.Prefab, pool);
            Transform poolParent = new GameObject("Pool:" + pool.Prefab.name).transform;
            poolParent.parent = transform;
            pool.Initialize(poolParent);
        }
    }

    /// <summary>
    /// 根据传入的prefab返回对象池中的预备对象
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab)
    {
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.Log("字典中没有包含这个key");
            return null;
        }
        return dictionary[prefab].PreparedObject();
    }

    public static GameObject Release(GameObject prefab,Vector3 position)
    {
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.Log("字典中没有包含这个key");
            return null;
        }
        return dictionary[prefab].PreparedObject(position);
    }

    public static GameObject Release(GameObject prefab, Vector3 position,Quaternion rotation)
    {
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.Log("字典中没有包含这个key");
            return null;
        }
        return dictionary[prefab].PreparedObject(position, rotation);
    }

    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation,Vector3 localScale)
    {
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.Log("字典中没有包含这个key");
            return null;
        }
        return dictionary[prefab].PreparedObject(position, rotation, localScale);
    }
}
