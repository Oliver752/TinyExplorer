using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityTutorial.PlayerControl;

public class PauseMenuController : MonoBehaviour
{
    [Header("Video Background")]
    public VideoPlayer pauseBackgroundVideo;

    [Header("Player Control")]
    public PlayerController playerController;

    [Header("References")]
    public GameObject menuRoot; // PausePanel
    public IntroSequenceController introController;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.P;
    public bool pauseAudioListener = true; // not used

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

        Cursor.visible = pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;

        if (playerController != null)
            playerController.canMove = !pause;

        if (pauseBackgroundVideo != null)
        {
            if (pause) pauseBackgroundVideo.Play();
            else pauseBackgroundVideo.Pause();
        }

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
        // ✅ Save full state (scene + player transform + health)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
{
    SaveSystem.SaveFromPlayer(playerObj.transform);

    PlayerHealth ph = playerObj.GetComponent<PlayerHealth>();
    if (ph != null)
        SaveSystem.SavePlayerHealth(ph.GetCurrentHealth());

    SaveSystem.SaveAllEnemiesInScene(); // ✅ add this
}

        else
        {
            // fallback: at least save scene name so Continue can load something
            PlayerPrefs.SetInt("HAS_SAVE", 1);
            PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();
            Debug.LogWarning("[PauseMenuController] Player not found to save position; saved only scene name.");
        }

        Time.timeScale = 1f;

        AudioListener.pause = false;
        IsPaused = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (menuRoot != null)
            menuRoot.SetActive(false);

        if (GameMusicManager.instance != null)
            GameMusicManager.instance.StopAndRewind();

        SceneManager.LoadScene("MainMenu");
    }
}
