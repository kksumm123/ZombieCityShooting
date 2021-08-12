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
        Throw, // ����ź, ����ź, etc
    }

    public WeaponType type;
    public AnimatorOverrideController overrideController;

    public int power = 20;
    [SerializeField] int randomPower = 4;
    public float delay = 0.2f;

    public float knockBackForce = 1;

    [Header("��")]
    public GameObject bullet;
    public Transform bulletSpawnPosition;
    public Light bulletLight;
    public int bulletCountInClip = 15; //źâ ���� �Ѿ� ��
    public int maxBulletCountInClip = 30; //źâ �ִ� �Ѿ� ��
    public int allBulletCount = 500; // ������ ��ü �Ѿ� ��
    public int maxBulletCount = 500; // ���������� �ִ� �Ѿ� ��
    public float reloadTime = 1; // �������ð�

    [Header("��������")]
    public float attackStartTime = 0.4f;
    public float attackTime = 0.1f;
    public Collider attackCollider;

    [Header("��ô����")]
    public GameObject throwGo;

    private void Start()
    {
        power += Random.Range(-randomPower, randomPower);
    }
    internal void Init()
    {
        allBulletCount = Mathf.Min(allBulletCount, maxBulletCount);
        int reloadCount = Mathf.Min(allBulletCount, maxBulletCountInClip);
        allBulletCount -= reloadCount;
        bulletCountInClip = reloadCount;
    }
}
