using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;
    GameObject bullet;
    Transform bulletSpawnPosition;
    const string bulletString = "Bullet";
    RectTransform recoilDetail;
    Vector2 recoilDetailOriginSize;

    [SerializeField] float speed = 5;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        bullet = (GameObject)Resources.Load(bulletString);
        bulletSpawnPosition = transform.Find("BulletSpawnPosition");
        recoilDetail = GameObject.Find("Canvas").transform.Find("RecoilUI/RecoilDetail").GetComponent<RectTransform>();
        recoilDetailOriginSize = recoilDetail.sizeDelta;
    }


    void Update()
    {
        Move();
        Fire();
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
            move.Normalize();

            transform.Translate(speed * Time.deltaTime * move, Space.World);

            State = StateType.Run;
        }
        else
            State = StateType.Idle;

        animator.SetFloat("DirX", move.x);
        animator.SetFloat("DirY", move.z);
        animator.SetFloat("Speed", move.sqrMagnitude);
    }
    #endregion Move

    #region Fire
    float shootDelayEndTime;
    Quaternion bulletRotation;
    void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            if (shootDelayEndTime < Time.time)
            {
                shootDelayEndTime = Time.time + shootDelay;
                StartCoroutine(ShootCo());
                State = StateType.Shoot;
                animator.Play("Shoot");
                IncreaseRecoil();
                Instantiate(bullet, bulletSpawnPosition.position, CalculateRecoil(transform.rotation));
            }
        }
        else
            DecreaseRecoil();
    }



    float recoilValue = 0f;
    float recoilMaxValue = 1.5f;
    float recoilLerpValue = 0.1f;
    void IncreaseRecoil()
    {
        recoilValue = Mathf.Lerp(recoilValue, recoilMaxValue, recoilLerpValue);
        recoilDetail.sizeDelta = recoilDetailOriginSize + (recoilDetailOriginSize * recoilValue);
    }
    void DecreaseRecoil()
    {
        recoilValue = Mathf.Lerp(recoilValue, 0, recoilLerpValue);
        recoilDetail.sizeDelta = recoilDetailOriginSize + (recoilDetailOriginSize * recoilValue);
    }

    Vector3 recoil;
    Quaternion CalculateRecoil(Quaternion rotation)
    {
        recoil = new Vector3(Random.Range(-recoilValue, recoilValue), Random.Range(-recoilValue, recoilValue), 0);
        return Quaternion.Euler(rotation.eulerAngles + recoil);
    }

    [SerializeField] float shootDelay = 0.05f;
    IEnumerator ShootCo()
    {
        while (Time.time < shootDelayEndTime)
            yield return null;
    }
    #endregion Fire

    #region Methods
    float fastAimingDistance = 0.2f;
    float lookatRotationValue = 0.05f;
    void RotationSlerp(Vector3 move)
    {
        if (Vector3.Distance(transform.forward, move) < fastAimingDistance)
            transform.forward = Vector3.Slerp(transform.forward, move, lookatRotationValue * 10);
        else
            transform.forward = Vector3.Slerp(transform.forward, move, lookatRotationValue);
    }
    #endregion Methods

    [SerializeField] StateType m_state;

    StateType State
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

    enum StateType
    {
        Idle,
        Run,
        Shoot,
        Hit,
        Death,
    }
}
