using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar_HDR : StatsBar
{

    [SerializeField]Text percentText;

    void SetPercentText()
    {
        percentText.text = Mathf.RoundToInt(targetFillAmount * 100f) + "%";
    }

    public override void Initialized(float currentValue, float maxValue)
    {
        base.Initialized(currentValue, maxValue);
        SetPercentText();
    }

    protected override IEnumerator BufferedFillingCorountine(Image image)
    {
        SetPercentText();
        return base.BufferedFillingCorountine(image);
    }
}
