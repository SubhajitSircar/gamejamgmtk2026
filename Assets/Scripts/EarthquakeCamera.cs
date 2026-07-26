using UnityEngine;

public class EarthquakeCamera : MonoBehaviour
{
    [Header("Earthquake Settings")]
    public float maxIntensity = 0.25f;
    public float shakeSpeed = 25f;
    public float tremorSpeed = 1.5f;

    // We store the current offset so we can subtract it before adding the next one
    private Vector3 currentShakeOffset = Vector3.zero;

    void LateUpdate()
    {
        // 1. UNDO LAST FRAME'S SHAKE:
        // This gives the clean, intended camera position back to the Cinematic script
        transform.localPosition -= currentShakeOffset;

        if (GameManager.instance == null || !GameManager.instance.isDuelActive)
        {
            // Reset the offset and do nothing if the duel is over
            currentShakeOffset = Vector3.zero;
            return;
        }

        // 2. CALCULATE THE NEW SHAKE:
        // Using unscaledTime ensures the earthquake continues at full speed during slow-mo!
        float tremor = Mathf.PerlinNoise(Time.unscaledTime * tremorSpeed, 10f);
        float currentIntensity = tremor * maxIntensity;

        float x = (Mathf.PerlinNoise(Time.unscaledTime * shakeSpeed, 0f) - 0.5f) * 2f * currentIntensity;
        float y = (Mathf.PerlinNoise(0f, Time.unscaledTime * shakeSpeed) - 0.5f) * 2f * currentIntensity;

        currentShakeOffset = new Vector3(x, y, 0f);

        // 3. APPLY THE NEW SHAKE:
        // We add this right on top of wherever the Cinematic script moved the camera this frame
        transform.localPosition += currentShakeOffset;
    }
}