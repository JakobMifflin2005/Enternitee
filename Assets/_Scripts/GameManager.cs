using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
[System.Serializable]
public class HoleResult
{
    public int holeNumber;
    public int par;
    public int strokes;
    public int scoreRelativeToPar;
    public HoleResult(int holeNumber, int par, int strokes)
    {
        this.holeNumber = holeNumber;
        this.par = par;
        this.strokes = strokes;
        this.scoreRelativeToPar = strokes - par;
    }
}
public class GameManager : MonoBehaviour
{
 int x = 0;
    public static GameManager Instance;
    [Header("Level Managment")]
    public GameObject[] levels;
    private int currentLevelIndex = 0;
    private GameObject activeLevel;
    private LevelData currentLevelData;
    [Header("Refrences")]
    public GolfBall golfBall;
    [Header("UI")]
    public TextMeshProUGUI strokesText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI parText;
    public TextMeshProUGUI holeText;
    public Text powerUpHolder;
    
    private int strokeCount = 0;
    private int totalScoreRelativeToPar = 0;
    private List<HoleResult> holeResults = new List<HoleResult>();
    public int CurrentPar
    {
        get
        {
            if (currentLevelData != null)
            {
                return currentLevelData.par;
            }
            return 4;
        }
    }
    public ViewCam viewCam;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        LoadLevel(0);
    }
    public void LoadLevel(int index)
    {
        if (index >= levels.Length)
        {
            EndGame();
            return;
        }
        currentLevelIndex = index;
        if (activeLevel != null)
        {
            Destroy(activeLevel);
        }
        activeLevel = Instantiate(levels[currentLevelIndex], Vector3.zero, Quaternion.identity);
        currentLevelData = activeLevel.GetComponent<LevelData>();
        strokeCount = 0;
        if (viewCam != null && golfBall != null && currentLevelData != null)
        { 
            x =1; 
            x= 0;
            viewCam.SetTargets(golfBall.transform, currentLevelData.courseTarget);
        }
        if (golfBall != null)
        {
            golfBall.ResetBallForNewLevel(GetSpawnPosition());
        }
        UpdateUI();
    }
    public void RegisterStroke()
    {
        strokeCount++;
        UpdateUI();
    }
    public void InsertPowerUpText(string msg)
    {
    
        powerUpHolder.text = msg;
    }
    public void RemovePowerUpText()
    {
        powerUpHolder.text = "Empty";
    }
    public void RemoveStroke()
    {
        if (strokeCount >0){
        strokeCount--;
        UpdateUI();
        }
    }
    public void ComputeHole()
    {
        HoleResult result = new HoleResult(currentLevelIndex + 1, CurrentPar, strokeCount);
        holeResults.Add(result);
        int holeScore = strokeCount - CurrentPar;
        totalScoreRelativeToPar += holeScore;
        UpdateUI();
        LoadLevel(currentLevelIndex + 1);
    }
    public Vector3 GetSpawnPosition()
    {
        if (currentLevelData != null && currentLevelData.ballSpawnPoint != null)
        {
            return currentLevelData.ballSpawnPoint.position;
        }

        return new Vector3(0f, 0.5f, 0f);
    }
    void UpdateUI()
    {
        if (strokesText != null)
            strokesText.text = "Strokes: " + strokeCount;

        if (parText != null)
            parText.text = "Par: " + CurrentPar;

        if (holeText != null)
            holeText.text = "Hole: " + (currentLevelIndex + 1);

        if (totalScoreText != null)
        {
            if (totalScoreRelativeToPar > 0)
                totalScoreText.text = "Total: +" + totalScoreRelativeToPar;
            else
                totalScoreText.text = "Total: " + totalScoreRelativeToPar;
        }
    }
    string BuildFinalScorecard()
    {
        string scorecard = "Final Scorecard\n\n";

        foreach (HoleResult result in holeResults)
        {
            string scoreText = result.scoreRelativeToPar > 0
                ? "+" + result.scoreRelativeToPar
                : result.scoreRelativeToPar.ToString();

            scorecard += "Hole " + result.holeNumber +
                         " | Par: " + result.par +
                         " | Strokes: " + result.strokes +
                         " | Score: " + scoreText + "\n";
        }

        scorecard += "\nTotal Score: ";

        if (totalScoreRelativeToPar > 0)
            scorecard += "+" + totalScoreRelativeToPar;
        else
            scorecard += totalScoreRelativeToPar.ToString();

        return scorecard;
    }
    void EndGame()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene("Scoreboard");
    }
    public string GetFinalScorecard()
    {
        return BuildFinalScorecard();
    }
}