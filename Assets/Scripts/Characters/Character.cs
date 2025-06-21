using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    public int
        currentHP,
        maxHP, 
        attack, 
        defense;

    [SerializeField]
    protected HealthBarUI healthBarUI;

    #region Getter
    public int GetAttack()
    {
        return attack; 
    }

    public int GetDefense()
    {
        return defense; 
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    #endregion

    public void ReduceHealthBy(int amount)
    {
        currentHP -= amount; 
        UpdateUI();
    }

    protected void OnEnable()
    {
        currentHP = maxHP;
        UpdateUI(); 
    }

    // Updates health bar UI
    public void UpdateUI()
    {
        healthBarUI.UpdateHealthBar(currentHP, maxHP);
    }

    public bool IsAlive()
    {
        return currentHP > 0; 
    }
    public virtual void Die()
    {
        Debug.Log($"{this.gameObject.name} died");
    }
}
