using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected Animator animator;

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
        CreateTextEffect(value.ToNumber(), "TextEffect", position, color);
    }
    public static void CreateTextEffect(string str, string prefabName, Vector3 position, Color color, Transform parent = null)
    {
        var memoryGo = (GameObject)Resources.Load(prefabName);
        var randomPos = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.1f, 0.1f));
        var newGo = Instantiate(memoryGo, position + randomPos, Camera.main.transform.rotation);
        if (parent)
            newGo.transform.parent = parent;
        var textMeshPro = newGo.GetComponentInChildren<TextMeshPro>();
        textMeshPro.text = str;
        var colorGradient = textMeshPro.colorGradient;
        colorGradient.bottomLeft = color;
        textMeshPro.colorGradient = colorGradient;
        Destroy(newGo, 1.5f);
    }
}
