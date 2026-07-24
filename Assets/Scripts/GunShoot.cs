using System.Collections;
using UnityEngine;
using TMPro;

public class GunShoot : MonoBehaviour
{
    [Header("Core References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public TextMeshProUGUI reloadText;
    public LineRenderer laserSight;

    [Header("Gun Mechanics")]
    public KeyCode shootKey = KeyCode.Space;
    public int maxAmmo = 3;
    public float reloadTime = 3f;
    public float laserFadeTime = 1f;

    [Header("Laser Visuals")]
    public float centerDividerX = 0f; // The exact X position of your middle line
    public float laserStopOffset = 0.15f; // How far before the line it should stop

    private int currentAmmo;
    private bool isReloading = false;
    private float currentLaserAlpha = 1f;
    private Color laserColor;

    void Start()
    {
        currentAmmo = maxAmmo;
        if (reloadText != null) reloadText.text = "";

        laserSight.positionCount = 2;

        // FIX 1: This now automatically grabs the exact color you set in the Line Renderer!
        laserColor = laserSight.startColor;
    }

    void Update()
    {
        if (GameManager.instance == null || !GameManager.instance.isDuelActive || isReloading)
        {
            laserSight.enabled = false;
            return;
        }

        laserSight.enabled = true;
        UpdateLaserSight();

        if (Input.GetKeyDown(shootKey) && currentAmmo > 0)
        {
            Shoot();
        }
    }

    void UpdateLaserSight()
    {
        Vector3 startPos = firePoint.position;
        Vector3 direction = firePoint.right;

        float distanceToCenter = 20f;

        if (Mathf.Abs(direction.x) > 0.001f)
        {
            // FIX 2: We calculate the stopping point based on which way the gun is pointing
            float targetX = centerDividerX;

            if (direction.x > 0)
                targetX -= laserStopOffset; // Left Player stops slightly before the line
            else
                targetX += laserStopOffset; // Right Player stops slightly before the line

            float t = (targetX - startPos.x) / direction.x;
            if (t > 0) distanceToCenter = t;
        }

        Vector3 endPos = startPos + (direction * distanceToCenter);

        laserSight.SetPosition(0, startPos);
        laserSight.SetPosition(1, endPos);

        Color currentColor = laserColor;
        currentColor.a = currentLaserAlpha;
        laserSight.startColor = currentColor;
        laserSight.endColor = currentColor;
    }

    void Shoot()
    {
        currentAmmo--;
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        StartCoroutine(FadeLaser());

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator FadeLaser()
    {
        currentLaserAlpha = 0f;
        float elapsed = 0f;

        while (elapsed < laserFadeTime)
        {
            elapsed += Time.deltaTime;
            currentLaserAlpha = Mathf.Lerp(0f, 1f, elapsed / laserFadeTime);
            yield return null;
        }
        currentLaserAlpha = 1f;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        laserSight.enabled = false;
        if (reloadText != null) reloadText.text = "RELOADING...";

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        if (reloadText != null) reloadText.text = "";
        isReloading = false;
    }
}