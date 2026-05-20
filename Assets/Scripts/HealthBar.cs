using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar Fill")]
    public Image healthBarFill;

    [Header("Health Settings")]
    public float maxHealth = 10f;
    public float currentHealth;

    [Header("Smooth Settings")]
    public float smoothSpeed = 5f;
    private float targetFill;

    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 5f;
    private Vector3 originalPosition;
    private bool isShaking = false;

    [Header("Pulse Settings")]
    public float criticalThreshold = 0.3f;
    public float pulseSpeed = 3f;
    private bool isPulsing = false;
    private Coroutine pulseCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        targetFill = 1f;

        if (healthBarFill != null)
        {
            originalPosition = healthBarFill.rectTransform.localPosition;
            // Raghad: set pivot to left so bar shrinks from right side
            healthBarFill.rectTransform.pivot = new Vector2(0f, 0.5f);
            healthBarFill.color = Color.green;
            healthBarFill.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void Update()
    {
        if (healthBarFill == null) return;

        // Raghad: smooth sliding toward target
        float currentFill = healthBarFill.rectTransform.localScale.x;
        float newFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);
        healthBarFill.rectTransform.localScale = new Vector3(newFill, 1f, 1f);

        // Raghad: smooth color — green → yellow → red
        Color targetColor;
        if (newFill > 0.6f)
            targetColor = Color.green;
        else if (newFill > 0.3f)
            targetColor = Color.yellow;
        else
            targetColor = Color.red;

        healthBarFill.color = Color.Lerp(healthBarFill.color, targetColor, Time.deltaTime * smoothSpeed);

        // Raghad: start pulsing when critical
        if (newFill <= criticalThreshold && !isPulsing)
        {
            isPulsing = true;
            pulseCoroutine = StartCoroutine(PulseEffect());
        }
        else if (newFill > criticalThreshold && isPulsing)
        {
            isPulsing = false;
            if (pulseCoroutine != null)
                StopCoroutine(pulseCoroutine);
        }
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        targetFill = currentHealth / maxHealth;

        if (!isShaking)
            StartCoroutine(ShakeEffect());
    }

    IEnumerator ShakeEffect()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
            healthBarFill.rectTransform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        healthBarFill.rectTransform.localPosition = originalPosition;
        isShaking = false;
    }

    IEnumerator PulseEffect()
    {
        while (isPulsing)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            if (healthBarFill != null)
                healthBarFill.color = Color.Lerp(Color.red, new Color(1f, 0.3f, 0f), pulse);
            yield return null;
        }
    }
}