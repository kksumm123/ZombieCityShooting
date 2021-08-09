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
}
