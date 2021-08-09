using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : GaugeUI<HealthUI>
{

}
public class GaugeUI<T> : SingletonMonoBehavior<T> where T : SingletonBase
{
    protected TextMeshProUGUI valueText;
    public Image[] images;
    public Sprite enable, current, disable;
    protected override void OnInit()
    {
        valueText = transform.Find("ValueText").GetComponent<TextMeshProUGUI>();
    }

    internal void SetGauge(int value, int maxValue)
    {
        valueText.text = $"{value} / {maxValue}";
        float percent = (float)value / maxValue;
        int currentCount = Mathf.RoundToInt(percent * images.Length);
        for (int i = 0; i < images.Length; i++)
        {
            if (i == currentCount - 1)
                images[i].sprite = current;
            else if (i < currentCount - 1)
                images[i].sprite = enable;
            else
                images[i].sprite = disable;
        }
    }
}
