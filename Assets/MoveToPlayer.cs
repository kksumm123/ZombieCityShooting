using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class MoveToPlayer : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] float maxSpeed = 20;
    [SerializeField] float duration = 3;
    bool isAttached = false;
    TweenerCore<float, float, FloatOptions> dotweenHandle;
    IEnumerator OnTriggerEnter(Collider other)
    {
        if (isAttached == false)
        {
            if (other.CompareTag("Player"))
            {
                isAttached = true;
                agent = GetComponent<NavMeshAgent>();
                dotweenHandle = DOTween.To(() => agent.speed, (x) => agent.speed = x, maxSpeed, duration);

                while (other != null)
                {
                    Debug.Log("2");
                    agent.destination = other.transform.position;
                    yield return null;
                }
            }
        }
    }
    void OnDestroy()
    {
        Debug.Log("1");
        StopAllCoroutines();
        dotweenHandle.Kill();
    }
}
