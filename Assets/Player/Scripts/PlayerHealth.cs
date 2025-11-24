using UnityEngine;
using UnityTutorial.Logging;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        GameLogger.Instance.Log("takedamage", transform.position, "", (int)amount);
        Debug.Log($"Player took {amount} damage, remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        GameLogger.Instance.Log("lose", transform.position);
        isDead = true;

        Debug.Log("Player died!");

        // Show Game Over panel
        GameOverManager.Instance.ShowGameOver();

        // Disable player shooting
        PlayerShooting shooting = GetComponent<PlayerShooting>();
        if (shooting != null) shooting.enabled = false;

        // If you later add a custom movement script, you can disable it here
        // For now, movement just stops because you control it manually in your code
    }
}
