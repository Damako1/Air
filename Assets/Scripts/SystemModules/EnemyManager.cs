using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{

    #region 变量

    /// <summary>
    /// 随机返回敌人列表中的元素
    /// </summary>
    public GameObject randomEnemy => enemyList.Count == 0 ? null : enemyList[Random.Range(0, enemyList.Count)];

    /// <summary>
    /// 敌人波数
    /// </summary>
    public int WaveNumber => waveNumber;

    /// <summary>
    /// 每波间隔时间
    /// </summary>
    public float TimeBeteenWaves => timeBetweenWaves;

    /// <summary>
    /// 敌人波数UI
    /// </summary>
    [Header("敌人波数UI")]
    [SerializeField] GameObject waveUI;

    /// <summary>
    /// 是否生成敌人
    /// </summary>
    [Header("是否生成敌人")]
    [SerializeField] bool isSpawnEnemy = true;

    /// <summary>
    /// 敌人预制体数组
    /// </summary>
    [Header("敌人预制体数组")]
    [SerializeField] GameObject[] enemyPrefabs;

    /// <summary>
    /// 敌人生成间隔时间
    /// </summary>
    [Header("敌人生成间隔时间")]
    [SerializeField] float timeBetweenSpawns = 1f;

    /// <summary>
    /// 每波最小敌人数量
    /// </summary>
    [Header("每波最小敌人数量")]
    [SerializeField] int minEnemyAmount = 4;

    /// <summary>
    /// 每波最大敌人数量
    /// </summary>
    [Header("每波最大敌人数量")]
    [SerializeField] int maxEnemyAmonnt = 10;

    /// <summary>
    /// 每波间隔时间
    /// </summary>
    [Header("每波间隔时间")]
    [SerializeField] float timeBetweenWaves = 1f;

    /// <summary>
    /// 敌人波数
    /// </summary>
    int waveNumber = 1;

    /// <summary>
    /// 每波敌人数量
    /// </summary>
    int enemyAmount;

    /// <summary>
    /// 敌人列表,记录所有生成的敌人,当敌人被消灭时移除敌人
    /// </summary>
    List<GameObject> enemyList;


    /// <summary>
    /// 等待生成间隔时间
    /// </summary>
    WaitForSeconds waitTimeBetweenSpawns;

    /// <summary>
    /// 等待直到没有敌人
    /// </summary>
    WaitUntil waituntilNoEnemy;//WaitUntil会等待直到完成某个条件

    /// <summary>
    /// 等待每波间隔时间
    /// </summary>
    WaitForSeconds waitTimeBetweenWaves;

    #endregion

    #region 生命周期

    protected override void Awake()
    {
        base.Awake();
        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        enemyList = new List<GameObject>();
        waituntilNoEnemy = new WaitUntil(() => enemyList.Count == 0);
        waitTimeBetweenWaves = new WaitForSeconds(timeBetweenWaves);
    }

    IEnumerator Start()
    {
        while (isSpawnEnemy)
        {
            yield return waituntilNoEnemy;//判断场景上是否有敌人
            waveUI.SetActive(true);//将敌人波数UI启用

            yield return waitTimeBetweenWaves;//等待一段时间

            waveUI.SetActive(false);//将敌人波数UI禁用

            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));//随机生成
        }

    }
    #endregion

    #region 方法

    /// <summary>
    /// 随机生成敌人协程
    /// </summary>
    /// <returns></returns>
    IEnumerator RandomlySpawnCoroutine()
    {
        enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / 3, maxEnemyAmonnt);
        for (int i = 0; i < enemyAmount; i++)
        {
            enemyList.Add(PoolManager.Release(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));
            yield return waitTimeBetweenSpawns;
        }
        waveNumber++;
    }

    /// <summary>
    /// 将列表中的敌人移除
    /// </summary>
    /// <param name="enemy"></param>
    public void RemoveFromList(GameObject enemy) => enemyList.Remove(enemy);

    #endregion
}
