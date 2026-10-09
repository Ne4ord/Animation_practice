using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public PlayerController player;
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        if (player != null)
        {
            slider.maxValue = player.maxHealth;
            slider.value = player.maxHealth;
        }
    }

    void Update()
    {
        if (player != null)
        {
            slider.value = player.currentHealth;
        }
    }
}