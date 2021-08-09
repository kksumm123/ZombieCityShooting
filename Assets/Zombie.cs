using DG.Tweening;
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
            StopCo(fsmHandle);

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

        yield return new WaitForSeconds(Random.Range(0.1f, 1f));

        CurrentFSM = ChaseFSM;

        isLive = true;

        while (isLive)
        { //���¸� ������ �ݺ��ؼ� �����ϴ� �κ�
            var previousFSM = CurrentFSM;

            fsmHandle = StartCoroutine(CurrentFSM());

            // FSM �ȿ��� ���� �߻��� ���� ���� ���� ���� �����ϱ� ���ؼ� �߰���
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
        {
            agent.destination = target.position;
            animator.SetTrigger("SetRun");
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
        else
            animator.SetTrigger("SetIdle");

        SetAttackOrChaseFSM();
    }

    private void SetAttackOrChaseFSM()
    {
        if (IsAttackableTarget())
        {
            // Ÿ���� ���ݹ��� �ȿ� ���Դ°�?
            if (TargetIsInAttackArea())
                CurrentFSM = AttackFSM;
            else
                CurrentFSM = ChaseFSM;
        }
        else
        {
            print("��ȸ�ϱ� �����ؾ���");
            // ���� ������ Ÿ�� ã��

            // ���� ������ Ÿ�� ���ٸ�
            // ��ȸ�ϱ�, Ȥ�� ���ڸ� ������ �ֱ�
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
        var enemyColliders = Physics.OverlapSphere(
            sphereCollider.transform.position, sphereCollider.radius, enemyLayer);
        foreach (var enemy in enemyColliders)
        {
            enemy.GetComponent<Player>().TakeHit(power);
        }

        // �ִϸ��̼� ���� ������ ���
        yield return new WaitForSeconds(attackAnimationTime - attackPreDelay);

        // �̵����ǵ� ����
        SetOriginalSpeed();

        // ���� �ĵ����̸�ŭ ���
        yield return new WaitForSeconds(attackPostDelay);

        // FSm ����
        SetAttackOrChaseFSM();
    }

    #endregion ChaseFSM

    #region TakeHit
    public void TakeHit(int damage, Transform attackerTr, float knockBackForce)
    {
        if (hp > 0)
        {
            base.TakeHit(damage);
            // �ڷ� �з�����
            KnockBackMove(attackerTr.forward, knockBackForce);
            // �ǰ� ����Ʈ ����(��)
            CreateBloodEffect(attackerTr.position);

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
            Die();
            yield break;
        }

        yield return new WaitForSeconds(TakeHitStopTime);
        SetOriginalSpeed();

        CurrentFSM = ChaseFSM;
    }
    [SerializeField] Material dieMaterial;
    [SerializeField] float dieMaterialDuration = 2;
    void Die()
    {
        isLive = false;
        StageManager.Instance.AddScore(rewardScore);

        animator.Play("Die");
        // ���׸��� ��ü
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var item in renderers)
        {
            item.sharedMaterial = dieMaterial;
        }
        //dieMaterial.SetFloat("_Progress", 1);
        // ��ü�Ǵ� ���� �����ְ� �ı�
        DOTween.To(() => 1f, (x) => dieMaterial.SetFloat("_Progress", x), 0.2f, dieMaterialDuration)
               .OnComplete(() => Destroy(gameObject));

    }
    #endregion TakeHit

    #region Methods

    [SerializeField] float knockBackNoise = 0.1f;
    [SerializeField] float knockBackForce = 0.5f;
    [SerializeField] float knockBackDuration = 0.5f;
    [SerializeField] Ease knockBackEase = Ease.OutExpo;
    void KnockBackMove(Vector3 toKnockBackDirection, float _knockBackForce)
    {
        toKnockBackDirection.x += Random.Range(-knockBackNoise, knockBackNoise);
        toKnockBackDirection.z += Random.Range(-knockBackNoise, knockBackNoise);
        toKnockBackDirection.y = 0;
        toKnockBackDirection.Normalize();
        transform.Translate(toKnockBackDirection * knockBackForce * _knockBackForce, Space.World);
        transform.DOMove(transform.position + toKnockBackDirection * knockBackForce * _knockBackForce, knockBackDuration)
                 .SetEase(knockBackEase);
    }

    private void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
    void SetOriginalSpeed()
    {
        agent.speed = originSpeed;
    }
    #endregion Methods
}
