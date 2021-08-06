using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] Color dayColor;
    [SerializeField] Color nightColor;
    [ContextMenu("DayLight로 설정하기")]
    void SetDayLight()
    {
        // 디렉셔널 라이트만 켜기
        // Light를 가지고 있는 모든 오브젝트 탐색
        var allLights = FindObjectsOfType<Light>();
        foreach (var item in allLights)
        {
            if (item.type == LightType.Directional)
                item.enabled = true;
            else
                item.enabled = false;
        }
        RenderSettings.ambientLight = dayColor;
    }
    [ContextMenu("NightLight로 설정하기")]
    void SetNightLight()
    {
        // 디렉셔널 라이트만 끄기
        // Light를 가지고 있는 모든 오브젝트 탐색
        var allLights = FindObjectsOfType<Light>();
        foreach (var item in allLights)
        {
            if (item.type == LightType.Directional)
                item.enabled = false;
            else
                item.enabled = true;
        }
        RenderSettings.ambientLight = nightColor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
            ChangeDayLight();

        if (Input.GetKeyDown(KeyCode.Alpha6))
            ChangeNightLight();
    }

    Dictionary<Light, float> allLightMap; //= new Dictionary<Light, float>();
    [SerializeField] float changeDuration = 3;
    void ChangeDayLight()
    {
        if (allLightMap == null)
        {
            var _allLights = FindObjectsOfType<Light>();
            foreach (var item in _allLights)
            {
                allLightMap[item] = item.intensity;
            }
        }

        // 밤에서 낮으로 변함 -> 디렉셔널 라이트 점점 밝게, 다른 라이트는 점점 어둡게
        foreach (var item in allLightMap)
        {
            item.Key.enabled = true;
            if (item.Key.type == LightType.Directional)
                DOTween.To(() => 0, (x) => item.Key.intensity = x, item.Value, changeDuration)
                       .SetLink(gameObject);
            else
                DOTween.To(() => item.Value, (x) => item.Key.intensity = x, 0, changeDuration)
                       .SetLink(gameObject);
        }
    }

    void ChangeNightLight()
    {
        if (allLightMap == null)
        {
            var _allLights = FindObjectsOfType<Light>();
            foreach (var item in _allLights)
            {
                allLightMap[item] = item.intensity;
            }
        }

        // 낮에서 밤으로 변함 -> 디렉셔널 라이트 점점 어둡, 다른 라이트는 점점 밝게
        foreach (var item in allLightMap)
        {
            item.Key.enabled = true;
            if (item.Key.type == LightType.Directional)
                DOTween.To(() => item.Value, (x) => item.Key.intensity = x, 0, changeDuration)
                       .SetLink(gameObject);
            else
                DOTween.To(() => 0, (x) => item.Key.intensity = x, item.Value, changeDuration)
                       .SetLink(gameObject);
        }
    }
}
