using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    public Transform target;

    [SerializeField] int hp = 100;
    IEnumerator Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        target = FindObjectOfType<Player>().transform;

        while (hp > 0)
        {
            if (target)
                agent.destination = target.position;
            animator.Play("Run");
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }
}
