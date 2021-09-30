using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 状态条基类
public class StatsBar : MonoBehaviour
{
    #region 变量

    [Header("后面的填充图片")]
    [SerializeField] Image fillImageBack;

    [Header("前面的填充图片")]
    [SerializeField] Image fillImageFront;

    [Header("状态条的填充速度")]
    [SerializeField] float fillSpeed = 0.1f;

    [Header("延迟填充时间")]
    [SerializeField] float fillDelay = 0.5f;

    [Header("是否需要延迟填充")]
    [SerializeField] bool delayFill = true;

    float currentFillAmount;

    // 目标填充值
    protected float targetFillAmount;

    // 线性插值的第三参数
    float t;

    // 等待延迟填充时间
    WaitForSeconds waitForDelayFill;

    // 延迟填充协程暂时变量
    Coroutine bufferedFillingCorountine;

    Canvas canvas;


    #endregion

    #region 生命周期

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    #endregion

    #region 方法

    /// <summary>
    /// 初始化状态条
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    public virtual void Initialized(float currentValue, float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    /// <summary>
    /// 更新状态条
    /// </summary>
    public void UpdateStats(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;

        //避免协程重复开启
        if (bufferedFillingCorountine != null)
        {
            StopCoroutine(bufferedFillingCorountine);
        }

        //如果状态减少
        if (currentFillAmount > targetFillAmount)
        {
            //让前面的imageAmount立刻变为目标填充值
            fillImageFront.fillAmount = targetFillAmount;
            //让后面的imageAmount慢慢减少 
            bufferedFillingCorountine = StartCoroutine(BufferedFillingCorountine(fillImageBack));
        }

        //如果状态增加
        if (currentFillAmount < targetFillAmount)
        {
            //让后面的imageAmount立刻变为目标填充值
            fillImageBack.fillAmount = targetFillAmount;
            //让前面的imageAmount慢慢增加
            bufferedFillingCorountine = StartCoroutine(BufferedFillingCorountine(fillImageFront));
        }
    }

    /// <summary>
    /// 缓慢填充协程
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    protected virtual IEnumerator BufferedFillingCorountine(Image image)
    {
        if (delayFill)
        {
            yield return waitForDelayFill;
        }

        t = 0f;

        while (t < 1f)
        {
            var previousFillAmout = currentFillAmount;
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(previousFillAmout, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }

    }



    #endregion

}
