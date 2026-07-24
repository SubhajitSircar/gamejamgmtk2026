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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit a head
        if (collision.gameObject.name == "Head")
        {
            // Go up the hierarchy to find the main Player root and grab our new script
            PlayerDeath deathScript = collision.transform.root.GetComponent<PlayerDeath>();

            if (deathScript != null)
            {
                // Trigger the death sequence, passing the bullet's current speed direction
                deathScript.TriggerDeath(rb.velocity.x);
            }
        }

        // Destroy the bullet after impact
        Destroy(gameObject);
    }
}