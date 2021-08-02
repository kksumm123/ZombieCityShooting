using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int power = 20;
    [SerializeField] float speed = 20;
    [SerializeField] float destroyTime = 1;
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * transform.forward, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            var zombie = other.GetComponent<Zombie>();
            zombie.TakeHit(power, transform);
            Destroy(gameObject);
        }
    }
}
