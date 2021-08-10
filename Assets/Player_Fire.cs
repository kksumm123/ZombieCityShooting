using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : Actor
{
    const string bulletString = "Bullet";
    GameObject Bullet => currentWeapon.bullet;
    Transform BulletSpawnPosition => currentWeapon.bulletSpawnPosition;


    float shootDelayEndTime;
    bool isFiring = false;
    [SerializeField] float shootDelay = 0.05f;

    int BulletCountInClip //탄창 현재 총알 수
    {
        get => currentWeapon.bulletCountInClip;
        set => currentWeapon.bulletCountInClip = value;
    }
    int MaxBulletCountInClip => currentWeapon.maxBulletCountInClip; //탄창 최대 총알 수
    int AllBulletCount // 소유한 전체 총알 수
    {
        get => currentWeapon.allBulletCount;
        set => currentWeapon.allBulletCount = value;
    }
    int MaxBulletCount => currentWeapon.maxBulletCount; // 소유가능한 최대 총알 수
    float ReloadTime => currentWeapon.reloadTime; // 재장전시간
    void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            if (BulletCountInClip > 0)
            {
                isFiring = true;
                if (shootDelayEndTime < Time.time)
                {
                    animator.SetTrigger("FireStart");
                    shootDelayEndTime = Time.time + shootDelay;
                    switch (currentWeapon.type)
                    {
                        case WeaponInfo.WeaponType.Gun:
                            BulletCountInClip--;
                            IncreaseRecoil();
                            AmmoUI.Instance.SetBulletCount(BulletCountInClip, MaxBulletCountInClip, BulletCountInClip + AllBulletCount, MaxBulletCount);
                            currentWeapon.StartCoroutine(InstantiateBilletAndFlashBulletCo());
                            break;
                        case WeaponInfo.WeaponType.Melee:
                            // 무기의 콜라이더 활성화, 무기가 휘둘리며 충돌하도록
                            currentWeapon.StartCoroutine(MeleeAttackCo());
                            break;
                    }
                }
            }
            else
            {
                if (reloadAlertDelayEndTime < Time.time)
                {
                    reloadAlertDelayEndTime = Time.time + reloadAlertDelay;
                    CreateTextEffect("Reload!", "TalkEffect", transform.position, Color.white, transform);
                }
            }
        }
        else
            EndFiring();
    }
    [SerializeField] float reloadAlertDelay = 1.5f;
    float reloadAlertDelayEndTime;
    IEnumerator MeleeAttackCo()
    {
        yield return new WaitForSeconds(currentWeapon.attackStartTime);
        currentWeapon.attackCollider.enabled = true;
        yield return new WaitForSeconds(currentWeapon.attackTime);
        currentWeapon.attackCollider.enabled = false;
    }

    private void EndFiring()
    {
        isFiring = false;
        DecreaseRecoil();
    }

    GameObject bulletLight;
    public float bulletFlashTime = 0.03f;
    private IEnumerator InstantiateBilletAndFlashBulletCo()
    {
        yield return null;
        var bulletGo = Instantiate(Bullet, BulletSpawnPosition.position, CalculateRecoil(transform.rotation));
        bulletGo.GetComponent<Bullet>().knockBackForce = currentWeapon.knockBackForce;

        bulletLight.SetActive(true);
        yield return new WaitForSeconds(bulletFlashTime);
        bulletLight.SetActive(false);
    }

    float recoilValue = 0f;
    float recoilMaxValue = 1.5f;
    float recoilLerpValue = 0.1f;
    void IncreaseRecoil()
    {
        recoilValue = Mathf.Lerp(recoilValue, recoilMaxValue, recoilLerpValue);
    }
    void DecreaseRecoil()
    {
        recoilValue = Mathf.Lerp(recoilValue, 0, recoilLerpValue);

    }

    Vector3 recoil;
    Quaternion CalculateRecoil(Quaternion rotation)
    {
        recoil = new Vector3(Random.Range(-recoilValue, recoilValue), Random.Range(-recoilValue, recoilValue), 0);
        return Quaternion.Euler(rotation.eulerAngles + recoil);
    }

    public void OnZombieEnter(Collider other)
    {
        var zombie = other.GetComponent<Zombie>();
        zombie.TakeHit(currentWeapon.power, transform, currentWeapon.knockBackForce);
    }
}
