using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class MoveToPlayer : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] float maxSpeed = 20;
    [SerializeField] float duration = 3;
    bool isAttached = false;

    IEnumerator OnTriggerEnter(Collider other)
    {
        if (isAttached == false)
        {
            if (other.CompareTag("Player"))
            {
                isAttached = true;
                agent = GetComponent<NavMeshAgent>();
                DOTween.To(() => agent.speed, (x) => agent.speed = x, maxSpeed, duration);

                while (other != null)
                {
                    agent.destination = other.transform.position;
                    yield return null;
                }
            }
        }
    }
}
