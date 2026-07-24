using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CinematicCamera : MonoBehaviour
{
    public static CinematicCamera instance;

    private Camera mainCam;
    private float originalSize;
    private Vector3 originalPos;

    // Track all bullets currently triggering a killcam
    private List<Transform> activeBullets = new List<Transform>();
    private bool isKillCamActive = false;

    // The two temporary pop-up cameras
    private Camera splitCamLeft;
    private Camera splitCamRight;

    void Awake()
    {
        instance = this;
        mainCam = GetComponent<Camera>();
        originalSize = mainCam.orthographicSize;
        originalPos = transform.position;
    }

    public void RegisterKillCam(Transform bullet)
    {
        if (!activeBullets.Contains(bullet))
        {
            activeBullets.Add(bullet);
        }

        // If the routine isn't running yet, start it!
        if (!isKillCamActive)
        {
            StartCoroutine(KillCamRoutine());
        }
    }

    IEnumerator KillCamRoutine()
    {
        isKillCamActive = true;
        Time.timeScale = 0.05f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Loop runs as long as there is at least 1 bullet in the air
        while (activeBullets.Count > 0)
        {
            // Clean up any bullets that just hit the head and destroyed themselves
            activeBullets.RemoveAll(b => b == null);

            if (activeBullets.Count == 0) break; // All bullets have exploded!

            // ==========================================
            // MODE 1: SINGLE HIT
            // ==========================================
            if (activeBullets.Count == 1)
            {
                // Ensure no split screens exist
                CleanupSplitCameras();

                Vector3 targetPos = new Vector3(activeBullets[0].position.x, activeBullets[0].position.y, -10f);
                transform.position = Vector3.Lerp(transform.position, targetPos, 15f * Time.unscaledDeltaTime);
                mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, 2.5f, 10f * Time.unscaledDeltaTime);
            }

            // ==========================================
            // MODE 2: THE DRAW (SPLIT SCREEN)
            // ==========================================
            else if (activeBullets.Count >= 2)
            {
                // Smoothly pull the main background camera back to normal so the background looks good
                transform.position = Vector3.Lerp(transform.position, originalPos, 10f * Time.unscaledDeltaTime);
                mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, originalSize, 10f * Time.unscaledDeltaTime);

                // If the split cameras don't exist yet, create them instantly!
                if (splitCamLeft == null)
                    splitCamLeft = CreateSplitCamera("SplitCam_Left", new Rect(0f, 0f, 0.495f, 1f)); // Left half with a tiny center gap
                if (splitCamRight == null)
                    splitCamRight = CreateSplitCamera("SplitCam_Right", new Rect(0.505f, 0f, 0.495f, 1f)); // Right half

                // Lock the sub-cameras to their respective bullets
                UpdateSplitCamera(splitCamLeft, activeBullets[0]);
                UpdateSplitCamera(splitCamRight, activeBullets[1]);
            }

            yield return null;
        }

        // ==========================================
        // THE IMPACT & RESOLUTION
        // ==========================================
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.25f);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        mainCam.orthographicSize = originalSize;
        transform.position = originalPos;

        CleanupSplitCameras();
        isKillCamActive = false;

        StartCoroutine(ScreenShake(0.2f, 0.4f));
    }

    // Helper function to dynamically spawn a camera and set its screen space
    Camera CreateSplitCamera(string name, Rect viewport)
    {
        GameObject camObj = new GameObject(name);
        Camera newCam = camObj.AddComponent<Camera>();

        newCam.orthographic = true;
        newCam.orthographicSize = 2.5f; // Keep them zoomed in for the detail shot
        newCam.rect = viewport;         // Force them into their half of the screen
        newCam.clearFlags = CameraClearFlags.Depth;
        newCam.depth = 1;               // Ensure they draw ON TOP of the main camera

        return newCam;
    }

    void UpdateSplitCamera(Camera cam, Transform target)
    {
        if (target != null)
        {
            // Smoothly track the bullet inside the pop-up window
            Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10f);
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, 15f * Time.unscaledDeltaTime);
        }
    }

    void CleanupSplitCameras()
    {
        if (splitCamLeft != null) Destroy(splitCamLeft.gameObject);
        if (splitCamRight != null) Destroy(splitCamRight.gameObject);
    }

    IEnumerator ScreenShake(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-1f, 1f) * magnitude;
            float y = originalPos.y + Random.Range(-1f, 1f) * magnitude;
            transform.position = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPos;
    }
}