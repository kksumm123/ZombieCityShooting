using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using UnityEngine.Animations.Rigging;

public partial class Player : Actor
{
    CapsuleCollider capsuleCol;
    [SerializeField] WeaponInfo mainWeapon;
    [SerializeField] WeaponInfo subWeapon;
    [SerializeField] WeaponInfo throwWeapon;
    [SerializeField] WeaponInfo currentWeapon;
    [SerializeField] Transform rightWeaponPosition;

    [SerializeField] float speed = 5;
    [SerializeField] float shootingSpeed = 2.5f;
    new protected void Awake()
    {
        base.Awake();

    }
    new void Start()
    {
        base.Start();
        hp = 300;
        capsuleCol = GetComponent<CapsuleCollider>();

        WeaponInit(mainWeapon);
        WeaponInit(subWeapon);

        ChangeWeapon(mainWeapon);

        HealthUI.Instance.SetGauge(hp, maxHp);
        AmmoUI.Instance.SetBulletCount(BulletCountInClip, MaxBulletCountInClip, BulletCountInClip + AllBulletCount, MaxBulletCount);

        var vcs = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var item in vcs)
        {
            item.Follow = transform;
            item.LookAt = transform;
        }

        StartCoroutine(SettingLookAtTargetCo());
    }

    IEnumerator SettingLookAtTargetCo()
    {
        var multiAimConstraint = GetComponentInChildren<MultiAimConstraint>();
        var rigbuilder = GetComponentInChildren<RigBuilder>();
        while (State != StateType.Die)
        {
            List<Zombie> allZombies = Zombie.allZombies;
            Transform lastTartget = null;
            if (allZombies.Count > 0)
            {
                var nearestZombie = allZombies.OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
                                              .First();
                if (nearestZombie.transform != lastTartget)
                {
                    lastTartget = nearestZombie.transform;
                    var sourceObjects = multiAimConstraint.data.sourceObjects;
                    sourceObjects.Clear();
                    sourceObjects.Add(new WeightedTransform(lastTartget, 1));
                    multiAimConstraint.data.sourceObjects = sourceObjects;
                    rigbuilder.Build();
                }
            }
            else
            {
                //multiAimConstraint.data.sourceObjects.Clear();
                // Clear ���൵ �ε����� ��������(transform ���� �����)
                multiAimConstraint.data.sourceObjects = new WeightedTransformArray();
                rigbuilder.Build();
            }
            yield return new WaitForSeconds(1);
        }
    }
    private void WeaponInit(WeaponInfo weaponInfo)
    {
        if (weaponInfo)
        {
            weaponInfo = Instantiate(weaponInfo, transform);
            weaponInfo.Init();
            weaponInfo.gameObject.SetActive(false);
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
                ReloadBullet();
                ChangeWeapon();
                //if (Input.GetKeyDown(KeyCode.Tab))
                //    ToggleChangeWeapon();
            }
        }
    }

    void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeWeapon(mainWeapon);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeWeapon(subWeapon);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeWeapon(throwWeapon);
    }
    #region ReloadBullet
    void ReloadBullet()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadBulletCo());
        }
    }

    IEnumerator ReloadBulletCo()
    {
        State = StateType.Reload;
        animator.SetTrigger("Reload");
        int reloadCount = Mathf.Min(AllBulletCount, MaxBulletCountInClip);
        AmmoUI.Instance.StartReload(reloadCount, MaxBulletCountInClip, BulletCountInClip + AllBulletCount, MaxBulletCount, ReloadTime);
        yield return new WaitForSeconds(ReloadTime);
        State = StateType.Idle;
        AllBulletCount -= reloadCount;
        BulletCountInClip = reloadCount;
    }
    #endregion ReloadBullet

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

            float forwardDegree = transform.forward.VectorToDegree();
            float moveDegree = move.VectorToDegree();
            float dirRadian = (moveDegree - forwardDegree + 90) * Mathf.PI / 180; //���Ȱ�
            Vector3 dir;
            dir.x = Mathf.Cos(dirRadian);// 
            dir.z = Mathf.Sin(dirRadian);//

            animator.SetFloat("DirX", dir.x);
            animator.SetFloat("DirY", dir.z);

            transform.Translate(
                (isFiring == true ? shootingSpeed : speed)
                * rollingSpeedMult * Time.deltaTime * move, Space.World);
            State = StateType.Run;

            //if (Mathf.RoundToInt(transform.forward.x) == 1 || Mathf.RoundToInt(transform.forward.x) == -1)
            //{
            //    animator.SetFloat("DirX", transform.forward.z * move.z);
            //    animator.SetFloat("DirY", transform.forward.x * move.x);
            //}
            //else
            //{
            //    animator.SetFloat("DirX", transform.forward.x * move.x);
            //    animator.SetFloat("DirY", transform.forward.z * move.z);
            //}
        }
        else
            State = StateType.Idle;

        animator.SetFloat("Speed", move.sqrMagnitude);
    }

    Plane plane = new Plane(new Vector3(0, 1, 0), 0);
    void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = hitPoint - transform.position;
            dir.y = 0;
            dir.Normalize();
            RotationSlerp(dir);
        }
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
            HealthUI.Instance.SetGauge(hp, maxHp);
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
    bool toggleWeapon = false;
    void ToggleChangeWeapon()
    {
        ChangeWeapon((toggleWeapon == true ? mainWeapon : subWeapon));
        toggleWeapon = !toggleWeapon;
    }
    GameObject currentWeaponGo;
    void ChangeWeapon(WeaponInfo newWeapon)
    {
        Destroy(currentWeaponGo);

        var weaponInfo = Instantiate(newWeapon, rightWeaponPosition);
        weaponInfo.transform.localPosition = newWeapon.gameObject.transform.localPosition;
        weaponInfo.transform.localRotation = newWeapon.gameObject.transform.localRotation;
        weaponInfo.transform.localScale = newWeapon.gameObject.transform.localScale;
        currentWeapon = weaponInfo;

        animator.runtimeAnimatorController = currentWeapon.overrideController;
        currentWeaponGo = currentWeapon.gameObject;

        if (currentWeapon.attackCollider != null)
            currentWeapon.attackCollider.enabled = false;

        //bullet = (GameObject)Resources.Load(bulletString);
        if (currentWeapon.bulletLight != null)
            bulletLight = currentWeapon.bulletLight.gameObject;
        shootDelay = currentWeapon.delay;
    }

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
        Reload,
    }
    #endregion State
}
