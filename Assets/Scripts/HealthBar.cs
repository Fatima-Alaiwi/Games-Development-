using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar UI Components")]
    public Image healthBarFill;
    public RectTransform containerContainer;

    [Header("Health Settings")]
    public float maxHealth = 10f;
    public float currentHealth;

    [Header("Smooth Settings")]
    public float smoothSpeed = 5f;
    private float targetFill;

    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 5f;
    private Vector2 containerOriginalPosition;
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

        if (containerContainer == null && healthBarFill != null)
        {
            containerContainer = healthBarFill.rectTransform.parent as RectTransform;
        }

        if (containerContainer != null)
        {
            containerOriginalPosition = containerContainer.anchoredPosition;
        }

        if (healthBarFill != null)
        {
            healthBarFill.rectTransform.pivot = new Vector2(0f, 0.5f);
            healthBarFill.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            healthBarFill.color = new Color(0.357f, 0.773f, 0.071f); // #5BC512
        }
    }

    void Update()
    {
        if (healthBarFill == null) return;

        float currentScaleX = healthBarFill.rectTransform.localScale.x;
        float newFill = Mathf.Lerp(currentScaleX, targetFill, Time.deltaTime * smoothSpeed);
        healthBarFill.rectTransform.localScale = new Vector3(newFill, 1f, 1f);

        Color targetColor;
        if (newFill > 0.6f)
            targetColor = new Color(0.357f, 0.773f, 0.071f); // #5BC512
        else if (newFill > 0.3f)
            targetColor = Color.yellow;
        else
            targetColor = Color.red;

        if (!isPulsing)
        {
            healthBarFill.color = Color.Lerp(healthBarFill.color, targetColor, Time.deltaTime * smoothSpeed);
        }

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

        if (!isShaking && gameObject.activeInHierarchy)
            StartCoroutine(ShakeEffect());
    }

    IEnumerator ShakeEffect()
    {
        if (containerContainer == null) yield break;

        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
            containerContainer.anchoredPosition = containerOriginalPosition + new Vector2(offsetX, offsetY);
            elapsed += Time.deltaTime;
            yield return null;
        }

        containerContainer.anchoredPosition = containerOriginalPosition;
        isShaking = false;
    }

    IEnumerator PulseEffect()
    {
        while (isPulsing)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            if (healthBarFill != null)
            {
                healthBarFill.color = Color.Lerp(Color.red, new Color(1f, 0.3f, 0f), pulse);
            }
            yield return null;
        }
    }
}