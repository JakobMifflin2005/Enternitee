using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultSceneController : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    public string startSceneName = "StartScene";
    public string gameplaySceneName = "_Scene_0 1";

    public float waitTime = 3f;

    void Start()
    {
        StartCoroutine(ResultRoutine());
    }

    IEnumerator ResultRoutine()
    {
        bool won = false;

        if (GameManager.Instance != null)
        {
            won = GameManager.Instance.DidPlayerWin();
        }

        if (resultText != null)
        {
            resultText.text = won ? "YOU WIN!" : "YOU LOSE!";
        }

        yield return new WaitForSeconds(waitTime);

        if (won)
        {
            if (GameManager.Instance != null)
            {
                Destroy(GameManager.Instance.gameObject);
            }

            SceneManager.LoadScene(startSceneName);
        }
        else
        {
            if (GameManager.Instance != null)
            {
                Destroy(GameManager.Instance.gameObject);
            }

            SceneManager.LoadScene(gameplaySceneName);
        }
    }
}