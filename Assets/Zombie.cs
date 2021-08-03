using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Zombie : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    Transform target;

    [SerializeField] int hp = 100;
    float originSpeed;

    Coroutine fsmHandle;
    Func<IEnumerator> m_currentFsm;
    Func<IEnumerator> CurrentFSM
    {
        get { return m_currentFsm; }
        set
        {
            m_currentFsm = value;
            fsmHandle = null;
        }
    }
    IEnumerator Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        originSpeed = agent.speed;
        target = FindObjectOfType<Player>().transform;
        bloodParticle = (GameObject)Resources.Load("BloodParticle");

        CurrentFSM = ChaseFSM;

        while (true) // 상태를 무한히 반복해서 실행하는 부분.
        {
            fsmHandle = StartCoroutine(CurrentFSM());
            while (fsmHandle != null)
                yield return null;
        }
    }

    #region ChaseFSM
    [SerializeField] float attackDistance = 3;
    IEnumerator ChaseFSM()
    {
        if (target)
            agent.destination = target.position;
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        // 타겟이 공격범위 안에 들어왔는가?
        if (TargetIsInAttackArea())
            CurrentFSM = AttackFSM;
        else
            CurrentFSM = ChaseFSM;
    }

    private bool TargetIsInAttackArea()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance < attackDistance;
    }
    private IEnumerator AttackFSM()
    {
        // 공격 애니메이션 하기
        // 이동 스피드 0
        // 특정시간 지나면 충돌메시 사용 충돌감지
        // 애니메이션 끝날 때까지 대기
        // 이동스피드 복구
        // FSm 지정
    }

    #endregion ChaseFSM

    #region TakeHit
    public void TakeHit(int damage, Transform bulletTr)
    {
        if (hp > 0)
        {
            StopCoroutine(fsmHandle);

            hp -= damage;
            // 뒤로 밀려나게
            KnockBackMove(bulletTr.forward);
            // 피격 이펙트 생성(피)
            CreateBloodEffect(bulletTr);
            CurrentFSM = TakeHitFSM;
        }
    }

    float TakeHitStopTime = 0.1f;
    IEnumerator TakeHitFSM()
    {
        animator.Play($"TakeHit{Random.Range(1, 3)}");

        // 이동 스피드를 잠시 0으로
        agent.speed = 0;

        if (hp <= 0)
        {
            GetComponent<Collider>().enabled = false;
            yield return new WaitForSeconds(2);
            Die();
        }

        yield return new WaitForSeconds(TakeHitStopTime);
        SetOriginalSpeed();

        CurrentFSM = ChaseFSM;
    }

    void SetOriginalSpeed()
    {
        agent.speed = originSpeed;
    }

    void Die()
    {
        animator.Play("Die");
        Destroy(gameObject, 2);
    }
    #endregion TakeHit

    #region Methods
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
    #endregion Methods
}
