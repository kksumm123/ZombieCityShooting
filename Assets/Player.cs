using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 5;
    float lookatRotationValue = 0.05f;


    Vector3 move;
    void Update()
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
        }
    }
}
