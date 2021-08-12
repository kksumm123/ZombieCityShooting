using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    ProjectileArc projectileArc;
    public Cursor cursor;
    [SerializeField] float speed = 10;
    void Start()
    {
        projectileArc = GetComponent<ProjectileArc>();
        firePoint = transform;
        cursor = FindObjectOfType<Cursor>();
    }

    void Update()
    {
        SetTargetWithSpeed(cursor.transform.position, speed);

        // 수류탄 발사 구현
        if (Input.GetMouseButtonDown(1))
        {
            // 수류탄 생성
            // 리지드 바디에 포스 줘서 날리자
            var newGo = Instantiate(grenadeGo, firePoint.position, Quaternion.identity);
            newGo.transform.forward = direction;
            float degree = -currentAngle * Mathf.Rad2Deg; // !앵글 -주의
            newGo.transform.Rotate(degree, 0, degree); // y각도를 위해 x, z 회전
            var newGoRigid = newGo.GetComponent<Rigidbody>();
            newGoRigid.velocity = newGo.transform.forward * speed;
        }
    }

    public GameObject grenadeGo;
    public Transform firePoint;
    public float currentAngle;
    Vector3 direction;
    public void SetTargetWithSpeed(Vector3 point, float speed)
    {
        //currentSpeed = speed;

        direction = point - firePoint.position;
        float yOffset = direction.y;
        direction = Math3d.ProjectVectorOnPlane(Vector3.up, direction);
        float distance = direction.magnitude;

        float angle0, angle1;
        bool targetInRange = ProjectileMath.LaunchAngle(speed, distance, yOffset, Physics.gravity.magnitude, out angle0, out angle1);

        if (targetInRange)
            currentAngle = angle0;

        projectileArc.UpdateArc(speed, distance, Physics.gravity.magnitude, currentAngle, direction, targetInRange);
        //SetTurret(direction, currentAngle * Mathf.Rad2Deg);

        //currentTimeOfFlight = ProjectileMath.TimeOfFlight(currentSpeed, currentAngle, -yOffset, Physics.gravity.magnitude);
    }
}
