using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RankingData : PlayerPrefsData<RankingData>
{
    public RankingData(string key) : base(key)
    {
        var saveData = LoadData();
        ranking = saveData.ranking;
    }
    public List<int> ranking = new List<int>();
    public int myInt;
    public int myString;
}

public class RankingUI : SingletonMonoBehavior<RankingUI>
{
    //테스트 로직
    public int tempScore = 100;
    [ContextMenu("점수 추가")]
    void TestFn()
    {
        InsertScore();
    }
    public static void InsertScore()
    {
        Instance.ShowRanking(Instance.tempScore);
    }

    RankingData rankingData;
    RankingUIItem baseItem;

    protected override void OnInit()
    {
        rankingData = new RankingData("RankingData");
        baseItem = GetComponentInChildren<RankingUIItem>();
    }

    int maxCount = 10;
    public void ShowRanking(int currentScore)
    {
        base.Show();

        // 랭킹을 보여주자
        int minScore = 0;

        if (rankingData.ranking.Count > 0)
            minScore = rankingData.ranking[rankingData.ranking.Count - 1];
        if (minScore < currentScore)
        {
            rankingData.ranking.Add(currentScore);
            rankingData.ranking.Sort();
            if (rankingData.ranking.Count > maxCount)
                rankingData.ranking.RemoveRange(maxCount, rankingData.ranking.Count - maxCount);
            rankingData.SaveData();
        }
    }
}
