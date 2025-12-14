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

    private const string KEY_PLAYER_HP  = "SAVE_PLAYER_HP";

    // Flag used to skip intro when continuing
    private const string KEY_SKIP_INTRO = "SKIP_INTRO_ON_LOAD";

    // Enemy prefixes
    private const string ENEMY_ALIVE = "ENEMY_ALIVE_";
    private const string ENEMY_HP    = "ENEMY_HP_";
    private const string ENEMY_PX    = "ENEMY_PX_";
    private const string ENEMY_PY    = "ENEMY_PY_";
    private const string ENEMY_PZ    = "ENEMY_PZ_";
    private const string ENEMY_RX    = "ENEMY_RX_";
    private const string ENEMY_RY    = "ENEMY_RY_";
    private const string ENEMY_RZ    = "ENEMY_RZ_";

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

    public static void SavePlayerHealth(float currentHealth)
    {
        PlayerPrefs.SetFloat(KEY_PLAYER_HP, currentHealth);
        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Saved player health: " + currentHealth);
    }

    public static float GetSavedPlayerHealth(float fallback)
    {
        return PlayerPrefs.GetFloat(KEY_PLAYER_HP, fallback);
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

    // ---------------- ENEMIES ----------------

    public static void SaveEnemy(string id, Vector3 pos, Quaternion rot, float hp, bool alive)
    {
        if (string.IsNullOrEmpty(id)) return;

        PlayerPrefs.SetInt(ENEMY_ALIVE + id, alive ? 1 : 0);
        PlayerPrefs.SetFloat(ENEMY_HP + id, hp);

        PlayerPrefs.SetFloat(ENEMY_PX + id, pos.x);
        PlayerPrefs.SetFloat(ENEMY_PY + id, pos.y);
        PlayerPrefs.SetFloat(ENEMY_PZ + id, pos.z);

        Vector3 e = rot.eulerAngles;
        PlayerPrefs.SetFloat(ENEMY_RX + id, e.x);
        PlayerPrefs.SetFloat(ENEMY_RY + id, e.y);
        PlayerPrefs.SetFloat(ENEMY_RZ + id, e.z);
    }

    public static bool HasEnemyData(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return PlayerPrefs.HasKey(ENEMY_ALIVE + id);
    }

    public static bool GetEnemyAlive(string id)
    {
        return PlayerPrefs.GetInt(ENEMY_ALIVE + id, 1) == 1;
    }

    public static float GetEnemyHealth(string id, float fallback)
    {
        return PlayerPrefs.GetFloat(ENEMY_HP + id, fallback);
    }

    public static Vector3 GetEnemyPosition(string id, Vector3 fallback)
    {
        return new Vector3(
            PlayerPrefs.GetFloat(ENEMY_PX + id, fallback.x),
            PlayerPrefs.GetFloat(ENEMY_PY + id, fallback.y),
            PlayerPrefs.GetFloat(ENEMY_PZ + id, fallback.z)
        );
    }

    public static Quaternion GetEnemyRotation(string id, Quaternion fallback)
    {
        Vector3 e = new Vector3(
            PlayerPrefs.GetFloat(ENEMY_RX + id, fallback.eulerAngles.x),
            PlayerPrefs.GetFloat(ENEMY_RY + id, fallback.eulerAngles.y),
            PlayerPrefs.GetFloat(ENEMY_RZ + id, fallback.eulerAngles.z)
        );
        return Quaternion.Euler(e);
    }

    public static void SaveAllEnemiesInScene()
    {
        var enemies = Object.FindObjectsOfType<EnemySaveState>(true);
        foreach (var e in enemies)
            e.SaveNow();

        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Saved enemies: " + enemies.Length);
    }

    // ---------------- CLEAR ----------------

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
        PlayerPrefs.DeleteKey(KEY_PLAYER_HP);

        // NOTE: enemy keys are not enumerated here.
        // For your demo, it's okay. If you want a full wipe, we can store a list of enemy IDs and delete them too.

        PlayerPrefs.Save();
        Debug.Log("[SaveSystem] Save cleared (player keys).");
    }
}
