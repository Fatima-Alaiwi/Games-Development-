using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarFill;
    public float maxHealth = 100f;
    public float currentHealth;

    // TEMPORARY TEST - delete later
    public float testHealth = 100f;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateBar();
    }

    void Update()
    {
        SetHealth(testHealth);
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        UpdateBar();
    }

void UpdateBar()
{
    if (healthBarFill != null)
    {
        float fill = currentHealth / maxHealth;
        healthBarFill.rectTransform.localScale = new Vector3(fill, 1, 1);

        // Change color based on health
        if (fill > 0.6f)
        {
            // Above 60% — Green
            healthBarFill.color = Color.green;
        }
        else if (fill > 0.3f)
        {
            // Between 30% and 60% — Yellow
            healthBarFill.color = Color.yellow;
        }
        else
        {
            // Below 30% — Red
            healthBarFill.color = Color.red;
        }
    }
}
}