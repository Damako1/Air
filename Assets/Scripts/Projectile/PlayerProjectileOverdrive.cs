using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileOverdrive : PlayerProjectile
{
    #region 变量
    [SerializeField] ProjectileGuidanceSystem projectileGuidanceSystem;
    #endregion

    #region 生命周期
    protected override void OnEnable()
    {
        SetTarget(EnemyManager.Instance.randomEnemy);
        if (target==null)
        {
            base.OnEnable();
        }
        else
        {
            StartCoroutine(projectileGuidanceSystem.HomingCorotine(target));
        }
    }

    #endregion
}
