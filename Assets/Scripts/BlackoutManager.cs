using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BlackoutManager : MonoBehaviour
{
    public static BlackoutManager instance; // Makes it accessible from other scripts

    [Header("Lighting")]
    public Light2D globalLight;

    [Header("Audio")]
    public AudioSource boomSound;

    private bool hasBlackoutTriggered = false;
    private Camera mainCam;

    // Memory variables to remember what the lights looked like before the dark
    private Color originalSkyColor;
    private float originalLightIntensity;

    void Awake()
    {
        instance = this;
        mainCam = Camera.main;

        // Take a snapshot of the original lighting the moment the level loads
        if (mainCam != null) originalSkyColor = mainCam.backgroundColor;
        if (globalLight != null) originalLightIntensity = globalLight.intensity;
    }

    void Update()
    {
        if (!hasBlackoutTriggered && GameManager.instance != null && GameManager.instance.isDuelActive)
        {
            TriggerBlackout();
        }
    }

    void TriggerBlackout()
    {
        hasBlackoutTriggered = true;

        if (globalLight != null) globalLight.intensity = 0f;
        if (mainCam != null) mainCam.backgroundColor = Color.black;

        if (boomSound != null) boomSound.Play();
        if (CameraShake.instance != null) CameraShake.instance.Shake(0.4f, 0.3f);
    }

    // NEW: Called the exact moment a bullet hits a player
    public void RestoreLights()
    {
        StartCoroutine(FlashbulbEffect());
    }

    IEnumerator FlashbulbEffect()
    {
        // 1. Instantly snap the background back to blue
        if (mainCam != null) mainCam.backgroundColor = originalSkyColor;

        // 2. Flash the global light super bright (3x intensity)
        if (globalLight != null) globalLight.intensity = 3f;

        // 3. Smoothly fade the light back to normal during the slow-mo kill cam
        float elapsed = 0f;
        float duration = 0.5f; // We use a short duration because time is slowed down

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Unscaled time ignores the slow-mo!

            if (globalLight != null)
            {
                globalLight.intensity = Mathf.Lerp(3f, originalLightIntensity, elapsed / duration);
            }
            yield return null;
        }

        // Ensure it resets perfectly at the end
        if (globalLight != null) globalLight.intensity = originalLightIntensity;
    }
}