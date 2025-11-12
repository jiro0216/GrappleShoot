using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;

    public GameObject bulletPrefab;

    public float fireRate = 2f;

    private bool canShoot = true;

    public float bulletForce = 20f;

    public float recoilForce = 50f;

    public Rigidbody2D playerRb;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            Shoot();
        }
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }


    void Shoot()
    {
        //Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        //Add force to bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        ApplyRecoil();

        //Start cooldown
        StartCoroutine(ShootCooldown());
    }

    void ApplyRecoil()
    {
        // Apply force in the opposite direction of the shot
        Vector2 recoilDirection = -firePoint.up;
        playerRb.AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);
    }

}