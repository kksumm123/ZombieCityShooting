using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Zombie : Actor
{
    NavMeshAgent agent;
    Transform target;
    SphereCollider sphereCollider;

    [SerializeField] int rewardScore = 100;
    [SerializeField] int power = 20;
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

    bool isLive = false;
    new IEnumerator Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        originSpeed = agent.speed;
        target = FindObjectOfType<Player>().transform;
        sphereCollider = GetComponentInChildren<SphereCollider>(true);
        enemyLayer = 1 << LayerMask.NameToLayer("Player");

        CurrentFSM = ChaseFSM;

        isLive = true;

        while (isLive)
        { //상태를 무한히 반복해서 실행하는 부분
            var previousFSM = CurrentFSM;

            fsmHandle = StartCoroutine(CurrentFSM());

            // FSM 안에서 에러 발생시 무한 루프 도는 것을 방지하기 위해서 추가함
            if (fsmHandle == null && previousFSM == CurrentFSM)
                yield return null;

            while (fsmHandle != null)
                yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    #region ChaseFSM
    float attackDistance = 1;
    IEnumerator ChaseFSM()
    {
        if (target)
            agent.destination = target.position;
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        SetAttackOrChaseFSM();
    }

    private void SetAttackOrChaseFSM()
    {
        if (IsAttackableTarget())
        {
            // 타겟이 공격범위 안에 들어왔는가?
            if (TargetIsInAttackArea())
                CurrentFSM = AttackFSM;
            else
                CurrentFSM = ChaseFSM;
        }
        else
        {
            print("배회하기 구현해야함");
            // 공격 가능한 타겟 찾기

            // 공격 가능한 타겟 없다면
            // 배회하기, 혹은 제자리 가만히 있기
        }
    }

    bool IsAttackableTarget()
    {
        if (target.GetComponent<Player>().State == Player.StateType.Die)
            return false;
        return true;
    }

    private bool TargetIsInAttackArea()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance < attackDistance;
    }

    float attackPreDelay = 0.4f;
    float attackAnimationTime = 0.8f;
    float attackPostDelay = 1f;
    [SerializeField] LayerMask enemyLayer;
    IEnumerator AttackFSM()
    {
        // 타겟 바라보기
        var lookAtPosition = target.position;
        lookAtPosition.y = transform.position.y;
        transform.LookAt(lookAtPosition);

        // 공격 애니메이션 하기
        animator.SetTrigger("Attack");

        // 이동 스피드 0
        agent.speed = 0;

        // 공격 타이밍까지 대기(특정 시간 지나면)
        yield return new WaitForSeconds(attackPreDelay);

        // 충돌메시 사용 충돌감지
        var enemyColliders = Physics.OverlapSphere(
            sphereCollider.transform.position, sphereCollider.radius, enemyLayer);
        foreach (var enemy in enemyColliders)
        {
            enemy.GetComponent<Player>().TakeHit(power);
        }

        // 애니메이션 끝날 때까지 대기
        yield return new WaitForSeconds(attackAnimationTime - attackPreDelay);

        // 이동스피드 복구
        SetOriginalSpeed();

        // 공격 후딜레이만큼 대기
        yield return new WaitForSeconds(attackPostDelay);

        // FSm 지정
        SetAttackOrChaseFSM();
    }

    #endregion ChaseFSM

    #region TakeHit
    public void TakeHit(int damage, Transform bulletTr)
    {
        if (hp > 0)
        {
            StopCo(fsmHandle);

            hp -= damage;
            // 뒤로 밀려나게
            KnockBackMove(bulletTr.forward);
            // 피격 이펙트 생성(피)
            CreateBloodEffect(bulletTr.position);
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
            //피격 모션 대기
            yield return new WaitForSeconds(1);
            Die();
            yield break;
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
        isLive = false;
        StageManager.Instance.AddScore(rewardScore);
        animator.Play("Die");
        Destroy(gameObject, 2);
    }
    #endregion TakeHit

    #region Methods

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

    private void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }

    #endregion Methods
}
