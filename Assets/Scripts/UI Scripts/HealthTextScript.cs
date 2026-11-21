using TMPro;
using UnityEngine;

public class HealthTextScript : MonoBehaviour
{
    private TMP_Text healthText;

    private void Start()
    {
        if (healthText == null)
        {
            healthText = GetComponent<TMP_Text>();
        }
    }

    public void SetHealthText(int health, int maxHealth)
    {
        healthText.text = "Health: " + health + "/" + maxHealth;
    }
}
