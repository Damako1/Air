using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : Singleton<PlayerEnergy>
{
    #region 变量

    [Header("能量消耗时间")]
    [SerializeField] float overdriveInterval = 0.1f;

    /// <summary>
    /// 能量条
    /// </summary>
    [Header("能量条")]
    [SerializeField] EnergyBar energyBar;

    /// <summary>
    /// 能量最大值
    /// </summary>
    public const int MAX = 100;
    /// <summary>
    /// 百分比值
    /// </summary>
    public const int PERCENT = 1;

    /// <summary>
    /// 当前能量值
    /// </summary>
    int energy;

    WaitForSeconds waitForOverdriveInterval;

    //是否能获取能量
    bool available = true;
    #endregion



    #region 生命周期
    protected override void Awake()
    {
        base.Awake();
        waitForOverdriveInterval = new WaitForSeconds(overdriveInterval);
    }

    private void OnEnable()
    {
        PlayerOverdrive.on += PlayerOverdriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    private void Start()
    {
        energyBar.Initialized(energy, MAX);
    }

    private void OnDisable()
    {
        PlayerOverdrive.on -= PlayerOverdriveOn;
        PlayerOverdrive.off -= PlayerOverdriveOff;
    }

    #endregion

    #region 方法

    /// <summary>
    /// 获取能量
    /// </summary>
    /// <param name="value"></param>
    public void Obtain(int value)
    {
        if (energy == MAX|| !available||!gameObject.activeSelf) return;

        energy=Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateStats(energy, MAX);
    }
    /// <summary>
    /// 使用能量
    /// </summary>
    /// <param name="value"></param>
    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateStats(energy, MAX);

        if (energy==0&&!available)
        {
            PlayerOverdrive.off.Invoke();
        }
    }

    /// <summary>
    /// 判断当前能量值是否足够支付消耗能量值
    /// </summary>
    public bool IsEnough(int value)
    {
        return energy >= value;
    }

    private void PlayerOverdriveOn()
    {
        available = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }

    private void PlayerOverdriveOff()
    {
        available = true;
        StopCoroutine(nameof(KeepUsingCoroutine));
    }


    IEnumerator KeepUsingCoroutine()
    {
        while (gameObject.activeSelf&&energy>0)
        {
            //每经过0.1秒
            yield return waitForOverdriveInterval;

            //消耗1%的能量
            Use(PERCENT);

        }
    }
    #endregion

}
