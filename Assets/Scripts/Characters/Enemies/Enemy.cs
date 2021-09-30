using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    /// <summary>
    /// 敌人死亡奖励能量
    /// </summary>
    [Header("敌人死亡奖励能量")]
    [SerializeField]int deathEnergyBonus = 3;

    public override void Die()
    {
        PlayerEnergy.Instance.Obtain(deathEnergyBonus);
        EnemyManager.Instance.RemoveFromList(gameObject);
        base.Die();
    }
}
