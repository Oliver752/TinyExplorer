using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private const string KEY_HAS_SAVE   = "HAS_SAVE";
    private const string KEY_SCENE      = "SavedScene";
    private const string KEY_PX         = "SAVE_PX";
    private const string KEY_PY         = "SAVE_PY";
    private const string KEY_PZ         = "SAVE_PZ";
    private const string KEY_RX         = "SAVE_RX";
    private const string KEY_RY         = "SAVE_RY";
    private const string KEY_RZ         = "SAVE_RZ";

    // Flag used to skip intro when continuing
    private const string KEY_SKIP_INTRO = "SKIP_INTRO_ON_LOAD";

    public static bool HasSave() => PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1;

    public static void SaveFromPlayer(Transform player)
    {
        if (player == null) return;

        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.SetString(KEY_SCENE, SceneManager.GetActiveScene().name);

        Vector3 p = player.position;
        PlayerPrefs.SetFloat(KEY_PX, p.x);
        PlayerPrefs.SetFloat(KEY_PY, p.y);
        PlayerPrefs.SetFloat(KEY_PZ, p.z);

        Vector3 r = player.eulerAngles;
        PlayerPrefs.SetFloat(KEY_RX, r.x);
        PlayerPrefs.SetFloat(KEY_RY, r.y);
        PlayerPrefs.SetFloat(KEY_RZ, r.z);

        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Saved scene + player transform.");
    }

    public static string GetSavedScene()
        => PlayerPrefs.GetString(KEY_SCENE, "");

    public static Vector3 GetSavedPosition()
        => new Vector3(
            PlayerPrefs.GetFloat(KEY_PX, 0f),
            PlayerPrefs.GetFloat(KEY_PY, 0f),
            PlayerPrefs.GetFloat(KEY_PZ, 0f)
        );

    public static Quaternion GetSavedRotation()
        => Quaternion.Euler(
            PlayerPrefs.GetFloat(KEY_RX, 0f),
            PlayerPrefs.GetFloat(KEY_RY, 0f),
            PlayerPrefs.GetFloat(KEY_RZ, 0f)
        );

    public static void RequestSkipIntroOnce()
    {
        PlayerPrefs.SetInt(KEY_SKIP_INTRO, 1);
        PlayerPrefs.Save();
    }

    public static bool ConsumeSkipIntroOnce()
    {
        int v = PlayerPrefs.GetInt(KEY_SKIP_INTRO, 0);
        if (v == 1)
        {
            PlayerPrefs.SetInt(KEY_SKIP_INTRO, 0);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public static void ClearSave()
    {
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 0);

        PlayerPrefs.DeleteKey(KEY_SCENE);
        PlayerPrefs.DeleteKey(KEY_PX);
        PlayerPrefs.DeleteKey(KEY_PY);
        PlayerPrefs.DeleteKey(KEY_PZ);
        PlayerPrefs.DeleteKey(KEY_RX);
        PlayerPrefs.DeleteKey(KEY_RY);
        PlayerPrefs.DeleteKey(KEY_RZ);

        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Save cleared.");
    }
}
