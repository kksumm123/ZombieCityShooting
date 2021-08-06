using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(
//    fileName = "New Weapon Info"
//    , menuName = "Scriptable Object/Weapon Info")]
//public class WeaponInfo : ScriptableObject
public class WeaponInfo : MonoBehaviour
{
    public AnimatorOverrideController overrideController;

    public int damage = 20;
    public float delay = 0.2f;
    public int maxBulletCount;

    public GameObject bullet;
    public Transform bulletSpawnPosition;
    public Light bulletLight;
}
