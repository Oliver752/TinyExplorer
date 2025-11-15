using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Call from UI Button OnClick()
    public void StartGame(string sceneName)
    {
        // load gameplay scene
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // Works in build; does nothing in editor
        Application.Quit();
    }

    public void OpenOptions()
    {
        // show options UI or toggle a panel
        // implement your options panel logic here
        Debug.Log("Open Options clicked");
    }
}
