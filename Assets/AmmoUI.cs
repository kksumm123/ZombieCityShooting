using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoUI : GaugeUI<AmmoUI>
{
    internal void SetBulletCount(int bulletCountInClip, int maxBulletCountInClip, int allBuletCount, int maxBulletCount)
    {
        SetGauge(bulletCountInClip, maxBulletCountInClip);
        valueText.text = $"{allBuletCount} / {maxBulletCount}";
    }
}
