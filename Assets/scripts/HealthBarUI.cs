using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public PlayerController player;
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        // Если игрок не назначен в инспекторе, ищем его по тегу
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.GetComponent<PlayerController>();
        }

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
