using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色类
/// </summary>
public class Character : MonoBehaviour
{
    /// <summary>
    /// 死亡特效
    /// </summary>
    [Header("死亡特效")]
    [SerializeField] GameObject deathVFX;

    [Header("角色死亡时爆炸音效")]
    [SerializeField] AudioData[] deathSFX;

    /// <summary>
    /// 最大生命值
    /// </summary>
    [Header("最大生命值")]
    [SerializeField] protected float maxHealth;

    /// <summary>
    /// 头顶血条
    /// </summary>
    [Header("头顶血条")]
    [SerializeField]StatsBar onHeadHealthBar;

    /// <summary>
    /// 是否显示头顶血条
    /// </summary>
    [Header("是否显示头顶血条")]
    [SerializeField] bool showOnHeadHealthBar = true;


    /// <summary>
    /// 当前生命值
    /// </summary>
    protected float health;

    /// <summary>
    /// 开启头顶血条
    /// </summary>
    public void ShowOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(true);
        onHeadHealthBar.Initialized(health, maxHealth);
    }

    /// <summary>
    /// 关闭头顶血条
    /// </summary>
    public void HideOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        health = maxHealth;//对象每次被启用时生命值为最大生命值

        if (showOnHeadHealthBar)
        {
            ShowOnHeadHealthBar();
        }
        else
        {
            HideOnHeadHealthBar();
        }
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (showOnHeadHealthBar&& gameObject.activeSelf)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Die()
    {
        health = 0f;

        AudioManager.Instance.PlayRandomSFX(deathSFX);//播放爆炸音效
        PoolManager.Release(deathVFX, transform.position);//播放死亡特效
        gameObject.SetActive(false);//禁用该物体
    }

    /// <summary>
    /// 恢复生命值
    /// </summary>
    /// <param name="value">恢复的生命值</param>
    public virtual void RestoreHealth(float value)
    {
        #region 合一成下面一句
        //if (health == maxHealth) return;
        //health = Mathf.Clamp(health, 0f, maxHealth);
        #endregion
        health = Mathf.Clamp(health + value, 0f, maxHealth);

        if (showOnHeadHealthBar)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }
    }

    /// <summary>
    /// 持续恢复生命值
    /// </summary>
    /// <param name="waitTime">协程等待时间</param>
    /// <param name="percent">百分比值</param>
    /// <returns></returns>
    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime,float percent )
    {
        while (health <maxHealth)
        {
            yield return waitTime;
            RestoreHealth(maxHealth*percent);
        }
    }

    /// <summary>
    /// 持续受到伤害
    /// </summary>
    /// <param name="waitTime">协程等待时间</param>
    /// <param name="percent">百分比值</param>
    /// <returns></returns>
    protected IEnumerator DamageOverTimeCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (health > 0f)
        {
            yield return waitTime;
            TakeDamage(maxHealth * percent);
        }
    }
}
