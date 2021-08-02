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
        bloodParticle = (GameObject)Resources.Load("BloodParticle");

        while (hp > 0)
        {
            if (target)
                agent.destination = target.position;
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            // 여기서 걸려서 idle이 되는거같아요 
        }
    }

    public void TakeHit(int damage, Transform bulletTr)
    {
        hp -= damage;
        animator.Play($"TakeHit{Random.Range(1, 3)}");
        // 피격 이펙트 생성(피)
        CreateBloodEffect(bulletTr);

        // 뒤로 밀려나게
        KnockBackMove(bulletTr.forward);

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

    GameObject bloodParticle;
    void CreateBloodEffect(Transform bulletTr)
    {
        Instantiate(bloodParticle, bulletTr.position, Quaternion.identity);
    }

    [SerializeField] float knockBackNoise = 0.1f;
    [SerializeField] float knockBackDistance = 0.1f;
    void KnockBackMove(Vector3 toKnockBackDirection)
    {
        toKnockBackDirection.x += Random.Range(-knockBackNoise, knockBackNoise);
        toKnockBackDirection.z += Random.Range(-knockBackNoise, knockBackNoise);
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
