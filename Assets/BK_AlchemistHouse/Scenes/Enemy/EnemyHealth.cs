using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 60f;  // Spider HP
    private float currentHealth;

    private Animator animator;
    private NavMeshAgent agent;
    private SpiderAI ai;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<SpiderAI>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"Spider took {amount} damage, remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // stop AI
        if (agent != null) agent.isStopped = true;
        if (ai != null) ai.enabled = false;

        // play Die animation
        if (animator != null)
            animator.SetTrigger("Die");

        // disable collider
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // destroy the spider after a short delay (so animation can play)
        Destroy(gameObject, 1.5f); // 1.5 seconds delay
    }
}
