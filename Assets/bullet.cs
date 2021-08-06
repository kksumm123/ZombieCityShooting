using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int power = 20;
    [SerializeField] int randomPower = 4;
    [SerializeField] float speed = 20;
    [SerializeField] float destroyTime = 1;
    [SerializeField] float knockBackForce = 0.1f;
    void Start()
    {
        Destroy(gameObject, destroyTime);
        power += Random.Range(-randomPower, randomPower);
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
            zombie.TakeHit(power, transform, knockBackForce);
            Destroy(gameObject);
        }
    }
}
