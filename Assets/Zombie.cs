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
    SphereCollider sphereCollider;

    [SerializeField] int hp = 100;
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
    IEnumerator Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        originSpeed = agent.speed;
        target = FindObjectOfType<Player>().transform;
        sphereCollider = GetComponentInChildren<SphereCollider>(true);
        bloodParticle = (GameObject)Resources.Load("BloodParticle");
        enemyLayer = 1 << LayerMask.NameToLayer("Player");

        CurrentFSM = ChaseFSM;

        while (true) // ���¸� ������ �ݺ��ؼ� �����ϴ� �κ�.
        {
            fsmHandle = StartCoroutine(CurrentFSM());
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
        var endTime = Time.time + Random.Range(0.5f, 2f);
        while (Time.time < endTime)
        {
            // Ÿ���� ���ݹ��� �ȿ� ���Դ°�?
            if (TargetIsInAttackArea())
                CurrentFSM = AttackFSM;

            yield return null;
        }

        CurrentFSM = ChaseFSM;
    }

    private bool TargetIsInAttackArea()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance < attackDistance;
    }

    float attackPreDelay = 0.4f;
    float attackAnimationTime = 0.8f;
    Collider[] enemyColliders;
    [SerializeField] LayerMask enemyLayer;
    private IEnumerator AttackFSM()
    {
        // Ÿ�� �ٶ󺸱�
        var lookAtPosition = target.position;
        lookAtPosition.y = transform.position.y;
        transform.LookAt(lookAtPosition);

        // ���� �ִϸ��̼� �ϱ�
        animator.SetTrigger("Attack");

        // �̵� ���ǵ� 0
        agent.speed = 0;

        // ���� Ÿ�ֱ̹��� ���(Ư�� �ð� ������)
        yield return new WaitForSeconds(attackPreDelay);

        // �浹�޽� ��� �浹����
        enemyColliders = Physics.OverlapSphere(
            sphereCollider.transform.position
            , sphereCollider.radius, enemyLayer);
        foreach (var enemy in enemyColliders)
        {
            enemy.GetComponent<Player>().TakeHit(power);
        }

        // �ִϸ��̼� ���� ������ ���
        yield return new WaitForSeconds(attackAnimationTime - attackPreDelay);

        // �̵����ǵ� ����
        SetOriginalSpeed();

        // FSm ����
        CurrentFSM = ChaseFSM;
    }

    #endregion ChaseFSM

    #region TakeHit
    public void TakeHit(int damage, Transform bulletTr)
    {
        if (hp > 0)
        {
            StopCoroutine(fsmHandle);

            hp -= damage;
            // �ڷ� �з�����
            KnockBackMove(bulletTr.forward);
            // �ǰ� ����Ʈ ����(��)
            CreateBloodEffect(bulletTr);
            CurrentFSM = TakeHitFSM;
        }
    }

    float TakeHitStopTime = 0.1f;
    IEnumerator TakeHitFSM()
    {
        animator.Play($"TakeHit{Random.Range(1, 3)}");

        // �̵� ���ǵ带 ��� 0����
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
