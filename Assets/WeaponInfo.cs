using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(
//    fileName = "New Weapon Info"
//    , menuName = "Scriptable Object/Weapon Info")]
//public class WeaponInfo : ScriptableObject
public class WeaponInfo : MonoBehaviour
{
    public enum WeaponType
    {
        Gun,
        Melee, // ��������, �Ѿ�, None, Etc
    }

    public WeaponType type;
    public AnimatorOverrideController overrideController;

    public int power = 20;
    [SerializeField] int randomPower = 4;
    public float delay = 0.2f;

    [Header("��")]
    public int maxBulletCount = 6;
    public GameObject bullet;
    public Transform bulletSpawnPosition;
    public Light bulletLight;

    [Header("��������")]
    public float attackStartTime = 0.2f;
    public float attackTime = 0.4f;
    public Collider attackCollider;

    private void Start()
    {
        power += Random.Range(-randomPower, randomPower);
    }
}
