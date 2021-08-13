using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingUI : SingletonMonoBehavior<RankingUI>
{
    RankingUIItem baseItem;

    protected override void OnInit()
    {
        baseItem = GetComponentInChildren<RankingUIItem>();
    }
    public void ShowRanking()
    {
        base.Show();

        // 랭킹을 보여주자
    }
}
