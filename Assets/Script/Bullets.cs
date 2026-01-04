using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("General Settings")]
    public float lifetime = 5f;              // auto-destroy bullet

    [Header("Effects")]
    public GameObject hitEffect;
    public GameObject missEffect;          // optional visual effect
                                           //public ShockWaveManager callShockWave;

    [SerializeField] private ShockWaveManager shockWaveManager;

    //ShockWaveManager callShockWave;


    public bool destroyOnHit = true;         // bullet disappears on hit

    [Header("Enemy Settings")]
    public bool instantKill = true;          // delete enemy immediately
    public string enemyTag = "Enemy";        // enemy tag

    [Header("Explosion Settings")]
    public bool hasExplosion = false;        // enable/disable explosion
    public float explosionRadius = 5f;
    public float explosionForce = 200f;

    [Header("Collision Settings")]
    public LayerMask obstacleLayer;          // obstacle layers to explode on

    private bool exploded = false;

    public TimeManager timeManager;



    private void Start()
    {
        Destroy(gameObject, lifetime); // auto cleanup

        // if (callShockWave == null)
        // {
        //     callShockWave = FindObjectOfType<ShockWaveManager>();
        //     if (callShockWave == null)
        //         Debug.LogWarning("No ShockWaveManager found in scene!");
        // }
    }

    private void Awake()
    {
        shockWaveManager = FindObjectOfType<ShockWaveManager>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag(enemyTag))
        {
            HandleEnemyHit(other);


            shockWaveManager.CallShockWave(transform.position);

            timeManager.DoSlowmotion();

            if (destroyOnHit)
                Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // check if collided object's layer is in obstacleLayer
        if ((obstacleLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            if (exploded) return;
            exploded = true;

            // spawn effect
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);

                Destroy(effect, 1f);
            }

            // call shockwave at collision point
            if (shockWaveManager != null)
            {
                Debug.Log("Calling ShockWave");
                shockWaveManager.CallShockWave(transform.position);
            }
            else
            {
                Debug.LogWarning("No ShockWaveManager assigned to bullet!");
            }



            if (hasExplosion)
                Explode();

            Destroy(gameObject);
        }
    }

    void HandleEnemyHit(Collider2D enemy)
    {
        // spawn hit effect
        // Vector2 hitPos = enemy.transform.position;

        if (missEffect != null)
        {
            GameObject effect = Instantiate(missEffect, enemy.transform.position, Quaternion.identity);
            Instantiate(hitEffect, enemy.transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        if (instantKill)
        {
            Destroy(enemy.gameObject);

        }
    }

    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);


        foreach (Collider2D nearby in colliders)
        {
            Rigidbody2D rb = nearby.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (rb.position - (Vector2)transform.position).normalized;
                float distance = Vector2.Distance(rb.position, transform.position);
                float force = explosionForce * (1 - (distance / explosionRadius));

                rb.AddForce(direction * force, ForceMode2D.Impulse);

            }

            // kill enemies in explosion radius
            if (nearby.CompareTag(enemyTag) && instantKill)
            {
                Destroy(nearby.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (hasExplosion)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
