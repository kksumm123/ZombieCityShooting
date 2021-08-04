using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : SingletonMonoBehavior<ScoreUI>
{

    TextMeshProUGUI scoreText;
    TextMeshProUGUI highScoreText;
    protected override void OnInit()
    {
        scoreText = transform.Find("Value").GetComponent<TextMeshProUGUI>();
        highScoreText = transform.Find("HighValue").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateUI(int score, int highScore)
    {
        scoreText.text = score.ToNumber();
        highScoreText.text = "High : " + highScore.ToNumber();
    }
}
