using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToPlayer : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] float maxSpeed = 20;
    [SerializeField] float duration = 3;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //agent.speed
        }
    }
}
