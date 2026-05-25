using UnityEngine;

public class LightPulser : MonoBehaviour
{
    private Light targetLight;

    [Header("Pulse Settings")]
    [Tooltip("The lowest intensity the light will drop to.")]
    public float minIntensity = 0.5f;

    [Tooltip("The highest intensity the light will reach.")]
    public float maxIntensity = 3.5f;

    [Tooltip("How fast the light pulses.")]
    public float pulseSpeed = 2f;

    void Awake()
    {
        // Automatically find the Light component attached to this GameObject
        targetLight = GetComponent<Light>();
    }

    void Update()
    {
        if (targetLight == null) return;

        // Use Mathf.Sin to smoothly transition between -1 and 1 over time
        float sinWave = Mathf.Sin(Time.time * pulseSpeed);

        // Normalize the sin wave from (-1 to 1) into a (0 to 1) range
        float normalizedValue = (sinWave + 1f) / 2f;

        // Lerp the light intensity between your minimum and maximum choices
        targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, normalizedValue);
    }
}