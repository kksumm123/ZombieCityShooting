using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public partial class Player : Actor
{
    CapsuleCollider capsuleCol;
    Transform bulletSpawnPosition;
    [SerializeField] WeaponInfo currentWeapon;
    [SerializeField] Transform rightWeaponPosition;

    [SerializeField] float speed = 5;
    [SerializeField] float shootingSpeed = 2.5f;
    new void Start()
    {
        base.Start();
        hp = 300;

        capsuleCol = GetComponent<CapsuleCollider>();
        animator.runtimeAnimatorController = currentWeapon.overrideController;
        var weaponInfo = Instantiate(currentWeapon, rightWeaponPosition);
        weaponInfo.transform.localPosition = currentWeapon.gameObject.transform.localPosition;
        weaponInfo.transform.localRotation = currentWeapon.gameObject.transform.localRotation;
        weaponInfo.transform.localScale = currentWeapon.gameObject.transform.localScale;
        currentWeapon = weaponInfo;
        if (currentWeapon.attackCollider != null)
            currentWeapon.attackCollider.enabled = false;

        bullet = (GameObject)Resources.Load(bulletString);
        //bulletSpawnPosition = GameObject.Find("BulletSpawnPosition").transform;
        //bulletLight = GetComponentInChildren<Light>(true).gameObject;
        bulletSpawnPosition = currentWeapon.bulletSpawnPosition;
        if (currentWeapon.bulletLight != null)
            bulletLight = currentWeapon.bulletLight.gameObject;
        shootDelay = currentWeapon.delay;

        var vcs = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var item in vcs)
        {
            item.Follow = transform;
            item.LookAt = transform;
        }
    }

    void Update()
    {
        if (State != StateType.Hit && State != StateType.Die)
        {
            if (State != StateType.Roll)
            {
                LookAtMouse();
                Move();
                Fire();
                Roll();
            }
        }
    }

    Plane plane = new Plane(new Vector3(0, 1, 0), 0);
    void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = hitPoint - transform.position;
            dir.y = transform.position.y;
            dir.Normalize();
            RotationSlerp(dir);
        }
    }
    #region Move
    Vector3 move;
    void Move()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move.z = 1;
        if (Input.GetKey(KeyCode.S)) move.z = -1;
        if (Input.GetKey(KeyCode.A)) move.x = -1;
        if (Input.GetKey(KeyCode.D)) move.x = 1;

        if (move != Vector3.zero)
        {
            Vector3 relateMove = Vector3.zero;
            // forward * move.z �Ҵ�
            relateMove = Camera.main.transform.forward * move.z;
            // right * move.x �Ҵ� (+= ���ִ� ������ forward �� ���� �����ֱ� ����)
            relateMove += Camera.main.transform.right * move.x;
            relateMove.y = 0;
            move = relateMove;

            move.Normalize();

            transform.Translate(
                (isFiring == true ? shootingSpeed : speed)
                * rollingSpeedMult * Time.deltaTime * move, Space.World);
            State = StateType.Run;

            if (Mathf.RoundToInt(transform.forward.x) == 1 || Mathf.RoundToInt(transform.forward.x) == -1)
            {
                animator.SetFloat("DirX", transform.forward.z * move.z);
                animator.SetFloat("DirY", transform.forward.x * move.x);
            }
            else
            {
                animator.SetFloat("DirX", transform.forward.x * move.x);
                animator.SetFloat("DirY", transform.forward.z * move.z);
            }
        }
        else
            State = StateType.Idle;

        animator.SetFloat("Speed", move.sqrMagnitude);
    }
    #endregion Move

    #region Roll
    void Roll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(RollCo());
    }

    [SerializeField] AnimationCurve rollingSpeedAC;
    float rollingSpeedMult = 1;
    [SerializeField] float rollingSpeedUserMult = 1;
    IEnumerator RollCo()
    {
        animator.SetBool("Fire", false);

        // ������ �ִϸ��̼� ����
        State = StateType.Roll;
        animator.SetTrigger("Roll");

        // ������ ���� �̵� ���ǵ� ������
        float startTime = Time.time;
        float endTime = startTime + rollingSpeedAC[rollingSpeedAC.length - 1].time;
        while (Time.time < endTime)
        {
            float time = Time.time - startTime;
            rollingSpeedMult = rollingSpeedAC.Evaluate(time) * rollingSpeedUserMult;

            transform.Translate(speed * rollingSpeedMult * Time.deltaTime * transform.forward, Space.World);
            EndFiring();
            yield return null;
        }

        State = StateType.Idle;

        // ȸ�� ������ ó�� �ٶ󺸴� �������� ����
        // �Ѿ˱���, �����̴°� ����, ���콺 �ٶ󺸴°� ����.
    }
    #endregion Roll

    #region TakeHit
    new public void TakeHit(int damage)
    {
        if (hp > 0)
        {
            base.TakeHit(damage);
            CreateBloodEffect(capsuleCol.transform.position);
            animator.SetTrigger("TakeHit");

            if (hp <= 0)
                DieCo();
        }
    }

    [SerializeField] float diePreDelayTime = 0.4f;
    IEnumerator DieCo()
    {
        State = StateType.Die;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(diePreDelayTime);
        animator.SetTrigger("Die");
    }
    #endregion TakeHit

    #region Methods
    float fastAimingDistance = 0.2f;
    float lookatRotationValue = 0.05f;
    void RotationSlerp(Vector3 dir)
    {
        if (Vector3.Distance(transform.forward, dir) < fastAimingDistance)
            transform.forward = Vector3.Slerp(transform.forward, dir, lookatRotationValue * 10);
        else
            transform.forward = Vector3.Slerp(transform.forward, dir, lookatRotationValue);
    }
    #endregion Methods

    #region State
    [SerializeField] StateType m_state;

    public StateType State
    {
        get => m_state;
        set
        {
            if (m_state == value)
                return;

            print($"{m_state} -> {value}");
            m_state = value;
        }
    }

    public enum StateType
    {
        Idle,
        Run,
        Shoot,
        Hit,
        Die,
        Roll,
    }
    #endregion State
}
