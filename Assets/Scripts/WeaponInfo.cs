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
        Melee, // 근접공격, 총알, None, Etc
        Throw, // 수류탄, 연막탄, etc
    }

    public WeaponType type;
    public AnimatorOverrideController overrideController;

    public int power = 20;
    [SerializeField] int randomPower = 4;
    public float delay = 0.2f;

    public float knockBackForce = 1;

    [Header("총")]
    public GameObject bullet;
    public Transform bulletSpawnPosition;
    public Light bulletLight;
    public int bulletCountInClip = 15; //탄창 현재 총알 수
    public int maxBulletCountInClip = 30; //탄창 최대 총알 수
    public int allBulletCount = 500; // 소유한 전체 총알 수
    public int maxBulletCount = 500; // 소유가능한 최대 총알 수
    public float reloadTime = 1; // 재장전시간

    [Header("근접공격")]
    public float attackStartTime = 0.4f;
    public float attackTime = 0.1f;
    public Collider attackCollider;

    [Header("투척공격")]
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
