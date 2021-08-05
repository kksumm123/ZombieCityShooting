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
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent = GetComponent<NavMeshAgent>();

            DOTween.To(() => 1f, (x) => agent.speed = x, maxSpeed, duration);
        }
    }
}
