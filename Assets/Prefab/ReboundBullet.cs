using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ReboundBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private int maxBounces = 3;

    [Header("VFX")]
    public GameObject sparkEffectPrefab;

    private Rigidbody2D rb;
    private Vector2 previousVelocity;
    private int bounceCount;

    // Tracks if we already told the camera to zoom in
    private bool isKillCamTriggered = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.velocity = transform.right * speed;
        previousVelocity = rb.velocity;
    }

    private void Update()
    {
        // --- THE MISSING KILL CAM TRIGGER ---
        if (!isKillCamTriggered && rb.velocity.sqrMagnitude > 0.1f)
        {
            // Shoot a laser 2 units forward in the current direction of travel
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rb.velocity.normalized, 2f);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.name == "Head")
                {
                    isKillCamTriggered = true;

                    if (CinematicCamera.instance != null)
                    {
                        CinematicCamera.instance.RegisterKillCam(transform);
                    }
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // Store velocity before a collision changes it.
        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            previousVelocity = rb.velocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. CHECK FOR PLAYER HIT
        PlayerDeath deathScript = collision.gameObject.GetComponentInParent<PlayerDeath>();
        if (deathScript != null)
        {
            deathScript.TriggerDeath(previousVelocity.x);
            Destroy(gameObject);
            return;
        }

        // 2. CHECK FOR BULLET CLASH
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (sparkEffectPrefab != null)
            {
                Instantiate(sparkEffectPrefab, collision.GetContact(0).point, Quaternion.identity);
            }
            if (CameraShake.instance != null) CameraShake.instance.Shake(0.1f, 0.15f);

            Destroy(gameObject);
            return;
        }

        // 3. BOUNCE OFF ENVIRONMENT
        if (collision.contactCount > 0)
        {
            Bounce(collision.GetContact(0).normal);
        }
    }

    private void Bounce(Vector2 surfaceNormal)
    {
        // Reflect the incoming direction.
        Vector2 reflectedDirection = Vector2.Reflect(previousVelocity.normalized, surfaceNormal);

        // Apply reflected velocity.
        rb.velocity = reflectedDirection * speed;

        // Rotate bullet to face its new direction.
        float angle = Mathf.Atan2(reflectedDirection.y, reflectedDirection.x) * Mathf.Rad2Deg;
        rb.rotation = angle;

        bounceCount++;

        // Remove bullet after maximum number of rebounds.
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
        }
        else
        {
            // Reset the trigger just in case the bullet grazed the head's raycast radius but bounced off a wall instead!
            isKillCamTriggered = false;
        }
    }
}