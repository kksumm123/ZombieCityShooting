using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    Transform target;

    [SerializeField] int hp = 100;
    float originSpeed;
    IEnumerator Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        originSpeed = agent.speed;
        target = FindObjectOfType<Player>().transform;

        while (hp > 0)
        {
            if (target)
                agent.destination = target.position;
            animator.Play("Run");
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }

    public void TakeHit(int damage, Vector3 toKnockBackDirection)
    {
        hp -= damage;
        animator.Play($"TakeHit{Random.Range(1, 3)}");
        // 피격 이펙트 생성(피)

        // 뒤로 밀려나게
        KnockBackMove(toKnockBackDirection);

        // 이동 스피드를 잠시 0으로
        agent.speed = 0;
        CancelInvoke(nameof(SetTakeHitSpeedCo));
        Invoke(nameof(SetTakeHitSpeedCo), TakeHitStopTime);

        if (hp <= 0)
        {
            GetComponent<Collider>().enabled = false;
            Invoke(nameof(Die), 1);
        }
    }

    [SerializeField] float knockBackDistance = 0.1f;
    void KnockBackMove(Vector3 toKnockBackDirection)
    {
        toKnockBackDirection.y = 0;
        toKnockBackDirection.Normalize();
        transform.Translate(toKnockBackDirection * knockBackDistance, Space.World);
    }

    float TakeHitStopTime = 0.1f;
    void SetTakeHitSpeedCo()
    {
        agent.speed = originSpeed;
    }

    void Die()
    {
        animator.Play("Die");
        Destroy(gameObject, 1);
    }
}
