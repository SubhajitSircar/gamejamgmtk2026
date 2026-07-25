using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ReboundBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private int maxBounces = 3;

    private Rigidbody2D rb;
    private Vector2 previousVelocity;
    private int bounceCount;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.velocity = transform.right * speed;
        previousVelocity = rb.velocity;
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
        if (!collision.gameObject.CompareTag("Rebounder"))
            return;

        if (collision.contactCount == 0)
            return;

        Bounce(collision.GetContact(0).normal);
    }

    private void Bounce(Vector2 surfaceNormal)
    {
        // Reflect the incoming direction.
        Vector2 reflectedDirection =
            Vector2.Reflect(previousVelocity.normalized, surfaceNormal);

        // Apply reflected velocity.
        rb.velocity = reflectedDirection * speed;

        // Rotate bullet to face its new direction.
        float angle = Mathf.Atan2(
            reflectedDirection.y,
            reflectedDirection.x
        ) * Mathf.Rad2Deg;

        rb.rotation = angle;

        bounceCount++;

        // Remove bullet after maximum number of rebounds.
        if (bounceCount >= maxBounces)
        {
            Destroy(gameObject);
        }
    }
}