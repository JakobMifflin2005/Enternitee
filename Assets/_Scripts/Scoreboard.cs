using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            scoreText.text = GameManager.Instance.GetFinalScorecard();
        }
    }
}
