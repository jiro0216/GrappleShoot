using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float fireRate = 0.5f;
    public float bulletForce = 20f;
    public float recoilForce = 50f;

    public Rigidbody2D playerRb;

    private bool canShoot = true;
    private ShockWaveManager shockWave;

    private void Awake()
    {
        // shockWave = FindObjectOfType<ShockWaveManager>();
        // if (shockWave == null)
        //     Debug.LogWarning("No ShockWaveManager found in scene!");
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            // if (shockWave != null)
            //     shockWave.CallShockWave();

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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        ApplyRecoil();
        StartCoroutine(ShootCooldown());
    }

    void ApplyRecoil()
    {
        Vector2 recoilDirection = -firePoint.up;
        playerRb.AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);
    }
}
