using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;       // visual rope
    public LayerMask grappleLayer;          // layers you can grapple to
    public GameObject hitEffect;            // effect on grapple hit  

    private Rigidbody2D rb;

    [Header("Settings")]
    public float maxDistance = 10f;         // max grapple range
    public float pullSpeed = 10f;           // how fast you pull
    public float stopDistance = 1f;         // distance to stop near hook

    private Vector2 grapplePoint;
    private bool isGrappling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Setup the line renderer if missing
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
        }
    }

    void Update()
    {
        // Press and hold right mouse button to grapple
        if (Input.GetMouseButtonDown(1))
        {
            TryStartGrapple();
        }

        // Release button to stop grappling
        if (Input.GetMouseButtonUp(1))
        {
            StopGrapple();
        }

        if (isGrappling)
        {
            UpdateGrapple();
        }
    }

    void TryStartGrapple()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // keep in 2D plane
        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleLayer);

        if (hit.collider != null)
        {
            grapplePoint = hit.point;
            isGrappling = true;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);

            GameObject effect = Instantiate(hitEffect, grapplePoint, Quaternion.identity);
            Destroy(effect, 0.5f);

            Debug.Log("Grappled to: " + hit.collider.name);
        }
        else
        {
            Debug.Log("No valid grapple target found.");
        }
    }

    void UpdateGrapple()
    {
        // Update rope line
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);

        // Move player toward grapple point
        Vector2 direction = (grapplePoint - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, grapplePoint);

        if (distance > stopDistance)
        {
            rb.linearVelocity = direction * pullSpeed;
        }
        else
        {
            StopGrapple();
        }
    }

    void StopGrapple()
    {
        isGrappling = false;
        rb.linearVelocity = Vector2.zero;
        lineRenderer.positionCount = 0;
        Debug.Log("Grapple stopped.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
