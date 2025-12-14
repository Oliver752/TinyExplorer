using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private bool isDead = false;

    [Header("UI Feedback")]
    public DamageFlash damageFlash;

    [Header("UI")]
    public GameObject crosshair;

    private void Start()
    {
        // Default to max
        currentHealth = maxHealth;

        // ✅ If continuing, restore saved HP (if present)
        if (SaveSystem.HasSave())
        {
            float saved = SaveSystem.GetSavedPlayerHealth(currentHealth);
            SetHealthSilently(saved);
        }
    }

    public float GetCurrentHealth() => currentHealth;

    private void SetHealthSilently(float value)
    {
        currentHealth = Mathf.Clamp(value, 0f, maxHealth);

        // If it was 0, treat as dead (but you probably won't continue in that state)
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            isDead = true;
        }

        Debug.Log($"[PlayerHealth] Loaded health: {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(float amount)
    {
        if (FirebaseGameAnalytics.Instance != null && FirebaseGameAnalytics.Instance.IsReady)
        {
            FirebaseGameAnalytics.Instance.LogGameplayEvent(
                "takedamage",
                transform.position,
                gameObject.name,
                Mathf.RoundToInt(amount)
            );
        }

        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"Player took {amount} damage, remaining health: {currentHealth}");

        if (damageFlash != null)
        {
            damageFlash.Flash();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public float CurrentHealthPercent()
    {
        if (maxHealth <= 0f) return 0f;
        return currentHealth / maxHealth;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player died!");

        if (crosshair != null)
            crosshair.SetActive(false);

        var healthUI = FindObjectOfType<PlayerHealthUI>();
        if (healthUI != null)
            healthUI.gameObject.SetActive(false);

        GameOverManager.Instance.ShowGameOver();

        PlayerShooting shooting = GetComponent<PlayerShooting>();
        if (shooting != null) shooting.enabled = false;
    }
}
