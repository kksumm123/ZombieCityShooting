using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected Animator animator;

    [SerializeField] protected int hp = 100;
    protected void Start()
    {
        animator = GetComponentInChildren<Animator>();
        bloodParticle = (GameObject)Resources.Load("BloodParticle");
    }

    GameObject bloodParticle;
    protected void CreateBloodEffect(Vector3 hitPoint)
    {
        Instantiate(bloodParticle, hitPoint, Quaternion.identity);
    }

}
