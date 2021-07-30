using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    public Transform target;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        target = FindObjectOfType<Player>().transform;
    }
    void Update()
    {
        if (target)
        {
            agent.SetDestination(target.position);
            animator.Play("Run");
        }
    }
}
