//using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    private Rigidbody2D rb;

    private bool isKillCamTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (!isKillCamTriggered)
        {
            // RaycastAll shoots a laser that hits EVERYTHING in the next 2 units
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rb.velocity.normalized, 2f);

            // Loop through everything the laser touched
            foreach (RaycastHit2D hit in hits)
            {
                // STRICT CHECK: Did ANY of those objects happen to be named "Head"?
                if (hit.collider != null && hit.collider.gameObject.name == "Head")
                {
                    isKillCamTriggered = true;

                    if (CinematicCamera.instance != null)
                    {
                        CinematicCamera.instance.RegisterKillCam(transform);
                    }
                    break; // We found the head, stop checking!
                }
            }
        }
    }

    [Header("VFX")]
    public GameObject sparkEffectPrefab; // Add this variable at the very top of your script!

    // ... your other code ...

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. CHECK FOR BULLET CLASH FIRST
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Spawn the sparks at the exact point where they touched
            if (sparkEffectPrefab != null)
            {
                // Grab the contact point
                Vector2 contactPoint = collision.GetContact(0).point;
                Instantiate(sparkEffectPrefab, contactPoint, Quaternion.identity);
            }

            // Optional Juice: Add a tiny micro-shake to the camera to sell the impact
            if (CameraShake.instance != null)
            {
                CameraShake.instance.Shake(0.1f, 0.15f);
            }

            // Destroy this bullet and STOP running the rest of the code
            Destroy(gameObject);
            return;
        }

        // 2. CHECK FOR PLAYER HIT
        PlayerDeath deathScript = collision.gameObject.GetComponentInParent<PlayerDeath>();

        if (deathScript != null)
        {
            deathScript.TriggerDeath(rb.velocity.x);
        }

        Destroy(gameObject);
    }
}