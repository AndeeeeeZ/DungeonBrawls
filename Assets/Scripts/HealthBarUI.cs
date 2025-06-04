using UnityEngine;

public class HealthBarUI: MonoBehaviour
{
    [Header("Player Health Bar")]
    [SerializeField]
    private RectTransform healthBar;

    [SerializeField, Range(0f, 1f)]
    private float healthBarFillPercentage;

    private void Start()
    {
        healthBarFillPercentage = 1f; 
    }

    private void Update()
    {
        // healthBar.localScale = new Vector3(healthBarFillPercentage, healthBar.localScale.y, healthBar.localScale.z);
    }

    public void UpdateHealthBar(int currentHP, int maxHP)
    {
        healthBarFillPercentage = Mathf.Clamp01(((float)currentHP / (float)maxHP));
        healthBar.localScale = new Vector3(healthBarFillPercentage, healthBar.localScale.y, healthBar.localScale.z);
    }
}

