using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    Animator animator;
    RectTransform recoilDetail;
    Vector2 recoilDetailOriginSize;

    [SerializeField] float speed = 5;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        bullet = (GameObject)Resources.Load(bulletString);
        recoilDetail = GameObject.Find("Canvas").transform.Find("RecoilUI/RecoilDetail").GetComponent<RectTransform>();
        recoilDetailOriginSize = recoilDetail.sizeDelta;
    }


    void Update()
    {
        LookAtMouse();
        Move();
        Fire();
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
    #endregion State
}
