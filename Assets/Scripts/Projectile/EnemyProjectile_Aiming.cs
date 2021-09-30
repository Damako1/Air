using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 会瞄准玩家的子弹
public class EnemyProjectile_Aiming : Projectile
{

    #region 生命周期

    private void Awake()
    {
        SetTarget(GameObject.FindGameObjectWithTag("Player"));
    }

    protected override void OnEnable()
    {
        StartCoroutine(nameof(MoveDirectionCoroutine));
        base.OnEnable();
    }
    #endregion

    #region 方法

    /// <summary>
    /// 移动方向协程 (为了解决，在子弹被启用的瞬间，他的位置参数由于浮点数的问
    /// 题，可能会存在误差，应该先稍等一个极短的瞬间，来让引擎取得
    /// 精确的参数值)
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveDirectionCoroutine()
    {
        yield return null;
        if (target.activeSelf)
        {
            moveDirection = (target.transform.position - transform.position).normalized;
        }
    }


    #endregion

}
