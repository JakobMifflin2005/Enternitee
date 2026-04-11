using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    private string mainGameSceneName = "_Scene_0";

    public void LoadMainGame()
    {
        SceneManager.LoadScene(mainGameSceneName);
    }
}
