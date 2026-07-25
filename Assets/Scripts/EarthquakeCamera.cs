using UnityEngine;

public class EarthquakeCamera : MonoBehaviour
{
    [Header("Earthquake Settings")]
    public float maxIntensity = 0.25f;
    public float shakeSpeed = 25f;
    public float tremorSpeed = 1.5f;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (GameManager.instance == null ||
            !GameManager.instance.isDuelActive)
        {
            transform.localPosition = originalPosition;
            return;
        }

        // Slowly changes between weak and strong shaking
        float tremor =
            Mathf.PerlinNoise(Time.time * tremorSpeed, 10f);

        float currentIntensity = tremor * maxIntensity;

        float x =
            (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f)
            * 2f * currentIntensity;

        float y =
            (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f)
            * 2f * currentIntensity;

        transform.localPosition =
            originalPosition + new Vector3(x, y, 0f);
    }
}