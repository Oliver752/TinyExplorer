using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("References")]
    public GameObject menuRoot; // PausePanel
    public IntroSequenceController introController;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.P;
    public bool pauseAudioListener = true;

    public static bool IsPaused { get; set; }

    void Start()
    {
        if (menuRoot != null) 
            menuRoot.SetActive(false);

        SetPaused(false, immediate: true);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            TogglePause();
    }

    public void TogglePause() => SetPaused(!IsPaused);

    public void ContinueGame() => SetPaused(false);

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void SetPaused(bool pause, bool immediate = false)
    {
        IsPaused = pause;

        if (menuRoot != null) 
            menuRoot.SetActive(pause);

        Time.timeScale = pause ? 0f : 1f;
        if (pauseAudioListener) 
            AudioListener.pause = pause;

        Cursor.visible = pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;

        if (introController != null)
        {
            if (pause) introController.PauseIntro();
            else introController.ResumeIntro();
        }
    }

    void OnDestroy()
    {
        if (IsPaused) SetPaused(false, immediate: true);
    }

    // RETURN TO MAIN MENU
    public void ReturnToMainMenu()
    {
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        AudioListener.pause = false;
        IsPaused = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (menuRoot != null) 
            menuRoot.SetActive(false);

        SceneManager.LoadScene("MainMenu");
        
    }

    
}
