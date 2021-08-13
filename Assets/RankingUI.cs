using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerPrefsData<T>
{
    public PlayerPrefsData(string _key)
    {
        key = _key;
    }

    string key;
    public T LoadData()
    {
        T record = JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
        if (record == null)
        {
            Debug.LogWarning("record == null");
            return default(T);
        }

        Debug.LogWarning("Load Complete");
        return record;
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(this);

        try
        {
            PlayerPrefs.SetString(key, json);
            Debug.Log("json:" + json);
        }
        catch (System.Exception err)
        {
            Debug.Log("Got: " + err);
        }
    }
}

[System.Serializable]
public class RankingData : PlayerPrefsData<RankingData>
{
    public RankingData(string key) : base(key)
    {
        var savedData = LoadData();
        if (savedData != null)
        {
            ranking = savedData.ranking;
        }
    }
    public List<int> ranking = new List<int>();
}

public class RankingUI : SingletonMonoBehavior<RankingUI>
{
    // 테스트 로직.
    public int tempScore = 100;
    [ContextMenu("점수 추가")]
    public void InsertScore()
    {
        ShowRanking(tempScore);
    }

    public RankingData rankingData;

    RankingUIItem baseItem;
    protected override void OnInit()
    {
        baseItem = GetComponentInChildren<RankingUIItem>();
        baseItem.gameObject.SetActive(false);
        rankingData = new RankingData("NewRanking");
    }

    public int maxCount = 10;
    internal void ShowRanking(int currentScore)
    {
        base.Show();

        //랭킹을 보여주자.

        // -> 10개 이상일때 마지막껄 빼고 
        // 10개 미만이면 더하기만 하자.
        InsertScore(currentScore);

        ShowRankingUI();
    }

    private void ShowRankingUI()
    {
        baseItem.gameObject.SetActive(true);
        foreach (var item in rankingData.ranking)
        {
            var newItem = Instantiate(baseItem, baseItem.transform.parent);
            newItem.SetData(item);
            ChildObject.Add(newItem.gameObject);
        }
        baseItem.gameObject.SetActive(false);
    }

    private void InsertScore(int currentScore)
    {
        if (rankingData.ranking.Count >= 10)
        {
            int minScore = rankingData.ranking[rankingData.ranking.Count - 1];

            if (minScore < currentScore)
            {
                rankingData.ranking.Add(currentScore);
                rankingData.ranking.Sort();
                rankingData.ranking.Reverse(); // 정렬된걸 뒤집는다.
                rankingData.ranking.RemoveAt(rankingData.ranking.Count - 1);
                rankingData.SaveData();
            }
        }
        else
        {
            rankingData.ranking.Add(currentScore);
            rankingData.SaveData();
        }
    }
}