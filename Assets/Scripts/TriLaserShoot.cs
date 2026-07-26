using System.Collections;
using UnityEngine;
using TMPro;

public class TriLaserShoot : MonoBehaviour
{
    [Header("Core References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public TextMeshProUGUI reloadText;

    [Header("Laser Decoys")]
    public LineRenderer[] lasers = new LineRenderer[3];
    public float centerDividerX = 0f;
    public float maxSwayAngle = 25f; // How far up/down the lasers sweep

    // We give each laser a different speed so they uncouple and cross over each other
    private float[] swaySpeeds = { 2f, 3.5f, 5f };
    private float[] currentAngles = new float[3];
    private int realLaserIndex;

    [Header("Gun Mechanics")]
    public KeyCode shootKey = KeyCode.Space;
    public int maxAmmo = 3;
    public float reloadTime = 3f;

    private int currentAmmo;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = maxAmmo;
        if (reloadText != null) reloadText.text = "";

        // Randomly assign which of the 3 lasers is the real one at the start of the round
        realLaserIndex = Random.Range(0, 3);
    }

    void OnDisable()
    {
        // Whenever this script is turned off (like during the typing phase), 
        // force all the line renderers to turn off too!
        foreach (var laser in lasers)
        {
            if (laser != null)
            {
                laser.enabled = false;
            }
        }
    }

    void Update()
    {
        if (GameManager.instance == null || !GameManager.instance.isDuelActive || isReloading)
        {
            foreach (var laser in lasers) laser.enabled = false;
            return;
        }

        foreach (var laser in lasers) laser.enabled = true;

        CalculateLaserSway();
        DrawLasers();

        if (Input.GetKeyDown(shootKey) && currentAmmo > 0)
        {
            Shoot();
        }
    }

    void CalculateLaserSway()
    {
        // Use Sine waves to calculate a smooth back-and-forth angle for each laser
        for (int i = 0; i < 3; i++)
        {
            // Time.time keeps it moving smoothly. Multiplying by swaySpeeds makes them out of sync.
            currentAngles[i] = Mathf.Sin(Time.time * swaySpeeds[i]) * maxSwayAngle;
        }
    }

    void DrawLasers()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 startPos = firePoint.position;

            // Rotate the mathematical direction of the laser by its current sway angle
            Quaternion swayRotation = Quaternion.Euler(0, 0, currentAngles[i]);
            Vector3 direction = swayRotation * firePoint.right;

            float distanceToCenter = 20f;

            if (Mathf.Abs(direction.x) > 0.001f)
            {
                float targetX = centerDividerX;
                // Offset based on direction so it doesn't overlap the divider
                if (direction.x > 0) targetX -= 0.15f;
                else targetX += 0.15f;

                float t = (targetX - startPos.x) / direction.x;
                if (t > 0) distanceToCenter = t;
            }

            Vector3 endPos = startPos + (direction * distanceToCenter);

            lasers[i].SetPosition(0, startPos);
            lasers[i].SetPosition(1, endPos);
        }
    }

    void Shoot()
    {
        currentAmmo--;

        // 1. Randomly pick a new "true" laser for EVERY single shot!
        realLaserIndex = Random.Range(0, 3);

        // 2. Calculate the exact rotation of that newly chosen laser
        Quaternion trueAimRotation = firePoint.rotation * Quaternion.Euler(0, 0, currentAngles[realLaserIndex]);

        // 3. Spawn the bullet with that rotation
        GameObject spawnedBullet = Instantiate(bulletPrefab, firePoint.position, trueAimRotation);

        // 4. THE FIX: Hijack the Rigidbody and force it down the laser's path
        Rigidbody2D rb = spawnedBullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Calculate the actual directional vector of the laser
            Vector2 trueDirection = trueAimRotation * Vector3.right;

            // Force the velocity (20f is your bullet speed from the earlier physics setup!)
            rb.velocity = trueDirection * 20f;
        }

        if (CameraShake.instance != null)
            CameraShake.instance.Shake(0.1f, 0.2f);

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        foreach (var laser in lasers) laser.enabled = false;
        if (reloadText != null) reloadText.text = "RELOADING...";

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        if (reloadText != null) reloadText.text = "";
        isReloading = false;
    }
}