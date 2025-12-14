using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 60f;
    private float currentHealth;

    private Animator animator;
    private NavMeshAgent agent;
    private SpiderAI ai;

    private bool isDead = false;

    public bool IsDead => isDead;

    public float HealthPercent
    {
        get
        {
            if (maxHealth <= 0f) return 0f;
            return currentHealth / maxHealth;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<SpiderAI>();
    }

    public float GetCurrentHealth() => currentHealth;

    public void SetHealthSilently(float value)
    {
        currentHealth = Mathf.Clamp(value, 0f, maxHealth);
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            isDead = true;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage, remaining health: {currentHealth}");

        if (FirebaseGameAnalytics.Instance != null && FirebaseGameAnalytics.Instance.IsReady)
        {
            FirebaseGameAnalytics.Instance.LogGameplayEvent(
                "hitenemy",
                transform.position,
                gameObject.name,
                Mathf.RoundToInt(amount)
            );
        }

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // ✅ persist dead state immediately
        var saveId = GetComponent<EnemySaveID>();
        if (saveId != null && !string.IsNullOrEmpty(saveId.id))
        {
            SaveSystem.SaveEnemy(saveId.id, transform.position, transform.rotation, 0f, false);
            PlayerPrefs.Save();
        }

        EnemyHealthBar bar = GetComponentInChildren<EnemyHealthBar>();
        if (bar != null)
        {
            bar.gameObject.SetActive(false);
        }

        if (agent != null) agent.isStopped = true;
        if (ai != null) ai.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 1.5f);
    }
}
