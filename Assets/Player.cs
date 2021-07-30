using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;
    GameObject bullet;
    Transform bulletSpawnPosition;
    const string bulletString = "Bullet";
    [SerializeField] float speed = 5;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        bullet = (GameObject)Resources.Load(bulletString);
        bulletSpawnPosition = transform.Find("BulletSpawnPosition");
    }

    void Update()
    {
        Move();

        if (Input.GetMouseButton(0))
        {
            State = StateType.Shoot;

            Instantiate(bullet, bulletSpawnPosition.position, transform.rotation);
        }
    }
    public Vector3 myF;
    public Vector3 move;
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
            RotationSlerp(move);

            State = StateType.Run;
        }
        else
            State = StateType.Idle;
    }

    float fastAimingDistance = 0.2f;
    float lookatRotationValue = 0.05f;
    void RotationSlerp(Vector3 move)
    {
        if (Vector3.Distance(transform.forward, move) < fastAimingDistance)
            transform.forward = Vector3.Slerp(transform.forward, move, lookatRotationValue * 10);
        else
            transform.forward = Vector3.Slerp(transform.forward, move, lookatRotationValue);
    }

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
            animator.Play(m_state.ToString());
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
