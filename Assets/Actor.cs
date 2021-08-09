using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected Animator animator;
    static GameObject textEffectGo;

    [SerializeField] protected int hp = 100;
    protected int maxHp;
    protected void Awake()
    {
        maxHp = hp;
    }
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
    public Color damageColor = Color.red;
    protected void TakeHit(int damage)
    {
        hp -= damage;
        CreateTextEffect(damage, transform.position, damageColor);
    }
    public static void CreateTextEffect(int value, Vector3 position, Color color)
    {
        var randomPos = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.1f, 0.1f));
        var newGo = Instantiate(textEffectGo, position + randomPos, Camera.main.transform.rotation);
        var textMeshPro = newGo.GetComponent<TextMeshPro>();
        textMeshPro.text = value.ToNumber();
        var colorGradient = textMeshPro.colorGradient;
        colorGradient.bottomLeft = color;
        textMeshPro.colorGradient = colorGradient;
        Destroy(newGo, 1.5f);
    }
}
