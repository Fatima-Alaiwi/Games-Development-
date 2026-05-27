using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar UI Components")]
    public Image healthBarFill;

    [Header("Health Settings")]
    public float maxHealth = 10f;
    public float currentHealth;

    [Header("Smooth Settings")]
    public float smoothSpeed = 3f;

    private float targetFill = 1f;
    private float displayedFill = 1f;

    private Color greenColor = new Color(0.357f, 0.773f, 0.071f); // #5BC512
    private Color yellowColor = Color.yellow;
    private Color redColor = Color.red;

    void Start()
    {
        currentHealth = maxHealth;
        targetFill = 1f;
        displayedFill = 1f;

        if (healthBarFill != null)
        {
            healthBarFill.rectTransform.anchorMin = new Vector2(0f, 0f);
            healthBarFill.rectTransform.anchorMax = new Vector2(1f, 1f);
            healthBarFill.rectTransform.offsetMin = Vector2.zero;
            healthBarFill.rectTransform.offsetMax = Vector2.zero;
            healthBarFill.color = greenColor;
        }
    }

    void Update()
    {
        if (healthBarFill == null) return;

        // Smooth health bar size
        displayedFill = Mathf.MoveTowards(
            displayedFill,
            targetFill,
            Time.deltaTime * smoothSpeed
        );

        Vector2 aMax = healthBarFill.rectTransform.anchorMax;
        aMax.x = displayedFill;
        healthBarFill.rectTransform.anchorMax = aMax;
        healthBarFill.rectTransform.offsetMax = new Vector2(0f, healthBarFill.rectTransform.offsetMax.y);

        // Smooth color without shaking
        Color targetColor = GetSmoothHealthColor(displayedFill);

        healthBarFill.color = Color.Lerp(
            healthBarFill.color,
            targetColor,
            Time.deltaTime * smoothSpeed
        );
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        targetFill = currentHealth / maxHealth;
    }

    private Color GetSmoothHealthColor(float fillAmount)
    {
        if (fillAmount > 0.5f)
        {
            // Green to Yellow
            float t = Mathf.InverseLerp(1f, 0.5f, fillAmount);
            return Color.Lerp(greenColor, yellowColor, t);
        }
        else
        {
            // Yellow to Red
            float t = Mathf.InverseLerp(0.5f, 0f, fillAmount);
            return Color.Lerp(yellowColor, redColor, t);
        }
    }
}