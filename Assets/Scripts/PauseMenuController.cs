using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [Header("References")]
    public GameObject menuRoot; // your PausePanel

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.P;
    public bool pauseAudioListener = true;

    public static bool IsPaused { get; private set; }  // <-- GLOBAL FLAG

    void Start()
    {
        if (menuRoot != null) menuRoot.SetActive(false);
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
        IsPaused = pause;                                 // <-- set global

        if (menuRoot != null) menuRoot.SetActive(pause);

        Time.timeScale = pause ? 0f : 1f;
        if (pauseAudioListener) AudioListener.pause = pause;

        Cursor.visible = pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void OnDestroy()
    {
        if (IsPaused) SetPaused(false, immediate: true);
    }
}
