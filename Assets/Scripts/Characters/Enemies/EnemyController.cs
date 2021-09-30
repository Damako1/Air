using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人控制器
/// </summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// 敌人发射子弹音效
    /// </summary>
    [Header("敌人发射子弹音效")]
    [SerializeField] AudioData[] projectileLuanchSFX;

    //移动相关
    [Header("模型与物体中心点的偏差值X")]
    [SerializeField] float paddingX;

    [Header("模型与物体中心点的偏差值Y")]
    [SerializeField] float paddingY;

    [Header("敌人的移动速度")]
    [SerializeField] float moveSpeed = 2f;

    [Header("敌人移动时旋转角度")]
    [SerializeField] float moveRotationAngle = 25f;



    /// <summary>
    /// 最小开火间隔时间
    /// </summary>
    [Header("最小开火间隔时间")]
    [SerializeField] float minFireInterval;

    /// <summary>
    /// 最大开火间隔时间
    /// </summary>
    [Header("最大开火间隔时间")]
    [SerializeField] float maxFireInterval;


    [Header("敌人可能一次发射多个子弹")]
    [SerializeField]GameObject[] projectiles;

    /// <summary>
    /// 开火位置
    /// </summary>
    [Header("开火位置")]
    [SerializeField] Transform muzzle;


    private void OnEnable()// OnEnable 当对象被启用时会调用
    {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    /// <summary>
    /// 随机移动协程
    /// </summary>
    /// <returns></returns>
    IEnumerator RandomlyMovingCoroutine()
    {
        transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);
        Vector3 targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

        #region 原
        //循环条件：当敌人处于激活状态，还没有被玩家消灭时
        //while (gameObject.activeSelf)
        //{
        //    如果敌人还未到达目标位置
        //    if (Vector3.Distance(transform.position, targetPosition) > Mathf.Epsilon)
        //    {
        //        Debug.Log(Vector3.Distance(transform.position, targetPosition) + "|||||||" + (Vector3.Distance(transform.position, targetPosition) >= Mathf.Epsilon));
        //        继续前往目标位置
        //        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        //        给敌人添加移动旋转效果
        //        transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.y * moveRotationAngle, Vector3.right);

        //    }
        //    else
        //    {
        //        如果到达了，就再给一个新的目标位置
        //        targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
        //        Debug.Log("新的位置" + targetPosition);
        //    }
        //    yield return null;
        //}
        #endregion



        while (gameObject.activeSelf)
        {
            float moveUP = moveSpeed * Time.fixedDeltaTime;

            //如果敌人还未到达目标位置
            if (Vector3.Distance(transform.position, targetPosition) > moveUP)
            {
                //继续前往目标位置
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                //给敌人添加移动旋转效果
                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.y * moveRotationAngle, Vector3.right);

            }
            else
            {
                //如果到达了，就再给一个新的目标位置
                targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

            }
            yield return null;
        }

    }

    /// <summary>
    /// 随机开火协程
    /// </summary>
    /// <returns></returns>
    IEnumerator RandomlyFireCoroutine()
    { 
        while(gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));
            foreach (var item in projectiles)
            {
                PoolManager.Release(item, muzzle.position);
            }
            AudioManager.Instance.PlayRandomSFX(projectileLuanchSFX);
        }
    }
}
