using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public const string SHOOT = "Shoot";
    public const string RELOAD = "Reload";

    AudioSource audioSource;
    Animator weaponAnimator;

    public Transform muzzle;
    public WeaponData weaponData;

   public PlayerControllerGun myController;
    public int ammoCount { get; private set; }

    bool reloading = false;
    bool shooting = false;
    bool readyToShoot = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        weaponAnimator = GetComponentInChildren<Animator>();

        ammoCount = weaponData.maxAmmo;
        //UserInterface.Singleton.UpdateBulletCounter(ammoCount, weaponData.maxAmmo);
    }

    public void Shoot()
    {
        if(!readyToShoot || shooting || reloading || weaponData == null) return;

        if(ammoCount <= 0) { Reload(); return; }

        readyToShoot = false;
        shooting = true;
        
        UseAmmo();

        Invoke(nameof(ResetAttack), weaponData.fireRate);
        AttackRaycast();

        Instantiate(weaponData.fireEffect, muzzle);

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(weaponData.fireSound);
    
        weaponAnimator.Play(SHOOT);
        myController.PlayAnimation(SHOOT);
    }

    void UseAmmo()
    {
        ammoCount--;
        UserInterface.Singleton.UpdateBulletCounter(ammoCount, weaponData.maxAmmo);
    }

    public void Reload()
    {
        if(ammoCount == weaponData.maxAmmo || reloading) return;

        Debug.Log("Reload called");

        reloading = true;

        myController.PlayAnimation(RELOAD);
        weaponAnimator.Play(RELOAD);
        Invoke(nameof(ResetReload), weaponData.reloadTime);
    }

    void ResetReload()
    { 
        reloading = false;
        ammoCount = weaponData.maxAmmo;

        UserInterface.Singleton.UpdateBulletCounter(ammoCount, weaponData.maxAmmo);
    }

    void ResetAttack()
    {
        shooting = false;
        readyToShoot = true;
    }

    void AttackRaycast()
    {
        RaycastHit hit;
        RaycastHit[] hits;

        switch (weaponData.type)
        {
            case WeaponType.Bullet:
                if(Physics.Raycast(myController.cam.transform.position, myController.cam.transform.forward, out hit, weaponData.weaponRange))
                { HitTarget(hit); }
                break;
            case WeaponType.Piercing:
                hits = Physics.RaycastAll(myController.cam.transform.position, myController.cam.transform.forward, weaponData.weaponRange);

                if(hits.Length > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    { HitTarget(hits[i]); }
                }
                break;
        }
    }

    void HitTarget(RaycastHit hit)
{
    // Ignore trigger colliders so bullets only hit the physical collider
    if (hit.collider.isTrigger) return;

    Actor target = hit.transform.GetComponentInParent<Actor>();

    if (target != null)
    {
        target.TakeDamage(weaponData.attackDamage);
    }

    if (weaponData.hitEffect != null)
    {
        GameObject go = Instantiate(weaponData.hitEffect, hit.point, Quaternion.identity);
        Destroy(go, 4f);
    }
}
}
