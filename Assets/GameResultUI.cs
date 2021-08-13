using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUI : SingletonMonoBehavior<GameResultUI>
{

    Image star;
    TextMeshProUGUI yourScoreValue;
    TextMeshProUGUI highScoreValue;
    protected override void OnInit()
    {
        star = transform.Find("Star").GetComponent<Image>();
        yourScoreValue = transform.Find("YourScore/Value").GetComponent<TextMeshProUGUI>();
        highScoreValue = transform.Find("HighScore/Value").GetComponent<TextMeshProUGUI>();
    }
}
