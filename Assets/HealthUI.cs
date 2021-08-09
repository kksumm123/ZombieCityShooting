using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : SingletonMonoBehavior<HealthUI>
{
    TextMeshProUGUI valueText;
    public Image[] images;
    public Sprite enable, current, disable;
    protected override void OnInit()
    {
        valueText = transform.Find("ValueText").GetComponent<TextMeshProUGUI>();
    }

    internal void SetHP(int hp, int maxHp)
    {
        valueText.text = $"{hp} / {maxHp}";
        float percent = (float)hp / maxHp;
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
