using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonoBehavior<StageManager>
{
    public SaveInt gold;
    public SaveInt highScore;
    public int score;

    new void Awake()
    {
        base.Awake();
        highScore = new SaveInt("highScore");
        ScoreUIRefresh();
        GoldUIRefresh();
    }


    public void AddScore(int addScore)
    {
        score += addScore;
        if (highScore.Value < score)
        {
            highScore.Value = score;
        }

        ScoreUIRefresh();
    }

    void ScoreUIRefresh()
    {
        ScoreUI.Instance.UpdateUI(score, highScore.Value);
    }

    public void AddGold(int value)
    {
        gold += value;
        GoldUIRefresh();
    }

    void GoldUIRefresh()
    {
        GoldUI.Instance.UpdateUI(gold);
    }
}
