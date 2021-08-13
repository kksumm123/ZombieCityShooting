using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingUIItem : MonoBehaviour
{
    Image icon;
    TextMeshProUGUI scoreValue;
    Button button;

    void Start()
    {
        icon = transform.Find("Icon").GetComponent<Image>();
        scoreValue = transform.Find("ScoreValue").GetComponent<TextMeshProUGUI>();
        button = transform.Find("Button").GetComponent<Button>();
    }
}