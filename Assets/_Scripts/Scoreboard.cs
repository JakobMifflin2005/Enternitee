using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    [Header("Per Hole Data")]
    public TextMeshProUGUI[] parTexts;
    public TextMeshProUGUI[] strokeTexts;

    [Header("Overall Score")]
    public TextMeshProUGUI overallScoreText;

    void Start()
    {
        if (GameManager.Instance == null) return;

        List<HoleResult> results = GameManager.Instance.GetHoleResults();

        int totalScore = 0;

        for (int i = 0; i < results.Count; i++)
        {
            if (i < parTexts.Length)
                parTexts[i].text = results[i].par.ToString();

            if (i < strokeTexts.Length)
                strokeTexts[i].text = results[i].strokes.ToString();

            totalScore += results[i].scoreRelativeToPar;
        }

        if (overallScoreText != null)
        {
            overallScoreText.text = totalScore > 0
                ? "+" + totalScore
                : totalScore.ToString();
        }
    }
}
