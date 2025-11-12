using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("General Settings")]
    public float lifetime = 5f;              // auto-destroy bullet
    public GameObject hitEffect;             // optional visual effect
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

    private void Start()
    {
        Destroy(gameObject, lifetime); // auto cleanup
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            HandleEnemyHit(other);

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
                Destroy(effect, 0.5f);
            }

            if (hasExplosion)
                Explode();

            Destroy(gameObject);
        }
    }

    void HandleEnemyHit(Collider2D enemy)
    {
        // spawn hit effect
        if (hitEffect != null)
            Instantiate(hitEffect, enemy.transform.position, Quaternion.identity);

        // instant kill
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
