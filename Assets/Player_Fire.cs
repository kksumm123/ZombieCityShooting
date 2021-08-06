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

    [SerializeField] int bulletCountInClip; //źâ ���� �Ѿ� ��
    [SerializeField] int MaxBulletCountInClip; //źâ �ִ� �Ѿ� ��
    [SerializeField] int allBulletCount; // ������ ��ü �Ѿ� ��
    [SerializeField] int reloadTime; // �������ð�
    void Fire()
    {
        if (Input.GetMouseButton(0) && bulletCountInClip > 0)
        {
            if (shootDelayEndTime < Time.time)
            {
                isFiring = true;
                animator.SetTrigger("FireStart");
                shootDelayEndTime = Time.time + shootDelay;
                switch (currentWeapon.type)
                {
                    case WeaponInfo.WeaponType.Gun:
                        bulletCountInClip--;
                        IncreaseRecoil();
                        currentWeapon.StartCoroutine(InstantiateBilletAndFlashBulletCo());
                        break;
                    case WeaponInfo.WeaponType.Melee:
                        // ������ �ݶ��̴� Ȱ��ȭ, ���Ⱑ �ֵѸ��� �浹�ϵ���
                        currentWeapon.StartCoroutine(MeleeAttackCo());
                        break;
                }
            }
        }
        else
            EndFiring();
    }

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
    public float bulletFlashTime = 0.05f;
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
        zombie.TakeHit(currentWeapon.power, transform, currentWeapon.knockBackForce);
    }
}
