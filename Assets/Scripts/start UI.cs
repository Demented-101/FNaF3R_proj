using UnityEngine;
using UnityEngine.SceneManagement;

public class startUI : MonoBehaviour
{
    const string gameScene = "Assets/Scenes/MainGame.unity";

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Single);
    }

    public void Close()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
