using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultUI : SingletonMonoBehavior<GameResultUI>
{
    [System.Serializable]
    public class StarScore
    {
        public int minScore;
        public Sprite sprite;
    }
    [SerializeField] List<StarScore> starScores; 

    Image star;
    TextMeshProUGUI yourScoreValue;
    TextMeshProUGUI highScoreValue;
    protected override void OnInit()
    {
        star = transform.Find("Star").GetComponent<Image>();
        yourScoreValue = transform.Find("YourScore/Value").GetComponent<TextMeshProUGUI>();
        highScoreValue = transform.Find("HighScore/Value").GetComponent<TextMeshProUGUI>();
        

        transform.Find("MeneButotn/Restart").GetComponent<Button>().AddListener(this, OnClickReStart);
        transform.Find("MeneButotn/Home").GetComponent<Button>().AddListener(this, OnClickHome);
        transform.Find("MeneButotn/Ranking").GetComponent<Button>().AddListener(this, OnClickRanking);
    }

    void OnClickReStart()
    {
        SceneManager.LoadScene("Stage1");
    }

    void OnClickHome()
    {
        SceneManager.LoadScene("Title");
    }

    void OnClickRanking()
    {
        RankingUI.Instance.ShowRanking(Convert.ToInt32(yourScoreValue.text));
    }

    public void ShowResult(int score, SaveInt highScore)
    {
        base.Show();
        yourScoreValue.text = score.ToString();
        highScoreValue.text = highScore.ToString();
        star.sprite = GetStarSprite(score);
    }

    Sprite GetStarSprite(int score)
    {
        foreach (var item in starScores)
        {
            if (item.minScore > score)
                return item.sprite;
        }
        return starScores[starScores.Count - 1].sprite;
    }
}
