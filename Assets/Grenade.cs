using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] int power = 50;
    [SerializeField] float damageArea = 5;
    [SerializeField] float knockBackForce = 5;
    [SerializeField] float destroyDelay = 3;
    GameObject destroyEffect;
    LayerMask attackableLayer;

    IEnumerator Start()
    {
        attackableLayer = 1 << LayerMask.NameToLayer("Attackable");
        yield return new WaitForSeconds(destroyDelay);
        // ����鿡�� ������ ����

        var attackables = Physics.OverlapSphere(transform.position, damageArea, attackableLayer);
        foreach (var item in attackables)
        {
            transform.LookAt(item.transform.position);
            item.GetComponent<Zombie>().TakeHit(power, transform, knockBackForce);
        }

        // ���� ����Ʈ ǥ��
        //Instantiate(destroyEffect, transform.position, Quaternion.identity);
    }
}
