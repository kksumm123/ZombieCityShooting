using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : Actor
{
    const string bulletString = "Bullet";
    GameObject bullet;

    float shootDelayEndTime;
    bool isFiring = false;
    [SerializeField] float shootDelay = 0.05f;
    void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            if (shootDelayEndTime < Time.time)
            {
                isFiring = true;
                animator.SetTrigger("FireStart");
                shootDelayEndTime = Time.time + shootDelay;
                switch (mainWeapon.type)
                {
                    case WeaponInfo.WeaponType.Gun:
                        IncreaseRecoil();
                        StartCoroutine(InstantiateBilletAndFlashBulletCo());
                        break;
                    case WeaponInfo.WeaponType.Melee:
                        // 무기의 콜라이더 활성화, 무기가 휘둘리며 충돌하도록
                        StartCoroutine(MeleeAttackCo());
                        break;
                }
            }
        }
        else
            EndFiring();
    }

    IEnumerator MeleeAttackCo()
    {
        yield return new WaitForSeconds(mainWeapon.attackStartTime);
        mainWeapon.attackCollider.enabled = true;
        yield return new WaitForSeconds(mainWeapon.attackTime);
        mainWeapon.attackCollider.enabled = false;
    }

    private void EndFiring()
    {
        isFiring = false;
        DecreaseRecoil();
    }

    GameObject bulletLight;
    public float bulletFlashTime = 0.001f;
    private IEnumerator InstantiateBilletAndFlashBulletCo()
    {
        yield return null;
        Instantiate(bullet, bulletSpawnPosition.position, CalculateRecoil(transform.rotation));

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
        zombie.TakeHit(mainWeapon.power, transform, mainWeapon.knockBackForce);
    }
}
