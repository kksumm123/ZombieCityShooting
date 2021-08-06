using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] Color dayColor;
    [SerializeField] Color nightColor;
    [ContextMenu("DayLight�� �����ϱ�")]
    void SetDayLight()
    {
        // �𷺼ų� ����Ʈ�� �ѱ�
        // Light�� ������ �ִ� ��� ������Ʈ Ž��
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
    [ContextMenu("NightLight�� �����ϱ�")]
    void SetNightLight()
    {
        // �𷺼ų� ����Ʈ�� ����
        // Light�� ������ �ִ� ��� ������Ʈ Ž��
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

        // �㿡�� ������ ���� -> �𷺼ų� ����Ʈ ���� ���, �ٸ� ����Ʈ�� ���� ��Ӱ�
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

        // ������ ������ ���� -> �𷺼ų� ����Ʈ ���� ���, �ٸ� ����Ʈ�� ���� ���
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
