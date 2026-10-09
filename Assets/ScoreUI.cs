using UnityEngine;
using TMPro; // или using UnityEngine.UI дл€ обычного Text

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI text; // или public Text text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (GameManager.Instance != null)
            text.text = "—чЄт: " + GameManager.Instance.score;
    }
}