using UnityEngine;
using UnityEngine.Events;

public class HealthScript : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;

    private int currentHealth;
    public UnityEvent<int, int> HealthChange;

    private void Start()
    {
        currentHealth = maxHealth;
        HealthChange.Invoke(currentHealth, maxHealth);
    }

    public void Heal(int health)
    {
        if ((currentHealth += health) > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += health;
        }
        HealthChange.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthChange.Invoke(currentHealth, maxHealth);
    }

    public int GetHealth()
    {
        return currentHealth;
    }
}
