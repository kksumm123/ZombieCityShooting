using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected Animator animator;
    GameObject textEffectGo;

    [SerializeField] protected int hp = 100;
    protected void Start()
    {
        animator = GetComponentInChildren<Animator>();
        bloodParticle = (GameObject)Resources.Load("BloodParticle");
        textEffectGo = (GameObject)Resources.Load("TextEffect");
    }

    GameObject bloodParticle;
    protected void CreateBloodEffect(Vector3 hitPoint)
    {
        Instantiate(bloodParticle, hitPoint, Quaternion.identity);
    }
    public Color damageColor = Color.white;
    protected void TakeHit(int damage)
    {
        hp -= damage;
        CreateTextEffect(damage, damageColor);
    }
    protected void CreateTextEffect(int damage, Color color)
    {
        var randomPos = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.1f, 0.1f));
        var newGo = Instantiate(textEffectGo, transform.position + randomPos, Camera.main.transform.rotation);
        var textMeshPro = newGo.GetComponent<TextMeshPro>();
        textMeshPro.text = damage.ToNumber();
        textMeshPro.color = color;
        Destroy(newGo, 1.5f);
    }
}
