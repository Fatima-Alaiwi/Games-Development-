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
    }
}
}