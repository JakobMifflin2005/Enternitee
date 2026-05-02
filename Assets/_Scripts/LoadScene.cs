using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    [SerializeField]
    private string menuGameSceneName = "_Scene_0_1";

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(menuGameSceneName);
    }
}
