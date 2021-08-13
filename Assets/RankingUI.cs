using System;
using System.Collections;
using System.Collections.Generic;
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
            ranking = savedData.ranking;
    }
    public List<int> ranking = new List<int>();
    public int myInt;
    public int myString;
}

public class RankingUI : SingletonMonoBehavior<RankingUI>
{
    //�׽�Ʈ ����
    public int tempScore = 100;
    [ContextMenu("���� �߰�")]
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

        // ��ŷ�� ��������

        // 10�� ������ ����
        // �̸��̸� ���ϱ⸸
        if (rankingData.ranking.Count > maxCount)
        {
            int minScore = rankingData.ranking[rankingData.ranking.Count - 1];
            if (minScore < currentScore)
            {
                rankingData.ranking.Add(currentScore);
                rankingData.ranking.Sort();
                rankingData.ranking.RemoveAt(rankingData.ranking.Count - 1);
            }
        }
        else
        {
            rankingData.ranking.Add(currentScore);
        }

        rankingData.SaveData();
    }
}
