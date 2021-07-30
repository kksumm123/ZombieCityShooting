using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator;
    public GameObject bullet;
    public Transform bulletSpawnPosition;

    [SerializeField] float speed = 5;
    float lookatRotationValue = 0.05f;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Move();

        if (Input.GetMouseButton(0))
        {
            State = StateType.Shoot;
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Instantiate(bullet, transform.position, transform.rotation);
        }
    }

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
            transform.forward = Vector3.Slerp(transform.forward, move, lookatRotationValue);
            State = StateType.Run;
        }
        else
            State = StateType.Idle;
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
