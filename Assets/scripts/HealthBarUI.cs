using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public PlayerController player;
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.GetComponent<PlayerController>();
        }

        if (player != null)
        {
            slider.maxValue = player.maxHealth;
            slider.value = player.maxHealth;
            player.OnHealthChanged += UpdateHealthBar;
        }
    }

    
    void UpdateHealthBar(int newHealth)
    {
        slider.value = newHealth;
    }

    void OnDestroy()
    {
      
        if (player != null)
            player.OnHealthChanged -= UpdateHealthBar;
    }
}