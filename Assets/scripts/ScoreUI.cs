using UnityEngine;
using TMPro; 

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI text; 

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (GameManager.Instance != null)
            text.text = "Ñ÷¸ò: " + GameManager.Instance.score;
    }
}